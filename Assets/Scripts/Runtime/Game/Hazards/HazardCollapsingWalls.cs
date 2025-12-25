using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Collapsing Walls - Secondary walls collapse inward on 2 random sides
    /// Crushes anything in the collapse zone
    /// </summary>
    public class HazardCollapsingWalls : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_WallNorth;
        [SerializeField]
        private GameObject m_WallSouth;
        [SerializeField]
        private GameObject m_WallEast;
        [SerializeField]
        private GameObject m_WallWest;

        [SerializeField]
        private float m_CollapseSpeed = 3f;
        [SerializeField]
        private float m_CollapseDuration = 2f;
        [SerializeField]
        private float m_CollapseCooldown = 8f;
        [SerializeField]
        private int m_CrushDamage = 50;

        private float m_LastCollapseTime = -999f;
        private bool m_IsCollapsing = false;
        private GameObject[] m_ActiveWalls = new GameObject[2];
        private Vector3[] m_OriginalPositions = new Vector3[4];
        private float m_CollapseTimer = 0f;

        private enum WallDirection
        {
            North = 0,
            South = 1,
            East = 2,
            West = 3,
        }

        void Start()
        {
            // Store original positions
            if (m_WallNorth != null)
                m_OriginalPositions[0] = m_WallNorth.transform.position;
            if (m_WallSouth != null)
                m_OriginalPositions[1] = m_WallSouth.transform.position;
            if (m_WallEast != null)
                m_OriginalPositions[2] = m_WallEast.transform.position;
            if (m_WallWest != null)
                m_OriginalPositions[3] = m_WallWest.transform.position;

            Debug.Log("[CollapsingWalls] Initialized - Collapse Speed: " + m_CollapseSpeed);
        }

        void Update()
        {
            // Check if we should trigger a collapse
            if (!m_IsCollapsing && Time.time >= m_LastCollapseTime + m_CollapseCooldown)
            {
                TriggerRandomCollapse();
            }

            // Handle active collapse
            if (m_IsCollapsing)
            {
                UpdateCollapse();
            }
        }

        private void TriggerRandomCollapse()
        {
            // Select 2 random walls to collapse
            int[] directions = { 0, 1, 2, 3 };
            ShuffleArray(directions);

            m_ActiveWalls[0] = GetWallByDirection((WallDirection)directions[0]);
            m_ActiveWalls[1] = GetWallByDirection((WallDirection)directions[1]);

            // Filter out null walls
            int validCount = 0;
            for (int i = 0; i < 2; i++)
            {
                if (m_ActiveWalls[i] != null)
                    validCount++;
            }

            if (validCount < 1)
                return;  // Need at least one valid wall

            m_IsCollapsing = true;
            m_CollapseTimer = 0f;
            m_LastCollapseTime = Time.time;

            WallDirection dir1 = GetDirectionByWall(m_ActiveWalls[0]);
            WallDirection dir2 = m_ActiveWalls[1] != null
                ? GetDirectionByWall(m_ActiveWalls[1])
                : WallDirection.North;

            Debug.Log($"[CollapsingWalls] Collapse triggered on {dir1} and {dir2}");
        }

        private void UpdateCollapse()
        {
            m_CollapseTimer += Time.deltaTime;
            float progress = m_CollapseTimer / m_CollapseDuration;

            // Move walls inward
            for (int i = 0; i < 2; i++)
            {
                if (m_ActiveWalls[i] == null)
                    continue;

                Vector3 newPos = MoveWallInward(m_ActiveWalls[i], progress);
                m_ActiveWalls[i].transform.position = newPos;
            }

            // Check for crushes
            if (m_CollapseTimer >= m_CollapseDuration)
            {
                CheckForCrushes();
                m_IsCollapsing = false;
                ResetWalls();
            }
        }

        private Vector3 MoveWallInward(GameObject wall, float progress)
        {
            Vector3 originalPos = GetOriginalPosition(wall);
            Vector3 center = transform.position;
            Vector3 direction = (center - originalPos).normalized;

            // Distance to move (up to 5 units max)
            float distance = Mathf.Min(5f, m_CollapseSpeed * m_CollapseDuration * progress);

            return originalPos + direction * distance;
        }

        private void CheckForCrushes()
        {
            // Simple AABB check for entities in crush zone
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);

            foreach (Collider col in colliders)
            {
                // Damage player
                var player = col.GetComponent<FirstPersonController>();
                if (player != null)
                {
                    player.TakeDamage(m_CrushDamage);
                    Debug.Log($"[CollapsingWalls] Player crushed for {m_CrushDamage} damage!");
                }

                // Damage bot
                var bot = col.GetComponent<BotController>();
                if (bot != null && !bot.IsDead)
                {
                    bot.TakeDamage(m_CrushDamage);
                    Debug.Log($"[CollapsingWalls] Bot crushed for {m_CrushDamage} damage!");
                }
            }
        }

        private void ResetWalls()
        {
            if (m_WallNorth != null)
                m_WallNorth.transform.position = m_OriginalPositions[0];
            if (m_WallSouth != null)
                m_WallSouth.transform.position = m_OriginalPositions[1];
            if (m_WallEast != null)
                m_WallEast.transform.position = m_OriginalPositions[2];
            if (m_WallWest != null)
                m_WallWest.transform.position = m_OriginalPositions[3];

            m_ActiveWalls[0] = null;
            m_ActiveWalls[1] = null;
        }

        private GameObject GetWallByDirection(WallDirection dir)
        {
            return dir switch
            {
                WallDirection.North => m_WallNorth,
                WallDirection.South => m_WallSouth,
                WallDirection.East => m_WallEast,
                WallDirection.West => m_WallWest,
                _ => null
            };
        }

        private WallDirection GetDirectionByWall(GameObject wall)
        {
            if (wall == m_WallNorth) return WallDirection.North;
            if (wall == m_WallSouth) return WallDirection.South;
            if (wall == m_WallEast) return WallDirection.East;
            if (wall == m_WallWest) return WallDirection.West;
            return WallDirection.North;
        }

        private Vector3 GetOriginalPosition(GameObject wall)
        {
            if (wall == m_WallNorth) return m_OriginalPositions[0];
            if (wall == m_WallSouth) return m_OriginalPositions[1];
            if (wall == m_WallEast) return m_OriginalPositions[2];
            if (wall == m_WallWest) return m_OriginalPositions[3];
            return Vector3.zero;
        }

        private void ShuffleArray<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                // Swap
                T temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }

        public bool IsCollapsing => m_IsCollapsing;
    }
}
