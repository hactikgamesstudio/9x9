using Unity.Netcode;
using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Manages the flow of the Game part of the application
    /// </summary>
    public class GameApplication : BaseApplication<GameModel, GameView, GameController>
    {
        internal static GameApplication Instance { get; private set; }

        internal bool IsDedicatedServer =>
            NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }
    }
}
