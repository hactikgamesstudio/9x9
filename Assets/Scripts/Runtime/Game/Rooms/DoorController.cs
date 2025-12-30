using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Controls door opening/closing animations and player detection.
    /// Attach to door GameObjects in room prefabs.
    /// 
    /// **FOR BEGINNERS - HOW DOORS WORK:**
    /// Doors automatically open when players approach and close when they leave.
    /// After closing, the room rotates (Cube film mechanic).
    /// 
    /// **SETUP:**
    /// 1. Attach this script to door GameObjects (door_north, door_south, etc.)
    /// 2. Add BoxCollider component (set Is Trigger = true)
    /// 3. Set Door Close Speed and Rotate Room On Close in Inspector
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DoorController : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Door Object")]
        [Tooltip("The actual door GameObject (e.g., DoorCube) that will animate")]
        [SerializeField] private Transform m_DoorObject;

        [Header("Door Settings")]
        [Tooltip("How fast the door opens (units per second)")]
        [SerializeField] private float m_OpenSpeed = 2f;

        [Tooltip("How fast the door closes (units per second)")]
        [SerializeField] private float m_CloseSpeed = 1f;

        [Tooltip("How far the door slides when opening (local Y axis)")]
        [SerializeField] private float m_OpenDistance = 3f;

        [Tooltip("Should the room rotate after this door closes?")]
        [SerializeField] private bool m_RotateRoomOnClose = true;

        [Header("Audio")]
        [Tooltip("Sound when door opens")]
        [SerializeField] private AudioClip m_OpenSound;

        [Tooltip("Sound when door closes")]
        [SerializeField] private AudioClip m_CloseSound;

        #endregion

        #region Private Variables

        private Vector3 m_ClosedPosition;
        private Vector3 m_OpenPosition;
        private bool m_IsOpen = false;
        private bool m_IsMoving = false;
        private int m_PlayersInRange = 0;
        private AudioSource m_AudioSource;
        private RoomData m_ParentRoomData;

        #endregion

        #region Unity Lifecycle

        void Start()
        {
            // Validate door object reference
            if (m_DoorObject == null)
            {
                Debug.LogError($"Door {name} missing Door Object reference! Please assign DoorCube in Inspector.");
                return;
            }

            // Store closed position
            m_ClosedPosition = m_DoorObject.localPosition;
            m_OpenPosition = m_ClosedPosition + Vector3.up * m_OpenDistance;

            // Ensure collider is trigger
            Collider collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                Debug.LogWarning($"Door {name} collider is not set to trigger! Setting it now.");
                collider.isTrigger = true;
            }

            // Setup audio source
            m_AudioSource = GetComponent<AudioSource>();
            if (m_AudioSource == null && (m_OpenSound != null || m_CloseSound != null))
            {
                m_AudioSource = gameObject.AddComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;
                m_AudioSource.spatialBlend = 1f; // 3D sound
            }

            // Get parent room data
            m_ParentRoomData = GetComponentInParent<RoomData>();
        }

        void Update()
        {
            // Skip if door object not assigned
            if (m_DoorObject == null)
                return;

            // Move door towards target position
            if (m_IsMoving)
            {
                Vector3 targetPos = m_IsOpen ? m_OpenPosition : m_ClosedPosition;
                float speed = m_IsOpen ? m_OpenSpeed : m_CloseSpeed;

                m_DoorObject.localPosition = Vector3.MoveTowards(
                    m_DoorObject.localPosition,
                    targetPos,
                    speed * Time.deltaTime
                );

                // Check if reached target
                if (Vector3.Distance(m_DoorObject.localPosition, targetPos) < 0.01f)
                {
                    m_DoorObject.localPosition = targetPos;
                    m_IsMoving = false;

                    // Optional: invoke RoomGenerator.OnRoomExited via reflection (avoids hard dependency)
                    if (!m_IsOpen && m_RotateRoomOnClose && m_ParentRoomData != null)
                    {
                        var genType = System.Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.RoomGenerator, com.unity.template.multiplayer-ngo.runtime");
                        if (genType != null)
                        {
                            // Use modern API: FindObjectsByType with no sorting for performance
                            var generators = Object.FindObjectsByType(genType, FindObjectsSortMode.None);
                            if (generators != null && generators.Length > 0)
                            {
                                var onExit = genType.GetMethod("OnRoomExited");
                                if (onExit != null)
                                {
                                    try
                                    {
                                        onExit.Invoke(generators[0], new object[] { m_ParentRoomData.GridPosition });
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Debug.LogWarning($"DoorController: OnRoomExited failed: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Collision Detection

        /// <summary>
        /// Player entered door trigger zone - open the door.
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_PlayersInRange++;

                if (!m_IsOpen)
                {
                    OpenDoor();
                }
            }
        }

        /// <summary>
        /// Player left door trigger zone - close if no players remain.
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_PlayersInRange--;

                if (m_PlayersInRange <= 0 && m_IsOpen)
                {
                    m_PlayersInRange = 0; // Clamp to zero
                    CloseDoor();
                }
            }
        }

        #endregion

        #region Door Control

        /// <summary>
        /// Open the door (slide upward).
        /// </summary>
        public void OpenDoor()
        {
            // Check if this door is blocked/false
            if (m_ParentRoomData != null)
            {
                Vector3Int doorDirection = GetDoorDirection();
                if (!m_ParentRoomData.IsDoorOpen(doorDirection))
                {
                    Debug.Log($"Door {name} is blocked (false door) - cannot open");
                    return; // False door - stays closed
                }
            }

            m_IsOpen = true;
            m_IsMoving = true;

            if (m_AudioSource != null && m_OpenSound != null)
            {
                m_AudioSource.PlayOneShot(m_OpenSound);
            }

            Debug.Log($"Door {name} opening");
        }

        /// <summary>
        /// Close the door (slide downward).
        /// </summary>
        public void CloseDoor()
        {
            m_IsOpen = false;
            m_IsMoving = true;

            if (m_AudioSource != null && m_CloseSound != null)
            {
                m_AudioSource.PlayOneShot(m_CloseSound);
            }

            Debug.Log($"Door {name} closing");
        }

        /// <summary>
        /// Determine which direction this door faces based on name.
        /// </summary>
        private Vector3Int GetDoorDirection()
        {
            string doorName = name.ToLower();

            if (doorName.Contains("north")) return new Vector3Int(0, 0, 1);
            if (doorName.Contains("south")) return new Vector3Int(0, 0, -1);
            if (doorName.Contains("east")) return Vector3Int.right;
            if (doorName.Contains("west")) return Vector3Int.left;
            if (doorName.Contains("up")) return Vector3Int.up;
            if (doorName.Contains("down")) return Vector3Int.down;

            Debug.LogWarning($"Could not determine direction for door: {name}");
            return Vector3Int.zero;
        }

        #endregion

        #region Editor Helpers

        /// <summary>
        /// Draw gizmo showing door trigger zone.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = m_IsOpen ? Color.green : Color.red;
                Gizmos.matrix = transform.localToWorldMatrix;

                if (col is BoxCollider boxCol)
                {
                    Gizmos.DrawWireCube(boxCol.center, boxCol.size);
                }
            }
        }

        #endregion
    }
}
