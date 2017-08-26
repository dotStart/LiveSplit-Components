using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.PetThePup {
  /// <summary>
  /// Manages the complex splitting logic required for 
  /// </summary>
  public class PetThePupComponent : LogicComponent {
    public override string ComponentName => "Pet the Pup";
    public bool LayoutComponent { get; }
    public PetThePupSettings Settings { get; }
    public bool Disposed { get; private set; }

    private readonly TimerModel _timer;
    private readonly LiveSplitState _state;
    private readonly DoggoMemory _memory;
    private readonly DoggoRegistry _registry;

    public PetThePupComponent(LiveSplitState state) {
      this._state = state;
      this.Settings = new PetThePupSettings();

      this._timer = new TimerModel {CurrentState = state};
      this._timer.OnStart += this.OnTimerStart;

      this._memory = new DoggoMemory(this.Settings);
      this._memory.OnGameStart += this.OnGameStart;
      this._memory.OnGameReset += this.OnGameReset;
      this._memory.OnGameCrash += this.OnGameCrash;
      this._memory.OnPet += this.OnPet;
      
      this._registry = new DoggoRegistry();

      this._memory.Start();
    }

    /// <summary>
    /// Initializes the new timer model.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerStart(object sender, EventArgs e) {
      // TODO: Right now we do not account for loading screens between the different levels and thus
      // we do not need to initialize game time on our timer model (for now)
      // this._timer.InitializeGameTime();
    }

    /// <summary>
    /// Handles the game startup.
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    private void OnGameStart(object sender, EventArgs e) {
      Debug.WriteLine("Game is Starting");
      
      if (this.Settings.AllowRegistryAccess) {
        Debug.WriteLine("Deleting registry keys");
        this._registry.DeleteKeys();
      }
      
      if (this.Settings.AllowTimerReset) {
        this._timer.Reset();
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
      Debug.WriteLine("Game has been Reset");
      
      if (this.Settings.AllowTimerReset) {
        this._timer.Reset();
      }
    }

    /// <summary>
    /// Handles the game crash (or rage quit)
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    private void OnGameCrash(object sender, EventArgs e) {
      Debug.Write("Game process has died");
      this._timer.Pause();
    }

    /// <summary>
    /// Handles every pet that occurs within the game.
    /// </summary>
    /// <param name="sender">a sender</param>
    /// <param name="e">a set of event arguments</param>
    /// <exception cref="ArgumentOutOfRangeException">when the current split mode is not (yet) supported</exception>
    private void OnPet(object sender, EventArgs e) {
      Debug.Write("Pet!");
      uint doggoCount = this._registry.CapturedAmount;

      if (this.Settings.AllowRegistryAccess) {
        this._registry.Update();
      }
      
      switch (this.Settings.Mode) {
        case SplitMode.Never: break;
        case SplitMode.Every:
          this._timer.Split();
          break;
        case SplitMode.EveryUnique:
          if (doggoCount != this._registry.CapturedAmount) {
            this._timer.Split();
          }
          break;
        case SplitMode.AllUnique:
          if (this._registry.CapturedAll) {
            this._timer.Split();
          }
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <inheritdoc />
    public override void Dispose() {
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
