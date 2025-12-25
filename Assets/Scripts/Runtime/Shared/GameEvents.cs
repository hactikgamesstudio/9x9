using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Core app event types used across metagame and gameplay
    public class MatchEnteredEvent : AppEvent { }
    public class MatchLoadingEvent : AppEvent { }
    public class ExitMatchLoadingEvent : AppEvent { }

    public class StartMatchEvent : AppEvent
    {
        public bool IsServerStart { get; }
        public bool StartedByMatchmaker { get; }
        // Convenience flags for existing gameplay code
        public bool IsServer { get; }
        public bool IsClient { get; }

        public StartMatchEvent(bool isServerStart, bool startedByMatchmaker, bool isServer = false, bool isClient = false)
        {
            IsServerStart = isServerStart;
            StartedByMatchmaker = startedByMatchmaker;
            IsServer = isServer;
            IsClient = isClient;
        }

        /// <summary>
        /// Factory method: Compute IsServer/IsClient from NetworkManager state.
        /// </summary>
        public static StartMatchEvent CreateFromNetworkState(bool startedByMatchmaker = false)
        {
            bool isServer = Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsServer;
            bool isClient = Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsClient;
            return new StartMatchEvent(isServer, startedByMatchmaker, isServer, isClient);
        }
    }

    public class PlayerDisconnected : AppEvent
    {
        public ulong ClientId { get; }
        public PlayerDisconnected(ulong clientId) { ClientId = clientId; }
    }

    public class PlayerDiedEvent : AppEvent
    {
        public Player Player { get; }
        public PlayerDiedEvent(Player player) { Player = player; }
    }

    public class PlayerReachedExitEvent : AppEvent
    {
        public Player Winner { get; }
        public PlayerReachedExitEvent(Player winner) { Winner = winner; }
    }

    public class PlayerSignedIn : AppEvent
    {
        public bool Success { get; }
        public string PlayerId { get; }
        public PlayerSignedIn(bool success, string playerId)
        {
            Success = success;
            PlayerId = playerId;
        }
    }

    public class EndMatchEvent : AppEvent
    {
        public Player Winner { get; }
        public EndMatchEvent(Player winner) { Winner = winner; }
    }

    public class CountdownChangedEvent : AppEvent
    {
        public int NewValue { get; }
        public CountdownChangedEvent(int newValue) { NewValue = newValue; }
    }

    public class MatchResultComputedEvent : AppEvent
    {
        public ulong WinnerClientId { get; }
        public MatchResultComputedEvent(ulong winnerClientId) { WinnerClientId = winnerClientId; }
    }

    public class AllBotsEliminatedEvent : AppEvent { }
}
