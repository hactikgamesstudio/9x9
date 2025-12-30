using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Core
{
    /// <summary>
    /// Interface for menu manager to avoid circular assembly dependencies.
    /// Implemented by UI.MenuManager, consumed by Metagame.MainMenuView.
    /// </summary>
    public interface IMenuManager
    {
        /// <summary>
        /// Gets the main menu panel GameObject.
        /// </summary>
        GameObject MainMenuPanel { get; }

        /// <summary>
        /// Shows the main menu.
        /// </summary>
        void ShowMainMenu();
    }
}
