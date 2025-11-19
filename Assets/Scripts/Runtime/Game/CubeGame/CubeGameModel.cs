using Unity.Netcode;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Model for the 9x9 Cube Maze Game
    /// Stores game state, player positions, maze configuration, etc.
    /// </summary>
    public class CubeGameModel : Model<CubeGameApplication>
    {
        /// <summary>
        /// Reference to the networked maze synchronizer
        /// </summary>
        internal MazeDataSynchronizer MazeDataSynchronizer { get; set; }

        /// <summary>
        /// Has the maze been generated yet?
        /// </summary>
        internal bool MazeGenerated
        {
            get { return MazeDataSynchronizer != null && MazeDataSynchronizer.MazeGenerated.Value; }
            set
            {
                if (MazeDataSynchronizer != null)
                    MazeDataSynchronizer.MazeGenerated.Value = value;
            }
        }

        /// <summary>
        /// Has the match started (all players spawned)?
        /// </summary>
        internal bool MatchStarted { get; set; }

        /// <summary>
        /// Has the match ended (player won or all died)?
        /// </summary>
        internal bool MatchEnded { get; set; }

        /// <summary>
        /// Allow player reconnection after disconnect?
        /// </summary>
        internal bool AllowReconnection { get; set; } = false;

        /// <summary>
        /// Current game mode being played
        /// </summary>
        internal GameMode CurrentGameMode { get; set; } = GameMode.NewGame;

        /// <summary>
        /// Number of players alive
        /// </summary>
        internal NetworkVariable<int> PlayersAlive { get; set; } = new NetworkVariable<int>(0);

        /// <summary>
        /// Match timer (optional - for Battle Royale mode)
        /// </summary>
        internal NetworkVariable<uint> MatchTimer { get; set; } = new NetworkVariable<uint>(0);

        void Awake()
        {
            MazeDataSynchronizer = Object.FindFirstObjectByType<MazeDataSynchronizer>();
            if (MazeDataSynchronizer == null)
            {
                Debug.LogWarning(
                    "[9x9] MazeDataSynchronizer not found! Network synchronization disabled."
                );
            }
        }
    }
}
