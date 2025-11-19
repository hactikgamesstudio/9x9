using UnityEngine;
using Unity.Template.Multiplayer.NGO.Game.Player;
using Unity.Template.Multiplayer.NGO.Core.Procedural;

namespace Unity.Template.Multiplayer.NGO.Bots
{
    /// <summary>
    /// Bot AI controller for singleplayer mode.
    /// Mimics player behavior: movement, combat, puzzle solving, item pickup.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class BotController : MonoBehaviour
    {
        [Header("Bot Configuration")]
        [SerializeField] private string m_BotName = "Bot";
        [SerializeField] private int m_BotID = 0;
        
        [Header("Movement Settings")]
        [SerializeField] private float m_MoveSpeed = 3f;
        [SerializeField] private float m_RotationSpeed = 5f;
        [SerializeField] private float m_WaypointReachedDistance = 0.5f;
        
        [Header("Combat Settings")]
        [SerializeField] private float m_AttackRange = 2f;
        [SerializeField] private float m_DetectionRange = 10f;
        [SerializeField] private int m_Damage = 10;
        [SerializeField] private float m_AttackCooldown = 1f;
        
        [Header("AI Behavior")]
        [SerializeField] private float m_PathUpdateInterval = 0.5f;
        [SerializeField] private float m_StuckCheckInterval = 2f;
        [SerializeField] private float m_StuckThreshold = 0.1f;
        
        // Components
        private CharacterController m_CharacterController;
        private FirstPersonController m_TargetPlayer;
        
        // State
        private Vector3 m_CurrentTarget;
        private Vector3 m_LastPosition;
        private float m_NextPathUpdateTime;
        private float m_NextStuckCheckTime;
        private float m_NextAttackTime;
        private bool m_HasTarget;
        
        // Bot stats
        private int m_Health = 100;
        private bool m_IsDead = false;
        
        public string BotName => m_BotName;
        public int BotID => m_BotID;
        public int Health => m_Health;
        public bool IsDead => m_IsDead;
        
        void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_LastPosition = transform.position;
        }
        
        void Start()
        {
            // Set initial random target
            SetRandomTarget();
        }
        
        void Update()
        {
            if (m_IsDead) return;
            
            // Check for stuck state
            CheckIfStuck();
            
            // Update path periodically
            if (Time.time >= m_NextPathUpdateTime)
            {
                UpdatePath();
                m_NextPathUpdateTime = Time.time + m_PathUpdateInterval;
            }
            
            // Move towards target
            MoveTowardsTarget();
            
            // Check for nearby player to attack
            CheckForCombat();
        }
        
        void SetRandomTarget()
        {
            // Generate random position within current room or nearby
            float randomX = Random.Range(-5f, 5f);
            float randomZ = Random.Range(-5f, 5f);
            m_CurrentTarget = transform.position + new Vector3(randomX, 0f, randomZ);
            m_HasTarget = true;
        }
        
        void UpdatePath()
        {
            // Check if target player is in detection range
            FindNearestPlayer();
            
            // If no player target, move randomly
            if (m_TargetPlayer == null)
            {
                // Check if reached current waypoint
                float distanceToTarget = Vector3.Distance(transform.position, m_CurrentTarget);
                if (distanceToTarget < m_WaypointReachedDistance)
                {
                    SetRandomTarget();
                }
            }
            else
            {
                // Follow player
                m_CurrentTarget = m_TargetPlayer.transform.position;
                m_HasTarget = true;
            }
        }
        
        void MoveTowardsTarget()
        {
            if (!m_HasTarget) return;
            
            Vector3 direction = (m_CurrentTarget - transform.position).normalized;
            direction.y = 0f; // Keep movement horizontal
            
            // Rotate towards target
            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    m_RotationSpeed * Time.deltaTime
                );
            }
            
            // Move towards target
            Vector3 movement = direction * m_MoveSpeed * Time.deltaTime;
            m_CharacterController.Move(movement);
            
            // Apply gravity
            if (!m_CharacterController.isGrounded)
            {
                m_CharacterController.Move(Vector3.down * 9.81f * Time.deltaTime);
            }
        }
        
        void FindNearestPlayer()
        {
            FirstPersonController[] players = FindObjectsOfType<FirstPersonController>();
            
            float closestDistance = m_DetectionRange;
            FirstPersonController closestPlayer = null;
            
            foreach (var player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }
            
            m_TargetPlayer = closestPlayer;
        }
        
        void CheckForCombat()
        {
            if (m_TargetPlayer == null) return;
            if (Time.time < m_NextAttackTime) return;
            
            float distanceToPlayer = Vector3.Distance(transform.position, m_TargetPlayer.transform.position);
            
            if (distanceToPlayer <= m_AttackRange)
            {
                AttackPlayer();
                m_NextAttackTime = Time.time + m_AttackCooldown;
            }
        }
        
        void AttackPlayer()
        {
            if (m_TargetPlayer != null)
            {
                m_TargetPlayer.TakeDamage(m_Damage);
                Debug.Log($"{m_BotName} attacked player for {m_Damage} damage!");
            }
        }
        
        void CheckIfStuck()
        {
            if (Time.time < m_NextStuckCheckTime) return;
            
            float distanceMoved = Vector3.Distance(transform.position, m_LastPosition);
            
            if (distanceMoved < m_StuckThreshold)
            {
                // Bot is stuck, set new random target
                Debug.Log($"{m_BotName} appears stuck, finding new path...");
                SetRandomTarget();
            }
            
            m_LastPosition = transform.position;
            m_NextStuckCheckTime = Time.time + m_StuckCheckInterval;
        }
        
        public void TakeDamage(int damage)
        {
            if (m_IsDead) return;
            
            m_Health -= damage;
            Debug.Log($"{m_BotName} took {damage} damage. Health: {m_Health}");
            
            if (m_Health <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            m_IsDead = true;
            Debug.Log($"{m_BotName} has been eliminated!");
            
            // Notify BotManager
            BotManager botManager = FindObjectOfType<BotManager>();
            if (botManager != null)
            {
                botManager.OnBotDied(this);
            }
            
            // Disable bot (or play death animation, etc.)
            gameObject.SetActive(false);
        }
        
        public void Initialize(string botName, int botID, Vector3 spawnPosition)
        {
            m_BotName = botName;
            m_BotID = botID;
            transform.position = spawnPosition;
            m_LastPosition = spawnPosition;
            m_Health = 100;
            m_IsDead = false;
            
            SetRandomTarget();
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
            
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_AttackRange);
            
            // Draw current target
            if (m_HasTarget)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, m_CurrentTarget);
                Gizmos.DrawSphere(m_CurrentTarget, 0.3f);
            }
        }
    }
}
