namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Event fired when a player reaches the exit room
    /// </summary>
    internal class PlayerReachedExitEvent : AppEvent
    {
        public Player WinningPlayer { get; set; }

        public PlayerReachedExitEvent(Player player)
        {
            WinningPlayer = player;
        }
    }

    /// <summary>
    /// Event fired when a player dies from hazards or combat
    /// </summary>
    internal class PlayerDiedEvent : AppEvent
    {
        public Player DeadPlayer { get; set; }
        public string CauseOfDeath { get; set; }

        public PlayerDiedEvent(Player player, string cause = "Unknown")
        {
            DeadPlayer = player;
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
