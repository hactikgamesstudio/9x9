using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Laser Beam - Continuous damage while in contact
    /// </summary>
    public class HazardLaser : HazardBase
    {
        private float m_LastDamageTime = -999f;

        protected override void Start()
        {
            m_HazardType = "Laser";
            base.Start();
        }

        protected override void OnTriggerStay(Collider other)
        {
            // Deal damage at intervals
            if (Time.time >= m_LastDamageTime + m_Config.DamageInterval)
            {
                DamageTarget(other);
                m_LastDamageTime = Time.time;
            }
        }
    }
}
