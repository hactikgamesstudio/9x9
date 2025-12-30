using UnityEngine;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Shared;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    /// <summary>
    /// Main menu panel displaying the primary navigation options.
    /// Options: Single Player, Multiplayer, Profile, Options, Quit
    /// </summary>
    public class MainMenuPanel : UIMenuPanel
{
    public override void GoBack()
    {
        // Main menu has nowhere to go back to
        Debug.Log("Already at main menu");
    }

    public void OnSinglePlayerClicked()
    {
        Debug.Log("[MainMenuPanel] Single Player clicked");
        menuManager.ShowSinglePlayerMenu();
    }

    public void OnMultiplayerClicked()
    {
        Debug.Log("[MainMenuPanel] Multiplayer clicked");
        menuManager.ShowMultiplayerMenu();
    }

    public void OnProfileClicked()
    {
        Debug.Log("[MainMenuPanel] Profile clicked");
        menuManager.ShowProfileMenu();
    }

    public void OnOptionsClicked()
    {
        Debug.Log("[MainMenuPanel] Options clicked");
        menuManager.ShowOptionsMenu();
    }

    public void OnQuitClicked()
    {
        Debug.Log("[MainMenuPanel] Quit clicked");
        menuManager.QuitGame();
    }
}
}
