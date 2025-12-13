using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    /// <summary>
    /// Central menu manager that handles navigation between menu panels.
    /// Manages state transitions and ensures only one menu panel is active at a time.
    /// </summary>
    public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private MainMenuPanel mainMenuPanel;

    [SerializeField]
    private SinglePlayerMenuPanel singlePlayerMenuPanel;

    [SerializeField]
    private NewGamePanel newGamePanel;

    // Public property to access main menu panel
    public MainMenuPanel MainMenuPanel => mainMenuPanel;

    private UIMenuPanel currentPanel;

    private void Start()
    {
        Debug.Log("MenuManager started");
        // Initialize with main menu
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        Debug.Log("ShowMainMenu called");
        if (mainMenuPanel == null)
        {
            Debug.LogError("Main Menu Panel is NULL - not assigned in MenuManager!");
            return;
        }
        HideAllPanels();
        mainMenuPanel.gameObject.SetActive(true);
        Debug.Log($"Main Menu Panel activated: {mainMenuPanel.gameObject.name}");
        currentPanel = mainMenuPanel;
    }

    public void ShowSinglePlayerMenu()
    {
        HideAllPanels();
        singlePlayerMenuPanel.gameObject.SetActive(true);
        currentPanel = singlePlayerMenuPanel;
    }

    public void ShowNewGameMenu()
    {
        HideAllPanels();
        newGamePanel.gameObject.SetActive(true);
        currentPanel = newGamePanel;
    }

    public void ShowMultiplayerMenu()
    {
        Debug.Log("Multiplayer menu selected - not yet implemented");
    }

    public void ShowProfileMenu()
    {
        Debug.Log("Profile menu selected - not yet implemented");
    }

    public void ShowOptionsMenu()
    {
        Debug.Log("Options menu selected - not yet implemented");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HideAllPanels()
    {
        Debug.Log("HideAllPanels called");
        mainMenuPanel.gameObject.SetActive(false);
        singlePlayerMenuPanel.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(false);
        Debug.Log("All panels hidden");
    }
}
}
