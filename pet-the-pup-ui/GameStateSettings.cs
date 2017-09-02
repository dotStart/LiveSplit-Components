using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.dotStart.Common;

namespace LiveSplit.dotStart.PetThePup.UI {
  /// <summary>
  /// Provides a set of settings which define how the game state component shows up inside of
  /// the LiveSplit UI.
  /// </summary>
  public partial class GameStateSettings : ComponentSettings {
    public bool DisplayTotalPups { get; set; }
    public bool DisplayUniquePups { get; set; }
    public bool DisplayTotalConversations { get; set; }
    public bool DisplayLastDiscoveredPup { get; set; }
    public bool DisplayRemainingPups {
      get => this._displayRemainingPups;
      set {
        this._displayRemainingPups = value;
        this._numRemainingPups.Enabled = value;
      }
    }
    public uint RemainingPupAmount { get; set; }
    public bool DisplayPetStatistics {
      get => this._displayPetStatistics;
      set {
        this._displayPetStatistics = value;
        this._numPetStatistics.Enabled = value;
      }
    }
    public uint PetStatisticsAmount { get; set; }

    private const bool DefaultDisplayTotalPups = false;
    private const bool DefaultDisplayUniquePups = true;
    private const bool DefaultDisplayTotalConversations = true;
    private const bool DefaultDisplayLastDiscoveredPup = true;
    private const bool DefaultDisplayRemainingPups = true;
    private const uint DefaultRemainingPupAmount = 5;
    private const bool DefaultDisplayPetStatistics = false;
    private const uint DefaultPetStatisticsAmount = 3;

    private bool _displayRemainingPups;
    private bool _displayPetStatistics;
    
    public GameStateSettings() {
      this.InitializeComponent();

      this._chkDisplayTotalPups.DataBindings.Add("Checked", this, "DisplayTotalPups", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._chkDisplayUniquePups.DataBindings.Add("Checked", this, "DisplayUniquePups", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._chkDisplayTotalConversations.DataBindings.Add("Checked", this,
        "DisplayTotalConversations", false, DataSourceUpdateMode.OnPropertyChanged);
      this._chkDisplayLastDiscoveredPup.DataBindings.Add("Checked", this,
        "DisplayLastDiscoveredPup", false, DataSourceUpdateMode.OnPropertyChanged);
      this._chkDisplayRemainingPups.DataBindings.Add("Checked", this, "DisplayRemainingPups", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._numRemainingPups.DataBindings.Add("Value", this, "RemainingPupAmount", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._chkDisplayPetStatistics.DataBindings.Add("Checked", this, "DisplayPetStatistics", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._numPetStatistics.DataBindings.Add("Value", this, "PetStatisticsAmount", false,
        DataSourceUpdateMode.OnPropertyChanged);

      this._numRemainingPups.Maximum = Enum.GetValues(typeof(Puppy)).Length;

      this.DisplayTotalPups = DefaultDisplayTotalPups;
      this.DisplayUniquePups = DefaultDisplayUniquePups;
      this.DisplayTotalConversations = DefaultDisplayTotalConversations;
      this.DisplayLastDiscoveredPup = DefaultDisplayLastDiscoveredPup;
      this.DisplayRemainingPups = DefaultDisplayRemainingPups;
      this.RemainingPupAmount = DefaultRemainingPupAmount;
      this.DisplayPetStatistics = DefaultDisplayPetStatistics;
      this.PetStatisticsAmount = DefaultPetStatisticsAmount;
    }

    /// <inheritdoc />
    protected override void GetSettings(XmlDocument document, XmlElement root) {
      root.AppendChild(WriteElement(document, "DisplayTotalPups", this.DisplayTotalPups));
      root.AppendChild(WriteElement(document, "DisplayUniquePups", this.DisplayUniquePups));
      root.AppendChild(WriteElement(document, "DisplayTotalConservations", this.DisplayTotalConversations));
      root.AppendChild(WriteElement(document, "DisplayLastDiscoveredPup",
        this.DisplayLastDiscoveredPup));
      root.AppendChild(WriteElement(document, "DisplayRemainingPups", this.DisplayRemainingPups));
      root.AppendChild(WriteElement(document, "RemainingPupAmount", this.RemainingPupAmount));
      root.AppendChild(WriteElement(document, "DisplayPetStatistics", this.DisplayPetStatistics));
      root.AppendChild(WriteElement(document, "PetStatisticsAmount", this.PetStatisticsAmount));
    }

    /// <inheritdoc />
    public override void SetSettings(XmlNode settings) {
      this.DisplayTotalPups = ReadElement(settings, "DisplayTotalPups", DefaultDisplayTotalPups);
      this.DisplayUniquePups = ReadElement(settings, "DisplayUniquePups", DefaultDisplayUniquePups);
      this.DisplayTotalConversations = ReadElement(settings, "DisplayTotalConversations",
        DefaultDisplayTotalConversations);
      this.DisplayLastDiscoveredPup = ReadElement(settings, "DisplayLastDiscoveredPup",
        DefaultDisplayLastDiscoveredPup);
      this.DisplayRemainingPups =
        ReadElement(settings, "DisplayRemainingPups", DefaultDisplayRemainingPups);
      this.RemainingPupAmount =
        ReadElement(settings, "RemainingPupAmount", DefaultRemainingPupAmount);
      this.DisplayPetStatistics =
        ReadElement(settings, "DisplayPetStatistics", DefaultDisplayPetStatistics);
      this.PetStatisticsAmount =
        ReadElement(settings, "PetStatisticsAmount", DefaultPetStatisticsAmount);
      
      this._numRemainingPups.Enabled = this.DisplayRemainingPups;
      this._numPetStatistics.Enabled = this.DisplayPetStatistics;
    }
  }
}
