using System;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int m_MaxHealth = 100;


        private int m_CurrentHealth;
        public event Action<int> HealthChanged;

        [Header("Movement")]
        [Tooltip("Walking speed in units per second")]
        [SerializeField] private float m_WalkSpeed = 5f;


        
        [Tooltip("Sprint speed when holding Shift")]
        [SerializeField] private float m_SprintSpeed = 8f;


        
        [Tooltip("Jump height in units")]
        [SerializeField] private float m_JumpHeight = 1.5f;


        
        [Tooltip("Gravity force")]
        [SerializeField] private float m_Gravity = -9.81f;



        [Header("Mouse Look")]
        [Tooltip("Mouse sensitivity multiplier")]
        [SerializeField] private float m_MouseSensitivity = 2f;


        
        [Tooltip("Maximum vertical look angle (prevents over-rotation)")]
        [SerializeField] private float m_MaxLookAngle = 80f;


        
        [Tooltip("Camera transform for mouse look (auto-finds if null)")]
        [SerializeField] private Transform m_CameraTransform;



        [Header("Ground Check")]
        [SerializeField] private float m_GroundCheckDistance = 0.2f;


        [SerializeField] private LayerMask m_GroundMask = 1; // Default layer



        private CharacterController m_CharacterController;
        private Vector3 m_Velocity;
        private float m_RotationX = 0f;
        private bool m_IsGrounded;
        private Vector2 m_MoveInput;
        private Vector2 m_LookInput;
        private bool m_JumpInput;
        private bool m_SprintInput;

        public int Health => m_CurrentHealth;
        public int MaxHealth => m_MaxHealth;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            
            // Auto-find camera if not assigned
            if (m_CameraTransform == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    m_CameraTransform = cam.transform;
                }
            }

            // Start with cursor UNLOCKED for menu navigation
            // Player can lock cursor later by clicking in-game
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            m_CurrentHealth = m_MaxHealth;
        }

        private void Update()
        {
            HandleInput();
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleInput()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            // With PlayerInput set to "Invoke Unity Events", inputs are fed via callbacks below.
            // Mouse inputs handled directly in code - no cursor locking
#else
            // Legacy Input Manager polling
            m_MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            m_LookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (Input.GetButtonDown("Jump") && m_IsGrounded) m_JumpInput = true;
            m_SprintInput = Input.GetKey(KeyCode.LeftShift);
#endif
        }

        private void HandleMouseLook()
        {
            if (m_CameraTransform == null)
                return;


            // Horizontal rotation (Y-axis) - rotate the player body
            float mouseX = m_LookInput.x * m_MouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);

            // Vertical rotation (X-axis) - rotate the camera
            float mouseY = m_LookInput.y * m_MouseSensitivity;
            m_RotationX -= mouseY;
            m_RotationX = Mathf.Clamp(m_RotationX, -m_MaxLookAngle, m_MaxLookAngle);
            m_CameraTransform.localRotation = Quaternion.Euler(m_RotationX, 0f, 0f);
        }

        private void HandleMovement()
        {
            // Use CharacterController's isGrounded for reliability
            m_IsGrounded = m_CharacterController.isGrounded;

            // Reset vertical velocity when grounded
            if (m_IsGrounded && m_Velocity.y < 0)
            {
                m_Velocity.y = -2f; // Small downward force to keep grounded
            }

            // Calculate movement direction
            Vector3 moveDirection = transform.right * m_MoveInput.x + transform.forward * m_MoveInput.y;


            moveDirection.Normalize();

            // Apply speed (sprint or walk)
            float currentSpeed = m_SprintInput ? m_SprintSpeed : m_WalkSpeed;
            Vector3 move = moveDirection * currentSpeed;

            // Apply jump
            if (m_JumpInput && m_IsGrounded)
            {
                m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2f * m_Gravity);
                m_JumpInput = false;
            }

            // Apply gravity
            m_Velocity.y += m_Gravity * Time.deltaTime;

            // Combine horizontal movement and vertical velocity
            move.y = m_Velocity.y;

            // Move the character
            m_CharacterController.Move(move * Time.deltaTime);
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;


            m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - amount);
            HealthChanged?.Invoke(m_CurrentHealth);

            if (m_CurrentHealth == 0)
                Die();

        }

        public void Heal(int amount)
        {
            if (amount <= 0)
                return;


            m_CurrentHealth = Mathf.Min(m_MaxHealth, m_CurrentHealth + amount);
            HealthChanged?.Invoke(m_CurrentHealth);
        }

        private void Die()
        {
            // Handle player death: disable input, play animation, respawn logic, etc.
            enabled = false;
            UnityEngine.Debug.Log($"{name} died.");
            
            // Broadcast death event via current game application (safer across assemblies)
            var app = CustomNetworkManager.Singleton?.CurrentGameApp;
            if (app != null)
            {
                var player = GetComponent<Player>();
                app.Broadcast(new PlayerDiedEvent(player));
            }
        }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // PlayerInput event callbacks
        public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            m_MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            m_LookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed && m_IsGrounded)
            {
                m_JumpInput = true;
            }
        }

        public void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            m_SprintInput = context.performed;
        }
#endif
    }
}
