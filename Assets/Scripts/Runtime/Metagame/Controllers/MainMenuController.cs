using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
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
                View.MultiplayerButton.SetEnabled(false);
            }
        }

        void OnExitedMatchmakerQueue(ExitedMatchmakerQueueEvent evt)
        {
            // Re-enable multiplayer button after fully exiting matchmaker queue
            if (View.MultiplayerButton != null)
            {
                View.MultiplayerButton.SetEnabled(true);
            }
        }

        void OnStartSinglePlayerMode(StartSinglePlayerModeEvent evt)
        {
            View.Hide();
            
            // Handle different game modes
            switch (evt.GameMode)
            {
                case GameMode.Continue:
                    // Load saved game
                    PlayerProfileManager.LoadGame();
                    break;
                    
                case GameMode.Coop:
                    // Start co-op mode (host a game for friends to join)
                    Debug.Log("Starting Co-op mode");
                    break;
                    
                case GameMode.NewGame:
                default:
                    // Start fresh single player game
                    Debug.Log("Starting new single player game");
                    break;
            }
            
            CustomNetworkManager.Singleton.InitializeNetworkLogic(true, false);
        }
    }
}
