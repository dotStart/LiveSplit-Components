using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace LiveSplit.dotStart.PetThePup {
  /// <summary>
  /// Allows the component to gain access to the puppy library in order to identify the amount of
  /// unique pups petted per run.
  /// 
  /// This is a non-standard feature and causes the game to loose its actual save-game which is why
  /// this particular feature has to be specifically enabled.
  /// </summary>
  public class PuppyGallery {
    public uint Discovered => (uint) this._discoveredPuppies.Count;
    public Puppy Latest { get; private set; }
    public ReadOnlyCollection<Puppy> Remaining => new ReadOnlyCollection<Puppy>(this._remainingPuppies);

    public event EventHandler<Puppy> OnPuppyDiscovered;
    public event EventHandler OnAllPuppiesDiscovered;

    /// <summary>
    /// Retrieves or creates a puppy gallery instance as needed.
    /// </summary>
    public static PuppyGallery Instance { get; private set; }
    
    private const string RegistryKey = @"Software\will herring\Pet The Pup at the Party";

    private readonly List<Puppy> _discoveredPuppies = new List<Puppy>();
    private readonly List<Puppy> _remainingPuppies = new List<Puppy>();

    private PuppyGallery() {
      Instance = this;
    }
    
    /// <summary>
    /// Computes the name of a unity registry key.
    /// </summary>
    /// <param name="text">a key name</param>
    /// <returns>a key including its hash</returns>
    private static string ComputeUnityKey(string text) {
      return text + "_h" + text.Aggregate<char, uint>(5381, (current, c) => current * 33 ^ c);
    }

    /// <summary>
    /// Deletes all game keys.
    /// </summary>
    public void DeleteKeys() {
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
            Debug.Write("[Puppy Gallery] Deleting " + RegistryKey + "\\" + ComputeUnityKey(puppy.ToString()) + " ... ");
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

    /// <summary>
    /// Updates the amount and set of discovered doggos.
    /// </summary>
    public void Update() {
      using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, false)) {
        // the key itself is not defined (e.g. the game has yedt to be executed)
        if (key == null) {
          return;
        }

        try {
          foreach (Puppy puppy in Enum.GetValues(typeof(Puppy))) {
            // since this process is pretty terrible and slow we'll omit all puppies that we know
            // have already been discovered within this session
            if (this._discoveredPuppies.Contains(puppy)) {
              continue;
            }

            // retrieve the key value in order to figure out whether the key has been populated by
            // the game as a result of the discovery and if so push it to the list of known values
            object keyValue = key.GetValue(ComputeUnityKey(puppy.ToString()));

            if (keyValue != null && (int) keyValue == 1) {
              Debug.WriteLine("[Puppy Gallery] Key " + RegistryKey + "\\" + ComputeUnityKey(puppy.ToString()) + " has changed");

              this._discoveredPuppies.Add(puppy);
              this._remainingPuppies.Remove(puppy);
              this.Latest = puppy;
              
              this.OnPuppyDiscovered?.Invoke(this, puppy);

              // if we discovered all puppies, we'll also invoke the respective event to notify
              // whatever component is listening in
              if (this._discoveredPuppies.Count == Enum.GetValues(typeof(Puppy)).Length) {
                this.OnAllPuppiesDiscovered?.Invoke(this, EventArgs.Empty);
              }
            }
          }
        } catch (Exception ex) {
          // simply report any problems we discover to the developer console as we do not wish to
          // bother the user if anything goes wrong
          Debug.Write("Failed to access registry: " + ex.Message);
        }
      }
    }
  }
}
