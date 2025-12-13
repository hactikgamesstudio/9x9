using UnityEngine;
using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    internal class MainMenuController : Controller<MetagameApplication>
    {
        MainMenuView View => App.View.MainMenu;

        void Awake()
        {
            AddListener<EnterMatchmakerQueueEvent>(OnEnterMatchmakerQueue);
            AddListener<ExitMatchmakerQueueEvent>(OnExitMatchmakerQueue);
            AddListener<MatchLoadingEvent>(OnMatchLoading);
            AddListener<ExitedMatchmakerQueueEvent>(OnExitedMatchmakerQueue);
            AddListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
        }

        void OnDestroy()
        {
            RemoveListeners();
        }

        internal override void RemoveListeners()
        {
            RemoveListener<MatchLoadingEvent>(OnMatchLoading);
            RemoveListener<EnterMatchmakerQueueEvent>(OnEnterMatchmakerQueue);
            RemoveListener<ExitMatchmakerQueueEvent>(OnExitMatchmakerQueue);
            RemoveListener<ExitedMatchmakerQueueEvent>(OnExitedMatchmakerQueue);
            RemoveListener<StartSinglePlayerModeEvent>(OnStartSinglePlayerMode);
        }

        void OnMatchLoading(MatchLoadingEvent evt)
        {
            if (!CustomNetworkManager.Singleton.AutoConnectOnStartup)
            {
                return; //then we're starting a match from the matchmaker
            }
            View.Hide();
            App.View.LoadingScreen.Show();
        }

        void OnEnterMatchmakerQueue(EnterMatchmakerQueueEvent evt)
        {
            View.Hide();
        }

        void OnExitMatchmakerQueue(ExitMatchmakerQueueEvent evt)
        {
            View.Show();
            // Re-enable multiplayer button after exiting matchmaker
            if (View.MultiplayerButton != null)
            {
                View.MultiplayerButton.interactable = false;
            }
        }

        void OnExitedMatchmakerQueue(ExitedMatchmakerQueueEvent evt)
        {
            // Re-enable multiplayer button after fully exiting matchmaker queue
            if (View.MultiplayerButton != null)
            {
                View.MultiplayerButton.interactable = true;
            }
        }

        void OnStartSinglePlayerMode(StartSinglePlayerModeEvent evt)
        {
            UnityEngine.Debug.Log($"[MainMenuController] OnStartSinglePlayerMode called with GameMode: {evt.GameMode}, BotCount: {evt.BotCount}");
            View.Hide();
            
            // Store bot count for game initialization
            if (CustomNetworkManager.Singleton != null)
            {
                // Use reflection to set bot count if CustomNetworkManager supports it
                var botCountField = CustomNetworkManager.Singleton.GetType().GetField("m_BotCount", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (botCountField != null)
                {
                    botCountField.SetValue(CustomNetworkManager.Singleton, evt.BotCount);
                    UnityEngine.Debug.Log($"[MainMenu] Set bot count to {evt.BotCount}");
                }
            }
            
            // Handle different game modes
            switch (evt.GameMode)
            {
                case GameMode.Continue:
                    // Load saved game
                    UnityEngine.Debug.Log("[MainMenuController] Loading saved game...");
                    PlayerProfileManager.LoadGame();
                    break;
                    
                case GameMode.Coop:
                    // Start co-op mode (host a game for friends to join)
                    UnityEngine.Debug.Log($"[MainMenuController] Starting Co-op mode with {evt.BotCount} bots");
                    break;
                    
                case GameMode.NewGame:
                default:
                    // Start fresh single player game
                    UnityEngine.Debug.Log($"[MainMenuController] Starting new single player game with {evt.BotCount} bots");
                    break;
            }
            
            UnityEngine.Debug.Log("[MainMenuController] Initializing network logic for singleplayer");
            CustomNetworkManager.Singleton.InitializeNetworkLogic(true, false);
        }
    }
}
