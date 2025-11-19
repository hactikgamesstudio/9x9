using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// HUD controller for displaying player health and inventory.
    ///
    /// **FOR BEGINNERS - UNITY UI SYSTEMS:**
    /// Unity has three UI systems (confusing, I know!):
    ///
    /// 1. IMGUI (old, code-only, for editor tools)
    /// 2. uGUI (Unity UI) - Most common, uses Canvas + UI components
    /// 3. UI Toolkit (new, HTML/CSS-like, for modern projects)
    ///
    /// This script uses uGUI (Unity UI) because it's the most beginner-friendly.
    ///
    /// **KEY CONCEPTS:**
    /// - Canvas: Root UI container (required for all UI)
    /// - Image: Displays sprites/textures
    /// - Text / TextMeshPro: Displays text
    /// - Slider: Progress bar (we use for health bar)
    /// - Layout Groups: Auto-arrange child UI elements
    ///
    /// **COMPARISON TO GODOT:**
    /// Godot: UI Toolkit with Control nodes (Label, TextureProgress, etc.)
    /// Unity: uGUI with Canvas and UI components (Text, Image, Slider, etc.)
    /// Both work similarly but different naming!
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Health Display")]
        [Tooltip("Slider component for health bar (uses Slider.value)")]
        [SerializeField]
        private Slider m_HealthBar;

        [Tooltip("Text showing health numbers (e.g., '75/100')")]
        [SerializeField]
        private TextMeshProUGUI m_HealthText;

        [Tooltip("Image component that fills based on health (alternative to Slider)")]
        [SerializeField]
        private Image m_HealthFillImage;

        [Header("Inventory Hotbar")]
        [Tooltip("Container holding inventory slot texts (e.g., GridLayoutGroup)")]
        [SerializeField]
        private Transform m_SlotsContainer;

        [Tooltip("How many inventory slots to display (max 10 recommended)")]
        [SerializeField]
        private int m_MaxSlots = 5;

        [Header("Crosshair")]
        [Tooltip("Crosshair image (center of screen)")]
        [SerializeField]
        private Image m_Crosshair;

        [Header("References")]
        [Tooltip("Reference to player (auto-finds if null)")]
        [SerializeField]
        private FirstPersonController m_Player;
        
        #endregion
        
        #region Private Variables
        
        /// <summary>
        /// Array of UI text components for each inventory slot.
        /// Dynamically created or found from m_SlotsContainer children.
        /// </summary>
        private TextMeshProUGUI[] m_SlotTexts;
        
        #endregion
        
        #region Unity Lifecycle
        
        /// <summary>
        /// Initialize HUD. Find player if not assigned, subscribe to events.
        ///
        /// **FOR BEGINNERS - FINDING OBJECTS:**
        /// - GameObject.FindGameObjectWithTag("Player") finds object by tag
        /// - FindObjectOfType<T>() finds first object with component T
        /// - Assigning in Inspector is faster/safer than searching
        /// </summary>
        void Start()
        {
            // Find player if not assigned
            if (m_Player == null)
            {
                // Updated to modern API: FindFirstObjectByType is preferred over deprecated FindObjectOfType
                m_Player = Object.FindFirstObjectByType<FirstPersonController>();
                if (m_Player == null)
                {
                    Debug.LogError("HUDController: Could not find FirstPersonController in scene!");
                }
            }
            
            // Subscribe to player health changes
            // **FOR BEGINNERS - EVENT SUBSCRIPTION:**
            // This is like Godot's: player.health_changed.connect(_on_health_changed)
            // In C#: player.HealthChanged += OnHealthChanged;
            if (m_Player != null)
            {
                m_Player.HealthChanged += OnHealthChanged;
                
                // Set initial health display
                UpdateHealthDisplay(m_Player.Health, m_Player.MaxHealth);
            }
            
            // Subscribe to inventory changes
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.InventoryChanged += OnInventoryChanged;
            }
            
            // Setup inventory slots
            SetupInventorySlots();
            
            // Initial inventory update
            OnInventoryChanged();
        }
        
        /// <summary>
        /// Cleanup: Unsubscribe from events to prevent memory leaks.
        ///
        /// **FOR BEGINNERS - WHY UNSUBSCRIBE?**
        /// If you don't unsubscribe:
        /// 1. HUD gets destroyed (scene change, player dies, etc.)
        /// 2. Player event still tries to call HUD's method
        /// 3. Error! NullReferenceException or worse
        /// 
        /// Always unsubscribe in OnDestroy() to prevent this.
        /// </summary>
        void OnDestroy()
        {
            if (m_Player != null)
            {
                m_Player.HealthChanged -= OnHealthChanged;
            }
            
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.InventoryChanged -= OnInventoryChanged;
            }
        }
        
        #endregion
        
        #region Health Display
        
        /// <summary>
        /// Called when player health changes.
        /// Updates health bar, fill image, and text.
        /// </summary>
        /// <param name="currentHealth">New health value</param>
        void OnHealthChanged(int currentHealth)
        {
            if (m_Player != null)
            {
                UpdateHealthDisplay(currentHealth, m_Player.MaxHealth);
            }
        }
        
        /// <summary>
        /// Updates all health UI elements.
        ///
        /// **FOR BEGINNERS - UI COMPONENTS:**
        /// - Slider.value: 0 to Slider.maxValue (we use 0 to 1, then multiply by max health)
        /// - Image.fillAmount: 0 to 1 (percentage filled)
        /// - TextMeshPro.text: String to display
        /// </summary>
        void UpdateHealthDisplay(int current, int max)
        {
            float healthPercent = (float)current / max;
            
            // Update slider
            if (m_HealthBar != null)
            {
                m_HealthBar.maxValue = max;
                m_HealthBar.value = current;
            }
            
            // Update fill image
            if (m_HealthFillImage != null)
            {
                m_HealthFillImage.fillAmount = healthPercent;
            }
            
            // Update text
            if (m_HealthText != null)
            {
                m_HealthText.text = $"{current} / {max}";
            }
        }
        
        #endregion
        
        #region Inventory Display
        
        /// <summary>
        /// Creates or finds inventory slot UI elements.
        /// </summary>
        void SetupInventorySlots()
        {
            if (m_SlotsContainer == null)
            {
                Debug.LogWarning(
                    "HUDController: Slots container not assigned. Inventory won't display.");
                return;
            }
            
            // Get existing slot texts from children
            TextMeshProUGUI[] existingSlots = m_SlotsContainer.GetComponentsInChildren<TextMeshProUGUI>();
            
            if (existingSlots.Length >= m_MaxSlots)
            {
                // Use existing slots
                m_SlotTexts = new TextMeshProUGUI[m_MaxSlots];
                for (int i = 0; i < m_MaxSlots; i++)
                {
                    m_SlotTexts[i] = existingSlots[i];
                    m_SlotTexts[i].text = ""; // Clear initial text
                }
            }
            else
            {
                Debug.LogWarning(
                    $"HUDController: Need {m_MaxSlots} slot texts, but only found {existingSlots.Length}. " +
                    "Create TextMeshPro children under the slots container.");
                m_SlotTexts = existingSlots;
            }
        }
        
        /// <summary>
        /// Called when inventory changes. Updates hotbar display.
        /// In Godot this was: func _on_inventory_changed()
        /// </summary>
        void OnInventoryChanged()
        {
            if (m_SlotTexts == null || m_SlotTexts.Length == 0)
            {
                return;
            }
            
            if (InventorySystem.Instance == null)
            {
                return;
            }
            
            // Get items from inventory
            var items = InventorySystem.Instance.Items;
            
            int slotIndex = 0;
            
            // Fill slots with item data
            foreach (var kvp in items)
            {
                if (slotIndex >= m_SlotTexts.Length)
                {
                    break; // No more slots to fill
                }
                
                string itemId = kvp.Key;
                int count = kvp.Value;
                
                // Format: "health_potion x3"
                m_SlotTexts[slotIndex].text = $"{itemId} x{count}";
                slotIndex++;
            }
            
            // Clear remaining slots
            while (slotIndex < m_SlotTexts.Length)
            {
                m_SlotTexts[slotIndex].text = "";
                slotIndex++;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Manually set health (alternative to event subscription).
        /// Kept for compatibility with Godot version.
        /// </summary>
        public void SetHealth(int value)
        {
            if (m_Player != null)
            {
                UpdateHealthDisplay(value, m_Player.MaxHealth);
            }
        }
        
        /// <summary>
        /// Shows or hides crosshair.
        /// </summary>
        public void SetCrosshairVisible(bool visible)
        {
            if (m_Crosshair != null)
            {
                m_Crosshair.enabled = visible;
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        /// <summary>
        /// Validates setup in Inspector. Warns about missing references.
        ///
        /// **FOR BEGINNERS - ONVALIDATE():**
        /// Called when you change values in the Inspector.
        /// Use it to validate setup and provide helpful warnings.
        /// Makes debugging easier during level design!
        /// </summary>
        void OnValidate()
        {
            // Ensure max slots is reasonable
            m_MaxSlots = Mathf.Clamp(m_MaxSlots, 1, 10);
            
            // Warn about missing components
            if (m_HealthBar == null && m_HealthFillImage == null)
            {
                Debug.LogWarning("HUDController: No health display assigned (Slider or Fill Image).");
            }
        }
        
        #endregion
    }
}
