using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// First-person player controller with movement, jumping, sprinting, and health management.
    /// This is a direct port from Godot's CharacterBody3D.gd script.
    ///
    /// **FOR BEGINNERS:**
    /// - CharacterController is Unity's built-in component for player movement with collision
    /// - Input System (new) replaces the old Input.GetAxis system
    /// - Events (C# events/delegates) work like Godot signals for communication between scripts
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
#region Inspector Variables (Godot @export equivalent)

        [Header("Movement Settings")]
        [Tooltip("Normal walking speed in units per second")]
        [SerializeField]
        private float m_Speed = 5f;

        [Tooltip("Sprint speed when holding shift")]
        [SerializeField]
        private float m_SprintSpeed = 7f;

        [Tooltip("Initial upward velocity when jumping")]
        [SerializeField]
        private float m_JumpVelocity = 4.5f;

        [Header("Camera Settings")]
        [Tooltip("Mouse sensitivity for looking around")]
        [SerializeField]
        private float m_Sensitivity = 0.3f;

        [Tooltip("Reference to the camera (usually a child object)")]
        [SerializeField]
        private Camera m_Camera;

        [Tooltip("Minimum camera pitch angle (looking down)")]
        [SerializeField]
        private float m_MinCameraPitch = -90f;

        [Tooltip("Maximum camera pitch angle (looking up)")]
        [SerializeField]
        private float m_MaxCameraPitch = 90f;

        [Tooltip("Normal field of view")]
        [SerializeField]
        private float m_NormalFOV = 85f;

        [Tooltip("Sprint field of view (zoom effect)")]
        [SerializeField]
        private float m_SprintFOV = 110f;

        [Tooltip("FOV transition speed")]
        [SerializeField]
        private float m_FOVTransitionSpeed = 5f;

        [Header("Health Settings")]
        [Tooltip("Maximum health points")]
        [SerializeField]
        private int m_MaxHealth = 100;

#endregion

#region Private Variables

        private CharacterController m_CharacterController;
        private float m_CameraPitch; // Current X rotation of camera
        private float m_Yaw; // Current Y rotation of player body
        private Vector3 m_Velocity;
        private float m_CurrentSpeed;
        private int m_Health;
        private float m_TargetFOV;

        // Input values from new Input System
        private Vector2 m_MoveInput;
        private Vector2 m_LookInput;
        private bool m_JumpInput;
        private bool m_SprintInput;

#endregion

#region Events (Godot signals equivalent)

        /// <summary>
        /// Event fired when health changes. Other scripts (like HUD) can subscribe to this.
        /// In Godot this was: signal health_changed(new_health: int)
        /// In C#: public event System.Action<int> HealthChanged;
        /// </summary>
        public event System.Action<int> HealthChanged;

#endregion

#region Properties (Public access to private data)

        public int Health => m_Health;
        public int MaxHealth => m_MaxHealth;

#endregion

#region Unity Lifecycle Methods

        /// <summary>
        /// Called when script instance is loaded. Similar to Godot's _ready().
        /// **FOR BEGINNERS:**
        /// - Awake() is called before Start(), use for component references
        /// - Start() is called before first frame, use for initialization
        /// - GetComponent<T>() finds a component attached to the same GameObject
        /// </summary>
        void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();

            // If camera not assigned in inspector, try to find it
            if (m_Camera == null)
            {
                m_Camera = GetComponentInChildren<Camera>();
            }

            m_Health = m_MaxHealth;
            m_CurrentSpeed = m_Speed;
            m_TargetFOV = m_NormalFOV;
        }

        /// <summary>
        /// Called on first frame. Good for finding other objects in scene.
        /// </summary>
        void Start()
        {
            // Lock and hide cursor (Godot's MOUSE_MODE_CAPTURED)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Notify HUD of initial health
            HealthChanged?.Invoke(m_Health);
        }

        /// <summary>
        /// Called every frame. Use for input and non-physics updates.
        /// In Godot this was handled in _process() and _input().
        /// **FOR BEGINNERS:**
        /// - Update() runs every frame (variable framerate)
        /// - FixedUpdate() runs at fixed intervals (for physics)
        /// - Delta time = Time.deltaTime (time since last frame)
        /// </summary>
        void Update()
        {
            HandleMouseLook();
            HandleFOVTransition();
        }

        /// <summary>
        /// Physics update - called at fixed intervals. Similar to Godot's _physics_process(delta).
        /// Use for movement and physics calculations.
        /// </summary>
        void FixedUpdate()
        {
            HandleMovement();
        }

#endregion

#region Input Handling (New Input System callbacks)

        /// <summary>
        /// Called by Input System when WASD/arrow keys are pressed.
        /// **FOR BEGINNERS:**
        /// - These methods are called automatically by Unity's Input System
        /// - You need to set up Input Actions in Project Settings > Input System
        /// - context.ReadValue<Vector2>() gets the input value
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            m_MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            m_LookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            m_JumpInput = context.ReadValueAsButton();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            m_SprintInput = context.ReadValueAsButton();

            if (m_SprintInput)
            {
                m_CurrentSpeed = m_SprintSpeed;
                m_TargetFOV = m_SprintFOV;
            }
            else
            {
                m_CurrentSpeed = m_Speed;
                m_TargetFOV = m_NormalFOV;
            }
        }

#endregion

#region Movement & Camera Logic

        /// <summary>
        /// Handles mouse look (camera rotation). In Godot this was in _input().
        /// </summary>
        void HandleMouseLook()
        {
            // Apply mouse look (Y axis controls pitch, X axis controls yaw)
            m_CameraPitch -= m_LookInput.y * m_Sensitivity * Time.deltaTime * 100f;
            m_CameraPitch = Mathf.Clamp(m_CameraPitch, m_MinCameraPitch, m_MaxCameraPitch);

            m_Yaw += m_LookInput.x * m_Sensitivity * Time.deltaTime * 100f;

            // Apply rotations
            if (m_Camera != null)
            {
                m_Camera.transform.localRotation = Quaternion.Euler(m_CameraPitch, 0f, 0f);
            }
            transform.rotation = Quaternion.Euler(0f, m_Yaw, 0f);
        }

        /// <summary>
        /// Smoothly transitions FOV when sprinting. In Godot this was direct assignment.
        /// </summary>
        void HandleFOVTransition()
        {
            if (m_Camera != null)
            {
                m_Camera.fieldOfView = Mathf.Lerp(
                    m_Camera.fieldOfView,
                    m_TargetFOV,
                    Time.deltaTime * m_FOVTransitionSpeed
                );
            }
        }

        /// <summary>
        /// Handles player movement and jumping. In Godot this was in _physics_process().
        /// **FOR BEGINNERS:**
        /// - CharacterController.Move() is like Godot's move_and_slide()
        /// - CharacterController.isGrounded checks if touching ground
        /// - Transform.TransformDirection converts local direction to world direction
        /// </summary>
        void HandleMovement()
        {
            // Apply gravity (CharacterController doesn't do this automatically)
            if (!m_CharacterController.isGrounded)
            {
                m_Velocity.y -= 9.81f * Time.fixedDeltaTime;
            }
            else
            {
                // Reset Y velocity when grounded
                m_Velocity.y = -2f; // Small negative to keep grounded
            }

            // Handle jumping
            if (m_JumpInput && m_CharacterController.isGrounded)
            {
                m_Velocity.y = m_JumpVelocity;
            }

            // Calculate movement direction relative to player rotation
            Vector3 moveDirection = new Vector3(m_MoveInput.x, 0f, m_MoveInput.y);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.Normalize();

            // Apply horizontal movement
            if (moveDirection.magnitude > 0.1f)
            {
                m_Velocity.x = moveDirection.x * m_CurrentSpeed;
                m_Velocity.z = moveDirection.z * m_CurrentSpeed;
            }
            else
            {
                // Decelerate to zero (like Godot's move_toward)
                m_Velocity.x = Mathf.MoveTowards(
                    m_Velocity.x,
                    0f,
                    m_CurrentSpeed * Time.fixedDeltaTime * 2f
                );
                m_Velocity.z = Mathf.MoveTowards(
                    m_Velocity.z,
                    0f,
                    m_CurrentSpeed * Time.fixedDeltaTime * 2f
                );
            }

            // Move the character
            m_CharacterController.Move(m_Velocity * Time.fixedDeltaTime);
        }

#endregion

#region Health Management

        /// <summary>
        /// Reduces player health by specified amount. Called by hazards.
        /// In Godot this was: func take_damage(amount: float)
        /// </summary>
        public void TakeDamage(int amount)
        {
            m_Health = Mathf.Clamp(m_Health - amount, 0, m_MaxHealth);
            HealthChanged?.Invoke(m_Health);

            if (m_Health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Restores player health by specified amount.
        /// In Godot this was: func heal(amount: int)
        /// </summary>
        public void Heal(int amount)
        {
            m_Health = Mathf.Clamp(m_Health + amount, 0, m_MaxHealth);
            HealthChanged?.Invoke(m_Health);
        }

        /// <summary>
        /// Called when player dies. Reloads the scene.
        /// **FOR BEGINNERS:**
        /// - SceneManager.LoadScene() is Unity's way to change scenes
        /// - SceneManager.GetActiveScene() gets the current scene
        /// - You need: using UnityEngine.SceneManagement; at top of file
        /// </summary>
        void Die()
        {
            Debug.Log("Player died, reloading scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

#endregion

#region Editor Helpers

        /// <summary>
        /// Called when values change in the Inspector. Useful for validation.
        /// Only runs in the Unity Editor, not in builds.
        /// </summary>
        void OnValidate()
        {
            m_Speed = Mathf.Max(0f, m_Speed);
            m_SprintSpeed = Mathf.Max(m_Speed, m_SprintSpeed);
            m_JumpVelocity = Mathf.Max(0f, m_JumpVelocity);
            m_Sensitivity = Mathf.Max(0.01f, m_Sensitivity);
            m_MaxHealth = Mathf.Max(1, m_MaxHealth);
        }

#endregion
    }
}
