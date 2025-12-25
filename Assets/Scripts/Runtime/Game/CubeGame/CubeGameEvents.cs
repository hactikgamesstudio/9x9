namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Networked synchronizer for maze data
    /// </summary>
    public class MazeDataSynchronizer : Unity.Netcode.NetworkBehaviour
    {
        public Unity.Netcode.NetworkVariable<bool> MazeGenerated =
            new Unity.Netcode.NetworkVariable<bool>(false);
    }
}
