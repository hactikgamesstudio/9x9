using System.Collections.Generic;
using UnityEngine;
using Unity.Template.Multiplayer.NGO.Core.Procedural;

namespace Unity.Template.Multiplayer.NGO.Bots
{
    /// <summary>
    /// Manages bot spawning, lifecycle, and tracking for singleplayer mode.
    /// Spawns bots at corner positions similar to multiplayer spawns.
    /// </summary>
    public class BotManager : MonoBehaviour
    {
        [Header("Bot Configuration")]
        [SerializeField] private GameObject m_BotPrefab;
        [SerializeField] private int m_MaxBots = 7; // 8 total players (1 human + 7 bots)
        [SerializeField] private bool m_SpawnBotsAtStart = true;
        
        [Header("Bot Prefab Settings")]
        [SerializeField] private string m_BotNamePrefix = "Bot";
        
        // Bot tracking
        private List<BotController> m_ActiveBots = new List<BotController>();
        private int m_BotIDCounter = 0;
        
        // Room generator reference
        private RoomGenerator m_RoomGenerator;
        
        public List<BotController> ActiveBots => m_ActiveBots;
        public int ActiveBotCount => m_ActiveBots.Count;
        public int MaxBots => m_MaxBots;
        
        void Awake()
        {
            m_RoomGenerator = FindObjectOfType<RoomGenerator>();
            
            if (m_RoomGenerator == null)
            {
                Debug.LogWarning("BotManager: No RoomGenerator found. Bots may not spawn correctly.");
            }
        }
        
        void Start()
        {
            if (m_SpawnBotsAtStart)
            {
                SpawnBots();
            }
        }
        
        /// <summary>
        /// Spawns bots at corner spawn positions (same as multiplayer spawns).
        /// </summary>
        public void SpawnBots()
        {
            if (m_BotPrefab == null)
            {
                Debug.LogError("BotManager: Bot prefab is not assigned!");
                return;
            }
            
            // Get spawn positions from room generator
            Vector3[] spawnPositions = GetSpawnPositions();
            
            if (spawnPositions.Length == 0)
            {
                Debug.LogError("BotManager: No spawn positions available!");
                return;
            }
            
            // Spawn bots (skip first spawn position for player)
            int botsToSpawn = Mathf.Min(m_MaxBots, spawnPositions.Length - 1);
            
            for (int i = 0; i < botsToSpawn; i++)
            {
                // Use spawn positions starting from index 1 (index 0 is for player)
                Vector3 spawnPosition = spawnPositions[i + 1];
                SpawnBot(spawnPosition);
            }
            
            Debug.Log($"BotManager: Spawned {botsToSpawn} bots.");
        }
        
        /// <summary>
        /// Spawns a single bot at the specified position.
        /// </summary>
        public BotController SpawnBot(Vector3 position)
        {
            GameObject botObject = Instantiate(m_BotPrefab, position, Quaternion.identity);
            botObject.name = $"{m_BotNamePrefix}_{m_BotIDCounter}";
            
            BotController bot = botObject.GetComponent<BotController>();
            
            if (bot == null)
            {
                Debug.LogError("BotManager: Bot prefab does not have BotController component!");
                Destroy(botObject);
                return null;
            }
            
            // Initialize bot
            string botName = $"{m_BotNamePrefix} {m_BotIDCounter + 1}";
            bot.Initialize(botName, m_BotIDCounter, position);
            
            m_ActiveBots.Add(bot);
            m_BotIDCounter++;
            
            Debug.Log($"BotManager: Spawned {botName} at {position}");
            
            return bot;
        }
        
        /// <summary>
        /// Gets spawn positions from RoomGenerator corner rooms.
        /// </summary>
        private Vector3[] GetSpawnPositions()
        {
            if (m_RoomGenerator != null)
            {
                // RoomGenerator has 8 corner spawn positions for 9x9x9 grid
                return m_RoomGenerator.GetSpawnPositions();
            }
            else
            {
                // Fallback: hardcoded positions if RoomGenerator not available
                Debug.LogWarning("BotManager: Using fallback spawn positions.");
                return GetFallbackSpawnPositions();
            }
        }
        
        /// <summary>
        /// Fallback spawn positions for 8 corners of the cube grid.
        /// </summary>
        private Vector3[] GetFallbackSpawnPositions()
        {
            float roomSize = 10f; // Default room size
            int gridSize = 9;
            float halfGrid = (gridSize - 1) * roomSize / 2f;
            
            return new Vector3[]
            {
                new Vector3(-halfGrid, 1f, -halfGrid), // Corner 0,0,0
                new Vector3(halfGrid, 1f, -halfGrid),  // Corner 8,0,0
                new Vector3(-halfGrid, 1f, halfGrid),  // Corner 0,0,8
                new Vector3(halfGrid, 1f, halfGrid),   // Corner 8,0,8
                new Vector3(-halfGrid, halfGrid, -halfGrid), // Corner 0,8,0
                new Vector3(halfGrid, halfGrid, -halfGrid),  // Corner 8,8,0
                new Vector3(-halfGrid, halfGrid, halfGrid),  // Corner 0,8,8
                new Vector3(halfGrid, halfGrid, halfGrid)    // Corner 8,8,8
            };
        }
        
        /// <summary>
        /// Called when a bot dies. Removes from active list.
        /// </summary>
        public void OnBotDied(BotController bot)
        {
            if (m_ActiveBots.Contains(bot))
            {
                m_ActiveBots.Remove(bot);
                Debug.Log($"BotManager: {bot.BotName} removed from active bots. {m_ActiveBots.Count} bots remaining.");
            }
            
            // Check win condition: if all bots dead, player wins
            CheckWinCondition();
        }
        
        /// <summary>
        /// Checks if all bots are eliminated (player wins).
        /// </summary>
        private void CheckWinCondition()
        {
            if (m_ActiveBots.Count == 0)
            {
                Debug.Log("BotManager: All bots eliminated! Player wins!");
                // TODO: Trigger win event for game controller
            }
        }
        
        /// <summary>
        /// Clears all active bots (useful for restarting game).
        /// </summary>
        public void ClearAllBots()
        {
            foreach (var bot in m_ActiveBots)
            {
                if (bot != null)
                {
                    Destroy(bot.gameObject);
                }
            }
            
            m_ActiveBots.Clear();
            m_BotIDCounter = 0;
            
            Debug.Log("BotManager: All bots cleared.");
        }
        
        /// <summary>
        /// Gets bot by ID.
        /// </summary>
        public BotController GetBotByID(int botID)
        {
            foreach (var bot in m_ActiveBots)
            {
                if (bot.BotID == botID)
                {
                    return bot;
                }
            }
            return null;
        }
        
        void OnDestroy()
        {
            ClearAllBots();
        }
    }
}
