using UnityEngine;

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
        menuManager.ShowSinglePlayerMenu();
    }

    public void OnMultiplayerClicked()
    {
        menuManager.ShowMultiplayerMenu();
    }

    public void OnProfileClicked()
    {
        menuManager.ShowProfileMenu();
    }

    public void OnOptionsClicked()
    {
        menuManager.ShowOptionsMenu();
    }

    public void OnQuitClicked()
    {
        menuManager.QuitGame();
    }
}
