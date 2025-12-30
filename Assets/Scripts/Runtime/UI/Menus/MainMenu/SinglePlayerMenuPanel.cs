using UnityEngine;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Shared;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    /// <summary>
    /// Single Player menu panel with options for New Game, Continue, and Co-op.
    /// </summary>
    public class SinglePlayerMenuPanel : UIMenuPanel
{
    public override void GoBack()
    {
        menuManager.ShowMainMenu();
    }

    public void OnNewGameClicked()
    {
        Debug.Log("[SinglePlayerMenuPanel] New Game clicked");
        menuManager.ShowNewGameMenu();
    }

    public void OnContinueClicked()
    {
        Debug.Log("[SinglePlayerMenuPanel] Continue clicked - not yet implemented");
    }

    public void OnCoopClicked()
    {
        Debug.Log("[SinglePlayerMenuPanel] Co-op clicked - not yet implemented");
    }
}
}
