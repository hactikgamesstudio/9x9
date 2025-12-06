using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems; // For duplicate EventSystem pruning
using Unity.Template.Multiplayer.NGO.Core;
#if UNITY_SERVER || ENABLE_UCS_SERVER
using Unity.Services.Authentication.Server;
#endif

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// A custom network manager that implements additional setup logic and rules
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class CustomNetworkManager : MonoBehaviour
    {
        internal static event Action OnConfigurationLoaded;
        const string k_DefaultServerListenAddress = "0.0.0.0";
        public static CustomNetworkManager Singleton { get; private set; }
        public static ConfigurationManager Configuration { get; private set; }
        internal static MultiplayAssignment s_AssignmentForCurrentGame;
        // Null-safe bot flag (Configuration may be created AfterSceneLoad)
        public bool UsingBots => Configuration != null && Configuration.GetBool(ConfigurationManager.k_EnableBots);
        public bool HasConfiguration => Configuration != null;
        public bool GetBoolSafe(string key, bool defaultValue = false) => Configuration != null ? Configuration.GetBool(key) : defaultValue;
        public int GetIntSafe(string key, int defaultValue = 0) => Configuration != null ? Configuration.GetInt(key) : defaultValue;
#if UNITY_EDITOR
        public static bool s_AreTestsRunning = false;
#endif
        internal bool AutoConnectOnStartup
        {
            get
            {
                bool startAutomatically = Configuration != null && Configuration.GetBool(ConfigurationManager.k_Autoconnect);
#if UNITY_EDITOR
                startAutomatically |= s_AreTestsRunning;
#endif
                return startAutomatically;
            }
        }

        internal bool IsClient => m_NetworkManager.IsClient;
        internal bool IsServer => m_NetworkManager.IsServer;
        internal bool IsHost => m_NetworkManager.IsHost;

        internal Action ReturnToMetagame;
        internal int ExpectedPlayers { get; private set; } = 2;
        internal byte BotsSpawned { get; private set; } = 0;
        bool m_PreparedGame = true;

        [SerializeField]
        [Tooltip("The game application prefab to instantiate when starting a match (e.g., CubeGameApplication)")]
        GameObject m_GameAppPrefab;
        BaseApplication m_GameApp;

        [SerializeField]
        Player m_BotPrefab;

        internal HashSet<Player> ReadyPlayers { get; private set; }
        NetworkManager m_NetworkManager;

        void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            // Configuration intentionally deferred to AfterSceneLoad; Awake only wires events.
            m_NetworkManager = GetComponent<NetworkManager>();
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
            m_NetworkManager.OnServerStarted += OnServerStarted;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnApplicationStarted()
        {
            // Attempt to bind Singleton if not yet assigned (first scene load)
            if (Singleton == null)
            {
                var existing = UnityEngine.Object.FindFirstObjectByType<CustomNetworkManager>();
                if (existing != null)
                {
                    Singleton = existing;
                }
            }

            // If still no instance, defer entirely to a later Awake() (no warning spam)
            if (Singleton == null)
            {
                return;
            }

            // Create configuration only if not already created by Awake fallback
            if (Configuration == null)
            {
                Configuration = new ConfigurationManager(
                    Singleton,
                    ConfigurationManager.k_DevConfigFile,
                    OnConfigurationLoadedCallback
                );
            }
        }

        static void OnConfigurationLoadedCallback(ConfigurationManager configurationManager)
        {
            Configuration = configurationManager;
            OnConfigurationLoaded?.Invoke();
            if (Configuration.GetMultiplayerRole() != MultiplayerRoleFlags.Server)
            {
                //note: this is a good place where to load player-specific configuration (I.E: Audio/video settings)
            }
            /* note: this is the entry point for all autoconnected instances (including standalone servers)
            note 2: waiting a frame seems to be necessary to avoid race conditions related to serialization and network setup when using bots in Host autoconnect mode*/
            _ = Singleton.StartCoroutine(
                CoroutinesHelper.WaitAndDo(
                    CoroutinesHelper.WaitAFrame(),
                    () => Singleton.InitializeNetworkLogic(false, false)
                )
            );
        }

        public void SetConfiguration(ConfigurationManager configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Initializes the application's network-related behaviour according to the circumstances
        /// </summary>
        /// <param name="gameMode">The game mode to initialize</param>
        /// <param name="startedByUser">Was the setup manually started by the user, I.E: when starting a game manually in single player mode?</param>
        /// <param name="startedByMatchmaker">Was the setup automatically started by the matchmaker?</param>
        public void InitializeNetworkLogic(bool startedByUser, bool startedByMatchmaker)
        {
            // Shutdown any existing network session cleanly
            if (IsClient || IsServer)
            {
                m_NetworkManager.Shutdown();
            }

            // Ensure NetworkConfig has no null or invalid NetworkPrefab entries before starting networking
            SanitizeNetworkPrefabs();

            ExpectedPlayers = Configuration.GetInt(ConfigurationManager.k_MaxPlayers);
            if (ExpectedPlayers < 1)
            {
                ExpectedPlayers = 2; // Default to 2 players minimum
            }

            // Offline singleplayer (no Netcode session started)
            if (startedByUser && !startedByMatchmaker)
            {
                InstantiateGameApplication();
                // BotManager will be added by the game application if needed
                UnityEngine.Debug.Log("[9x9] Offline singleplayer initialized (local game + bots).");
                
                // Broadcast StartMatchEvent to trigger game initialization (maze generation, player spawn)
                // In offline mode, this replaces the network OnServerGameReadyToStart() call
                if (m_GameApp != null)
                {
                    m_GameApp.Broadcast(new StartMatchEvent(true, false));
                    UnityEngine.Debug.Log("[9x9] Broadcast StartMatchEvent for offline game initialization.");
                }
                
                return;
            }

            // Matchmaker flow (client only)
            if (startedByMatchmaker)
            {
                if (IsClient)
                {
                    UnityEngine.Debug.Log("Already connected!");
                    return;
                }
                StartClientWithMatchmakerData();
                return;
            }

            var commandLineArgumentsParser = new CommandLineArgumentsParser();
            ushort listeningPort = commandLineArgumentsParser.ServerPort != -1
                ? (ushort)commandLineArgumentsParser.ServerPort
                : (ushort)Configuration.GetInt(ConfigurationManager.k_Port);

            // Auto-connect for dedicated servers or development builds
            if (AutoConnectOnStartup)
            {
                AutoConnect(listeningPort);
            }
        }


        void StartClientWithMatchmakerData()
        {
            UnityEngine.Debug.Log(
                $"Attempting to connect to: {s_AssignmentForCurrentGame.Ip}:{s_AssignmentForCurrentGame.Port}"
            );
            ushort listeningPort = (ushort)s_AssignmentForCurrentGame.Port;
            SetNetworkPortAndAddress(
                listeningPort,
                s_AssignmentForCurrentGame.Ip,
                k_DefaultServerListenAddress
            );
            _ = m_NetworkManager.StartClient();
        }

        void AutoConnect(ushort listeningPort)
        {
            MultiplayerRoleFlags multiplayerRole = Configuration.GetMultiplayerRole();
            switch (multiplayerRole)
            {
                case MultiplayerRoleFlags.Client:
                    if (IsClient)
                    {
                        UnityEngine.Debug.Log("Already connected!");
                        return;
                    }
                    SetNetworkPortAndAddress(
                        listeningPort,
                        Configuration.GetString(ConfigurationManager.k_ServerIP),
                        k_DefaultServerListenAddress
                    );
                    _ = m_NetworkManager.StartClient();
                    break;
                case MultiplayerRoleFlags.Server:
                    UnityEngine.Debug.Log(
                        $"Starting server on port {listeningPort}, expecting {ExpectedPlayers} players"
                    );
                    Application.targetFrameRate = 60; //lock framerate on dedicated servers
                    OnServerMarkServerAsReadyToAcceptPlayers(listeningPort);
                    break;
                case MultiplayerRoleFlags.ClientAndServer:
                    UnityEngine.Debug.Log(
                        $"Starting Host on port {listeningPort}, expecting {ExpectedPlayers} players"
                    );
                    SetNetworkPortAndAddress(
                        listeningPort,
                        k_DefaultServerListenAddress,
                        k_DefaultServerListenAddress
                    );
                    SanitizeNetworkPrefabs();
                    _ = m_NetworkManager.StartHost();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks for null, duplicate, or invalid NetworkPrefab entries and logs guidance.
        /// Use the editor validator (Tools/Netcode/Validate && Fix) to auto-fix.
        /// </summary>
        void SanitizeNetworkPrefabs()
        {
            try
            {
                if (m_NetworkManager == null || m_NetworkManager.NetworkConfig == null || m_NetworkManager.NetworkConfig.Prefabs == null)
                {
                    return;
                }

                var prefabsList = m_NetworkManager.NetworkConfig.Prefabs.Prefabs;
                if (prefabsList == null)
                {
                    return;
                }

                var seen = new HashSet<GameObject>();
                int invalid = 0;
                for (int i = 0; i < prefabsList.Count; i++)
                {
                    var entry = prefabsList[i];
                    if (entry.Prefab == null || !entry.Prefab.TryGetComponent<NetworkObject>(out _) || seen.Contains(entry.Prefab))
                    {
                        invalid++;
                    }
                    else
                    {
                        _ = seen.Add(entry.Prefab);
                    }
                }
                if (invalid > 0)
                {
                    UnityEngine.Debug.LogWarning($"[Netcode] Detected {invalid} invalid NetworkPrefab entries. Use Tools/Netcode/Validate && Fix to clean them.", this);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[Netcode] SanitizeNetworkPrefabs encountered an issue: {ex.Message}", this);
            }
        }

        void OnServerMarkServerAsReadyToAcceptPlayers(ushort listeningPort)
        {
#if UNITY_SERVER || ENABLE_UCS_SERVER
            Task.Run(() => OnServerMarkServerAsReadyToAcceptPlayersAsync(listeningPort));
            return;
#else
            SetNetworkPortAndAddress(
                listeningPort,
                k_DefaultServerListenAddress,
                k_DefaultServerListenAddress
            );
            m_NetworkManager.StartServer();
            UnityEngine.Debug.Log("[Server] Server is ready to accept players");
#endif
        }

#if UNITY_SERVER || ENABLE_UCS_SERVER
        IMultiplaySessionManager m_SessionManager;
        async Task OnServerMarkServerAsReadyToAcceptPlayersAsync(ushort listeningPort)
        {
            if (UnityServices.Instance.GetMultiplayerService() != null)
            {
                await ServerAuthenticationService.Instance.SignInFromServerAsync();
                var token = ServerAuthenticationService.Instance.AccessToken;

                var callbacks = new MultiplaySessionManagerEventCallbacks();
                callbacks.Allocated += CallbacksOnAllocated;

                var sessionManagerOptions = new MultiplaySessionManagerOptions()
                {
                    SessionOptions = new SessionOptions() { MaxPlayers = 2 }.WithDirectNetwork(
                        k_DefaultServerListenAddress,
                        k_DefaultServerListenAddress,
                        listeningPort
                    ),

                    MultiplayServerOptions = new MultiplayServerOptions(
                        serverName: "Dummy",
                        gameType: "TemplateGame",
                        buildId: "0",
                        map: "TemplateMap"
                    ),
                    Callbacks = callbacks,
                };
                m_SessionManager =
                    await MultiplayerServerService.Instance.StartMultiplaySessionManagerAsync(
                        sessionManagerOptions
                    );

                //continue this after the allocation happened
                async void CallbacksOnAllocated(IMultiplayAllocation obj)
                {
                    var session = m_SessionManager.Session;
                    await m_SessionManager.SetPlayerReadinessAsync(true);
                    UnityEngine.Debug.Log("[Multiplay] Server is ready to accept players");
                }
            }
        }
#endif

        void SetNetworkPortAndAddress(ushort port, string address, string serverListenAddress)
        {
            var transport = GetComponent<UnityTransport>();
            if (transport == null) //happens during Play Mode Tests
            {
                return;
            }
            transport.SetConnectionData(address, port, serverListenAddress);
        }

        void OnServerStarted()
        {
            ReadyPlayers = new HashSet<Player>();
            m_PreparedGame = false;
            if (UsingBots)
            {
                OnServerInstantiateBots();
            }
        }

        void OnServerInstantiateBots()
        {
            BotsSpawned = 0;
            bool isDedicatedServer = m_NetworkManager.IsServer && !m_NetworkManager.IsClient;
            int totalPlayersCountToReach = ExpectedPlayers;
            if (isDedicatedServer)
            {
                if (m_NetworkManager.ConnectedClients.Count == 0)
                {
                    totalPlayersCountToReach--; //leave room to at least one human
                }
            }

            while (
                (m_NetworkManager.ConnectedClients.Count + BotsSpawned) < totalPlayersCountToReach
            )
            {
                _ = InstantiateBotGamePlayer();
            }
        }

        Player InstantiateBotGamePlayer()
        {
            Player bot = Instantiate(m_BotPrefab, Vector3.zero, Quaternion.identity);
            bot.GetComponent<NetworkObject>().Spawn();
            BotsSpawned++;
            return bot;
        }

        internal void OnServerQuitAfter(float seconds)
        {
            UnityEngine.Debug.Log($"[Server] quitting game in {seconds} seconds!");
            _ = StartCoroutine(CoroutinesHelper.WaitAndDo(new WaitForSeconds(seconds), OnServerQuit));
        }

        void OnServerQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        void OnClientDisconnected(ulong ClientId)
        {
            UnityEngine.Debug.Log($"Client {ClientId} disconnected");
            if (IsServer)
            {
                _ = ReadyPlayers.RemoveWhere(p => p.NetworkObject == m_NetworkManager.ConnectedClients[ClientId].PlayerObject);
                if (m_GameApp != null) //the game already started
                {
                    m_GameApp.Broadcast(new PlayerDisconnected(ClientId));
                }
            }
        }

        void OnClientConnected(ulong ClientId)
        {
            if (IsClient)
            {
                UnityEngine.Debug.Log($"Local client {ClientId} connected, waiting for other players...");
                if (MetagameApplication.Instance)
                {
                    MetagameApplication.Instance.Broadcast(new MatchLoadingEvent());
                }
            }
            else
            {
                UnityEngine.Debug.Log($"Remote client {ClientId} connected");
            }

            if (m_PreparedGame || !IsServer) //game should be prepared only once per server session
            {
                return;
            }
            if ((m_NetworkManager.ConnectedClients.Count + BotsSpawned) == ExpectedPlayers)
            {
                OnServerPrepareGame();
            }
        }

        internal void OnServerPlayerIsReady(Player player)
        {
            _ = ReadyPlayers.Add(player);
            if (ReadyPlayers.Count + BotsSpawned == ExpectedPlayers)
            {
                OnServerGameReadyToStart();
            }
        }

        void OnServerPrepareGame()
        {
            UnityEngine.Debug.Log("[Server] Preparing game");
            m_PreparedGame = true;
            InstantiateGameApplication();
            foreach (var connectionToClient in m_NetworkManager.ConnectedClients.Values)
            {
                connectionToClient
                    .PlayerObject.GetComponent<Player>()
                    .OnClientPrepareGameClientRpc();
            }
        }

        internal void InstantiateGameApplication()
        {
            if (m_GameAppPrefab != null)
            {
                m_GameApp = Instantiate(m_GameAppPrefab).GetComponent<BaseApplication>();
                PruneDuplicateEventSystems();
                return;
            }

            // ERROR: GameApp prefab must be assigned in Inspector!
            UnityEngine.Debug.LogError(
                "[NetworkManager] GameApp prefab is not assigned! "
                + "Please assign CubeGameApplication prefab in CustomNetworkManager Inspector. "
                + "Cannot proceed without game application."
            );
        }

        /// <summary>
        /// Ensures only one active EventSystem exists (Unity UI requirement).
        /// Keeps the first found and destroys subsequent ones, preferring to retain the pre-existing Metagame EventSystem.
        /// </summary>
        void PruneDuplicateEventSystems()
        {
            var systems = GameObject.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            if (systems == null || systems.Length <= 1)
            {
                return; // Nothing to prune
            }

            // Decide which to keep: keep the one that was in the scene before (not parented under newly instantiated game app)
            // If we cannot distinguish, keep the first and remove others.
            var toKeep = systems[0];
            foreach (var es in systems)
            {
                if (es == toKeep)
                {
                    continue;
                }
                // Destroy duplicate EventSystem roots to avoid runtime warning and potential input blackout
                UnityEngine.Debug.LogWarning("[UI] Destroying duplicate EventSystem: " + es.gameObject.name);
                Destroy(es.gameObject);
            }
        }

        internal BaseApplication CurrentGameApp => m_GameApp;

        internal void OnServerGameReadyToStart()
        {
            m_GameApp.Broadcast(new StartMatchEvent(true, false));
            foreach (var player in ReadyPlayers)
            {
                player.OnClientStartGameClientRpc();
            }
            ReadyPlayers.Clear();
        }

        /// <summary>
        /// Performs cleanup operation after a game
        /// </summary>
        internal void OnClientDoPostMatchCleanupAndReturnToMetagame()
        {
            if (IsClient)
            {
                m_NetworkManager.Shutdown();
            }
            Destroy(GameApplication.Instance.gameObject);
            ReturnToMetagame?.Invoke();
        }

        internal void OnEnteredMatchmaker()
        {
            s_AssignmentForCurrentGame = null;
        }
    }
}