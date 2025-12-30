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
            // Note: MetagameApplication check removed - handled by CustomNetworkManager instead
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
            // Start event is now broadcasted by CustomNetworkManager directly
            UnityEngine.Debug.Log("[Local client] Game starting");
        }

        [ServerRpc]
        internal void OnPlayerAskedToWinServerRpc()
        {
            OnServerPlayerAskedToWin();
        }

        internal void OnServerPlayerAskedToWin()
        {
            // Notify CustomNetworkManager to broadcast win event with player's OwnerClientId
            CustomNetworkManager.Singleton?.BroadcastPlayerWin(OwnerClientId);
        }
    }
}
