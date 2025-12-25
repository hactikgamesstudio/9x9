using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Base class for all hazards in the game
    /// Handles damage to players and bots
    /// </summary>
    public abstract class HazardBase : MonoBehaviour
    {
        [SerializeField]
        protected string m_HazardType = "Generic";

        [SerializeField]
        protected int m_Damage = 10;

        [SerializeField]
        protected bool m_DamagePlayer = true;

        [SerializeField]
        protected bool m_DamageBots = true;

        protected HazardConfig.HazardSettings m_Config;
        protected HashSet<Collider> m_BodiesInArea = new HashSet<Collider>();

        protected virtual void Start()
        {
            // Load config for this hazard type
            m_Config = HazardConfig.GetConfig(m_HazardType);
            if (m_Damage == 10) // Default value - use config instead
            {
                m_Damage = m_Config.Damage;
            }

            Debug.Log($"[{m_HazardType}] Initialized - Damage: {m_Damage}");
        }

        /// <summary>
        /// Called when something enters the hazard
        /// </summary>
        protected virtual void OnTriggerEnter(Collider other)
        {
            m_BodiesInArea.Add(other);
            DamageTarget(other);
        }

        /// <summary>
        /// Called while something is in the hazard
        /// Override for continuous damage hazards
        /// </summary>
        protected virtual void OnTriggerStay(Collider other)
        {
            // Override in derived classes for continuous damage
        }

        /// <summary>
        /// Called when something leaves the hazard
        /// </summary>
        protected virtual void OnTriggerExit(Collider other)
        {
            m_BodiesInArea.Remove(other);
        }

        /// <summary>
        /// Apply damage to a target
        /// </summary>
        protected virtual void DamageTarget(Collider target)
        {
            if (target == null)
                return;

            // Try to damage player
            if (m_DamagePlayer)
            {
                var player = target.GetComponent<FirstPersonController>();
                if (player != null)
                {
                    player.TakeDamage(m_Damage);
                    Debug.Log($"[{m_HazardType}] Player took {m_Damage} damage");
                    return;
                }
            }

            // Try to damage bot
            if (m_DamageBots)
            {
                var bot = target.GetComponent<BotController>();
                if (bot != null && !bot.IsDead)
                {
                    bot.TakeDamage(m_Damage);
                    Debug.Log($"[{m_HazardType}] Bot took {m_Damage} damage");
                }
            }
        }

        /// <summary>
        /// Get all bodies currently in this hazard
        /// </summary>
        public HashSet<Collider> GetBodiesInArea()
        {
            return m_BodiesInArea;
        }
    }
}
