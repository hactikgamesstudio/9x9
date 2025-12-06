using UnityEngine;
using UnityEngine.UIElements;
using Unity.Template.Multiplayer.NGO.Core;
using UnityEngine.InputSystem;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    internal class MainMenuView : View<MetagameApplication>, PlayerInputActions.IUIActions
    {
        // Input Actions
        PlayerInputActions m_InputActions;
        
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
        DropdownField m_BotCountDropdown;
        Label m_BotCountLabel;
        
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
            if (m_ContinueButton != null)
                m_ContinueButton.RegisterCallback<ClickEvent>(OnClickContinue);
            else
                UnityEngine.Debug.LogError("[MainMenuView] continueButton not found in UI!");
                
            m_NewGameButton = m_Root.Q<Button>("newGameButton");
            if (m_NewGameButton != null)
                m_NewGameButton.RegisterCallback<ClickEvent>(OnClickNewGame);
            else
                UnityEngine.Debug.LogError("[MainMenuView] newGameButton not found in UI!");
                
            m_CoopButton = m_Root.Q<Button>("coopButton");
            if (m_CoopButton != null)
                m_CoopButton.RegisterCallback<ClickEvent>(OnClickCoop);
            else
                UnityEngine.Debug.LogError("[MainMenuView] coopButton not found in UI!");
                
            m_SpBackButton = m_Root.Q<Button>("spBackButton");
            if (m_SpBackButton != null)
                m_SpBackButton.RegisterCallback<ClickEvent>(OnClickSpBack);
            else
                UnityEngine.Debug.LogError("[MainMenuView] spBackButton not found in UI!");
            
            // Bot count dropdown (0-8 bots, default 3)
            m_BotCountDropdown = m_Root.Q<DropdownField>("botCountDropdown");
            if (m_BotCountDropdown == null)
            {
                // Create dropdown if not in UXML
                var botOptions = new System.Collections.Generic.List<string> 
                { "0 Bots", "1 Bot", "2 Bots", "3 Bots", "4 Bots", "5 Bots", "6 Bots", "7 Bots", "8 Bots" };
                m_BotCountDropdown = new DropdownField("Number of Bots", botOptions, 3);
                m_BotCountDropdown.name = "botCountDropdown";
                if (m_SinglePlayerMenu != null)
                {
                    m_SinglePlayerMenu.Insert(1, m_BotCountDropdown);
                }
            }
            else
            {
                var botOptions = new System.Collections.Generic.List<string> 
                { "0 Bots", "1 Bot", "2 Bots", "3 Bots", "4 Bots", "5 Bots", "6 Bots", "7 Bots", "8 Bots" };
                m_BotCountDropdown.choices = botOptions;
                m_BotCountDropdown.index = 3; // Default to 3 bots
            }
            
            m_BotCountLabel = m_Root.Q<Label>("botCountLabel");
            if (m_BotCountLabel == null)
            {
                m_BotCountLabel = new Label("Select bot count for singleplayer/co-op modes");
                m_BotCountLabel.name = "botCountLabel";
                if (m_SinglePlayerMenu != null)
                {
                    m_SinglePlayerMenu.Insert(2, m_BotCountLabel);
                }
            }
            else
            {
                m_BotCountLabel.text = "Select bot count for singleplayer/co-op modes";
            }

            _ = m_BotCountDropdown.RegisterValueChangedCallback(evt =>
            {
                // Update label with selected value (optional)
                UnityEngine.Debug.Log($"Bot count changed to: {evt.newValue}");
            });
            
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
            
            // Initialize Input Actions for UI navigation
            m_InputActions = new PlayerInputActions();
            m_InputActions.UI.SetCallbacks(this);
            m_InputActions.UI.Enable();
            
            // Initialize profile data
            UpdateProfileDisplay();
            
            // Initialize menu navigation with main menu
            ShowMenu(m_MainMenu);
        }

        private VisualElement m_CurrentMenu;
        private int m_CurrentButtonIndex = 0;
        private System.Collections.Generic.List<Button> m_CurrentMenuButtons = new System.Collections.Generic.List<Button>();
        private float m_LastNavigationTime = 0f;
        private const float NAVIGATION_DEBOUNCE = 0.2f; // Debounce time in seconds

        void NavigateMenu(int direction)
        {
            // Debounce rapid navigation inputs
            if (Time.time - m_LastNavigationTime < NAVIGATION_DEBOUNCE)
            {
                UnityEngine.Debug.Log($"[MainMenuView] Navigation debounced (too soon)");
                return;
            }
            m_LastNavigationTime = Time.time;
            
            // Update current menu if it changed
            if (m_CurrentMenu == null)
            {
                m_CurrentMenu = m_MainMenu;
            }
            
            // Rebuild button list for current menu
            RebuildMenuButtonList();
            
            if (m_CurrentMenuButtons.Count == 0)
                return;
            
            UnityEngine.Debug.Log($"[MainMenuView] Before navigation - Current index: {m_CurrentButtonIndex}, Button count: {m_CurrentMenuButtons.Count}, Direction: {direction}");
            
            // Remove highlight from previously focused button
            if (m_CurrentButtonIndex >= 0 && m_CurrentButtonIndex < m_CurrentMenuButtons.Count)
            {
                var oldButton = m_CurrentMenuButtons[m_CurrentButtonIndex];
                UnityEngine.Debug.Log($"[MainMenuView] Unhighlighting button at index {m_CurrentButtonIndex}: {oldButton.text}");
                oldButton.style.backgroundColor = new Color(0.239f, 0.467f, 0.761f, 1f); // Blue (default)
                oldButton.Blur();
            }
            
            // Move to next/previous button
            int oldIndex = m_CurrentButtonIndex;
            m_CurrentButtonIndex = (m_CurrentButtonIndex + direction + m_CurrentMenuButtons.Count) % m_CurrentMenuButtons.Count;
            
            UnityEngine.Debug.Log($"[MainMenuView] After calculation - Old index: {oldIndex}, New index: {m_CurrentButtonIndex}");
            
            // Highlight the new button
            var newButton = m_CurrentMenuButtons[m_CurrentButtonIndex];
            newButton.style.backgroundColor = new Color(1f, 0.784f, 0f, 1f); // Yellow (highlight)
            newButton.Focus();
            
            UnityEngine.Debug.Log($"[MainMenuView] Navigated to button index {m_CurrentButtonIndex}: {newButton.text}");
        }

        void RebuildMenuButtonList()
        {
            m_CurrentMenuButtons.Clear();
            // DON'T reset m_CurrentButtonIndex here - it breaks navigation!
            
            if (m_CurrentMenu == m_MainMenu)
            {
                if (m_SinglePlayerButton != null) m_CurrentMenuButtons.Add(m_SinglePlayerButton);
                if (m_MultiplayerButton != null) m_CurrentMenuButtons.Add(m_MultiplayerButton);
                if (m_ProfileButton != null) m_CurrentMenuButtons.Add(m_ProfileButton);
                if (m_OptionsButton != null) m_CurrentMenuButtons.Add(m_OptionsButton);
                if (m_QuitButton != null) m_CurrentMenuButtons.Add(m_QuitButton);
            }
            else if (m_CurrentMenu == m_SinglePlayerMenu)
            {
                if (m_ContinueButton != null) m_CurrentMenuButtons.Add(m_ContinueButton);
                if (m_NewGameButton != null) m_CurrentMenuButtons.Add(m_NewGameButton);
                if (m_CoopButton != null) m_CurrentMenuButtons.Add(m_CoopButton);
                if (m_SpBackButton != null) m_CurrentMenuButtons.Add(m_SpBackButton);
            }
            else if (m_CurrentMenu == m_MultiplayerMenu)
            {
                if (m_BattleRoyaleButton != null) m_CurrentMenuButtons.Add(m_BattleRoyaleButton);
                if (m_Mode3x3Button != null) m_CurrentMenuButtons.Add(m_Mode3x3Button);
                if (m_Mode5x5Button != null) m_CurrentMenuButtons.Add(m_Mode5x5Button);
                if (m_MpCoopButton != null) m_CurrentMenuButtons.Add(m_MpCoopButton);
                if (m_MpBackButton != null) m_CurrentMenuButtons.Add(m_MpBackButton);
            }
            else if (m_CurrentMenu == m_ProfileMenu)
            {
                if (m_ProfileBackButton != null) m_CurrentMenuButtons.Add(m_ProfileBackButton);
            }
            else if (m_CurrentMenu == m_OptionsMenu)
            {
                if (m_OptionsBackButton != null) m_CurrentMenuButtons.Add(m_OptionsBackButton);
            }
        }

        void ConfirmCurrentSelection()
        {
            if (m_CurrentMenuButtons.Count == 0)
                return;
            
            var focusedButton = m_CurrentMenuButtons[m_CurrentButtonIndex];
            if (focusedButton != null)
            {
                UnityEngine.Debug.Log($"[MainMenuView] Confirming button: {focusedButton.text}");
                
                // Programmatically trigger the button click
                // We need to use reflection to access the internal clicked event
                var clickEvent = typeof(Button).GetField("clicked", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (clickEvent != null)
                {
                    var clickedEventValue = clickEvent.GetValue(focusedButton) as System.Action;
                    clickedEventValue?.Invoke();
                    UnityEngine.Debug.Log($"[MainMenuView] Button clicked via reflection");
                }
                else
                {
                    // Fallback: manually call the appropriate handler
                    UnityEngine.Debug.Log($"[MainMenuView] Using fallback click handling");
                    
                    if (focusedButton == m_SinglePlayerButton) OnClickSinglePlayer(null);
                    else if (focusedButton == m_MultiplayerButton) OnClickMultiplayer(null);
                    else if (focusedButton == m_ProfileButton) OnClickProfile(null);
                    else if (focusedButton == m_OptionsButton) OnClickOptions(null);
                    else if (focusedButton == m_QuitButton) OnClickQuit(null);
                    else if (focusedButton == m_ContinueButton) OnClickContinue(null);
                    else if (focusedButton == m_NewGameButton) OnClickNewGame(null);
                    else if (focusedButton == m_CoopButton) OnClickCoop(null);
                    else if (focusedButton == m_SpBackButton) { ShowMenu(m_MainMenu); m_CurrentMenu = m_MainMenu; }
                    else if (focusedButton == m_BattleRoyaleButton) OnClickBattleRoyale(null);
                    else if (focusedButton == m_Mode3x3Button) OnClickMode3x3(null);
                    else if (focusedButton == m_Mode5x5Button) OnClickMode5x5(null);
                    else if (focusedButton == m_MpCoopButton) OnClickMpCoop(null);
                    else if (focusedButton == m_MpBackButton) { ShowMenu(m_MainMenu); m_CurrentMenu = m_MainMenu; }
                    else if (focusedButton == m_ProfileBackButton) { ShowMenu(m_MainMenu); m_CurrentMenu = m_MainMenu; }
                    else if (focusedButton == m_OptionsBackButton) { ShowMenu(m_MainMenu); m_CurrentMenu = m_MainMenu; }
                }
            }
        }

        void HandleBackButton()
        {
            if (m_CurrentMenu == m_MainMenu)
            {
                // From main menu, back means quit
                OnClickQuit(null);
            }
            else if (m_CurrentMenu == m_SinglePlayerMenu)
            {
                ShowMenu(m_MainMenu);
                m_CurrentMenu = m_MainMenu;
            }
            else if (m_CurrentMenu == m_MultiplayerMenu)
            {
                ShowMenu(m_MainMenu);
                m_CurrentMenu = m_MainMenu;
            }
            else if (m_CurrentMenu == m_ProfileMenu)
            {
                ShowMenu(m_MainMenu);
                m_CurrentMenu = m_MainMenu;
            }
            else if (m_CurrentMenu == m_OptionsMenu)
            {
                ShowMenu(m_MainMenu);
                m_CurrentMenu = m_MainMenu;
            }
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
            
            // Disable and dispose Input Actions
            if (m_InputActions != null)
            {
                m_InputActions.UI.Disable();
                m_InputActions.UI.RemoveCallbacks(this);
                m_InputActions.Dispose();
            }
        }

        // ===== MAIN MENU BUTTONS =====
        void OnClickSinglePlayer(ClickEvent evt)
        {
            UnityEngine.Debug.Log("[MainMenuView] Single Player button clicked");
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
            UnityEngine.Debug.Log("[MainMenuView] Continue button clicked");
            Broadcast(new StartSinglePlayerModeEvent { GameMode = GameMode.Continue });
        }
        
        void OnClickNewGame(ClickEvent evt)
        {
            UnityEngine.Debug.Log("[MainMenuView] New Game button clicked");
            UnityEngine.Debug.Log($"[MainMenuView] BotCountDropdown index: {m_BotCountDropdown?.index ?? -1}");
            
            int botCount = m_BotCountDropdown?.index ?? 3;
            UnityEngine.Debug.Log($"[MainMenuView] Broadcasting StartSinglePlayerModeEvent with BotCount: {botCount}");
            
            var evt2 = new StartSinglePlayerModeEvent 
            { 
                GameMode = GameMode.NewGame,
                BotCount = botCount
            };
            
            Broadcast(evt2);
            UnityEngine.Debug.Log("[MainMenuView] Event broadcasted");
        }
        
        void OnClickCoop(ClickEvent evt)
        {
            UnityEngine.Debug.Log("[MainMenuView] Coop button clicked");
            Broadcast(new StartSinglePlayerModeEvent 
            { 
                GameMode = GameMode.Coop,
                BotCount = m_BotCountDropdown?.index ?? 3
            });
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
            // Blur all buttons in the current menu before switching
            if (m_CurrentMenuButtons != null)
            {
                foreach (var button in m_CurrentMenuButtons)
                {
                    if (button != null)
                    {
                        button.style.backgroundColor = new Color(0.239f, 0.467f, 0.761f, 1f); // Blue (default)
                        button.Blur();
                    }
                }
            }
            
            m_MainMenu.style.display = DisplayStyle.None;
            m_SinglePlayerMenu.style.display = DisplayStyle.None;
            m_MultiplayerMenu.style.display = DisplayStyle.None;
            m_ProfileMenu.style.display = DisplayStyle.None;
            m_OptionsMenu.style.display = DisplayStyle.None;
            
            menuToShow.style.display = DisplayStyle.Flex;
            
            // Update current menu reference for keyboard navigation
            m_CurrentMenu = menuToShow;
            m_CurrentButtonIndex = 0;
            RebuildMenuButtonList();
            
            // Highlight first button in the new menu
            if (m_CurrentMenuButtons.Count > 0)
            {
                m_CurrentMenuButtons[0].style.backgroundColor = new Color(1f, 0.784f, 0f, 1f); // Yellow (highlight)
                m_CurrentMenuButtons[0].Focus();
                UnityEngine.Debug.Log($"[MainMenuView] Focused first button: {m_CurrentMenuButtons[0].text}");
            }
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

        // ===== INPUT ACTIONS CALLBACKS (IUIActions Interface) =====
        // These are called automatically by the Input System when UI actions are triggered
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
            // PassThrough actions fire constantly, so we need to filter
            Vector2 navigation = context.ReadValue<Vector2>();
            
            // Ignore zero/tiny values (deadzone for analog sticks)
            if (Mathf.Abs(navigation.x) < 0.1f && Mathf.Abs(navigation.y) < 0.1f)
                return;
            
            // Only respond once per button press (ignore continuous holding)
            if (!context.performed)
                return;
            
            UnityEngine.Debug.Log($"[MainMenuView] OnNavigate: {navigation} (phase: {context.phase})");
            
            // Vertical navigation (Y-axis: positive = up, negative = down in input terms)
            // But for UI, we want up to go to previous button (index -1)
            if (navigation.y > 0.5f)
            {
                UnityEngine.Debug.Log("[MainMenuView] Navigating UP");
                NavigateMenu(-1); // Up = previous button
            }
            else if (navigation.y < -0.5f)
            {
                UnityEngine.Debug.Log("[MainMenuView] Navigating DOWN");
                NavigateMenu(1); // Down = next button
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            UnityEngine.Debug.Log("[MainMenuView] OnSubmit pressed");
            ConfirmCurrentSelection();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            UnityEngine.Debug.Log("[MainMenuView] OnCancel pressed");
            HandleBackButton();
        }

        // These UI actions we don't need for menu navigation, but must implement the interface
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnScrollWheel(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
    }
}
