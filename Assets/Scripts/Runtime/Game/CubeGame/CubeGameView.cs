using Unity.Template.Multiplayer.NGO.Core;
using Unity.Template.Multiplayer.NGO.Runtime;
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
        private MonoBehaviour m_HUD;

        [Header("End Game UI")]
        [Tooltip("Victory/Defeat screen")]
        [SerializeField]
        private MonoBehaviour m_MatchRecap;

        internal MonoBehaviour HUD => m_HUD;
        internal MonoBehaviour MatchRecap => m_MatchRecap;

        void Awake()
        {
            if (App.IsDedicatedServer)
            {
                OnDedicatedServerDestroyViews();
            }
            else
            {
                // Ensure HUD exists; try reflection first, then create runtime fallback
                if (m_HUD == null)
                {
                    var hudType = System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.UI.HUD.HUDController");
                    if (hudType != null)
                        m_HUD = (MonoBehaviour)Object.FindAnyObjectByType(hudType);
                    
                    if (m_HUD == null)
                        CreateRuntimeHUD();
                }
                // Show HUD on game start
                if (m_HUD != null)
                {
                    m_HUD.gameObject.SetActive(true);
                }

                // Ensure Match Recap exists; try reflection first, then create runtime fallback
                if (m_MatchRecap == null)
                {
                    var recapType = System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.UI.HUD.MatchRecapView");
                    if (recapType != null)
                        m_MatchRecap = (MonoBehaviour)Object.FindAnyObjectByType(recapType);
                    
                    if (m_MatchRecap == null)
                        CreateRuntimeMatchRecap();
                }
                // Hide match recap until game ends
                if (m_MatchRecap != null)
                {
                    var hideMethod = m_MatchRecap.GetType().GetMethod("Hide");
                    if (hideMethod != null)
                        hideMethod.Invoke(m_MatchRecap, null);
                    else
                        m_MatchRecap.gameObject.SetActive(false);
                }
            }
        }

        void CreateRuntimeHUD()
        {
            var go = new GameObject("HUDCanvas_Runtime");
            var uiDoc = go.AddComponent<UIDocument>();
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            root.style.paddingLeft = 10;
            root.style.paddingTop = 10;
            root.style.width = new Length(100, LengthUnit.Percent);
            root.style.height = new Length(100, LengthUnit.Percent);

            var health = new Label("Health: 100");
            health.name = "healthLabel";
            health.style.fontSize = 16;
            health.style.color = Color.white;
            root.Add(health);

            var inventory = new Label("Inventory: (empty)");
            inventory.name = "inventoryLabel";
            inventory.style.fontSize = 14;
            inventory.style.color = Color.white;
            root.Add(inventory);

            uiDoc.rootVisualElement.Add(root);

            // Attach a lightweight HUDController if available via reflection
            var hudType = System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.UI.HUD.HUDController");
            if (hudType != null)
            {
                m_HUD = (MonoBehaviour)go.AddComponent(hudType);
            }
        }

        void CreateRuntimeMatchRecap()
        {
            var go = new GameObject("MatchRecapCanvas_Runtime");
            var uiDoc = go.AddComponent<UIDocument>();
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            root.style.alignItems = Align.Center;
            root.style.justifyContent = Justify.Center;
            root.style.width = new Length(100, LengthUnit.Percent);
            root.style.height = new Length(100, LengthUnit.Percent);

            var result = new Label("Match Over");
            result.name = "resultLabel";
            result.style.fontSize = 24;
            result.style.color = Color.white;
            root.Add(result);

            var backBtn = new Button(() =>
            {
                // Return to metagame via App broadcast
                App.Broadcast(new EndMatchEvent(null));
            })
            {
                text = "Return to Menu",
            };
            backBtn.name = "continueButton";
            backBtn.style.marginTop = 10;
            root.Add(backBtn);

            uiDoc.rootVisualElement.Add(root);

            // Attach a lightweight MatchRecapView if available via reflection
            var recapType = System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.MatchRecapView");
            if (recapType != null)
            {
                m_MatchRecap = (MonoBehaviour)go.AddComponent(recapType);
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
                try
                {
                    var showMethod = m_MatchRecap.GetType().GetMethod("Show");
                    if (showMethod != null)
                        showMethod.Invoke(m_MatchRecap, null);
                    else
                        m_MatchRecap.gameObject.SetActive(true);
                }
                catch { }

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

            UnityEngine.Debug.Log(
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
                try
                {
                    var showMethod = m_MatchRecap.GetType().GetMethod("Show");
                    if (showMethod != null)
                        showMethod.Invoke(m_MatchRecap, null);
                    else
                        m_MatchRecap.gameObject.SetActive(true);
                }
                catch { }

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

            UnityEngine.Debug.Log("[9x9] Defeat screen shown");
        }
    }
}
