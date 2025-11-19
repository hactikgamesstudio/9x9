using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Collectible pickup item that adds to the player's inventory when touched.
    /// Automatically destroys itself after being collected.
    /// 
    /// **FOR BEGINNERS - COLLISION DETECTION IN UNITY:**
    /// Unity has two main collision systems:
    /// 
    /// 1. COLLIDERS (Physical blocking):
    ///    - BoxCollider, SphereCollider, CapsuleCollider, MeshCollider
    ///    - Objects with colliders physically block each other
    ///    - Used for walls, floors, solid objects
    /// 
    /// 2. TRIGGERS (Detection only):
    ///    - Same collider components, but with "Is Trigger" checked
    ///    - Objects pass through, but trigger events are called
    ///    - Used for pickups, damage zones, checkpoints
    /// 
    /// This script uses TRIGGERS:
    /// - OnTriggerEnter() is called when another collider enters this trigger
    /// - We check if it's the player, add the item, then destroy this GameObject
    /// 
    /// **SETUP REQUIREMENTS:**
    /// 1. Attach this script to a GameObject
    /// 2. Add a Collider component (e.g., SphereCollider)
    /// 3. Check "Is Trigger" on the collider
    /// 4. Set the collider's Layer to "Pickup" (optional, for filtering)
    /// 5. Player must have a Collider or CharacterController to detect collision
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PickupItem : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Item Settings")]
        [Tooltip("Unique identifier for this item (e.g., 'health_potion', 'key_blue')")]
        [SerializeField] private string m_ItemId = "health_potion";
        
        [Tooltip("How many of this item to add to inventory")]
        [SerializeField] private int m_Quantity = 1;
        
        [Header("Visual Feedback")]
        [Tooltip("Should the pickup spin/rotate?")]
        [SerializeField] private bool m_Rotate = true;
        
        [Tooltip("Rotation speed in degrees per second")]
        [SerializeField] private float m_RotationSpeed = 90f;
        
        [Tooltip("Should the pickup bob up and down?")]
        [SerializeField] private bool m_Bob = true;
        
        [Tooltip("How high to bob (in units)")]
        [SerializeField] private float m_BobHeight = 0.3f;
        
        [Tooltip("How fast to bob")]
        [SerializeField] private float m_BobSpeed = 2f;
        
        [Header("Audio (Optional)")]
        [Tooltip("Sound to play when picked up")]
        [SerializeField] private AudioClip m_PickupSound;
        
        #endregion
        
        #region Private Variables
        
        private Vector3 m_StartPosition;
        private float m_BobTimer;
        
        #endregion
        
        #region Unity Lifecycle
        
        /// <summary>
        /// Called when script starts. Ensure collider is set to trigger mode.
        /// 
        /// **FOR BEGINNERS - GetComponent vs RequireComponent:**
        /// - RequireComponent<Collider> at the top ensures Unity adds a Collider if missing
        /// - GetComponent<Collider>() gets the reference to use it
        /// - This is like Godot's @onready var collider = $Collider
        /// </summary>
        void Start()
        {
            // Store starting position for bobbing animation
            m_StartPosition = transform.position;
            
            // Ensure collider is set to trigger
            Collider collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning($"{gameObject.name}: Collider is not set to trigger. Auto-fixing.");
                collider.isTrigger = true;
            }
        }
        
        /// <summary>
        /// Called every frame. Handle visual effects (rotation, bobbing).
        /// </summary>
        void Update()
        {
            // Rotation effect
            if (m_Rotate)
            {
                transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);
            }
            
            // Bobbing effect (sine wave motion)
            if (m_Bob)
            {
                m_BobTimer += Time.deltaTime * m_BobSpeed;
                float yOffset = Mathf.Sin(m_BobTimer) * m_BobHeight;
                transform.position = m_StartPosition + new Vector3(0f, yOffset, 0f);
            }
        }
        
        #endregion
        
        #region Collision Handling
        
        /// <summary>
        /// Called when another collider enters this trigger.
        /// In Godot this was: func _on_body_entered(body: Node)
        /// 
        /// **FOR BEGINNERS - WHAT IS A COLLIDER?**
        /// A Collider is a component that defines an object's physical shape.
        /// - other.gameObject is the GameObject that touched us
        /// - other.GetComponent<T>() tries to find a component of type T
        /// - We check for FirstPersonController to identify the player
        /// </summary>
        /// <param name="other">The collider that entered our trigger</param>
        void OnTriggerEnter(Collider other)
        {
            // Check if the object that touched us is the player
            // Method 1: Check for the player script component
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                CollectItem(player);
                return;
            }
            
            // Method 2: Check by tag (if player GameObject has tag "Player")
            if (other.CompareTag("Player"))
            {
                CollectItem(null);
            }
        }
        
        /// <summary>
        /// Handles the collection logic: add to inventory, play sound, destroy object.
        /// </summary>
        /// <param name="player">Reference to player (can be null if using tag method)</param>
        void CollectItem(FirstPersonController player)
        {
            // Add item to inventory
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddItem(m_ItemId, m_Quantity);
                Debug.Log($"Player collected {m_Quantity}x {m_ItemId}");
            }
            else
            {
                Debug.LogError("InventorySystem.Instance is null! Make sure InventorySystem exists in scene.");
            }
            
            // Play pickup sound if assigned
            if (m_PickupSound != null)
            {
                // Play sound at this position, not attached to GameObject
                // (GameObject will be destroyed, but sound should continue)
                AudioSource.PlayClipAtPoint(m_PickupSound, transform.position);
            }
            
            // Destroy this pickup object
            Destroy(gameObject);
        }
        
        #endregion
        
        #region Editor Helpers
        
        /// <summary>
        /// Draws a gizmo in the Scene view for easier level design.
        /// Shows the pickup's trigger radius and item ID.
        /// </summary>
        void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Yellow, semi-transparent
                
                // Draw based on collider type
                if (col is SphereCollider sphere)
                {
                    Gizmos.DrawWireSphere(transform.position, sphere.radius);
                }
                else if (col is BoxCollider box)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(box.center, box.size);
                }
            }
            
            // Draw text label in Scene view (Unity Editor only)
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                $"Pickup: {m_ItemId} x{m_Quantity}");
            #endif
        }
        
        /// <summary>
        /// Validates values in the Inspector. Prevents negative quantities.
        /// </summary>
        void OnValidate()
        {
            m_Quantity = Mathf.Max(1, m_Quantity);
            m_BobHeight = Mathf.Max(0f, m_BobHeight);
            m_BobSpeed = Mathf.Max(0f, m_BobSpeed);
        }
        
        #endregion
    }
}
