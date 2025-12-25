using Unity.Netcode;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    public class Player : NetworkBehaviour
    {
        /// <summary>
        /// Is this player currently alive?
        /// </summary>
        public bool IsAlive { get; set; } = true;
        
        /// <summary>
        /// Number of kills this player has
        /// </summary>
        public int Kills { get; set; } = 0;
        
        /// <summary>
        /// Number of times this player has died
        /// </summary>
        public int Deaths { get; set; } = 0;
        
        [ClientRpc]
        internal void OnClientPrepareGameClientRpc()
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            if (MetagameApplication.Instance)
            {
                MetagameApplication.Instance.Broadcast(new MatchEnteredEvent());
            }
            UnityEngine.Debug.Log("[Local client] Preparing game [Showing loading screen]");
            if (!IsServer) //the server already does this before asking clients to do the same
            {
                CustomNetworkManager.Singleton.InstantiateGameApplication();
            }
            OnClientReadyToStart();
        }

        internal void OnClientReadyToStart()
        {
            UnityEngine.Debug.Log("[Local client] Notifying server I'm ready");
            OnServerNotifiedOfClientReadinessServerRpc();
        }

        [ServerRpc]
        internal void OnServerNotifiedOfClientReadinessServerRpc()
        {
            UnityEngine.Debug.Log("[Server] I'm ready");
            CustomNetworkManager.Singleton.OnServerPlayerIsReady(this);
        }

        [ClientRpc]
        internal void OnClientStartGameClientRpc()
        {
            if (!IsLocalPlayer) { return; }
            var app = CustomNetworkManager.Singleton?.CurrentGameApp;
            if (app != null)
            {
                app.Broadcast(new StartMatchEvent(false, true));
            }
        }

        [ServerRpc]
        internal void OnPlayerAskedToWinServerRpc()
        {
            OnServerPlayerAskedToWin();
        }

        internal void OnServerPlayerAskedToWin()
        {
            var app = CustomNetworkManager.Singleton?.CurrentGameApp;
            if (app != null)
            {
                app.Broadcast(new EndMatchEvent(this));
            }
        }
    }
}
