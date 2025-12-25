using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Acid/Water Pool - Continuous damage and knockback
    /// Slows movement in the pool
    /// </summary>
    public class HazardAcidPool : HazardBase
    {
        [SerializeField] private float m_MovementSlowMultiplier = 0.5f;
        [SerializeField] private float m_KnockbackForce = 2f;
        private float m_LastDamageTime = -999f;

        protected override void Start()
        {
            m_HazardType = "Acid";
            base.Start();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            m_BodiesInArea.Add(other);
            
            // Apply initial knockback away from hazard
            ApplyKnockback(other);
            
            // Immediate damage on entry
            DamageTarget(other);
            m_LastDamageTime = Time.time;
        }

        protected override void OnTriggerStay(Collider other)
        {
            // Deal continuous damage
            if (Time.time >= m_LastDamageTime + m_Config.DamageInterval)
            {
                DamageTarget(other);
                m_LastDamageTime = Time.time;
            }
        }

        private void ApplyKnockback(Collider target)
        {
            // Push target away from hazard center
            var rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * m_KnockbackForce, ForceMode.Impulse);
            }
        }

        public float GetMovementMultiplier()
        {
            return m_MovementSlowMultiplier;
        }
    }
}
