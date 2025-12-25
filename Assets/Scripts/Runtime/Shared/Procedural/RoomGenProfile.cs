using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Scriptable Object defining room generation parameters for different game modes.
    /// Story/Co-op modes use deterministic seeds for repeatability.
    /// BR/5x5/3x3 modes use random seeds for variety.
    /// </summary>
    [CreateAssetMenu(fileName = "RoomGenProfile", menuName = "9x9/Room Generation Profile")]
    public class RoomGenProfile : ScriptableObject
    {
        [Header("Grid Configuration")]
        [Tooltip("Grid dimensions (e.g., 9 = 9×9×9, 5 = 5×5×5, 3 = 3×3×3)")]
        public int GridSize = 9;

        [Tooltip("World units per room cube")]
        public float RoomSize = 10f;

        [Header("Generation Mode")]
        [Tooltip("If true, generates all cells. If false, uses sparse generation with SparseDensity.")]
        public bool GenerateAllRooms = false;

        [Tooltip("Percentage of grid cells to generate in sparse mode (0.0–1.0). Recommended: 0.25–0.35")]
        [Range(0f, 1f)]
        public float SparseDensity = 0.3f;

        [Header("Randomization")]
        [Tooltip("0 = random seed each time (BR/5x5/3x3). Non-zero = deterministic seed (Story/Co-op).")]
        public int Seed = 0;

        [Header("Room Templates")]
        [Tooltip("Standard room prefabs for randomized selection")]
        public GameObject[] RoomTemplates;

        [Tooltip("Prefab for exit room (typically placed at center)")]
        public GameObject ExitRoomPrefab;

        [Tooltip("Prefab for spawn rooms (placed at corners)")]
        public GameObject SpawnRoomPrefab;

        [Header("Path Guarantees")]
        [Tooltip("Ensure pathfinding from all corners to center exit")]
        public bool GuaranteeCornerToCenterPaths = true;

        /// <summary>
        /// Returns the effective seed: if configured seed is 0, generates a fresh random one.
        /// </summary>
        public int GetEffectiveSeed()
        {
            return Seed == 0 ? Random.Range(int.MinValue, int.MaxValue) : Seed;
        }
    }
}
