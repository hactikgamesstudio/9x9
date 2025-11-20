using UnityEngine.UIElements;
using Unity.Template.Multiplayer.NGO.Core.Systems;
using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    internal class MainMenuView : View<MetagameApplication>
    {
        // Main Menu
        VisualElement m_MainMenu;
        Button m_SinglePlayerButton;
        Button m_MultiplayerButton;
        Button m_ProfileButton;
        Button m_OptionsButton;
        Button m_QuitButton;
        Label m_TitleLabel;
        
        // Single Player Menu
        VisualElement m_SinglePlayerMenu;
        Button m_ContinueButton;
        Button m_NewGameButton;
        Button m_CoopButton;
        Button m_SpBackButton;
        
        // Multiplayer Menu
        VisualElement m_MultiplayerMenu;
        Button m_BattleRoyaleButton;
        Button m_Mode3x3Button;
        Button m_Mode5x5Button;
        Button m_MpCoopButton;
        Button m_MpBackButton;
        
        // Profile Menu
        VisualElement m_ProfileMenu;
        Label m_PlayerNameLabel;
        Label m_WinsLabel;
        Label m_KillsLabel;
        ScrollView m_FriendsListScroll;
        Button m_ProfileBackButton;
        
        // Options Menu
        VisualElement m_OptionsMenu;
        Button m_OptionsBackButton;
        
        VisualElement m_Root;
        
        // Public properties for controller access
        internal Button MultiplayerButton => m_MultiplayerButton;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            m_Root = uiDocument.rootVisualElement;

            // Main Menu Elements
            m_MainMenu = m_Root.Q<VisualElement>("mainMenu");
            m_TitleLabel = m_Root.Q<Label>("titleLabel");
            m_TitleLabel.text = "9x9";
            
            m_SinglePlayerButton = m_Root.Q<Button>("singlePlayerButton");
            m_SinglePlayerButton.RegisterCallback<ClickEvent>(OnClickSinglePlayer);
            
            m_MultiplayerButton = m_Root.Q<Button>("multiplayerButton");
            m_MultiplayerButton.RegisterCallback<ClickEvent>(OnClickMultiplayer);
            
            m_ProfileButton = m_Root.Q<Button>("profileButton");
            m_ProfileButton.RegisterCallback<ClickEvent>(OnClickProfile);
            
            m_OptionsButton = m_Root.Q<Button>("optionsButton");
            m_OptionsButton.RegisterCallback<ClickEvent>(OnClickOptions);

            m_QuitButton = m_Root.Q<Button>("quitButton");
            m_QuitButton.RegisterCallback<ClickEvent>(OnClickQuit);
            
            // Single Player Submenu
            m_SinglePlayerMenu = m_Root.Q<VisualElement>("singlePlayerMenu");
            m_ContinueButton = m_Root.Q<Button>("continueButton");
            m_ContinueButton.RegisterCallback<ClickEvent>(OnClickContinue);
            m_NewGameButton = m_Root.Q<Button>("newGameButton");
            m_NewGameButton.RegisterCallback<ClickEvent>(OnClickNewGame);
            m_CoopButton = m_Root.Q<Button>("coopButton");
            m_CoopButton.RegisterCallback<ClickEvent>(OnClickCoop);
            m_SpBackButton = m_Root.Q<Button>("spBackButton");
            m_SpBackButton.RegisterCallback<ClickEvent>(OnClickSpBack);
            
            // Multiplayer Submenu
            m_MultiplayerMenu = m_Root.Q<VisualElement>("multiplayerMenu");
            m_BattleRoyaleButton = m_Root.Q<Button>("battleRoyaleButton");
            m_BattleRoyaleButton.RegisterCallback<ClickEvent>(OnClickBattleRoyale);
            m_Mode3x3Button = m_Root.Q<Button>("mode3x3Button");
            m_Mode3x3Button.RegisterCallback<ClickEvent>(OnClickMode3x3);
            m_Mode5x5Button = m_Root.Q<Button>("mode5x5Button");
            m_Mode5x5Button.RegisterCallback<ClickEvent>(OnClickMode5x5);
            m_MpCoopButton = m_Root.Q<Button>("mpCoopButton");
            m_MpCoopButton.RegisterCallback<ClickEvent>(OnClickMpCoop);
            m_MpBackButton = m_Root.Q<Button>("mpBackButton");
            m_MpBackButton.RegisterCallback<ClickEvent>(OnClickMpBack);
            
            // Profile Menu
            m_ProfileMenu = m_Root.Q<VisualElement>("profileMenu");
            m_PlayerNameLabel = m_Root.Q<Label>("playerNameLabel");
            m_WinsLabel = m_Root.Q<Label>("winsLabel");
            m_KillsLabel = m_Root.Q<Label>("killsLabel");
            m_FriendsListScroll = m_Root.Q<ScrollView>("friendsListScroll");
            m_ProfileBackButton = m_Root.Q<Button>("profileBackButton");
            m_ProfileBackButton.RegisterCallback<ClickEvent>(OnClickProfileBack);
            
            // Options Menu
            m_OptionsMenu = m_Root.Q<VisualElement>("optionsMenu");
            m_OptionsBackButton = m_Root.Q<Button>("optionsBackButton");
            m_OptionsBackButton.RegisterCallback<ClickEvent>(OnClickOptionsBack);

            CustomNetworkManager.OnConfigurationLoaded += OnGameConfigurationLoaded;
            
            // Initialize profile data
            UpdateProfileDisplay();
        }

        void OnGameConfigurationLoaded()
        {
            DisableControlsUnsupportedInAutoconnectMode();
        }

        void OnDisable()
        {
            // Main Menu
            m_SinglePlayerButton.UnregisterCallback<ClickEvent>(OnClickSinglePlayer);
            m_MultiplayerButton.UnregisterCallback<ClickEvent>(OnClickMultiplayer);
            m_ProfileButton.UnregisterCallback<ClickEvent>(OnClickProfile);
            m_OptionsButton.UnregisterCallback<ClickEvent>(OnClickOptions);
            m_QuitButton.UnregisterCallback<ClickEvent>(OnClickQuit);
            
            // Single Player
            m_ContinueButton.UnregisterCallback<ClickEvent>(OnClickContinue);
            m_NewGameButton.UnregisterCallback<ClickEvent>(OnClickNewGame);
            m_CoopButton.UnregisterCallback<ClickEvent>(OnClickCoop);
            m_SpBackButton.UnregisterCallback<ClickEvent>(OnClickSpBack);
            
            // Multiplayer
            m_BattleRoyaleButton.UnregisterCallback<ClickEvent>(OnClickBattleRoyale);
            m_Mode3x3Button.UnregisterCallback<ClickEvent>(OnClickMode3x3);
            m_Mode5x5Button.UnregisterCallback<ClickEvent>(OnClickMode5x5);
            m_MpCoopButton.UnregisterCallback<ClickEvent>(OnClickMpCoop);
            m_MpBackButton.UnregisterCallback<ClickEvent>(OnClickMpBack);
            
            // Profile
            m_ProfileBackButton.UnregisterCallback<ClickEvent>(OnClickProfileBack);
            
            // Options
            m_OptionsBackButton.UnregisterCallback<ClickEvent>(OnClickOptionsBack);
            
            CustomNetworkManager.OnConfigurationLoaded -= OnGameConfigurationLoaded;
        }

        // ===== MAIN MENU BUTTONS =====
        void OnClickSinglePlayer(ClickEvent evt)
        {
            ShowMenu(m_SinglePlayerMenu);
            // Check if save game exists and enable/disable Continue button
            m_ContinueButton.SetEnabled(PlayerProfileManager.HasSaveGame());
        }
        
        void OnClickMultiplayer(ClickEvent evt)
        {
            ShowMenu(m_MultiplayerMenu);
        }
        
        void OnClickProfile(ClickEvent evt)
        {
            UpdateProfileDisplay();
            ShowMenu(m_ProfileMenu);
        }
        
        void OnClickOptions(ClickEvent evt)
        {
            ShowMenu(m_OptionsMenu);
        }

        void OnClickQuit(ClickEvent evt)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        
        // ===== SINGLE PLAYER SUBMENU =====
        void OnClickContinue(ClickEvent evt)
        {
            Broadcast(new StartSinglePlayerModeEvent { GameMode = GameMode.Continue });
        }
        
        void OnClickNewGame(ClickEvent evt)
        {
            Broadcast(new StartSinglePlayerModeEvent { GameMode = GameMode.NewGame });
        }
        
        void OnClickCoop(ClickEvent evt)
        {
            Broadcast(new StartSinglePlayerModeEvent { GameMode = GameMode.Coop });
        }
        
        void OnClickSpBack(ClickEvent evt)
        {
            ShowMenu(m_MainMenu);
        }
        
        // ===== MULTIPLAYER SUBMENU =====
        void OnClickBattleRoyale(ClickEvent evt)
        {
            Broadcast(new EnterMatchmakerQueueEvent("BattleRoyale"));
        }
        
        void OnClickMode3x3(ClickEvent evt)
        {
            Broadcast(new EnterMatchmakerQueueEvent("3x3"));
        }
        
        void OnClickMode5x5(ClickEvent evt)
        {
            Broadcast(new EnterMatchmakerQueueEvent("5x5"));
        }
        
        void OnClickMpCoop(ClickEvent evt)
        {
            Broadcast(new EnterMatchmakerQueueEvent("Coop"));
        }
        
        void OnClickMpBack(ClickEvent evt)
        {
            ShowMenu(m_MainMenu);
        }
        
        // ===== PROFILE MENU =====
        void OnClickProfileBack(ClickEvent evt)
        {
            ShowMenu(m_MainMenu);
        }
        
        // ===== OPTIONS MENU =====
        void OnClickOptionsBack(ClickEvent evt)
        {
            ShowMenu(m_MainMenu);
        }
        
        // ===== HELPER METHODS =====
        void ShowMenu(VisualElement menuToShow)
        {
            m_MainMenu.style.display = DisplayStyle.None;
            m_SinglePlayerMenu.style.display = DisplayStyle.None;
            m_MultiplayerMenu.style.display = DisplayStyle.None;
            m_ProfileMenu.style.display = DisplayStyle.None;
            m_OptionsMenu.style.display = DisplayStyle.None;
            
            menuToShow.style.display = DisplayStyle.Flex;
        }
        
        void UpdateProfileDisplay()
        {
            var profile = PlayerProfileManager.GetCurrentProfile();
            m_PlayerNameLabel.text = $"Name: {profile.PlayerName}";
            m_WinsLabel.text = $"Wins: {profile.Wins}";
            m_KillsLabel.text = $"Kills: {profile.Kills}";
            
            // Populate friends list
            m_FriendsListScroll.Clear();
            foreach (var friend in profile.FriendsList)
            {
                var friendLabel = new Label(friend);
                friendLabel.style.fontSize = 20;
                friendLabel.style.color = new UnityEngine.Color(0.9f, 0.9f, 0.9f);
                friendLabel.style.marginBottom = 5;
                m_FriendsListScroll.Add(friendLabel);
            }
        }

        internal void DisableControlsUnsupportedInAutoconnectMode()
        {
            if (!CustomNetworkManager.Singleton.AutoConnectOnStartup)
            {
                return;
            }
            m_MultiplayerButton.SetEnabled(false);
            m_SinglePlayerButton.SetEnabled(false);
        }
    }
}
