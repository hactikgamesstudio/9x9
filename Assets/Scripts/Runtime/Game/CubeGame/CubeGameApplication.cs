using Unity.Netcode;
using Unity.Template.Multiplayer.NGO.Core;
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
        // Singleton instance for global access throughout the game session
        internal new static CubeGameApplication Instance { get; private set; }

        // Returns true if running as a headless dedicated server (no client rendering)
        internal bool IsDedicatedServer =>
            NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient;

        protected override void Awake()
        {
            // Initialize base MVC application structure (Model, View, Controller)
            base.Awake();

            // Set singleton instance for runtime access
            Instance = this;

            UnityEngine.Debug.Log("[9x9] CubeGameApplication initialized - Maze escape game ready");
        }

        void OnDestroy()
        {
            // Clear singleton reference when application is destroyed
            Instance = null;
        }
    }
}
