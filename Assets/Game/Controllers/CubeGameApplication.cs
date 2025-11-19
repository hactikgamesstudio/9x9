using Unity.Netcode;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// 9x9 Cube Maze Game Application
    /// Manages the procedurally-generated maze escape game inspired by "Cube" (1997)
    /// </summary>
    public class CubeGameApplication
        : BaseApplication<CubeGameModel, CubeGameView, CubeGameController>
    {
        internal static new CubeGameApplication Instance { get; private set; }

        internal bool IsDedicatedServer =>
            NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            Debug.Log("[9x9] CubeGameApplication initialized - Maze escape game ready");
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
