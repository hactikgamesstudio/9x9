using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Manages bot spawning, lifecycle, and cleanup for singleplayer sessions
    /// Handles bot AI updates and coordination with game state
    /// </summary>
    public class BotManager : MonoBehaviour
    {
        [Header("Bot Configuration")]
        [SerializeField]
        private Player m_BotPrefab;

        [SerializeField]
        private int m_MaxBots = 3;

        [SerializeField]
        private float m_SpawnDelay = 0.5f;

        [Header("Spawn Positions")]
        [SerializeField]
        private Vector3[] m_SpawnPositions = new Vector3[8]; // 8 corners of cube

        // Active bots
        private List<Player> m_ActiveBots = new List<Player>();
        private bool m_IsSpawning = false;

        void Awake()
        {
            if (m_BotPrefab == null)
                Debug.LogError("[BotManager] Bot prefab not assigned!");

            // Initialize spawn positions at cube corners if not set
            if (m_SpawnPositions.Length == 0 || m_SpawnPositions[0] == Vector3.zero)
            {
                InitializeDefaultSpawnPositions();
            }
        }

        /// <summary>
        /// Initialize default spawn positions at 9x9x9 cube corners
        /// </summary>
        void InitializeDefaultSpawnPositions()
        {
            float size = 8f * 10f; // 8 rooms * 10 units per room
            m_SpawnPositions = new Vector3[]
            {
                new Vector3(0, 2, 0),           // (0,0,0)
                new Vector3(size, 2, 0),       // (8,0,0)
                new Vector3(0, 2, size),       // (0,0,8)
                new Vector3(size, 2, size),    // (8,0,8)
                new Vector3(size / 2, 2, 0),   // Mid edge 1
                new Vector3(0, 2, size / 2),   // Mid edge 2
                new Vector3(size, 2, size / 2), // Mid edge 3
                new Vector3(size / 2, 2, size), // Mid edge 4
            };
        }

        /// <summary>
        /// Spawn a specified number of bots
        /// </summary>
        public void SpawnBots(int botCount)
        {
            if (m_IsSpawning)
                return;

            botCount = Mathf.Min(botCount, m_MaxBots);
            botCount = Mathf.Min(botCount, m_SpawnPositions.Length);

            StartCoroutine(SpawnBotsCoroutine(botCount));
        }

        /// <summary>
        /// Spawn bots with delay between each
        /// </summary>
        private System.Collections.IEnumerator SpawnBotsCoroutine(int botCount)
        {
            m_IsSpawning = true;

            for (int i = 0; i < botCount; i++)
            {
                SpawnSingleBot(i);
                yield return new WaitForSeconds(m_SpawnDelay);
            }

            m_IsSpawning = false;
            Debug.Log($"[BotManager] Spawned {botCount} bots");
        }

        /// <summary>
        /// Spawn a single bot at a specific spawn index
        /// </summary>
        void SpawnSingleBot(int spawnIndex)
        {
            if (m_BotPrefab == null || spawnIndex >= m_SpawnPositions.Length)
                return;

            // Instantiate bot at spawn position
            Player botInstance = Instantiate(
                m_BotPrefab,
                m_SpawnPositions[spawnIndex],
                Quaternion.identity
            );

            if (botInstance == null)
            {
                Debug.LogError("[BotManager] Failed to instantiate bot!");
                return;
            }

            // Setup bot
            botInstance.gameObject.name = $"Bot_{spawnIndex}";

            // Add BotController component
            BotController botController = botInstance.gameObject.AddComponent<BotController>();
            if (botController == null)
            {
                Debug.LogError("[BotManager] Failed to add BotController component!");
                Destroy(botInstance.gameObject);
                return;
            }

            // Track this bot
            m_ActiveBots.Add(botInstance);

            Debug.Log($"[BotManager] Spawned bot '{botInstance.gameObject.name}' at position {m_SpawnPositions[spawnIndex]}");
        }

        /// <summary>
        /// Remove all active bots
        /// </summary>
        public void DespawnAllBots()
        {
            foreach (Player bot in m_ActiveBots)
            {
                if (bot != null)
                {
                    Destroy(bot.gameObject);
                }
            }

            m_ActiveBots.Clear();
            Debug.Log("[BotManager] All bots despawned");
        }

        /// <summary>
        /// Get list of active bots
        /// </summary>
        public List<Player> GetActiveBots() => new List<Player>(m_ActiveBots);

        /// <summary>
        /// Get bot count
        /// </summary>
        public int GetBotCount() => m_ActiveBots.Count;

        /// <summary>
        /// Eliminate a specific bot
        /// </summary>
        public void EliminateBot(Player bot)
        {
            if (m_ActiveBots.Contains(bot))
            {
                m_ActiveBots.Remove(bot);
                Destroy(bot.gameObject);
                Debug.Log($"[BotManager] Bot eliminated: {bot.gameObject.name}");
            }
        }

        void OnDestroy()
        {
            DespawnAllBots();
        }
    }
}
