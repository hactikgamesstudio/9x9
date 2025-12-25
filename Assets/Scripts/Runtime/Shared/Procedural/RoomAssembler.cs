using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    [System.Flags]
    public enum RoomDoorFlags
    {
        None = 0,
        North = 1 << 0,
        South = 1 << 1,
        East = 1 << 2,
        West = 1 << 3,
        All = North | South | East | West
    }

    public class RoomAssembler : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private RoomPieceSet m_PieceSet;
        [SerializeField] private bool m_AssembleOnAwake = true;
        [SerializeField] private int m_SeedOverride = 0; // 0 = random at runtime

        [Header("Socket Names (find if not assigned)")]
        [SerializeField] private Transform m_SocketFloor;
        [SerializeField] private Transform m_SocketCeiling;
        [SerializeField] private Transform m_SocketNorth;
        [SerializeField] private Transform m_SocketSouth;
        [SerializeField] private Transform m_SocketEast;
        [SerializeField] private Transform m_SocketWest;

        [SerializeField] private string m_FloorSocketName = "socket_floor";
        [SerializeField] private string m_CeilingSocketName = "socket_ceiling";
        [SerializeField] private string m_NorthSocketName = "socket_north";
        [SerializeField] private string m_SouthSocketName = "socket_south";
        [SerializeField] private string m_EastSocketName = "socket_east";
        [SerializeField] private string m_WestSocketName = "socket_west";

        private readonly List<GameObject> m_SpawnedPieces = new List<GameObject>();

        public void Assemble(int? seed = null, RoomDoorFlags doorFlags = RoomDoorFlags.None)
        {
            if (m_PieceSet == null)
            {
                Debug.LogWarning($"[RoomAssembler] No RoomPieceSet assigned on {name}");
                return;
            }

            EnsureSockets();
            ClearSpawned();

            int resolvedSeed = seed ?? (m_SeedOverride != 0 ? m_SeedOverride : Random.Range(1, int.MaxValue));
            var rng = new System.Random(resolvedSeed);

            // Floor
            if (m_SocketFloor != null)
            {
                var prefab = m_PieceSet.GetRandomFloor(rng);
                Spawn(prefab, m_SocketFloor);
            }

            // Ceiling
            if (m_SocketCeiling != null)
            {
                var prefab = m_PieceSet.GetRandomCeiling(rng);
                Spawn(prefab, m_SocketCeiling);
            }

            // Walls (assume sockets face outward correctly)
            if (m_SocketNorth != null)
            {
                var prefab = (doorFlags & RoomDoorFlags.North) != 0
                    ? m_PieceSet.GetRandomDoorWallNorth(rng)
                    : m_PieceSet.GetRandomWallNorth(rng);
                Spawn(prefab, m_SocketNorth);
            }
            if (m_SocketSouth != null)
            {
                var prefab = (doorFlags & RoomDoorFlags.South) != 0
                    ? m_PieceSet.GetRandomDoorWallSouth(rng)
                    : m_PieceSet.GetRandomWallSouth(rng);
                Spawn(prefab, m_SocketSouth);
            }
            if (m_SocketEast != null)
            {
                var prefab = (doorFlags & RoomDoorFlags.East) != 0
                    ? m_PieceSet.GetRandomDoorWallEast(rng)
                    : m_PieceSet.GetRandomWallEast(rng);
                Spawn(prefab, m_SocketEast);
            }
            if (m_SocketWest != null)
            {
                var prefab = (doorFlags & RoomDoorFlags.West) != 0
                    ? m_PieceSet.GetRandomDoorWallWest(rng)
                    : m_PieceSet.GetRandomWallWest(rng);
                Spawn(prefab, m_SocketWest);
            }
        }

        private void Spawn(GameObject prefab, Transform socket)
        {
            if (prefab == null || socket == null) return;
            var go = Instantiate(prefab, socket.position, socket.rotation, socket);
            m_SpawnedPieces.Add(go);
        }

        private void ClearSpawned()
        {
            for (int i = m_SpawnedPieces.Count - 1; i >= 0; i--)
            {
                var go = m_SpawnedPieces[i];
                if (go != null) DestroyImmediate(go);
            }
            m_SpawnedPieces.Clear();

            // Also clear any existing children under sockets (for iteration in editor)
            foreach (var t in GetAllSockets())
            {
                if (t == null) continue;
                for (int i = t.childCount - 1; i >= 0; i--)
                {
                    var c = t.GetChild(i);
                    if (c != null) DestroyImmediate(c.gameObject);
                }
            }
        }

        private IEnumerable<Transform> GetAllSockets()
        {
            yield return m_SocketFloor;
            yield return m_SocketCeiling;
            yield return m_SocketNorth;
            yield return m_SocketSouth;
            yield return m_SocketEast;
            yield return m_SocketWest;
        }

        private void EnsureSockets()
        {
            if (m_SocketFloor == null) m_SocketFloor = FindSocket(m_FloorSocketName);
            if (m_SocketCeiling == null) m_SocketCeiling = FindSocket(m_CeilingSocketName);
            if (m_SocketNorth == null) m_SocketNorth = FindSocket(m_NorthSocketName);
            if (m_SocketSouth == null) m_SocketSouth = FindSocket(m_SouthSocketName);
            if (m_SocketEast == null) m_SocketEast = FindSocket(m_EastSocketName);
            if (m_SocketWest == null) m_SocketWest = FindSocket(m_WestSocketName);
        }

        private Transform FindSocket(string name)
        {
            var t = transform.Find(name);
            if (t == null)
            {
                // Try deep search
                var trs = GetComponentsInChildren<Transform>(true);
                foreach (var tr in trs)
                {
                    if (tr.name == name) return tr;
                }
            }
            return t;
        }

        private void Awake()
        {
            if (m_AssembleOnAwake)
            {
                Assemble(m_SeedOverride == 0 ? null : m_SeedOverride);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Keep sockets resolved in editor for quick iteration
            EnsureSockets();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            foreach (var t in GetAllSockets())
            {
                if (t == null) continue;
                Gizmos.DrawWireCube(t.position, Vector3.one * 0.2f);
                Gizmos.DrawLine(t.position, t.position + t.forward * 0.5f);
            }
        }
#endif
    }
}
