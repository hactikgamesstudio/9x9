using UnityEngine;
using System.Collections.Generic;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Room metadata and state tracking.
    /// Manages door open/close, hazards, pickups, and exit conditions.
    /// </summary>
    public class RoomData : MonoBehaviour
    {
        public Vector3Int GridPosition = Vector3Int.zero;
        public bool IsExitRoom = false;
        public bool IsSpawnRoom = false;
        private Dictionary<Vector3Int, bool> m_DoorStates = new Dictionary<Vector3Int, bool>();

        void Start()
        {
            m_DoorStates[Vector3Int.up] = true;
            m_DoorStates[Vector3Int.down] = true;
            m_DoorStates[Vector3Int.forward] = true;
            m_DoorStates[Vector3Int.back] = true;
            m_DoorStates[Vector3Int.right] = true;
            m_DoorStates[Vector3Int.left] = true;
        }

        public bool IsDoorOpen(Vector3Int direction)
        {
            m_DoorStates.TryGetValue(direction, out bool isOpen);
            return isOpen;
        }

        public void SetDoorOpen(Vector3Int direction, bool open)
        {
            m_DoorStates[direction] = open;
        }
    }
}
