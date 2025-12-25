using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Scriptable configuration for RoomGenerator defaults.
    /// Use to provide runtime fallbacks when scene wiring is incomplete.
    /// </summary>
    [CreateAssetMenu(
        fileName = "RoomGenConfig",
        menuName = "9x9/Room Generation Config",
        order = 0
    )]
    public class RoomGenConfig : ScriptableObject
    {
        [Header("Prefabs")]
        [Tooltip("Room prefabs to randomly choose from (standard rooms)")]
        public GameObject[] RoomTemplates;

        [Tooltip("Special room prefab for the center exit room (optional)")]
        public GameObject ExitRoomPrefab;

        [Tooltip("Special room prefab for corner spawn rooms (optional)")]
        public GameObject SpawnRoomPrefab;

        [Header("Grid & Generation")]
        [Tooltip("Size of the cube grid (9 = 9x9x9 = 729 rooms)")]
        public int GridSize = 9;

        [Tooltip("Physical size of each room in world units")]
        public float RoomSize = 10f;

        [Tooltip("Generate all 729 rooms or use sparse generation?")]
        public bool GenerateAllRooms = false;

        [Tooltip("If sparse, what percentage of rooms to generate (0.1 = 10%)")]
        [Range(0.1f, 1f)]
        public float SparseDensity = 0.3f;

        [Header("Doors & Rotation")]
        [Tooltip("Percentage of doors that are false/locked (0.0 to 0.5)")]
        [Range(0f, 0.5f)]
        public float FalseDoorChance = 0.15f;

        [Tooltip("Enable room rotation after doors close (Cube film mechanic)")]
        public bool EnableRoomRotation = true;

        [Tooltip("Time in seconds before rooms can rotate again")]
        public float RotationCooldown = 10f;

        [Header("Random Seed")]
        [Tooltip("Random seed for reproducible generation (0 = random)")]
        public int Seed = 0;
    }
}
