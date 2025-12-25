using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Gas Cloud - Continuous damage in a gas-filled area
    /// Slows player movement slightly
    /// </summary>
    public class HazardGas : HazardBase
    {
        [SerializeField] private float m_MovementSlowMultiplier = 0.7f;
        private float m_LastDamageTime = -999f;

        protected override void Start()
        {
            m_HazardType = "Gas";
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

            // Slow player movement if in gas
            var player = other.GetComponent<FirstPersonController>();
            if (player != null && m_DamagePlayer)
            {
                // Player can override this in their controller
                // For now, just log that they're in gas
            }
        }

        public float GetMovementMultiplier()
        {
            return m_MovementSlowMultiplier;
        }
    }
}
