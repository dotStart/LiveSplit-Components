using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveSplit.ComponentUtil;

namespace LiveSplit.PetThePup {
  /// <summary>
  /// Reads the game memory in order to decide when to split.
  /// </summary>
  public class DoggoMemory {
    public event EventHandler OnGameStart;
    public event EventHandler OnGameReset;
    public event EventHandler OnGameCrash;
    public event EventHandler OnPet;

    private readonly PetThePupSettings _settings;

    private readonly DeepPointer _levelPointer = new DeepPointer(0x10A5500);

    private readonly DeepPointer _puppyCounterPointer =
      new DeepPointer("mono.dll", 0x001F8CC0, 0xF0, 0x44, 0x58C);

    private Task _thread;

    // private List<int> _ignoredProcesses;
    private CancellationTokenSource _cancellationTokenSource;

    private bool _gameRunning;
    private uint _previousLevel;
    private uint _previousPuppyCount;

    public DoggoMemory(PetThePupSettings settings) {
      this._settings = settings;
    }

    public void Start() {
      // make sure we are only dealing with a single inspection thread
      if (this._thread != null && this._thread.Status == TaskStatus.Running) {
        throw new InvalidOperationException("Memory is already being inspected");
      }

      // this._ignoredProcesses = new List<int>();
      this._cancellationTokenSource = new CancellationTokenSource();
      this._thread = Task.Factory.StartNew(this.ProcessMemory);
    }

    /// <summary>
    /// Locates the game process.
    /// </summary>
    /// <returns>a game process or null</returns>
    private static Process FindProcess() {
      Process process = Process.GetProcessesByName("PetThePup")
        .FirstOrDefault(p => !p.HasExited /* && !this._ignoredProcesses.Contains(p.Id) */);

      if (process == null) {
        return null;
      }

      // TODO: Verify Game Compatibility

      return process;
    }

    private void ResetState() {
      this._gameRunning = false;
      this._previousLevel = 0;
      this._previousPuppyCount = 0;
    }

    /// <summary>
    /// Handles the actual process which reads values from the game memory in order to figure out
    /// when to split.
    /// </summary>
    private void ProcessMemory() {
      Debug.WriteLine("[Pet the Pup] Entering Memory Monitoring");
      Process game = null;

      while (!this._cancellationTokenSource.IsCancellationRequested) {
        try {
          // if we currently do not have a game process, try to locate it
          // if we cannot find a process, wait for 250ms before trying again
          if (game == null && (game = FindProcess()) == null) {
            Thread.Sleep(250);
            continue;
          }

          // if the game process has died, we'll pause the timer and return back to our initial
          // state
          if (game.HasExited) {
            Debug.WriteLine("Game has exited");
            
            game = null;
            this.ResetState();

            this.OnGameCrash?.Invoke(this, EventArgs.Empty);
            continue;
          }

          // evaluate whether the game has been started (unless we have already logged a game start
          // but no reset)
          uint level = this._levelPointer.Deref(game, (uint) 0);
          uint puppyCount = this._puppyCounterPointer.Deref(game, (uint) 0);

          if (!this._gameRunning) {
            // check whether the game has actually been started
            if (this.OnGameStart == null || level == 0 || level == 101 || puppyCount != 0) continue;

            this._gameRunning = true;
            this.OnGameStart(this, EventArgs.Empty);
          } else {
            // verify whether the game has been reset
            if (level == 0 || level == 101 || this._previousPuppyCount > puppyCount) {
              this.ResetState();
              this.OnGameReset?.Invoke(this, EventArgs.Empty);

              continue;
            }

            // verify whether we have just petted a pup
            if (this._previousPuppyCount < puppyCount && puppyCount != 0) {
              this.OnPet?.Invoke(this, EventArgs.Empty);
            }

            // store the previous values
            this._previousLevel = level;
            this._previousPuppyCount = puppyCount;
          }
        } catch (Exception ex) {
          // in case we catch any errors along the way we'll simply log them and move on
          // while this isn't entirely optimal it's our best shot at preventing data loss in case of
          // an unexpected problem with our memory access
          Debug.WriteLine(ex.ToString());
          Thread.Sleep(1000);
        }
      }
    }
  }
}
