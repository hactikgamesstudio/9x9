namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Available game modes for single player and multiplayer
    /// </summary>
    public enum GameMode
    {
        NewGame,        // Start fresh game (Story mode with deterministic generation)
        Continue,       // Load saved game
        Coop,           // Cooperative mode (deterministic generation)
        BattleRoyale,   // Last player standing (random generation)
        ThreeByThree,   // 3×3 grid variant (random generation)
        FiveByFive,     // 5×5 grid variant (random generation)
        Standard        // Default 9×9 grid (can be random or deterministic based on profile)
    }
}
