using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Spike hazard that deals instant damage with a cooldown period.
    /// When player touches spikes, they take damage and are immune for a short time.
    /// 
    /// **FOR BEGINNERS - COOLDOWN PATTERN:**
    /// Cooldown prevents spamming damage too quickly.
    /// 
    /// In Godot, you used: await get_tree().create_timer(cooldown).timeout
    /// In Unity, you can use:
    /// 1. Coroutines (IEnumerator with yield return)
    /// 2. Manual timer with Time.time
    /// 3. Invoke() method
    /// 
    /// This script uses method #2 (manual timer) for simplicity.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HazardSpikes : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Damage Settings")]
        [Tooltip("How much damage to deal on contact")]
        [SerializeField] private int m_Damage = 25;
        
        [Tooltip("Cooldown between damage applications (seconds)")]
        [SerializeField] private float m_Cooldown = 0.5f;
        
        [Header("Visual Feedback")]
        [Tooltip("Material to flash when dealing damage (optional)")]
        [SerializeField] private Material m_FlashMaterial;
        
        [Tooltip("How long to show flash material (seconds)")]
        [SerializeField] private float m_FlashDuration = 0.1f;
        
        #endregion
        
        #region Private Variables
        
        /// <summary>
        /// Tracks if hazard is currently in cooldown mode.
        /// **FOR BEGINNERS - BOOLEAN FLAGS:**
        /// Booleans (true/false) are perfect for "is this happening?" questions.
        /// We check this flag before dealing damage to prevent spam.
        /// </summary>
        private bool m_IsCoolingDown = false;
        
        /// <summary>
        /// Timestamp when cooldown will end.
        /// We compare Time.time (current game time) to this value.
        /// </summary>
        private float m_CooldownEndTime = 0f;
        
        /// <summary>
        /// Cached reference to renderer for visual effects.
        /// </summary>
        private Renderer m_Renderer;
        
        /// <summary>
        /// Original material before flashing.
        /// </summary>
        private Material m_OriginalMaterial;
        
        /// <summary>
        /// Timer for flash effect.
        /// </summary>
        private float m_FlashEndTime = 0f;
        
        #endregion
        
        #region Unity Lifecycle
        
        void Start()
        {
            // Ensure collider is set to trigger
            Collider collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning($"{gameObject.name}: Hazard collider should be a trigger. Auto-fixing.");
                collider.isTrigger = true;
            }
            
            // Cache renderer and original material
            m_Renderer = GetComponent<Renderer>();
            if (m_Renderer != null)
            {
                m_OriginalMaterial = m_Renderer.material;
            }
        }
        
        /// <summary>
        /// Update cooldown timer and visual effects.
        /// 
        /// **FOR BEGINNERS - TIME.TIME:**
        /// Time.time returns the number of seconds since the game started.
        /// We use it to track when cooldowns/timers should end.
        /// Example: If Time.time = 10.5 and cooldown is 0.5s, endTime = 11.0
        ///          When Time.time >= 11.0, cooldown is over.
        /// </summary>
        void Update()
        {
            // Check if cooldown is finished
            if (m_IsCoolingDown && Time.time >= m_CooldownEndTime)
            {
                m_IsCoolingDown = false;
            }
            
            // Check if flash effect should end
            if (m_FlashEndTime > 0f && Time.time >= m_FlashEndTime && m_Renderer != null)
            {
                m_Renderer.material = m_OriginalMaterial;
                m_FlashEndTime = 0f;
            }
        }
        
        #endregion
        
        #region Collision Handling
        
        /// <summary>
        /// Called when something enters the spike trigger.
        /// In Godot: func _on_body_entered(body: Node)
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            // Skip if still in cooldown
            if (m_IsCoolingDown) return;
            
            // Try to damage the entity that touched us
            // Method 1: Check if it has a take_damage method
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                DealDamage(player);
                return;
            }
            
            // Method 2: Check for generic IDamageable interface (advanced pattern)
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(m_Damage);
                StartCooldown();
            }
        }
        
        /// <summary>
        /// Deals damage to the player and starts cooldown.
        /// </summary>
        void DealDamage(FirstPersonController player)
        {
            player.TakeDamage(m_Damage);
            Debug.Log($"{gameObject.name} dealt {m_Damage} damage to player");
            
            StartCooldown();
            TriggerFlashEffect();
        }
        
        /// <summary>
        /// Starts the cooldown timer.
        /// </summary>
        void StartCooldown()
        {
            m_IsCoolingDown = true;
            m_CooldownEndTime = Time.time + m_Cooldown;
        }
        
        /// <summary>
        /// Triggers visual feedback when dealing damage.
        /// </summary>
        void TriggerFlashEffect()
        {
            if (m_FlashMaterial != null && m_Renderer != null)
            {
                m_Renderer.material = m_FlashMaterial;
                m_FlashEndTime = Time.time + m_FlashDuration;
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        /// <summary>
        /// Draws gizmo in Scene view.
        /// Red = dangerous hazard!
        /// </summary>
        void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red, semi-transparent
                
                if (col is BoxCollider box)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(box.center, box.size);
                }
                else if (col is SphereCollider sphere)
                {
                    Gizmos.DrawSphere(transform.position, sphere.radius);
                }
            }
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                $"Spikes: {m_Damage} dmg");
            #endif
        }
        
        void OnValidate()
        {
            m_Damage = Mathf.Max(0, m_Damage);
            m_Cooldown = Mathf.Max(0f, m_Cooldown);
            m_FlashDuration = Mathf.Max(0f, m_FlashDuration);
        }
        
        #endregion
    }
    
    #region IDamageable Interface (Optional Advanced Pattern)
    
    /// <summary>
    /// Interface for any object that can take damage.
    /// 
    /// **FOR BEGINNERS - WHAT IS AN INTERFACE?**
    /// An interface is a contract that says "any class implementing this must have these methods."
    /// 
    /// Example:
    /// - Player implements IDamageable (has TakeDamage method)
    /// - Enemy implements IDamageable (has TakeDamage method)
    /// - Hazard can damage both without knowing their specific types!
    /// 
    /// To use:
    /// 1. Make FirstPersonController implement IDamageable
    /// 2. Make Enemy (if you create one) implement IDamageable
    /// 3. Hazards can now damage anything with this interface
    /// 
    /// This is optional but makes code more flexible.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
    
    #endregion
}
