using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Global inventory system that persists across scenes.
    /// This is a Singleton pattern - only one instance exists in the game.
    /// 
    /// **FOR BEGINNERS - UNDERSTANDING SINGLETONS:**
    /// In Godot, you use AutoLoad to create global scripts accessible anywhere.
    /// In Unity, we use the Singleton pattern to achieve the same thing.
    /// 
    /// How it works:
    /// 1. Create a static Instance variable
    /// 2. In Awake(), check if Instance already exists
    /// 3. If it does, destroy this duplicate
    /// 4. If it doesn't, assign this as the Instance
    /// 5. Use DontDestroyOnLoad() to persist across scenes
    /// 
    /// Access from any script: InventorySystem.Instance.AddItem("health_potion", 1);
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        #region Singleton Pattern
        
        /// <summary>
        /// Static reference to the single instance.
        /// "Static" means it belongs to the class itself, not individual objects.
        /// You can access it from anywhere: InventorySystem.Instance
        /// </summary>
        public static InventorySystem Instance { get; private set; }
        
        /// <summary>
        /// Called when object is created. Enforces singleton pattern.
        /// </summary>
        void Awake()
        {
            // If an instance already exists and it's not this one
            if (Instance != null && Instance != this)
            {
                // Destroy this duplicate
                Destroy(gameObject);
                return;
            }
            
            // Set this as the instance
            Instance = this;
            
            // Prevent this object from being destroyed when loading new scenes
            // In Godot, AutoLoad scripts automatically persist
            DontDestroyOnLoad(gameObject);
        }
        
        #endregion
        
        #region Events (Godot signals)
        
        /// <summary>
        /// Event fired when inventory changes (item added/removed).
        /// In Godot: signal inventory_changed()
        /// In C#: public event System.Action InventoryChanged;
        /// 
        /// Other scripts subscribe like this:
        /// InventorySystem.Instance.InventoryChanged += OnInventoryChanged;
        /// 
        /// And unsubscribe in OnDestroy:
        /// InventorySystem.Instance.InventoryChanged -= OnInventoryChanged;
        /// </summary>
        public event System.Action InventoryChanged;
        
        #endregion
        
        #region Inventory Data
        
        /// <summary>
        /// Dictionary storing item IDs and quantities.
        /// In Godot: var items: Dictionary = {}
        /// In C#: private Dictionary<string, int> m_Items
        /// 
        /// **FOR BEGINNERS - WHAT IS A DICTIONARY?**
        /// Think of it like a real dictionary:
        /// - Key = the word you look up (item_id like "health_potion")
        /// - Value = the definition (quantity like 3)
        /// 
        /// Access: m_Items["health_potion"] returns 3
        /// Add: m_Items["health_potion"] = 5;
        /// Check if exists: m_Items.ContainsKey("health_potion")
        /// </summary>
        private Dictionary<string, int> m_Items = new Dictionary<string, int>();
        
        /// <summary>
        /// Read-only public access to items. External scripts can read but not modify directly.
        /// </summary>
        public IReadOnlyDictionary<string, int> Items => m_Items;
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Adds an item to the inventory.
        /// In Godot: func add_item(item_id: String, quantity: int = 1)
        /// 
        /// **FOR BEGINNERS - OPTIONAL PARAMETERS:**
        /// quantity = 1 means if you don't provide it, it defaults to 1
        /// Example: AddItem("sword") adds 1 sword
        ///          AddItem("arrows", 10) adds 10 arrows
        /// </summary>
        /// <param name="itemId">The unique identifier for the item</param>
        /// <param name="quantity">How many to add (default: 1)</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            // Check if item already exists in inventory
            if (m_Items.ContainsKey(itemId))
            {
                // Add to existing quantity
                m_Items[itemId] += quantity;
            }
            else
            {
                // Add new item with initial quantity
                m_Items[itemId] = quantity;
            }
            
            // Notify subscribers that inventory changed
            // The ? is a "null-conditional operator" - only invokes if not null
            InventoryChanged?.Invoke();
            
            Debug.Log($"Added {quantity}x {itemId}. Total: {m_Items[itemId]}");
        }
        
        /// <summary>
        /// Uses (consumes) one unit of an item.
        /// Returns true if successful, false if item doesn't exist or quantity is 0.
        /// In Godot: func use_item(item_id: String) -> bool
        /// </summary>
        /// <param name="itemId">The item to use</param>
        /// <returns>True if item was used, false if unavailable</returns>
        public bool UseItem(string itemId)
        {
            // Get current count (returns 0 if doesn't exist)
            int count = GetCount(itemId);
            
            if (count > 0)
            {
                m_Items[itemId]--;
                
                // Remove item from dictionary if quantity reaches 0
                if (m_Items[itemId] <= 0)
                {
                    m_Items.Remove(itemId);
                }
                
                InventoryChanged?.Invoke();
                Debug.Log($"Used {itemId}. Remaining: {GetCount(itemId)}");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the quantity of a specific item.
        /// Returns 0 if item doesn't exist.
        /// In Godot: func get_count(item_id: String) -> int
        /// </summary>
        /// <param name="itemId">The item to check</param>
        /// <returns>Quantity of the item (0 if not in inventory)</returns>
        public int GetCount(string itemId)
        {
            // TryGetValue is safer than direct access with m_Items[itemId]
            // It doesn't throw an error if the key doesn't exist
            if (m_Items.TryGetValue(itemId, out int count))
            {
                return count;
            }
            return 0;
        }
        
        /// <summary>
        /// Checks if inventory contains at least one of the specified item.
        /// </summary>
        /// <param name="itemId">The item to check</param>
        /// <returns>True if item exists in inventory</returns>
        public bool HasItem(string itemId)
        {
            return GetCount(itemId) > 0;
        }
        
        /// <summary>
        /// Removes all items from inventory.
        /// In Godot: func clear()
        /// </summary>
        public void Clear()
        {
            m_Items.Clear();
            InventoryChanged?.Invoke();
            Debug.Log("Inventory cleared");
        }
        
        /// <summary>
        /// Removes a specific item completely from inventory.
        /// </summary>
        /// <param name="itemId">The item to remove</param>
        public void RemoveItem(string itemId)
        {
            if (m_Items.Remove(itemId))
            {
                InventoryChanged?.Invoke();
                Debug.Log($"Removed all {itemId} from inventory");
            }
        }
        
        #endregion
        
        #region Debug Helpers
        
        /// <summary>
        /// Prints all inventory items to console. Useful for debugging.
        /// </summary>
        [ContextMenu("Print Inventory")]
        public void PrintInventory()
        {
            Debug.Log("=== INVENTORY ===");
            if (m_Items.Count == 0)
            {
                Debug.Log("(Empty)");
            }
            else
            {
                foreach (var kvp in m_Items)
                {
                    Debug.Log($"{kvp.Key}: x{kvp.Value}");
                }
            }
            Debug.Log("=================");
        }
        
        #endregion
    }
}
