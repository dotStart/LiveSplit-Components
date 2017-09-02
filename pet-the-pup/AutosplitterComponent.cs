using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.dotStart.PetThePup {
  /// <summary>
  /// Manages the complex splitting logic required for 
  /// </summary>
  public class AutosplitterComponent : LogicComponent {
    public override string ComponentName => "Pet the Pup Autosplitter";
    public bool LayoutComponent { get; }
    public AutosplitterSettings Settings { get; }
    public bool Disposed { get; private set; }

    private readonly TimerModel _timer;
    private readonly LiveSplitState _state;
    private readonly GameMemoryImpl _memory = GameMemoryImpl.Instance;
    private readonly PuppyGallery _registry = PuppyGallery.Instance;

    private bool _timerAlive;

    public AutosplitterComponent(LiveSplitState state) {
      this._state = state;
      this.Settings = new AutosplitterSettings();

      this._timer = new TimerModel {CurrentState = state};
      this._timer.OnStart += this.OnTimerStart;
      this._timer.OnPause += this.OnTimerPause;

      this._memory.OnGameStart += this.OnGameStart;
      this._memory.OnGameReset += this.OnGameReset;
      this._memory.OnProcessDied += this.OnGameCrash;
      this._memory.OnGameAdvance += this.OnAdvance;
      
      this._registry.Claim();
      this._registry.OnPuppyDiscovered += this.OnDiscoverPup;
      this._registry.OnAllPuppiesDiscovered += this.OnDiscoverAllPups;

      this._memory.Start();
    }

    /// <summary>
    /// Initializes the new timer model.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerStart(object sender, EventArgs e) {
      this._timerAlive = true;
      
      // TODO: Right now we do not account for loading screens between the different levels and thus
      // we do not need to initialize game time on our timer model (for now)
      // this._timer.InitializeGameTime();
      
      if (this.Settings.AllowRegistryAccess) {
        Debug.WriteLine("[Pet the Pup] Deleting registry keys");
        this._registry.DeleteKeys();
      }
    }

    /// <summary>
    /// Handles pausing of the timer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerPause(object sender, EventArgs e) {
      this._timerAlive = false;
    }

    /// <summary>
    /// Handles the game startup.
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    private void OnGameStart(object sender, EventArgs e) {
      Debug.WriteLine("[Pet the Pup] Game is Starting");
      
      if (this.Settings.AllowTimerReset) {
        this._timer.Reset();
      }
      
      if (this.Settings.AllowRegistryAccess) {
        this._registry.DeleteKeys();
        this._registry.Start();
      }

      if (this.Settings.AllowTimerStart) {
        this._timer.Start();
      }
    }

    /// <summary>
    /// Handles the game reset.
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    private void OnGameReset(object sender, EventArgs e) {
      Debug.WriteLine("[Pet the Pup] Game has been Reset");
    }

    /// <summary>
    /// Handles the game crash (or rage quit)
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    private void OnGameCrash(object sender, EventArgs e) {
      Debug.Write("[Pet the Pup] Game process has died");

      if (this._timerAlive) {
        this._timer.Pause();
      }
    }

    /// <summary>
    /// Handles every pet that occurs within the game.
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    /// <exception cref="ArgumentOutOfRangeException">when the current split mode is not (yet) supported</exception>
    private void OnAdvance(object sender, EventArgs e) {
      if (this.Settings.Mode != SplitMode.Every) {
        return;
      }
      
      this._timer.Split();
    }

    /// <summary>
    /// Handles every newly discovered pup within the session.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pup"></param>
    private void OnDiscoverPup(object sender, Puppy pup) {
      Debug.WriteLine("[Pet the Pup] Discovered pup " + pup);

      if (this.Settings.Mode != SplitMode.EveryUnique) {
        return;
      }
      
      this._timer.Split();
    }

    /// <summary>
    /// Handles the case of a player who discovered all pups.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDiscoverAllPups(object sender, EventArgs e) {
      Debug.WriteLine("[Pet the Pup] Discovered all pups in the gallery");

      if (this.Settings.Mode != SplitMode.AllUnique) {
        return;
      }
      
      this._timer.Split();
    }

    /// <inheritdoc />
    public override void Dispose() {
      this._timer.OnStart -= this.OnTimerStart;

      this._memory.OnGameStart -= this.OnGameStart;
      this._memory.OnGameReset -= this.OnGameReset;
      this._memory.OnProcessDied -= this.OnGameCrash;
      this._memory.OnGameAdvance -= this.OnAdvance;
      
      this._registry.OnPuppyDiscovered -= this.OnDiscoverPup;
      this._registry.OnAllPuppiesDiscovered -= this.OnDiscoverAllPups;
      
      this._registry.Dispose();
      this._memory.Dispose();
      
      this.Disposed = true;
    }

    /// <inheritdoc />
    public override XmlNode GetSettings(XmlDocument document) {
      return this.Settings.GetSettings(document);
    }

    /// <inheritdoc />
    public override Control GetSettingsControl(LayoutMode mode) {
      return this.Settings;
    }

    /// <inheritdoc />
    public override void SetSettings(XmlNode settings) {
      this.Settings.SetSettings(settings);
    }

    /// <inheritdoc />
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width,
      float height,
      LayoutMode mode) {
    }
  }
}
