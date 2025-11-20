using Unity.Template.Multiplayer.NGO.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main View for the 9x9 Cube Maze Game
    /// Manages all UI elements during gameplay
    /// </summary>
    public class CubeGameView : View<CubeGameApplication>
    {
        [Header("In-Game UI")]
        [Tooltip("HUD showing health, inventory, minimap, etc.")]
        [SerializeField]
        private HUDController m_HUD;

        [Header("End Game UI")]
        [Tooltip("Victory/Defeat screen")]
        [SerializeField]
        private MatchRecapView m_MatchRecap;

        internal HUDController HUD => m_HUD;
        internal MatchRecapView MatchRecap => m_MatchRecap;

        void Awake()
        {
            if (App.IsDedicatedServer)
            {
                OnDedicatedServerDestroyViews();
            }
            else
            {
                // Show HUD on game start
                if (m_HUD != null)
                {
                    m_HUD.gameObject.SetActive(true);
                }

                // Hide match recap until game ends
                if (m_MatchRecap != null)
                {
                    m_MatchRecap.Hide();
                }
            }
        }

        void OnDedicatedServerDestroyViews()
        {
            // Server doesn't need UI
            Destroy(gameObject);
        }

        /// <summary>
        /// Show the victory screen
        /// </summary>
        internal void ShowVictory(Player winner)
        {
            if (m_HUD != null)
            {
                m_HUD.gameObject.SetActive(false);
            }

            if (m_MatchRecap != null)
            {
                m_MatchRecap.Show();
                // Update result label with winner info
                var uiDoc = m_MatchRecap.GetComponent<UIDocument>();
                if (uiDoc != null)
                {
                    var resultLabel = uiDoc.rootVisualElement.Q<Label>("resultLabel");
                    if (resultLabel != null)
                    {
                        resultLabel.text =
                            winner != null ? $"Victory! {winner.name} won!" : "Match Over";
                    }
                }
            }

            Debug.Log(
                $"[9x9] Victory screen shown for player: {(winner != null ? winner.name : "None")}"
            );
        }

        /// <summary>
        /// Show the defeat screen
        /// </summary>
        internal void ShowDefeat()
        {
            if (m_HUD != null)
            {
                m_HUD.gameObject.SetActive(false);
            }

            if (m_MatchRecap != null)
            {
                m_MatchRecap.Show();
                // Update result label for defeat
                var uiDoc = m_MatchRecap.GetComponent<UIDocument>();
                if (uiDoc != null)
                {
                    var resultLabel = uiDoc.rootVisualElement.Q<Label>("resultLabel");
                    if (resultLabel != null)
                    {
                        resultLabel.text = "Defeat - All players died";
                    }
                }
            }

            Debug.Log("[9x9] Defeat screen shown");
        }
    }
}
