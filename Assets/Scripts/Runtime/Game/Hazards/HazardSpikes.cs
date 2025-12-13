using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Spike Trap - Instant damage with cooldown between hits
    /// </summary>
    public class HazardSpikes : HazardBase
    {
        private float m_LastDamageTime = -999f;

        protected override void Start()
        {
            m_HazardType = "Spikes";
            base.Start();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Check if we can damage (cooldown)
            if (Time.time < m_LastDamageTime + m_Config.Cooldown)
                return;

            m_BodiesInArea.Add(other);
            DamageTarget(other);
            m_LastDamageTime = Time.time;
        }

        protected override void OnTriggerStay(Collider other)
        {
            // Damage again if cooldown is up
            if (Time.time >= m_LastDamageTime + m_Config.Cooldown)
            {
                DamageTarget(other);
                m_LastDamageTime = Time.time;
            }
        }
    }
}
