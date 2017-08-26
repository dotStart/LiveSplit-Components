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

namespace LiveSplit.PetThePup {
  public partial class PetThePupSettings : UserControl {
    public bool AllowTimerStart { get; set; }
    public bool AllowTimerReset { get; set; }

    public bool AllowRegistryAccess {
      get => this._allowRegistryAccessValue;
      set {
        this._allowRegistryAccessValue = value;
        this.UpdateSplitMode();
      }
    }

    public SplitMode Mode {
      get {
        if (this.AllowRegistryAccess) {
          return this._splitModeValue;
        }

        if (this._splitModeValue == SplitMode.EveryUnique ||
            this._splitModeValue == SplitMode.AllUnique) {
          return DefaultMode;
        }

        return DefaultMode;
      }
      set => this._splitModeValue = value;
    }

    private const bool DefaultAllowTimerStart = true;
    private const bool DefaultAllowTimerReset = true;
    private const bool DefaultAllowRegistryAccess = false;
    private const SplitMode DefaultMode = SplitMode.Every;

    private bool _allowRegistryAccessValue;
    private SplitMode _splitModeValue;
    
    public PetThePupSettings() {
      this.InitializeComponent();

      this._allowTimerStart.DataBindings.Add("Checked", this, "AllowTimerStart", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._allowTimerReset.DataBindings.Add("Checked", this, "AllowTimerReset", false,
        DataSourceUpdateMode.OnPropertyChanged);
      this._allowRegistryAccess.DataBindings.Add("Checked", this, "AllowRegistryAccess",
        false, DataSourceUpdateMode.OnPropertyChanged);

      this.AllowTimerStart = DefaultAllowTimerStart;
      this.AllowTimerReset = DefaultAllowTimerReset;
      this.AllowRegistryAccess = DefaultAllowRegistryAccess;
      this._splitModeValue = DefaultMode;
      
      this._splitModeNever.CheckedChanged += this.OnSplitModeChanged;
      this._splitModeEvery.CheckedChanged += this.OnSplitModeChanged;
      this._splitModeEveryUnique.CheckedChanged += this.OnSplitModeChanged;
      this._splitModeAll.CheckedChanged += this.OnSplitModeChanged;
    }

    /// <summary>
    /// Converts the current state of this object into a single XML node.
    /// </summary>
    /// <param name="document">the document in which the new node resides</param>
    /// <returns>an XML node</returns>
    public XmlNode GetSettings(XmlDocument document) {
      XmlElement root = document.CreateElement("Settings");

      root.AppendChild(WriteElement(document, "AllowTimerStart", this.AllowTimerStart));
      root.AppendChild(WriteElement(document, "AllowTimerReset", this.AllowTimerReset));
      root.AppendChild(WriteElement(document, "AllowRegistryAccess", this.AllowRegistryAccess));
      root.AppendChild(WriteElement(document, "Mode", this.Mode));

      return root;
    }

    /// <summary>
    /// Copies the values represented by a passed XML node into the local object state.
    /// </summary>
    /// <param name="settings">an XML node</param>
    public void SetSettings(XmlNode settings) {
      this.AllowTimerStart = ReadElement(settings, "AllowTimerStart", DefaultAllowTimerStart);
      this.AllowTimerReset = ReadElement(settings, "AllowTimerReset", DefaultAllowTimerReset);
      this.AllowRegistryAccess =
        ReadElement(settings, "AllowRegistryAccess", DefaultAllowRegistryAccess);
      this.Mode = ReadElement(settings, "Mode", DefaultMode);
      
      this._splitModeNever.Checked = this.Mode == SplitMode.Never;
      this._splitModeEvery.Checked = this.Mode == SplitMode.Every;
      this._splitModeEveryUnique.Checked = this.Mode == SplitMode.EveryUnique;
      this._splitModeAll.Checked = this.Mode == SplitMode.AllUnique;
    }

    /// <summary>
    /// Handles changes to the split mode radio boxes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSplitModeChanged(object sender, EventArgs e) {
      if (this._splitModeEvery.Checked) {
        this.Mode = SplitMode.Every;
      } else if (this._splitModeEveryUnique.Checked) {
        this.Mode = SplitMode.EveryUnique;
      } else if (this._splitModeAll.Checked) {
        this.Mode = SplitMode.AllUnique;
      } else {
        this.Mode = SplitMode.Never;
      }

      this.UpdateSplitMode();
    }

    /// <summary>
    /// Updates the state of the split mode radio boxes.
    /// </summary>
    private void UpdateSplitMode() {
      this._splitModeEveryUnique.Enabled = this._splitModeAll.Enabled = this._allowRegistryAccessValue;
    }

    /// <summary>
    /// Reads a boolean value from the specified node within the passed element.
    /// </summary>
    /// <param name="root">a root or origin node</param>
    /// <param name="name">an element name</param>
    /// <param name="defaultValue">a default value</param>
    /// <returns>the node value</returns>
    private static bool ReadElement(XmlNode root, string name, bool defaultValue) {
      bool value = defaultValue;

      if (root[name] != null) {
        bool.TryParse(root[name].InnerText, out value);
      }

      return value;
    }

    /// <summary>
    /// Reads an enum value from the specified node within the passed element.
    /// </summary>
    /// <param name="root">a root or origin node</param>
    /// <param name="name">an element name</param>
    /// <param name="defaultValue">a default value</param>
    /// <typeparam name="V">an enum type</typeparam>
    /// <returns>the node value</returns>
    /// <exception cref="ArgumentException">when the passed type is not an enum</exception>
    private static V ReadElement<V>(XmlNode root, string name, V defaultValue)
      where V : struct, IConvertible {
      if (!typeof(V).IsEnum) {
        throw new ArgumentException("V must be an numerated type");
      }

      V value = defaultValue;

      if (root[name] != null) {
        Enum.TryParse(root[name].InnerText, out value);
      }

      return value;
    }

    /// <summary>
    /// Converts the passed name and value into a standard XML node.
    /// </summary>
    /// <param name="document">the document in which the new node resides</param>
    /// <param name="name">the element name</param>
    /// <param name="value">the current element value</param>
    /// <typeparam name="V">a value type</typeparam>
    /// <returns></returns>
    private static XmlElement WriteElement<V>(XmlDocument document, string name, V value) {
      XmlElement element = document.CreateElement(name);
      element.InnerText = value.ToString();
      return element;
    }
  }
}
