using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveSplit.dotStart.Common;
using Microsoft.Win32;

namespace LiveSplit.dotStart.PetThePup {
  /// <summary>
  /// Allows the component to gain access to the puppy library in order to identify the amount of
  /// unique pups petted per run.
  /// 
  /// This is a non-standard feature and causes the game to loose its actual save-game which is why
  /// this particular feature has to be specifically enabled.
  /// </summary>
  public class PuppyGallery : AbstractSharable {
    public uint Discovered { get; private set; }
    public Puppy Latest { get; private set; }
    public ReadOnlyCollection<Puppy> Remaining { get; private set; }
    public ReadOnlyDictionary<Puppy, uint> PetCount { get; private set; }

    public event EventHandler<Puppy> OnPuppyDiscovered;
    public event EventHandler OnAllPuppiesDiscovered;

    /// <summary>
    /// Retrieves or creates a puppy gallery instance as needed.
    /// </summary>
    public static PuppyGallery Instance {
      get {
        if (_instance == null) {
          return _instance = new PuppyGallery();
        }

        return _instance;
      }
    }

    private const string RegistryKey = @"Software\will herring\Pet The Pup at the Party";

    private static PuppyGallery _instance;

    private Task _task;
    private SynchronizationContext _synchronizationContext;

    private CancellationTokenSource _cancellationTokenSource;
    private readonly object _stateLock = new object();

    private Puppy _latest;
    private readonly List<Puppy> _discoveredPuppies = new List<Puppy>();
    private readonly List<Puppy> _remainingPuppies = new List<Puppy>();
    private readonly Dictionary<Puppy, uint> _petCount = new Dictionary<Puppy, uint>();

    private PuppyGallery() {
      this.Remaining = new ReadOnlyCollection<Puppy>(new List<Puppy>());
      this.PetCount = new ReadOnlyDictionary<Puppy, uint>(new Dictionary<Puppy, uint>());
    }

    /// <summary>
    /// Computes the name of a unity registry key.
    /// </summary>
    /// <param name="text">a key name</param>
    /// <returns>a key including its hash</returns>
    private static string ComputeUnityKey(string text) {
      return text + "_h" + text.Aggregate<char, uint>(5381, (current, c) => current * 33 ^ c);
    }

    public void Start() {
      if (this._task != null && this._task.Status == TaskStatus.Running) {
        return;
      }
      
      this._synchronizationContext = SynchronizationContext.Current;

      this.DeleteKeys();
      this._cancellationTokenSource = new CancellationTokenSource();
      
      this._task = Task.Factory.StartNew(this.Update);
    }

    public void Stop() {
      if (this._task == null || this._task.Status != TaskStatus.Running) {
        return;
      }
      
      this._cancellationTokenSource.Cancel();
      this._task.Wait();
    }

    public void Reset() {
      lock (this._stateLock) {
        Debug.WriteLine("[Pup Gallery] Resetting Statistics");

        this._petCount.Clear();
        this.RefreshPublicValues();
      }
    }

    /// <summary>
    /// Deletes all game keys.
    /// </summary>
    public void DeleteKeys() {
      lock (this._stateLock) {
        Debug.WriteLine("[Pup Gallery] Reverting puppy gallery");
        
        // delete the entire known list of discovered puppies
        this._discoveredPuppies.Clear();
        this._remainingPuppies.Clear();

        // delete the entire known list of puppies within the registry in order to force the game to
        // recreate the values and thus let us figure out which puppies were discovered during the
        // current session
        try {
          using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true)) {
            // if the key itself does not exist, simply abort as the game will create a new save game
            // structure in the desired state anyways
            if (key == null) {
              return;
            }

            foreach (Puppy puppy in Enum.GetValues(typeof(Puppy))) {
              Debug.Write("[Puppy Gallery] Deleting " + RegistryKey + "\\" +
                          ComputeUnityKey(puppy.ToString()) + " ... ");
              this._remainingPuppies.Add(puppy);

              // evaluate whether there is an actual value present for the puppy in question and if
              // not simply skip the action as it is already in the desired state
              if (key.GetValue(ComputeUnityKey(puppy.ToString())) == null) {
                Debug.WriteLine("MISSING");
                continue;
              }

              // otherwise delete the entire key to force the game to update it when we discover the
              // puppy in question once again
              key.DeleteValue(ComputeUnityKey(puppy.ToString()));
              Debug.WriteLine("OK");
            }
          }
        } catch (Exception ex) {
          // simply report any problems we discover to the developer console as we do not wish to
          // bother the user if anything goes wrong
          Debug.WriteLine("");
          Debug.WriteLine("Failed to access registry: " + ex.Message);
        }
      }
    }

    private void RefreshPublicValues() {
      if (this._synchronizationContext == null) {
        this.DoRefreshPublicValues();
        return;
      }
      
      this._synchronizationContext.Post(d => this.DoRefreshPublicValues(), false);
    }

    private void DoRefreshPublicValues() {
      uint discovered = (uint) this._discoveredPuppies.Count;
      Puppy puppy = this._latest;
      ReadOnlyCollection<Puppy> remaining =
        new ReadOnlyCollection<Puppy>(new List<Puppy>(this._remainingPuppies));
      Dictionary<Puppy, uint> petCount = new Dictionary<Puppy, uint>(this._petCount);
      
      this.Discovered = discovered;
      this.Latest = puppy;
      this.Remaining = remaining;
      this.PetCount = new ReadOnlyDictionary<Puppy, uint>(petCount);
    }

    /// <summary>
    /// Updates the amount and set of discovered doggos.
    /// </summary>
    private void Update() {
      Debug.WriteLine("[Puppy Gallery] Starting registry thread");
      
      while (!this._cancellationTokenSource.IsCancellationRequested) {
        lock (this._stateLock) {
          using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true)) {
            // the key itself is not defined (e.g. the game has yedt to be executed)
            if (key == null) {
              return;
            }

            try {
              foreach (Puppy puppy in Enum.GetValues(typeof(Puppy))) {
                // retrieve the key value in order to figure out whether the key has been populated by
                // the game as a result of the discovery and if so push it to the list of known values
                object keyValue = key.GetValue(ComputeUnityKey(puppy.ToString()));

                if (keyValue == null || (int) keyValue != 1) {
                  continue;
                }
                
                // if we are dealing with a puppy that has already been discovered within this run,
                // we'll simply update the pet counter and move on
                if (this._discoveredPuppies.Contains(puppy)) {
                  ++this._petCount[puppy];

                  this.RefreshPublicValues();
                } else {
                  // otherwise create the structure and remove the puppy from our list
                  Debug.WriteLine("[Puppy Gallery] Key " + RegistryKey + "\\" +
                                  ComputeUnityKey(puppy.ToString()) + " has changed");

                  this._latest = puppy;
                  this._discoveredPuppies.Add(puppy);
                  this._remainingPuppies.Remove(puppy);
                  this._petCount[puppy] = 1;

                  this.RefreshPublicValues();

                  this._synchronizationContext.Post(d => {
                    this.OnPuppyDiscovered?.Invoke(this, puppy);
                  }, false);

                  // if we discovered all puppies, we'll also invoke the respective event to notify
                  // whatever component is listening in
                  if (this._discoveredPuppies.Count == Enum.GetValues(typeof(Puppy)).Length) {
                    this._synchronizationContext.Post(
                      d => this.OnAllPuppiesDiscovered?.Invoke(this, EventArgs.Empty), false);
                  }
                }
                
                key.DeleteValue(ComputeUnityKey(puppy.ToString()));
              }
            } catch (Exception ex) {
              // simply report any problems we discover to the developer console as we do not wish to
              // bother the user if anything goes wrong
              Debug.WriteLine("Failed to access registry: " + ex.Message);
            }
          }

          Thread.Sleep(250);
        }
      }
      
      Debug.WriteLine("[Puppy Gallery] Stopping registry thread");
    }
  }
}
