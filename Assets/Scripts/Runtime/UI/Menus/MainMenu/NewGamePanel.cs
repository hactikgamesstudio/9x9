using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Template.Multiplayer.NGO.Runtime;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Shared;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu
{
    /// <summary>
    /// New Game configuration panel allowing players to select:
    /// - Number of bots (1-20) via dropdown
    /// - Difficulty level
    /// - Start the game
    /// </summary>
    public class NewGamePanel : UIMenuPanel
{
    [Header("Dropdowns (use TMP_Dropdown)")]
    [SerializeField] private TMP_Dropdown botCountDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI selectedBotCountText;
    [SerializeField] private TextMeshProUGUI selectedDifficultyText;

    private int selectedBotCount = 1;
    private string selectedDifficulty = "Normal";

    protected override void Start()
    {
        base.Start();
        if (startGameButton == null)
        {
            Debug.LogError("[NewGamePanel] startGameButton is NOT assigned. Assign it in the Inspector.");
        }
        if (botCountDropdown == null)
        {
            Debug.LogError("[NewGamePanel] botCountDropdown is NOT assigned. Assign it in the Inspector.");
        }
        if (difficultyDropdown == null)
        {
            Debug.LogError("[NewGamePanel] difficultyDropdown is NOT assigned. Assign it in the Inspector.");
        }
        InitializeBotCountDropdown();
        InitializeDifficultyDropdown();
        SetupButtonListeners();
    }

    private void InitializeBotCountDropdown()
    {
        botCountDropdown.ClearOptions();
        var botOptions = new System.Collections.Generic.List<string>();
        
        for (int i = 1; i <= 20; i++)
        {
            botOptions.Add(i.ToString());
        }
        
        botCountDropdown.AddOptions(botOptions);
        botCountDropdown.value = 0; // Default to 1 bot
        botCountDropdown.onValueChanged.AddListener(OnBotCountChanged);
    }

    private void InitializeDifficultyDropdown()
    {
        difficultyDropdown.ClearOptions();
        var difficultyOptions = new System.Collections.Generic.List<string>
        {
            "Easy",
            "Normal",
            "Hard",
            "Extreme"
        };
        
        difficultyDropdown.AddOptions(difficultyOptions);
        difficultyDropdown.value = 1; // Default to Normal
        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
    }

    private void SetupButtonListeners()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
            Debug.Log("[NewGamePanel] Start Game button listener attached");
        }
    }

    private void OnBotCountChanged(int index)
    {
        selectedBotCount = index + 1;
        if (selectedBotCountText != null)
        {
            selectedBotCountText.text = $"Bots: {selectedBotCount}";
        }
        Debug.Log($"Bot count changed to: {selectedBotCount}");
    }

    private void OnDifficultyChanged(int index)
    {
        selectedDifficulty = difficultyDropdown.options[index].text;
        if (selectedDifficultyText != null)
        {
            selectedDifficultyText.text = $"Difficulty: {selectedDifficulty}";
        }
        Debug.Log($"Difficulty changed to: {selectedDifficulty}");
    }

    private void OnStartGameClicked()
    {
        Debug.Log($"Starting game with {selectedBotCount} bots on {selectedDifficulty} difficulty");

        // Broadcast the start single player event via the metagame application
        var startEvent = new StartSinglePlayerModeEvent
        {
            GameMode = GameMode.NewGame,
            BotCount = selectedBotCount
        };

        var app = MetagameApplication.Instance;
        if (app == null)
        {
            Debug.LogError("[NewGamePanel] MetagameApplication.Instance is NULL. Ensure a MetagameApplication exists in MetagameScene.");
            return;
        }
        app.Broadcast(startEvent);

        Debug.Log($"Broadcasted StartSinglePlayerModeEvent with BotCount: {selectedBotCount}");
    }

    public override void GoBack()
    {
        menuManager.ShowSinglePlayerMenu();
    }
}
}
