using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Global inventory system that tracks all items the player carries.
    /// Singleton pattern ensures only one instance exists across scenes.
    /// Fires events when inventory changes so UI and other systems can react.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }
        
        /// <summary>
        /// Event fired whenever inventory changes (item added, removed, or cleared)
        /// </summary>
        public event Action InventoryChanged;
        
        /// <summary>
        /// Dictionary of all items player currently has.
        /// Key: Item ID (e.g., "health_potion")
        /// Value: Quantity of that item
        /// </summary>
        private Dictionary<string, int> m_Items = new Dictionary<string, int>();
        
        /// <summary>
        /// Public accessor for debugging/inspection
        /// </summary>
        public IReadOnlyDictionary<string, int> Items => m_Items;

        void Awake()
        {
            // Singleton pattern: only allow one instance
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[InventorySystem] Duplicate instance detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Persist across scene loads
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("[InventorySystem] Initialized as singleton");
        }

        /// <summary>
        /// Add item(s) to inventory
        /// </summary>
        /// <param name="itemId">Unique item identifier (e.g., "health_potion")</param>
        /// <param name="quantity">How many to add (default 1)</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogError("[InventorySystem] Cannot add item with null or empty ID");
                return;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning("[InventorySystem] Cannot add 0 or negative quantity");
                return;
            }

            // Add to dictionary (or increment if already exists)
            if (m_Items.ContainsKey(itemId))
            {
                m_Items[itemId] += quantity;
            }
            else
            {
                m_Items[itemId] = quantity;
            }

            Debug.Log($"[InventorySystem] Added {quantity}x {itemId} (total: {m_Items[itemId]})");
            InventoryChanged?.Invoke();
        }

        /// <summary>
        /// Remove item(s) from inventory
        /// </summary>
        /// <param name="itemId">Unique item identifier</param>
        /// <param name="quantity">How many to remove (default 1)</param>
        /// <returns>True if successful, false if item not found or insufficient quantity</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogError("[InventorySystem] Cannot remove item with null or empty ID");
                return false;
            }

            if (!m_Items.ContainsKey(itemId))
            {
                Debug.LogWarning($"[InventorySystem] Item '{itemId}' not found in inventory");
                return false;
            }

            if (m_Items[itemId] < quantity)
            {
                Debug.LogWarning($"[InventorySystem] Not enough {itemId} to remove (have {m_Items[itemId]}, trying to remove {quantity})");
                return false;
            }

            m_Items[itemId] -= quantity;

            // Remove entry if count reaches 0
            if (m_Items[itemId] <= 0)
            {
                m_Items.Remove(itemId);
                Debug.Log($"[InventorySystem] Removed last {itemId}");
            }
            else
            {
                Debug.Log($"[InventorySystem] Removed {quantity}x {itemId} (remaining: {m_Items[itemId]})");
            }

            InventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Check if player has item(s)
        /// </summary>
        /// <param name="itemId">Unique item identifier</param>
        /// <param name="quantity">Minimum quantity needed (default 1)</param>
        /// <returns>True if player has at least that many, false otherwise</returns>
        public bool HasItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            return m_Items.ContainsKey(itemId) && m_Items[itemId] >= quantity;
        }

        /// <summary>
        /// Get count of specific item
        /// </summary>
        /// <param name="itemId">Unique item identifier</param>
        /// <returns>Number of items, or 0 if not found</returns>
        public int GetItemCount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return 0;
            }

            return m_Items.ContainsKey(itemId) ? m_Items[itemId] : 0;
        }

        /// <summary>
        /// Clear all items from inventory
        /// </summary>
        public void Clear()
        {
            if (m_Items.Count == 0)
            {
                return;
            }

            m_Items.Clear();
            Debug.Log("[InventorySystem] Cleared all items");
            InventoryChanged?.Invoke();
        }

        /// <summary>
        /// Get total number of unique item types
        /// </summary>
        /// <returns>Count of different items (not total quantity)</returns>
        public int GetUniqueItemCount()
        {
            return m_Items.Count;
        }

        /// <summary>
        /// Debug output of current inventory
        /// </summary>
        public void LogInventory()
        {
            if (m_Items.Count == 0)
            {
                Debug.Log("[InventorySystem] Inventory is empty");
                return;
            }

            string itemList = string.Empty;
            foreach (var kvp in m_Items)
            {
                itemList += $"\n  - {kvp.Key}: {kvp.Value}";
            }

            Debug.Log($"[InventorySystem] Current inventory ({m_Items.Count} types):{itemList}");
        }
    }
}
