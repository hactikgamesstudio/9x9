using UnityEngine;
using UnityEngine.UI;
using Unity.Template.Multiplayer.NGO.Core;
using Unity.Template.Multiplayer.NGO.Runtime;

namespace Unity.Template.Multiplayer.NGO.Runtime.Metagame.Views
{
    public class MainMenuView : View<MetagameApplication>
    {
        [SerializeField]
        private MonoBehaviour menuManagerBehaviour;

        [SerializeField]
        private GameObject menuCanvas;

        // Public property for controller access
        public Button MultiplayerButton => GetMultiplayerButton();

        private Button GetMultiplayerButton()
        {
            // Find the multiplayer button in the imported menu system
            var mainMenuPanel = GetMainMenuPanelGameObject();
            if (mainMenuPanel == null)
            {
                return null;
            }

            // Look for the MultiplayerButton in the main menu panel
            var buttons = mainMenuPanel.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button != null && button.name == "MultiplayerButton")
                {
                    return button;
                }
            }
            return null;
        }

        /// <summary>
        /// Disable menu options that are not supported when auto-connect is enabled (dedicated server flow).
        /// Mirrors the old UI Toolkit behavior by making the SinglePlayer and Multiplayer buttons non-interactable.
        /// </summary>
        public void DisableControlsUnsupportedInAutoconnectMode()
        {
            if (CustomNetworkManager.Singleton == null || !CustomNetworkManager.Singleton.AutoConnectOnStartup)
            {
                return;
            }

            var mainMenuPanel = GetMainMenuPanelGameObject();
            if (mainMenuPanel == null)
            {
                return;
            }

            var buttons = mainMenuPanel.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button == null) continue;

                if (button.name == "MultiplayerButton" || button.name == "SinglePlayerButton")
                {
                    button.interactable = false;
                }
            }
        }

        void OnEnable()
        {
            ShowMainMenuViaManager();
        }

        internal new void Show()
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(true);
            }
            ShowMainMenuViaManager();
        }

        internal new void Hide()
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(false);
            }
        }

        IMenuManager MenuManager => menuManagerBehaviour as IMenuManager;

        GameObject GetMainMenuPanelGameObject()
        {
            return MenuManager?.MainMenuPanel;
        }

        void ShowMainMenuViaManager()
        {
            MenuManager?.ShowMainMenu();
        }
    }
}
