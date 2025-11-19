namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Available game modes for single player and multiplayer
    /// </summary>
    public enum GameMode
    {
        NewGame,        // Start fresh game
        Continue,       // Load saved game
        Coop,           // Cooperative mode
        BattleRoyale,   // Last player standing
        Mode3x3,        // 3x3 grid variant
        Mode5x5,        // 5x5 grid variant
        Standard        // Default 9x9 grid
    }
}
