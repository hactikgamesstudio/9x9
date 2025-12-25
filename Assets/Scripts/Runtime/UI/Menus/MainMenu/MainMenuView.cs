using UnityEngine;
using UnityEngine.UI;
using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    public class MainMenuView : View<MetagameApplication>
    {
        [SerializeField]
        private MenuManager menuManager;

        [SerializeField]
        private GameObject menuCanvas;

        // Public property for controller access
        internal Button MultiplayerButton => GetMultiplayerButton();

        private Button GetMultiplayerButton()
        {
            // Find the multiplayer button in the imported menu system
            if (menuManager != null && menuManager.MainMenuPanel != null)
            {
                // Look for the MultiplayerButton in the main menu panel
                var buttons = menuManager.MainMenuPanel.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    if (button.name == "MultiplayerButton")
                    {
                        return button;
                    }
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

            if (menuManager == null || menuManager.MainMenuPanel == null)
            {
                return;
            }

            var buttons = menuManager.MainMenuPanel.GetComponentsInChildren<Button>(true);
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
            if (menuManager != null)
            {
                menuManager.ShowMainMenu();
            }
        }

        internal new void Show()
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(true);
            }
            if (menuManager != null)
            {
                menuManager.ShowMainMenu();
            }
        }

        internal new void Hide()
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(false);
            }
        }
    }
}