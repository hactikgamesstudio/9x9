using System.Collections;
using System.Linq;
using Unity.Netcode;
using Unity.Template.Multiplayer.NGO.Core;
using Unity.Template.Multiplayer.NGO.Runtime;
using UnityEngine;
using NGOFirstPersonController = Unity.Template.Multiplayer.NGO.Runtime.FirstPersonController;
// Explicitly alias event types to avoid conflicts with duplicate definitions in 'Game' assembly
using NGOPlayerDiedEvent = Unity.Template.Multiplayer.NGO.Runtime.PlayerDiedEvent;
using NGOPlayerReachedExitEvent = Unity.Template.Multiplayer.NGO.Runtime.PlayerReachedExitEvent;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main controller for the 9x9 Cube Maze Game
    /// Handles game initialization, maze generation, player spawning, win/loss conditions
    /// </summary>
    public class CubeGameController : Controller<CubeGameApplication>
    {
        // Quick access to MVC components
        CubeGameModel Model => App.Model;
        CubeGameView View => App.View;

        [Header("Maze Generation")]
        [Tooltip("Reference to the RoomGenerator (will auto-find if not set)")]
        [SerializeField]
        private MonoBehaviour m_RoomGenerator;

        [Tooltip("Default room generation config (fills missing prefab references)")]
        [SerializeField]
        private ScriptableObject m_DefaultRoomGenConfig;

        [Header("Mode-Specific Generation Profiles")]
        [Tooltip("Profile for Story mode (deterministic seed, authored constraints)")]
        [SerializeField]
        private RoomGenProfile m_StoryModeProfile;

        [Tooltip("Profile for Co-op mode (deterministic seed, co-op constraints)")]
        [SerializeField]
        private RoomGenProfile m_CoopModeProfile;

        [Tooltip("Profile for Battle Royale (random seed, 9×9×9)")]
        [SerializeField]
        private RoomGenProfile m_BattleRoyaleProfile;

        [Tooltip("Profile for 5×5 mode (random seed, smaller grid)")]
        [SerializeField]
        private RoomGenProfile m_5x5Profile;

        [Tooltip("Profile for 3×3 mode (random seed, smallest grid)")]
        [SerializeField]
        private RoomGenProfile m_3x3Profile;

        private bool m_RoomGeneratorCreatedAtRuntime = false; // Tracks if RoomGenerator was created at runtime

        [Header("Offline Singleplayer")]
        [Tooltip(
            "Prefab containing a FirstPersonController + Camera for offline singleplayer fallback when no Netcode clients exist."
        )]
        [SerializeField]
        private GameObject m_OfflinePlayerPrefab;

        [Header("Bot System")]
        [SerializeField]
        private Player m_BotPrefab;

        [SerializeField]
        private int m_MaxBots = 3;

        private BotManager m_BotManager;

        void Awake()
        {
            // Subscribe to game lifecycle and player events
            AddListener<StartMatchEvent>(OnServerStartMatch);
            AddListener<EndMatchEvent>(OnServerMatchEnded);
            AddListener<PlayerDisconnected>(OnServerPlayerDisconnected);
            AddListener<NGOPlayerReachedExitEvent>(OnPlayerReachedExit);
            AddListener<NGOPlayerDiedEvent>(OnPlayerDied);
            AddListener<AllBotsEliminatedEvent>(OnAllBotsEliminated);

            // Treat offline singleplayer as 'server' for logic that usually runs on server
            // This allows testing and playing without network connection
            bool isServerOrOffline =
                (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
                || (
                    CustomNetworkManager.Singleton != null
                    && !CustomNetworkManager.Singleton.IsClient
                    && !CustomNetworkManager.Singleton.IsServer
                );
            if (!isServerOrOffline)
            {
                return; // Early exit for network clients (server handles generation)
            }

            // Auto-find RoomGenerator in scene if not manually assigned
            if (m_RoomGenerator == null)
            {
                // Use reflection to find RoomGenerator without hard type dependency
                var roomGenType =
                    System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.RoomGenerator");
                if (roomGenType != null)
                {
                    m_RoomGenerator = (MonoBehaviour)Object.FindAnyObjectByType(roomGenType);
                }

                // If still not found, create a fallback RoomGenerator at runtime (server-only)
                if (m_RoomGenerator == null && roomGenType != null)
                {
                    // Create fallback RoomGenerator if none exists in scene
                    var go = new GameObject("RoomGenerator");
                    m_RoomGenerator = (MonoBehaviour)go.AddComponent(roomGenType);
                    m_RoomGeneratorCreatedAtRuntime = true;
                    UnityEngine.Debug.LogWarning(
                        "[9x9 Server] RoomGenerator not found in scene. Created a runtime instance. "
                            + "Consider adding RoomGenerator to the CubeGameApplication prefab and wiring room templates."
                    );
                }
            }

            // Apply default config if available (advanced generator only)
            // Use reflection to support different RoomGenerator variants without hard dependencies
            if (m_DefaultRoomGenConfig != null)
            {
                var applyConfigMethod = m_RoomGenerator.GetType().GetMethod("ApplyConfig");
                if (applyConfigMethod != null)
                {
                    // If generator was created at runtime, apply full config (all settings)
                    if (m_RoomGeneratorCreatedAtRuntime)
                    {
                        _ = applyConfigMethod.Invoke(
                            m_RoomGenerator,
                            new object[] { m_DefaultRoomGenConfig, false, true }
                        );
                        UnityEngine.Debug.Log(
                            "[9x9 Server] Applied full RoomGenConfig to runtime-created RoomGenerator."
                        );
                    }
                    else
                    {
                        // For existing generator, only fill missing prefab references
                        _ = applyConfigMethod.Invoke(
                            m_RoomGenerator,
                            new object[] { m_DefaultRoomGenConfig, true, false }
                        );
                        UnityEngine.Debug.Log(
                            "[9x9 Server] Filled missing prefab references from RoomGenConfig."
                        );
                    }
                }
                else if (m_RoomGeneratorCreatedAtRuntime)
                {
                    UnityEngine.Debug.LogWarning(
                        "[9x9 Server] RoomGenerator variant has no ApplyConfig; ensure templates are assigned manually."
                    );
                }
            }
        }

        void OnDestroy()
        {
            // Unlock cursor when leaving gameplay
            DisableGameplayInput();

            // Unsubscribe from all events to prevent memory leaks
            RemoveListeners();

            // Cleanup runtime-created generator to avoid leaks across scenes/sessions
            if (m_RoomGeneratorCreatedAtRuntime && m_RoomGenerator != null)
            {
                Destroy(m_RoomGenerator.gameObject);
                m_RoomGenerator = null;
                m_RoomGeneratorCreatedAtRuntime = false;
            }

            // Cleanup bots
            if (m_BotManager != null)
            {
                m_BotManager.DespawnAllBots();
                Destroy(m_BotManager.gameObject);
                m_BotManager = null;
            }
        }

        protected override void RemoveListeners()
        {
            // Unsubscribe from all events registered in Awake
            RemoveListener<StartMatchEvent>(OnServerStartMatch);
            RemoveListener<EndMatchEvent>(OnServerMatchEnded);
            RemoveListener<PlayerDisconnected>(OnServerPlayerDisconnected);
            RemoveListener<NGOPlayerReachedExitEvent>(OnPlayerReachedExit);
            RemoveListener<NGOPlayerDiedEvent>(OnPlayerDied);
            RemoveListener<AllBotsEliminatedEvent>(OnAllBotsEliminated);
        }

        void OnServerPlayerDisconnected(PlayerDisconnected evt)
        {
            UnityEngine.Debug.Log($"[9x9 Server] Client {evt.ClientId} disconnected!");

            if (Model.AllowReconnection)
            {
                return; // Allow player to rejoin without penalty
            }

            // If match is active, decrement alive count and check for game over
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
                UnityEngine.Debug.Log("[9x9 Server] Starting maze escape game!");
                Model.MatchStarted = true;
                Model.MatchEnded = false;

                _ = StartCoroutine(OnServerInitializeMazeGame());
            }

            if (evt.IsClient)
            {
                UnityEngine.Debug.Log("[9x9 Client] Joining maze escape game!");
            }
        }

        /// <summary>
        /// Server-side maze initialization sequence
        /// Generates maze, spawns players, and starts mode-specific game logic
        /// </summary>
        IEnumerator OnServerInitializeMazeGame()
        {
            // Step 1: Apply mode-specific generation profile (seed, grid size, density)
            ScriptableObject activeProfile = GetProfileForCurrentMode();
            if (activeProfile != null && m_RoomGenerator != null)
            {
                var applyProfileMethod = m_RoomGenerator.GetType().GetMethod("ApplyProfile");
                if (applyProfileMethod != null)
                {
                    applyProfileMethod.Invoke(m_RoomGenerator, new object[] { activeProfile });
                    UnityEngine.Debug.Log(
                        $"[9x9 Server] Applied {Model.CurrentGameMode} profile: {activeProfile.name}"
                    );
                }
            }

            // Step 2: Generate the procedural maze (uses reflection for flexibility)
            if (m_RoomGenerator != null && !Model.MazeGenerated)
            {
                var generateMethod = m_RoomGenerator.GetType().GetMethod("Generate");
                if (generateMethod != null)
                {
                    UnityEngine.Debug.Log("[9x9 Server] Generating 9x9x9 maze...");
                    _ = generateMethod.Invoke(m_RoomGenerator, null);
                    Model.MazeGenerated = true;
                    yield return new WaitForSeconds(0.5f); // allow generation time
                }
                else
                {
                    UnityEngine.Debug.LogWarning(
                        "[9x9 Server] RoomGenerator variant has no Generate(); skipping maze creation."
                    );
                }
            }
            else if (m_RoomGenerator == null)
            {
                UnityEngine.Debug.LogError(
                    "[9x9 Server] RoomGenerator not found! Cannot generate maze."
                );
                yield break;
            }

            // Step 3: Count connected players (will be 0 in offline singleplayer)
            Model.PlayersAlive.Value = NetworkManager.Singleton.ConnectedClients.Count;
            UnityEngine.Debug.Log($"[9x9 Server] {Model.PlayersAlive.Value} players connected");

            // Offline singleplayer fallback: spawn local non-networked player if no clients connected
            if (Model.PlayersAlive.Value == 0 && m_OfflinePlayerPrefab != null)
            {
                // Get first corner spawn position using reflection
                Vector3 spawnPosition = Vector3.zero;
                var cornerSpawnMethod = m_RoomGenerator
                    .GetType()
                    .GetMethod("GetCornerSpawnPosition");
                if (cornerSpawnMethod != null)
                {
                    object result = cornerSpawnMethod.Invoke(m_RoomGenerator, new object[] { 0 });
                    if (result is Vector3 v)
                    {
                        spawnPosition = v;
                    }
                }
                var offlinePlayer = Instantiate(
                    m_OfflinePlayerPrefab,
                    spawnPosition,
                    Quaternion.identity
                );
                offlinePlayer.name = "OfflinePlayer";
                Model.PlayersAlive.Value = 1;
                UnityEngine.Debug.Log("[9x9 Offline] Spawned offline player at " + spawnPosition);
            }
            else if (Model.PlayersAlive.Value == 0 && m_OfflinePlayerPrefab == null)
            {
                // Emergency fallback: create minimal offline player to prevent black screen
                Vector3 spawnPosition = Vector3.zero;
                var cornerSpawnMethod = m_RoomGenerator
                    .GetType()
                    .GetMethod("GetCornerSpawnPosition");
                if (cornerSpawnMethod != null)
                {
                    object result = cornerSpawnMethod.Invoke(m_RoomGenerator, new object[] { 0 });
                    if (result is Vector3 v)
                    {
                        spawnPosition = v;
                    }
                }
                // Build simple player: CharacterController + FirstPersonController + Camera
                var offlinePlayerGO = new GameObject("OfflinePlayer_Fallback");
                offlinePlayerGO.transform.position = spawnPosition;
                var cc = offlinePlayerGO.AddComponent<CharacterController>();
                cc.center = new Vector3(0, 1, 0);
                cc.height = 2f;
                _ = offlinePlayerGO.AddComponent<NGOFirstPersonController>();
                var camGO = new GameObject("Camera");
                camGO.transform.SetParent(offlinePlayerGO.transform);
                camGO.transform.localPosition = new Vector3(0, 1.6f, 0);
                var cam = camGO.AddComponent<Camera>();
                cam.tag = "MainCamera"; // Mark as main camera for rendering
                Model.PlayersAlive.Value = 1;
                UnityEngine.Debug.LogWarning(
                    "[9x9 Offline] m_OfflinePlayerPrefab not assigned. Created fallback offline player at "
                        + spawnPosition
                );
            }

            // Step 4: Spawn networked players at the 8 cube corners
            if (Model.PlayersAlive.Value > 0 && NetworkManager.Singleton.ConnectedClients.Count > 0)
            {
                SpawnPlayersAtCorners();
            }

            // Step 4.5: Spawn bots for singleplayer or to fill player slots
            if (CustomNetworkManager.Singleton != null && CustomNetworkManager.Singleton.UsingBots)
            {
                SpawnBotsForGameMode();
            }

            // Step 5: Initialize mode-specific game logic (timers, objectives, etc.)
            switch (Model.CurrentGameMode)
            {
                case GameMode.BattleRoyale:
                    _ = StartCoroutine(RunBattleRoyaleTimer());
                    break;

                case GameMode.Coop:
                    UnityEngine.Debug.Log("[9x9 Server] Co-op mode: All players must reach exit");
                    break;

                default:
                    UnityEngine.Debug.Log("[9x9 Server] Standard mode: First to exit wins");
                    break;
            }

            // Step 6: Enable gameplay input - unlock cursor for mouse look
            EnableGameplayInput();
        }

        void EnableGameplayInput()
        {
            // Unlock cursor for mouse look during gameplay
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Debug.Log("[9x9 Gameplay] Cursor locked for mouse look");
        }

        void DisableGameplayInput()
        {
            // Unlock cursor when returning to menu
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Debug.Log("[9x9 Gameplay] Cursor unlocked for menu");
        }

        /// <summary>
        /// Spawn networked players at the 8 corner positions of the 9x9x9 cube
        /// Spreads players evenly for fairness in competitive modes
        /// </summary>
        void SpawnPlayersAtCorners()
        {
            if (m_RoomGenerator == null)
                return;

            var connectedClients = NetworkManager.Singleton.ConnectedClients.Values.ToList();
            int playerIndex = 0;

            foreach (var client in connectedClients)
            {
                // Get spawn position for this player's corner index
                Vector3 spawnPosition = Vector3.zero;
                var cornerSpawnMethod = m_RoomGenerator
                    .GetType()
                    .GetMethod("GetCornerSpawnPosition");
                if (cornerSpawnMethod != null)
                {
                    object result = cornerSpawnMethod.Invoke(
                        m_RoomGenerator,
                        new object[] { playerIndex }
                    );
                    if (result is Vector3 v)
                    {
                        spawnPosition = v;
                    }
                }
                else
                {
                    // Fallback: circular spawn pattern if RoomGenerator doesn't provide corners
                    float radius = 5f;
                    float angle = (playerIndex / (float)connectedClients.Count) * Mathf.PI * 2f;
                    spawnPosition = new Vector3(
                        Mathf.Cos(angle) * radius,
                        1f,
                        Mathf.Sin(angle) * radius
                    );
                }
                var player = client.PlayerObject?.GetComponent<Player>();

                if (player != null)
                {
                    player.transform.position = spawnPosition;
                    UnityEngine.Debug.Log(
                        $"[9x9 Server] Spawned player {playerIndex} at corner {spawnPosition}"
                    );
                }

                playerIndex++;
            }
        }

        /// <summary>
        /// Spawn bots to fill remaining player slots based on game mode
        /// Bots spawn at unused corner positions
        /// </summary>
        void SpawnBotsForGameMode()
        {
            if (m_BotPrefab == null)
            {
                UnityEngine.Debug.LogWarning("[9x9 Server] Bot prefab not assigned! Cannot spawn bots.");
                return;
            }

            // Create or retrieve BotManager
            if (m_BotManager == null)
            {
                var botManagerGO = new GameObject("BotManager");
                m_BotManager = botManagerGO.AddComponent<BotManager>();
            }

            // Determine bot count based on game mode
            int botsToSpawn = Model.CurrentGameMode switch
            {
                GameMode.Coop => Mathf.Max(0, m_MaxBots - NetworkManager.Singleton.ConnectedClients.Count),
                GameMode.BattleRoyale => m_MaxBots,
                GameMode.NewGame => Mathf.Max(1, m_MaxBots - NetworkManager.Singleton.ConnectedClients.Count),
                _ => Mathf.Max(0, m_MaxBots - NetworkManager.Singleton.ConnectedClients.Count)
            };

            if (botsToSpawn > 0)
            {
                UnityEngine.Debug.Log($"[9x9 Server] Spawning {botsToSpawn} bots for {Model.CurrentGameMode} mode");
                m_BotManager.SpawnBots(botsToSpawn);
                Model.PlayersAlive.Value += botsToSpawn;
            }
        }

        /// <summary>
        /// Battle Royale mode: 10-minute countdown with shrinking safe zone
        /// Survivors at timeout trigger end-of-match
        /// </summary>
        IEnumerator RunBattleRoyaleTimer()
        {
            Model.MatchTimer.Value = 600; // 10 minutes countdown

            while (Model.MatchTimer.Value > 0 && !Model.MatchEnded)
            {
                yield return CoroutinesHelper.OneSecond;
                Model.MatchTimer.Value--;

                // TODO: Implement shrinking safe zone or progressive hazard activation
            }

            // Time expired - determine winner from survivors
            if (!Model.MatchEnded)
            {
                var survivors = NetworkManager
                    .Singleton.ConnectedClients.Values.Select(c =>
                        c.PlayerObject.GetComponent<Player>()
                    )
                    .Where(p => p != null && p.IsAlive)
                    .ToList();

                ulong? winnerClientId = survivors.Count > 0 ? survivors.First().OwnerClientId : null;
                Broadcast(new EndMatchEvent(winnerClientId));
            }
        }

        /// <summary>
        /// Called when a player reaches the exit room (center of 9x9x9 cube)
        /// Handles different win conditions based on game mode
        /// </summary>
        void OnPlayerReachedExit(NGOPlayerReachedExitEvent evt)
        {
            UnityEngine.Debug.Log($"[9x9 Server] Player reached the exit!");

            switch (Model.CurrentGameMode)
            {
                case GameMode.Coop:
                    // In co-op, all players must reach exit before winning
                    // TODO: Track which players have reached exit and trigger win when all arrive
                    UnityEngine.Debug.Log(
                        "[9x9 Server] Co-op: Player reached exit, waiting for others..."
                    );
                    break;

                default:
                    // First player to exit wins
                    Broadcast(new EndMatchEvent(evt.WinnerClientId));
                    break;
            }
        }

        /// <summary>
        /// Called when a player dies (hazard, fall, PvP, etc.)
        /// Decrements alive count and checks for game over condition
        /// </summary>
        void OnPlayerDied(NGOPlayerDiedEvent evt)
        {
            Model.PlayersAlive.Value--;
            UnityEngine.Debug.Log(
                $"[9x9 Server] Player died. {Model.PlayersAlive.Value} players remaining"
            );

            // If all players dead, end match
            if (Model.PlayersAlive.Value <= 0)
            {
                Broadcast(new EndMatchEvent(null)); // No winner
            }
        }

        /// <summary>
        /// Called when all bots are eliminated in singleplayer/offline mode
        /// Triggers player victory condition
        /// </summary>
        void OnAllBotsEliminated(AllBotsEliminatedEvent evt)
        {
            UnityEngine.Debug.Log("[9x9] All bots eliminated! Player wins!");

            // Find the player and trigger victory
            var player = FindObjectOfType<Player>();
            if (player != null)
            {
                Broadcast(new EndMatchEvent(player.OwnerClientId));
            }
            else
            {
                // Fallback: show generic victory
                View.ShowVictory(null);
            }
        }

        /// <summary>
        /// Match ended - update stats, show results, and return to menu
        /// </summary>
        void OnServerMatchEnded(EndMatchEvent evt)
        {
            UnityEngine.Debug.Log(
                $"[9x9 Server] Match ended. Winner: {(evt.WinnerClientId.HasValue ? evt.WinnerClientId.Value : "None")}"
            );
            Model.MatchEnded = true;

            // Update persistent player stats (wins, kills, deaths, etc.)
            if (evt.WinnerClientId.HasValue)
            {
                // TODO: Increment wins, kills, playtime, etc. in PlayerProfile system
                // Note: Need to look up Player object by clientId if needed
            }

            // Show victory/defeat screen to all clients
            if (evt.WinnerClientId.HasValue)
            {
                View.ShowVictory(null);
            }
            else
            {
                View.ShowDefeat();
            }

            // Return to menu after 10 seconds
            _ = StartCoroutine(ReturnToMenuAfterDelay(10f));
        }

        /// <summary>
        /// Select the appropriate RoomGenProfile based on current game mode
        /// Determines seed, grid size, density, and room templates
        /// </summary>
        ScriptableObject GetProfileForCurrentMode()
        {
            switch (Model.CurrentGameMode)
            {
                case GameMode.NewGame:
                    // Story mode uses deterministic seed for repeatable level design
                    return m_StoryModeProfile;

                case GameMode.Coop:
                    return m_CoopModeProfile;

                case GameMode.BattleRoyale:
                    return m_BattleRoyaleProfile;

                case GameMode.FiveByFive:
                    return m_5x5Profile;

                case GameMode.ThreeByThree:
                    return m_3x3Profile;

                default:
                    UnityEngine.Debug.LogWarning(
                        $"[9x9] No profile defined for mode {Model.CurrentGameMode}, using default generator settings"
                    );
                    return null;
            }
        }

        /// <summary>
        /// Waits specified seconds then returns all players to main menu
        /// </summary>
        IEnumerator ReturnToMenuAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CustomNetworkManager.Singleton.OnClientDoPostMatchCleanupAndReturnToMetagame();
        }
    }
}
