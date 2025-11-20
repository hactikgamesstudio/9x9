namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Event fired when a player reaches the exit room
    /// </summary>
    public class PlayerReachedExitEvent
    {
        public ulong WinningPlayerId { get; set; }

        public PlayerReachedExitEvent(ulong playerId)
        {
            WinningPlayerId = playerId;
        }
    }

    /// <summary>
    /// Event fired when a player dies from hazards or combat
    /// </summary>
    public class PlayerDiedEvent
    {
        public ulong DeadPlayerId { get; set; }
        public string CauseOfDeath { get; set; }

        public PlayerDiedEvent(ulong playerId, string cause = "Unknown")
        {
            DeadPlayerId = playerId;
            CauseOfDeath = cause;
        }
    }

    /// <summary>
    /// Networked synchronizer for maze data
    /// </summary>
    public class MazeDataSynchronizer : Unity.Netcode.NetworkBehaviour
    {
        public Unity.Netcode.NetworkVariable<bool> MazeGenerated =
            new Unity.Netcode.NetworkVariable<bool>(false);
    }
}
