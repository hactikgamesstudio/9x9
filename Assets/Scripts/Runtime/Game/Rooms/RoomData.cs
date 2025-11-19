using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Stores metadata about a room in the 9x9x9 grid.
    /// Attach this to each room prefab.
    /// 
    /// **FOR BEGINNERS - WHAT IS ROOMDATA:**
    /// This component stores information about a room (like its grid position,
    /// which doors are open/blocked, and whether it's special like a spawn or exit room).
    /// </summary>
    public class RoomData : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Room Identity")]
        [Tooltip("Position in the 9x9x9 grid (set by RoomGenerator)")]
        public Vector3Int GridPosition;
        
        [Tooltip("Is this the exit room? (center of cube at 4,4,4)")]
        public bool IsExitRoom = false;
        
        [Tooltip("Is this a spawn room? (corners of cube)")]
        public bool IsSpawnRoom = false;
        
        [Header("Door References")]
        [Tooltip("Door GameObjects - set by RoomGenerator")]
        public GameObject DoorNorth;
        public GameObject DoorSouth;
        public GameObject DoorEast;
        public GameObject DoorWest;
        public GameObject DoorUp;
        public GameObject DoorDown;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Dictionary storing which doors are open (true) or false/blocked (false).
        /// Key: direction vector (e.g., Vector3Int.right for east door)
        /// Value: true = real door, false = false door (blocked)
        /// </summary>
        public Dictionary<Vector3Int, bool> Doors { get; private set; }
        
        /// <summary>
        /// Time of last rotation (for cooldown system)
        /// </summary>
        public float LastRotationTime { get; private set; }
        
        #endregion
        
        #region Unity Lifecycle
        
        void Awake()
        {
            // Initialize door dictionary
            Doors = new Dictionary<Vector3Int, bool>
            {
                { Vector3Int.right, true },      // East
                { Vector3Int.left, true },       // West
                { Vector3Int.up, true },         // Up
                { Vector3Int.down, true },       // Down
                { new Vector3Int(0, 0, 1), true },  // North
                { new Vector3Int(0, 0, -1), true }  // South
            };
            
            LastRotationTime = -999f; // Allow immediate rotation
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Check if a door in a specific direction is open (true) or blocked (false).
        /// </summary>
        /// <param name="direction">Direction vector (e.g., Vector3Int.right)</param>
        /// <returns>True if door is open, false if blocked</returns>
        public bool IsDoorOpen(Vector3Int direction)
        {
            if (Doors.ContainsKey(direction))
            {
                return Doors[direction];
            }
            
            Debug.LogWarning($"Door direction {direction} not found in room {name}");
            return false;
        }
        
        /// <summary>
        /// Rotate this room 90 degrees on a random axis (Cube film mechanic).
        /// </summary>
        /// <param name="cooldown">Minimum time between rotations</param>
        public void RotateRoom(float cooldown)
        {
            if (Time.time - LastRotationTime < cooldown) return;
            
            // Choose random axis (X, Y, or Z)
            int axis = Random.Range(0, 3);
            Vector3 rotationAxis = axis == 0 ? Vector3.right : (axis == 1 ? Vector3.up : Vector3.forward);
            
            // Rotate 90 degrees
            transform.Rotate(rotationAxis, 90f);
            
            LastRotationTime = Time.time;
            
            Debug.Log($"Room {name} at {GridPosition} rotated around {rotationAxis}");
        }
        
        #endregion
        
        #region Editor Helpers
        
        /// <summary>
        /// Draw gizmo showing room bounds and grid position.
        /// </summary>
        void OnDrawGizmos()
        {
            // Draw room bounds
            Gizmos.color = IsExitRoom ? Color.green : (IsSpawnRoom ? Color.cyan : Color.white);
            Gizmos.DrawWireCube(transform.position, Vector3.one * 10f);
            
            // Draw grid position label
#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 6f, 
                $"{GridPosition}\n{(IsExitRoom ? "EXIT" : IsSpawnRoom ? "SPAWN" : "")}"
            );
#endif
        }
        
        #endregion
    }
}
