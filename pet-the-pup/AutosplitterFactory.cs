using System;
using System.Reflection;
using System.Windows.Forms;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.dotStart.PetThePup {
  /// <summary>
  /// Constructs the pet the pup autosplitter component.
  /// </summary>
  public class AutosplitterFactory : IComponentFactory {
    public string ComponentName => "Pet the Pup Autosplitter";
    public string Description =>
      "Automatically starts, resets or splits based on the Pet the Pup game state.";
    public ComponentCategory Category => ComponentCategory.Control;

    public string UpdateName => this.ComponentName;
    public string UpdateURL => "https://dl.dotstart.tv/livesplit/";
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    public string XMLURL => "update.xml";

    private AutosplitterComponent _instance;
    
    /// <inheritdoc />
    public IComponent Create(LiveSplitState state) {
      // we only allow a single instance of the component to be present within the application
      if (this._instance != null && !this._instance.Disposed) {
        MessageBox.Show(
          "PetThePup splitter is already loaded (either in the Splits or Layout Editor)",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation
        );

        throw new Exception("Component is already loaded");
      }

      return this._instance = new AutosplitterComponent(state);
    }
  }
}
