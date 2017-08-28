using System;
using System.Reflection;
using System.Windows.Forms;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.dotStart.PetThePup.UI {
  /// <summary>
  /// Constructs GameStateComponent instances for use within LiveSplit's layout editor.
  /// </summary>
  public class GameStateFactory : IComponentFactory {
    public string ComponentName => "Pet the Pup";
    public string Description => "Displays information about the current state of the game.";
    public ComponentCategory Category => ComponentCategory.Information;
    
    public string UpdateName => this.ComponentName;
    public string UpdateURL => "https://dl.dotstart.tv/livesplit/";
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    public string XMLURL => "pet-the-pup/update.xml";

    /// <inheritdoc />
    public IComponent Create(LiveSplitState state) {
      return new GameStateComponent();
    }
  }
}
