using System;
using Unity.Netcode;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Handles AI behavior for bot players in singleplayer mode
    /// Implements movement, combat, item pickup, and puzzle solving
    /// Mimics FirstPersonController behavior for consistency
    /// </summary>
    public class BotController : MonoBehaviour
    {
        [Header("Bot Settings")]
        [SerializeField]
        private string m_BotName = "Bot";

        [SerializeField]
        private float m_MovementSpeed = 5f;

        [SerializeField]
        private float m_RotationSpeed = 2f;

        [SerializeField]
        private float m_UpdateInterval = 0.5f;

        [Header("Health")]
        [SerializeField]
        private int m_MaxHealth = 100;

        [Header("AI Behavior")]
        [SerializeField]
        private float m_PathfindingInterval = 1f;

        [SerializeField]
        private float m_DetectionRange = 20f;

        [SerializeField]
        private float m_CombatRange = 10f;

        // Health state
        private int m_CurrentHealth;
        public event Action<int> HealthChanged;
        
        // AI state
        private Player m_Player;
        private CharacterController m_CharacterController;
        private BotState m_CurrentState = BotState.Idle;
        private Vector3 m_CurrentTarget = Vector3.zero;
        private float m_StateTimer = 0f;
        private Vector3 m_Velocity = Vector3.zero;
        private const float k_Gravity = 9.81f;

        // Properties
        public int Health => m_CurrentHealth;
        public int MaxHealth => m_MaxHealth;
        public bool IsDead => m_CurrentHealth <= 0;

        void Awake()
        {
            m_Player = GetComponent<Player>();
            m_CharacterController = GetComponent<CharacterController>();

            if (m_Player == null)
                Debug.LogError($"[BotController] {gameObject.name} missing Player component!");

            if (m_CharacterController == null)
                Debug.LogError($"[BotController] {gameObject.name} missing CharacterController component!");
        }

        void Start()
        {
            // Initialize bot name in gameobject
            gameObject.name = m_BotName;
            m_CurrentHealth = m_MaxHealth;

        }

        void Update()
        {
            if (m_Player == null || m_CharacterController == null)
                return;

            m_StateTimer += Time.deltaTime;

            // Update AI behavior based on current state
            switch (m_CurrentState)
            {
                case BotState.Idle:
                    UpdateIdle();
                    break;

                case BotState.Patrolling:
                    UpdatePatrolling();
                    break;

                case BotState.ChasingPlayer:
                    UpdateChasingSomething();
                    break;

                case BotState.Combat:
                    UpdateCombat();
                    break;

                case BotState.SolvingPuzzle:
                    UpdateSolvingPuzzle();
                    break;

                case BotState.CollectingItem:
                    UpdateCollectingItem();
                    break;
            }

            // Apply gravity
            if (!m_CharacterController.isGrounded)
            {
                m_Velocity.y -= k_Gravity * Time.deltaTime;
            }
            else if (m_Velocity.y < 0)
            {
                m_Velocity.y = 0;
            }

            // Move the bot
            m_CharacterController.Move(m_Velocity * Time.deltaTime);
        }

        void UpdateIdle()
        {
            // Idle for a moment, then start patrolling
            if (m_StateTimer > 2f)
            {
                m_CurrentState = BotState.Patrolling;
                m_StateTimer = 0f;
            }

            m_Velocity = Vector3.Lerp(m_Velocity, Vector3.zero, Time.deltaTime * 2f);
        }

        void UpdatePatrolling()
        {
            // Simple patrol: move in a random direction, change direction occasionally
            if (m_StateTimer > m_PathfindingInterval)
            {
                // Pick a random direction
                Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
                randomDirection.y = 0; // Keep horizontal
                m_CurrentTarget = randomDirection.normalized;
                m_StateTimer = 0f;
            }

            // Move towards target
            Vector3 moveDirection = m_CurrentTarget.normalized;
            m_Velocity.x = moveDirection.x * m_MovementSpeed;
            m_Velocity.z = moveDirection.z * m_MovementSpeed;

            // Rotate towards movement direction
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
            }
        }

        void UpdateChasingSomething()
        {
            // TODO: Implement pathfinding to current target
            // For now, just move towards it
            Vector3 directionToTarget = (m_CurrentTarget - transform.position).normalized;
            directionToTarget.y = 0;

            if (directionToTarget.magnitude > 0.1f)
            {
                m_Velocity.x = directionToTarget.x * m_MovementSpeed;
                m_Velocity.z = directionToTarget.z * m_MovementSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
            }
        }

        void UpdateCombat()
        {
            // TODO: Implement combat behavior (shooting, dodging, etc.)
            Debug.Log($"[BotController] {m_BotName} - Combat state placeholder");
        }

        void UpdateSolvingPuzzle()
        {
            // TODO: Implement puzzle-solving logic
            Debug.Log($"[BotController] {m_BotName} - Puzzle solving state placeholder");
        }

        void UpdateCollectingItem()
        {
            // TODO: Implement item collection logic
            Debug.Log($"[BotController] {m_BotName} - Item collection state placeholder");
        }

        /// <summary>
        /// Take damage (called by hazards and weapons)
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - amount);
            HealthChanged?.Invoke(m_CurrentHealth);

            if (m_CurrentHealth == 0)
            {
                Debug.Log($"[BotController] {m_BotName} has been eliminated!");
                // Trigger death behavior - could disable, ragdoll, etc.
                enabled = false;
            }
        }

        /// <summary>
        /// Heal bot
        /// </summary>
        public void Heal(int amount)
        {
            if (IsDead) return;

            m_CurrentHealth = Mathf.Min(m_MaxHealth, m_CurrentHealth + amount);
            HealthChanged?.Invoke(m_CurrentHealth);
        }

        /// <summary>
        /// Set the bot's patrol target
        /// </summary>
        public void SetPatrolTarget(Vector3 target)
        {
            m_CurrentTarget = target;
            m_CurrentState = BotState.Patrolling;
        }

        /// <summary>
        /// Command bot to chase a target
        /// </summary>
        public void ChaseTarget(Vector3 targetPosition)
        {
            m_CurrentTarget = targetPosition;
            m_CurrentState = BotState.ChasingPlayer;
        }

        /// <summary>
        /// Get current bot state
        /// </summary>
        public BotState GetCurrentState() => m_CurrentState;

        /// <summary>
        /// Set bot state directly
        /// </summary>
        public void SetBotState(BotState newState)
        {
            m_CurrentState = newState;
            m_StateTimer = 0f;
        }
    }

    /// <summary>
    /// Bot AI states
    /// </summary>
    public enum BotState
    {
        Idle,
        Patrolling,
        ChasingPlayer,
        Combat,
        SolvingPuzzle,
        CollectingItem
    }
}
