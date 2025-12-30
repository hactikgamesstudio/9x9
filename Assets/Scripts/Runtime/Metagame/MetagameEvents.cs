using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    public class EnterMatchmakerQueueEvent : AppEvent
    {
        public string QueueName { get; private set; }

        public EnterMatchmakerQueueEvent(string queueName)
        {
            QueueName = queueName;
        }
    }

    public class StartSinglePlayerModeEvent : AppEvent
    {
        public GameMode GameMode { get; set; } = GameMode.NewGame;
        public int BotCount { get; set; } = 3; // Default to 3 bots
    }

    /// <summary>
    /// Called to stop the matchmaker
    /// </summary>
    public class ExitMatchmakerQueueEvent : AppEvent { }

    /// <summary>
    /// Called after the matchmaking stops
    /// </summary>
    public class ExitedMatchmakerQueueEvent : AppEvent { }

    // Use canonical definitions from Shared/GameEvents.cs for
    // MatchLoadingEvent, ExitMatchLoadingEvent, and PlayerSignedIn.
}
