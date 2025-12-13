using UnityEngine;

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
        menuManager.ShowNewGameMenu();
    }

    public void OnContinueClicked()
    {
        Debug.Log("Continue game - not yet implemented");
    }

    public void OnCoopClicked()
    {
        Debug.Log("Co-op mode - not yet implemented");
    }
}
}
