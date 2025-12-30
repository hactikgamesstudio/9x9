using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Electric Field - Pulsing electrical hazard
    /// Damages and pushes targets in waves
    /// </summary>
    public class HazardElectricField : HazardBase
    {
        [SerializeField] private float m_PulseRadius = 5f;
        [SerializeField] private float m_PushForce = 4f;
        [SerializeField] private AnimationCurve m_PulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float m_LastDamageTime = -999f;
        private float m_PulseTimer = 0f;

        protected override void Start()
        {
            m_HazardType = "Electric";
            base.Start();
        }

        void Update()
        {
            m_PulseTimer += Time.deltaTime;

            // Pulse every damage interval
            if (m_PulseTimer >= m_Config.DamageInterval)
            {
                ExecutePulse();
                m_PulseTimer = 0f;
            }
        }

        private void ExecutePulse()
        {
            // Find all objects in radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_PulseRadius);

            foreach (Collider col in colliders)
            {
                if (col == null) continue;

                // Damage
                DamageTarget(col);

                // Push away
                ApplyPush(col);
            }

            Debug.Log($"[Electric] Pulse! Hit {colliders.Length} targets");
        }

        private void ApplyPush(Collider target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Try rigidbody
            var rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * m_PushForce;
                return;
            }

            // Try character controller
            var cc = target.GetComponent<CharacterController>();
            if (cc != null)
            {
                // Character controller is handled by FirstPersonController
                // Just log for now
            }
        }
    }
}
