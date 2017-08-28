using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.dotStart.Common;

namespace LiveSplit.dotStart.PetThePup {
  public partial class AutosplitterSettings : ComponentSettings {
    
    /// <summary>
    /// Defines whether the component is allowed to start the timer when a new game is started.
    /// </summary>
    public bool AllowTimerStart { get; set; }
    
    /// <summary>
    /// Defines whether the component is allowed to reset the timer when the player leaves the
    /// actual game and returns to the main menu.
    /// </summary>
    public bool AllowTimerReset { get; set; }

    /// <summary>
    /// Defines whether the component is allowed to change the game registry in order to detect
    /// which unique pups have been found.
    /// </summary>
    public bool AllowRegistryAccess {
      get => this._allowRegistryAccessValue;
      set {
        this._allowRegistryAccessValue = value;
        this.UpdateSplitMode();
      }
    }

    /// <summary>
    /// Defines which type of logic to run in order to decide when to advance to a new split.
    /// </summary>
    public SplitMode Mode { get; set; }

    private const bool DefaultAllowTimerStart = true;
    private const bool DefaultAllowTimerReset = true;
    private const bool DefaultAllowRegistryAccess = false;
    private const SplitMode DefaultMode = SplitMode.Every;

    private bool _updating;
    private bool _allowRegistryAccessValue;
    
    public AutosplitterSettings() {
      this.InitializeComponent();

      this._allowTimerStart.DataBindings.Add("Checked", this, "AllowTimerStart", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._allowTimerReset.DataBindings.Add("Checked", this, "AllowTimerReset", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._allowRegistryAccess.DataBindings.Add("Checked", this, "AllowRegistryAccess",
        false, DataSourceUpdateMode.OnPropertyChanged);

      this.AllowTimerStart = DefaultAllowTimerStart;
      this.AllowTimerReset = DefaultAllowTimerReset;
      this._allowRegistryAccessValue = DefaultAllowRegistryAccess;
      this.Mode = DefaultMode;
      
      this.UpdateSplitMode();
    }

    /// <inheritdoc />
    protected override void GetSettings(XmlDocument document, XmlElement root) {
      root.AppendChild(WriteElement(document, "AllowTimerStart", this.AllowTimerStart));
      root.AppendChild(WriteElement(document, "AllowTimerReset", this.AllowTimerReset));
      root.AppendChild(WriteElement(document, "AllowRegistryAccess", this.AllowRegistryAccess));
      root.AppendChild(WriteElement(document, "Mode", this.Mode));
    }

    /// <inheritdoc />
    public override void SetSettings(XmlNode settings) {
      this.AllowTimerStart = ReadElement(settings, "AllowTimerStart", DefaultAllowTimerStart);
      this.AllowTimerReset = ReadElement(settings, "AllowTimerReset", DefaultAllowTimerReset);
      this._allowRegistryAccessValue =
        ReadElement(settings, "AllowRegistryAccess", DefaultAllowRegistryAccess);
      this.Mode = ReadElement(settings, "Mode", DefaultMode);
      
      this.UpdateSplitMode();
    }

    /// <summary>
    /// Handles changes to the split mode radio boxes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSplitModeChanged(object sender, EventArgs e) {
      // due to the fact that every change of the .Checked field actively causes the application to
      // re-evaluate the current mode, we'll have to postpone this update whenever the call
      // originates within the UpdateSplitMode method
      if (this._updating) {
        return;
      }
      
      if (this._splitModeEvery.Checked) {
        this.Mode = SplitMode.Every;
      } else if (this._splitModeEveryUnique.Checked) {
        this.Mode = SplitMode.EveryUnique;
      } else if (this._splitModeAll.Checked) {
        this.Mode = SplitMode.AllUnique;
      } else {
        this.Mode = SplitMode.Never;
      }
    }

    /// <summary>
    /// Updates the state of the split mode radio boxes.
    /// </summary>
    private void UpdateSplitMode() {
      if (!this._allowRegistryAccessValue && (this.Mode == SplitMode.EveryUnique || this.Mode == SplitMode.AllUnique)) {
        this.Mode = DefaultMode;
      }
      
      this._splitModeEveryUnique.Enabled = this._splitModeAll.Enabled = this._allowRegistryAccessValue;

      this._updating = true;
      {
        this._splitModeNever.Checked = this.Mode == SplitMode.Never;
        this._splitModeEvery.Checked = this.Mode == SplitMode.Every;
        this._splitModeEveryUnique.Checked = this.Mode == SplitMode.EveryUnique;
        this._splitModeAll.Checked = this.Mode == SplitMode.AllUnique;
      }
      this._updating = false;
      this.OnSplitModeChanged(null, EventArgs.Empty);
    }
  }
}
