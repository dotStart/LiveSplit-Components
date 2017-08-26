using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.dotStart.Common.Splitter {
  /// <summary>
  /// Provides a base to all implementations that directly interact with game memory in order to
  /// gain access to its state.
  /// 
  /// Typically these components are designed to advance the currently active split or remove load
  /// times from the resulting times, however, it may also be relied upon when displaying special
  /// information such as game states relevant to the respective category on-screen.
  /// </summary>
  public abstract class GameMemory {

    /// <summary>
    /// Evaluates whether a compatible game process has been located and is still alive.
    /// </summary>
    public bool Alive => this.Process != null && !this.Process.HasExited;

    /// <summary>
    /// Stores the currently selected process.
    /// 
    /// Note that this may also be null when no compatible process has been located by the
    /// implementation yet.
    /// </summary>
    public Process Process { get; private set; }

    /// <summary>
    /// Provides an event which is called when a compatible process is selected.
    /// </summary>
    public event EventHandler OnProcessSelected;
    
    /// <summary>
    /// Provides an event which is called when a selected process dies (e.g. is killed or gracefully
    /// exits).
    /// </summary>
    public event EventHandler OnProcessDied;
    
    /// <summary>
    /// Defines a list of known process names which this implementation will scan for.
    /// </summary>
    protected abstract ReadOnlyCollection<string> ProcessNames { get; }
    
    /// <summary>
    /// Stores the parent thread to which our events will be posted.
    /// </summary>
    protected SynchronizationContext ParentThread { get; private set; }
    
    /// <summary>
    /// Stores a list of process identifiers which were identified to be incompatible with our
    /// definition and as such will not be checked against until the memory implementation has been
    /// restarted or a process has been found.
    /// </summary>
    private readonly List<int> _blacklistedProcessIdentifiers = new List<int>();

    /// <summary>
    /// Manages a cancellation token which helps us to safely notify the polling thread of our
    /// request to shut down.
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Stores the thread which outsources the polling process.
    /// </summary>
    private Task _thread;

    /// <summary>
    /// Starts the memory polling background task.
    /// </summary>
    /// <exception cref="InvalidOperationException">when the thread is already alive</exception>
    public void Start() {
      // make sure we are not polling the memory yet
      if (this._thread != null && this._thread.Status == TaskStatus.Running) {
        throw new InvalidOperationException("Game Memory is already being inspected");
      }
      
      // prepare the state and spawn a new thread
      this.ResetValues();
      
      this.ParentThread = SynchronizationContext.Current;
      this._cancellationTokenSource = new CancellationTokenSource();
      this._thread = Task.Factory.StartNew(this.Update);
    }

    /// <summary>
    /// Stops the memory polling task.
    /// </summary>
    /// <exception cref="InvalidOperationException">when the thread is not alive</exception>
    public void Stop() {
      // make sure we are actually polling the memory
      if (this._cancellationTokenSource == null || this._thread == null ||
          this._thread.Status != TaskStatus.Running) {
        throw new InvalidOperationException("Game Memory is not being inspected");
      }
      
      // request the thread to shut down and wait for it to exit
      this._cancellationTokenSource.Cancel();
      this._thread.Wait();
    }

    /// <summary>
    /// Attempts to find a game process that this implementation considers compatible with its
    /// definitions.
    /// </summary>
    /// <returns>a compatible game process or null</returns>
    private Process FindProcess() {
      foreach (string processName in this.ProcessNames) {
        foreach (Process process in Process.GetProcessesByName(processName)) {
          // ensure the process we are dealing with has not been blacklisted and is still alive
          // before passing it on
          if (process.HasExited || this._blacklistedProcessIdentifiers.Contains(process.Id)) {
            continue;
          }
          
          // if the process is incompatible, we'll blacklist the process until we find something
          // compatible (in order to reduce the chance of running into re-assignment issues)
          if (!this.CheckCompatibility(process)) {
            this._blacklistedProcessIdentifiers.Add(process.Id);
            continue;
          }
          
          // the process seems to be compatible with our implementation definitions
          return process;
        }
      }

      return null;
    }

    /// <summary>
    /// Perform actual memory polling.
    /// </summary>
    private void Update() {
      Debug.WriteLine("[GameMemory] Starting poll process");

      while (!this._cancellationTokenSource.IsCancellationRequested) {
        // if we currently do not have a game process, try to locate it
        // if we cannot find a process, wait for 250ms before trying again
        if (this.Process == null) {
          this.Process = this.FindProcess();

          if (this.Process == null) {
            Thread.Sleep(250);
            continue;
          }
          
          Debug.WriteLine("[GameMemory] Selected compatible process #" + this.Process.Id + " (" + this.Process.ProcessName + ")");
          this.ParentThread.Post(d => this.OnProcessSelected?.Invoke(this, EventArgs.Empty), null);
          this._blacklistedProcessIdentifiers.Clear();
        }
        
        // evaluate whether the thread we are dealing with is still alive before we attempt anything
        // else
        if (this.Process.HasExited) {
          Debug.WriteLine("[GameMemory] Selected process #" + this.Process.Id + " (" + this.Process.ProcessName + ") has died");
          
          this.Process = null;
          this.ResetValues();
          
          this.ParentThread.Post(d => this.OnProcessDied?.Invoke(this, EventArgs.Empty), null);
          continue;
        }

        try {
          this.UpdateValues();
        } catch (Exception ex) {
          // all errors that occur within the update values block will be ignored quietly from our
          // perspective as we cannot risk to shutdown or block this thread
          Debug.WriteLine("An unexpected error has occurred while updating game memory values: " + ex.Message);
        }
      }
      
      Debug.WriteLine("[GameMemory] Stopping poll process");
    }
    
    /// <summary>
    /// Evaluates whether the selected process is compatible with the definitions provided by this
    /// game memory implementation.
    /// </summary>
    /// <param name="process">a candidate process</param>
    /// <returns>true if compatible, false otherwise</returns>
    protected virtual bool CheckCompatibility(Process process) {
      return true;
    }

    /// <summary>
    /// Resets the cached values to their default state.
    /// </summary>
    protected virtual void ResetValues() {
    }

    /// <summary>
    /// Updates the cached values based on the process state.
    /// </summary>
    protected abstract void UpdateValues();
  }
}
