using Unity.Netcode;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Networked synchronizer for maze data
    /// </summary>
    public class MazeDataSynchronizer : NetworkBehaviour
    {
        public NetworkVariable<bool> MazeGenerated =
            new NetworkVariable<bool>(false);
    }
}
