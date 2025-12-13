using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Lava Pool - High damage and strong knockback
    /// Melts anything that touches it
    /// </summary>
    public class HazardLava : HazardBase
    {
        [SerializeField] private float m_KnockbackForce = 8f;
        private float m_LastDamageTime = -999f;

        protected override void Start()
        {
            m_HazardType = "Lava";
            base.Start();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            m_BodiesInArea.Add(other);
            
            // Strong knockback immediately
            ApplyKnockback(other);
            
            // Immediate damage
            DamageTarget(other);
            m_LastDamageTime = Time.time;
        }

        protected override void OnTriggerStay(Collider other)
        {
            // Continuous damage in lava
            if (Time.time >= m_LastDamageTime + m_Config.DamageInterval)
            {
                DamageTarget(other);
                m_LastDamageTime = Time.time;
            }
            
            // Keep knockback active to push out of lava
            if (Time.time % 0.2f < Time.deltaTime)  // Every 0.2 seconds
            {
                ApplyKnockback(other);
            }
        }

        private void ApplyKnockback(Collider target)
        {
            // Push target away from lava violently
            var rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                rb.linearVelocity = direction * m_KnockbackForce;
            }
        }
    }
}
