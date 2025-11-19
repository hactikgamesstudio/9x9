using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Continuous damage hazard (laser beam) that damages while player remains in contact.
    /// Unlike spikes which deal instant damage, this deals damage at regular intervals.
    /// 
    /// **FOR BEGINNERS - CONTINUOUS VS INSTANT DAMAGE:**
    /// - HazardSpikes: Touch once = one damage instance, then cooldown
    /// - HazardLaser: Stay in contact = damage every X seconds continuously
    /// 
    /// Think of it like:
    /// - Spikes = touching a hot stove (instant ouch, then you pull away)
    /// - Laser = standing in fire (continuous damage as long as you stay)
    /// 
    /// Implementation uses a Dictionary to track each body in the laser:
    /// - Dictionary<GameObject, float> tracks when each body was last damaged
    /// - We check each frame and apply damage at intervals
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HazardLaser : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Damage Settings")]
        [Tooltip("Damage dealt per interval")]
        [SerializeField] private int m_Damage = 10;
        
        [Tooltip("Time between damage applications (seconds)")]
        [SerializeField] private float m_DamageInterval = 1.0f;
        
        [Header("Visual Effects")]
        [Tooltip("Particle system for laser beam (optional)")]
        [SerializeField] private ParticleSystem m_LaserEffect;
        
        [Tooltip("Light component to pulse when dealing damage (optional)")]
        [SerializeField] private Light m_LaserLight;
        
        [Tooltip("Color to flash when dealing damage")]
        [SerializeField] private Color m_DamageFlashColor = Color.red;
        
        #endregion
        
        #region Private Variables
        
        /// <summary>
        /// Dictionary tracking all objects currently in the laser.
        /// Key = GameObject that's in contact
        /// Value = Last time damage was dealt to this object
        /// 
        /// **FOR BEGINNERS - WHY USE A DICTIONARY?**
        /// We need to track multiple players/objects independently.
        /// If Player A and Player B both touch the laser:
        /// - Player A might have been damaged 0.5 seconds ago
        /// - Player B might have been damaged 0.1 seconds ago
        /// Dictionary lets us track each one separately!
        /// </summary>
        private Dictionary<GameObject, float> m_BodiesInArea = new Dictionary<GameObject, float>();
        
        /// <summary>
        /// List to temporarily store bodies to remove (can't modify dictionary while iterating).
        /// </summary>
        private List<GameObject> m_ToRemove = new List<GameObject>();
        
        /// <summary>
        /// Original light color (if light assigned).
        /// </summary>
        private Color m_OriginalLightColor;
        
        /// <summary>
        /// Flash timer for light effect.
        /// </summary>
        private float m_FlashEndTime = 0f;
        
        #endregion
        
        #region Unity Lifecycle
        
        void Start()
        {
            // Ensure collider is a trigger
            Collider collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning($"{gameObject.name}: Laser collider should be a trigger. Auto-fixing.");
                collider.isTrigger = true;
            }
            
            // Store original light color
            if (m_LaserLight != null)
            {
                m_OriginalLightColor = m_LaserLight.color;
            }
            
            // Start particle effect if assigned
            if (m_LaserEffect != null && !m_LaserEffect.isPlaying)
            {
                m_LaserEffect.Play();
            }
        }
        
        /// <summary>
        /// Continuously check all bodies in the laser and apply damage at intervals.
        /// 
        /// **FOR BEGINNERS - WHY UPDATE() INSTEAD OF FIXEDUPDATE()?**
        /// - FixedUpdate() is for physics (consistent timestep)
        /// - Update() is for game logic (variable timestep)
        /// 
        /// Damage doesn't need physics precision, so Update() is fine.
        /// We use Time.time for timing, which works the same in both.
        /// </summary>
        void Update()
        {
            // Check each body in the laser
            m_ToRemove.Clear();
            
            foreach (var kvp in m_BodiesInArea)
            {
                GameObject body = kvp.Key;
                float lastDamageTime = kvp.Value;
                
                // Check if body was destroyed (e.g., player died)
                if (body == null)
                {
                    m_ToRemove.Add(body);
                    continue;
                }
                
                // Check if enough time has passed since last damage
                if (Time.time >= lastDamageTime + m_DamageInterval)
                {
                    // Try to deal damage
                    FirstPersonController player = body.GetComponent<FirstPersonController>();
                    if (player != null)
                    {
                        player.TakeDamage(m_Damage);
                        m_BodiesInArea[body] = Time.time; // Update last damage time
                        TriggerFlashEffect();
                        Debug.Log($"{gameObject.name} dealt {m_Damage} damage to {body.name}");
                    }
                    else
                    {
                        // Try IDamageable interface
                        IDamageable damageable = body.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            damageable.TakeDamage(m_Damage);
                            m_BodiesInArea[body] = Time.time;
                            TriggerFlashEffect();
                        }
                    }
                }
            }
            
            // Remove destroyed bodies from dictionary
            // **FOR BEGINNERS - WHY SEPARATE LOOP?**
            // You can't modify a dictionary while iterating through it (causes error).
            // Solution: collect items to remove, then remove them after iteration.
            foreach (GameObject body in m_ToRemove)
            {
                m_BodiesInArea.Remove(body);
            }
            
            // Update flash effect
            if (m_FlashEndTime > 0f && Time.time >= m_FlashEndTime && m_LaserLight != null)
            {
                m_LaserLight.color = m_OriginalLightColor;
                m_FlashEndTime = 0f;
            }
        }
        
        #endregion
        
        #region Collision Handling
        
        /// <summary>
        /// Called when something enters the laser trigger.
        /// Adds the body to our tracking dictionary.
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            // Check if this body can be damaged
            bool canDamage = other.GetComponent<FirstPersonController>() != null ||
                            other.GetComponent<IDamageable>() != null;
            
            if (canDamage && !m_BodiesInArea.ContainsKey(other.gameObject))
            {
                // Add to tracking dictionary
                // Set initial damage time to 0 so damage is dealt immediately
                m_BodiesInArea[other.gameObject] = 0f;
                Debug.Log($"{other.gameObject.name} entered laser {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Called when something exits the laser trigger.
        /// Removes the body from our tracking dictionary.
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (m_BodiesInArea.ContainsKey(other.gameObject))
            {
                m_BodiesInArea.Remove(other.gameObject);
                Debug.Log($"{other.gameObject.name} exited laser {gameObject.name}");
            }
        }
        
        #endregion
        
        #region Visual Effects
        
        /// <summary>
        /// Triggers visual flash when dealing damage.
        /// </summary>
        void TriggerFlashEffect()
        {
            if (m_LaserLight != null)
            {
                m_LaserLight.color = m_DamageFlashColor;
                m_FlashEndTime = Time.time + 0.1f;
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        /// <summary>
        /// Draws gizmo in Scene view.
        /// Orange = continuous damage hazard.
        /// </summary>
        void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                // Orange color for continuous hazards
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                
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
                $"Laser: {m_Damage} dmg/{m_DamageInterval}s");
            #endif
        }
        
        /// <summary>
        /// Draws wire sphere for each body in the laser (debug visualization).
        /// Shows exactly what the script is tracking.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = Color.yellow;
            foreach (var kvp in m_BodiesInArea)
            {
                if (kvp.Key != null)
                {
                    Gizmos.DrawWireSphere(kvp.Key.transform.position, 0.5f);
                }
            }
        }
        
        void OnValidate()
        {
            m_Damage = Mathf.Max(0, m_Damage);
            m_DamageInterval = Mathf.Max(0.01f, m_DamageInterval);
        }
        
        #endregion
    }
}
