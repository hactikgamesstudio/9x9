using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    public class RoomGenerator : MonoBehaviour
    {
        [SerializeField] private int m_GridSize = 9;
        [SerializeField] private float m_RoomSize = 10f;
        private GameObject[,,] m_RoomGrid;

        public void ApplyProfile(RoomGenProfile profile)
        {
            if (profile == null) return;
            m_GridSize = profile.GridSize;
            m_RoomSize = profile.RoomSize;
        }

        public void Generate()
        {
            m_RoomGrid = new GameObject[m_GridSize, m_GridSize, m_GridSize];
            Debug.Log($"[RoomGenerator] Generated {m_GridSize}x{m_GridSize}x{m_GridSize} grid");
        }

        public Vector3 GetCornerSpawnPosition(int index)
        {
            Vector3[] corners = {
                Vector3.zero,
                Vector3.right * m_RoomSize * 8,
                Vector3.forward * m_RoomSize * 8,
                (Vector3.right + Vector3.forward) * m_RoomSize * 8,
                Vector3.up * m_RoomSize * 8,
                (Vector3.right + Vector3.up) * m_RoomSize * 8,
                (Vector3.forward + Vector3.up) * m_RoomSize * 8,
                (Vector3.right + Vector3.forward + Vector3.up) * m_RoomSize * 8
            };
            return corners[Mathf.Clamp(index % corners.Length, 0, corners.Length - 1)];
        }

        public Vector3 GetExitPosition()
        {
            return new Vector3(4, 4, 4) * m_RoomSize;
        }

        public void OnRoomExited(Vector3Int gridPosition)
        {
            Debug.Log($"[RoomGenerator] Exited room at {gridPosition}");
        }
    }
}
