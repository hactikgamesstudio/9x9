using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main controller for the 9x9 Cube Maze Game
    /// Handles game initialization, maze generation, player spawning, win/loss conditions
    /// </summary>
    public class CubeGameController : Controller<CubeGameApplication>
    {
        CubeGameModel Model => App.Model;
        CubeGameView View => App.View;

        [Header("Maze Generation")]
        [Tooltip("Reference to the RoomGenerator (will auto-find if not set)")]
        [SerializeField]
        private RoomGenerator m_RoomGenerator;

        [Tooltip("Default room generation config (fills missing prefab references)")]
        [SerializeField]
        private RoomGenConfig m_DefaultRoomGenConfig;

        private bool m_RoomGeneratorCreatedAtRuntime = false; // Tracks if RoomGenerator was created at runtime

        void Awake()
        {
            AddListener<StartMatchEvent>(OnServerStartMatch);
            AddListener<EndMatchEvent>(OnServerMatchEnded);
            AddListener<PlayerDisconnected>(OnServerPlayerDisconnected);
            AddListener<PlayerReachedExitEvent>(OnPlayerReachedExit);
            AddListener<PlayerDiedEvent>(OnPlayerDied);

            // Server-only logic for RoomGenerator initialization
            bool isServer = NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
            if (!isServer)
            {
                return; // Clients don't create or configure RoomGenerator
            }

            // Auto-find RoomGenerator if not assigned
            if (m_RoomGenerator == null)
            {
                m_RoomGenerator = Object.FindFirstObjectByType<RoomGenerator>();

                // If still not found, create a fallback RoomGenerator at runtime (server-only)
                if (m_RoomGenerator == null)
                {
                    var go = new GameObject("RoomGenerator");
                    m_RoomGenerator = go.AddComponent<RoomGenerator>();
                    m_RoomGeneratorCreatedAtRuntime = true;
                    Debug.LogWarning(
                        "[9x9 Server] RoomGenerator not found in scene. Created a runtime instance. "
                            + "Consider adding RoomGenerator to the CubeGameApplication prefab and wiring room templates."
                    );
                }
            }

            // Apply default config if available
            if (m_DefaultRoomGenConfig != null)
            {
                if (m_RoomGeneratorCreatedAtRuntime)
                {
                    // Full apply: set all values from config for runtime-created generator
                    m_RoomGenerator.ApplyConfig(
                        m_DefaultRoomGenConfig,
                        fillOnly: false,
                        applyScalarSettings: true
                    );
                    Debug.Log(
                        "[9x9 Server] Applied full RoomGenConfig to runtime-created RoomGenerator."
                    );
                }
                else
                {
                    // Hybrid apply: only fill missing prefab references, preserve inspector values
                    m_RoomGenerator.ApplyConfig(
                        m_DefaultRoomGenConfig,
                        fillOnly: true,
                        applyScalarSettings: false
                    );
                    Debug.Log("[9x9 Server] Filled missing prefab references from RoomGenConfig.");
                }
            }
            else if (m_RoomGeneratorCreatedAtRuntime)
            {
                Debug.LogWarning(
                    "[9x9 Server] No RoomGenConfig assigned. Runtime-created RoomGenerator has no templates. "
                        + "Assign m_DefaultRoomGenConfig in CubeGameController or add RoomGenerator to the prefab."
                );
            }
        }

        void OnDestroy()
        {
            RemoveListeners();

            // Cleanup any runtime-created generator to avoid leaks across scenes/sessions
            if (m_RoomGeneratorCreatedAtRuntime && m_RoomGenerator != null)
            {
                Destroy(m_RoomGenerator.gameObject);
                m_RoomGenerator = null;
                m_RoomGeneratorCreatedAtRuntime = false;
            }
        }

        internal override void RemoveListeners()
        {
            RemoveListener<StartMatchEvent>(OnServerStartMatch);
            RemoveListener<EndMatchEvent>(OnServerMatchEnded);
            RemoveListener<PlayerDisconnected>(OnServerPlayerDisconnected);
            RemoveListener<PlayerReachedExitEvent>(OnPlayerReachedExit);
            RemoveListener<PlayerDiedEvent>(OnPlayerDied);
        }

        void OnServerPlayerDisconnected(PlayerDisconnected evt)
        {
            Debug.Log($"[9x9 Server] Client {evt.ClientId} disconnected!");

            if (Model.AllowReconnection)
            {
                return; // Player can rejoin
            }

            // If in-game, check if we need to end the match
            if (Model.MatchStarted && !Model.MatchEnded)
            {
                Model.PlayersAlive.Value--;

                // In solo/co-op modes, if all players disconnect, end match
                if (Model.PlayersAlive.Value <= 0)
                {
                    Broadcast(new EndMatchEvent(null)); // No winner
                }
            }
        }

        void OnServerStartMatch(StartMatchEvent evt)
        {
            if (evt.IsServer)
            {
                Debug.Log("[9x9 Server] Starting maze escape game!");
                Model.MatchStarted = true;
                Model.MatchEnded = false;

                StartCoroutine(OnServerInitializeMazeGame());
            }

            if (evt.IsClient)
            {
                Debug.Log("[9x9 Client] Joining maze escape game!");
            }
        }

        /// <summary>
        /// Server-side maze initialization sequence
        /// </summary>
        IEnumerator OnServerInitializeMazeGame()
        {
            // Step 1: Generate the maze
            if (m_RoomGenerator != null && !Model.MazeGenerated)
            {
                Debug.Log("[9x9 Server] Generating 9x9x9 maze...");
                m_RoomGenerator.Generate();
                Model.MazeGenerated = true;

                // Wait for maze generation to complete
                yield return new WaitForSeconds(0.5f);
            }
            else if (m_RoomGenerator == null)
            {
                Debug.LogError("[9x9 Server] RoomGenerator not found! Cannot generate maze.");
                yield break;
            }

            // Step 2: Count connected players
            Model.PlayersAlive.Value = NetworkManager.Singleton.ConnectedClients.Count;
            Debug.Log($"[9x9 Server] {Model.PlayersAlive.Value} players connected");

            // Step 3: Spawn players at corner positions
            SpawnPlayersAtCorners();

            // Step 4: Start game-specific mode logic
            switch (Model.CurrentGameMode)
            {
                case GameMode.BattleRoyale:
                    StartCoroutine(RunBattleRoyaleTimer());
                    break;

                case GameMode.Coop:
                    Debug.Log("[9x9 Server] Co-op mode: All players must reach exit");
                    break;

                default:
                    Debug.Log("[9x9 Server] Standard mode: First to exit wins");
                    break;
            }
        }

        /// <summary>
        /// Spawn players at the 8 corner positions of the cube
        /// </summary>
        void SpawnPlayersAtCorners()
        {
            if (m_RoomGenerator == null)
                return;

            var connectedClients = NetworkManager.Singleton.ConnectedClients.Values.ToList();
            int playerIndex = 0;

            foreach (var client in connectedClients)
            {
                Vector3 spawnPosition = m_RoomGenerator.GetCornerSpawnPosition(playerIndex);

                // Teleport player to spawn position
                Player player = client.PlayerObject.GetComponent<Player>();
                if (player != null)
                {
                    player.transform.position = spawnPosition;
                    Debug.Log(
                        $"[9x9 Server] Spawned player {playerIndex} at corner {spawnPosition}"
                    );
                }

                playerIndex++;
            }
        }

        /// <summary>
        /// Battle Royale mode: Shrinking playable area over time
        /// </summary>
        IEnumerator RunBattleRoyaleTimer()
        {
            Model.MatchTimer.Value = 600; // 10 minutes

            while (Model.MatchTimer.Value > 0 && !Model.MatchEnded)
            {
                yield return CoroutinesHelper.OneSecond;
                Model.MatchTimer.Value--;

                // TODO: Implement shrinking safe zone or hazard escalation
            }

            // Time's up - end match
            if (!Model.MatchEnded)
            {
                var survivors = NetworkManager
                    .Singleton.ConnectedClients.Values.Select(c =>
                        c.PlayerObject.GetComponent<Player>()
                    )
                    .Where(p => p != null && p.IsAlive)
                    .ToList();

                Player winner = survivors.Count > 0 ? survivors.First() : null;
                Broadcast(new EndMatchEvent(winner));
            }
        }

        /// <summary>
        /// Called when a player reaches the exit room
        /// </summary>
        void OnPlayerReachedExit(PlayerReachedExitEvent evt)
        {
            Debug.Log($"[9x9 Server] Player {evt.WinningPlayer.name} reached the exit!");

            switch (Model.CurrentGameMode)
            {
                case GameMode.Coop:
                    // In co-op, all players must reach exit
                    // TODO: Track which players have reached exit
                    Debug.Log("[9x9 Server] Co-op: Player reached exit, waiting for others...");
                    break;

                default:
                    // First player to exit wins
                    Broadcast(new EndMatchEvent(evt.WinningPlayer));
                    break;
            }
        }

        /// <summary>
        /// Called when a player dies
        /// </summary>
        void OnPlayerDied(PlayerDiedEvent evt)
        {
            Model.PlayersAlive.Value--;
            Debug.Log($"[9x9 Server] Player died. {Model.PlayersAlive.Value} players remaining");

            // If all players dead, end match
            if (Model.PlayersAlive.Value <= 0)
            {
                Broadcast(new EndMatchEvent(null)); // No winner
            }
        }

        /// <summary>
        /// Match ended - show results
        /// </summary>
        void OnServerMatchEnded(EndMatchEvent evt)
        {
            Debug.Log(
                $"[9x9 Server] Match ended. Winner: {(evt.Winner != null ? evt.Winner.name : "None")}"
            );
            Model.MatchEnded = true;

            // Update player stats
            if (evt.Winner != null)
            {
                // TODO: Increment wins, kills, etc. in PlayerProfile
                PlayerProfileManager.UpdateStats(true, evt.Winner.Kills, evt.Winner.Deaths);
            }

            // Show victory/defeat screen to all clients
            if (evt.Winner != null)
            {
                View.ShowVictory(evt.Winner);
            }
            else
            {
                View.ShowDefeat();
            }

            // Return to menu after 10 seconds
            StartCoroutine(ReturnToMenuAfterDelay(10f));
        }

        IEnumerator ReturnToMenuAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CustomNetworkManager.Singleton.OnClientDoPostMatchCleanupAndReturnToMetagame();
        }
    }
}
