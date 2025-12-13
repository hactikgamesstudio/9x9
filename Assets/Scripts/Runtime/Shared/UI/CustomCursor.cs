using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Custom cursor that follows mouse and highlights UI buttons
    /// Shows visual notifications when cursor locks/unlocks
    /// </summary>
    public class CustomCursor : MonoBehaviour
    {
        [Header("Cursor Settings")]
        [SerializeField] private Texture2D m_CursorTexture;
        [SerializeField] private Texture2D m_CursorHoverTexture;
        [SerializeField] private Vector2 m_CursorHotspot = Vector2.zero;
        [SerializeField] private bool m_UseHardwareCursor = true;
        
        [Header("UI Highlighting")]
        [SerializeField] private bool m_HighlightButtons = true;
        [SerializeField] private Color m_HighlightColor = new Color(1f, 1f, 0.5f, 1f);
        [SerializeField] private float m_HighlightScale = 1.1f;

        [Header("Lock Notification")]
        [SerializeField] private bool m_ShowLockNotification = true;
        [SerializeField] private float m_NotificationDuration = 2f;
        
        private UIDocument m_UIDocument;
        private VisualElement m_Root;
        private VisualElement m_CurrentHoveredElement;
        private Color m_OriginalColor;
        private Scale m_OriginalScale;
        private bool m_IsInitialized = false;

        private Label m_LockNotificationLabel;
        private float m_NotificationTimer = 0f;
        private CursorLockMode m_LastLockState = CursorLockMode.None;
        
        void Start()
        {
            // Disable lock notifications completely
            m_ShowLockNotification = false;
            
            // Set cursor to always visible on menu (unlocked for menu navigation)
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            
            // Only apply custom cursor texture if provided AND properly configured
            // Most cursor textures fail validation, so we'll use default cursor instead
            if (m_CursorTexture != null && m_UseHardwareCursor)
            {
                try
                {
                    UnityEngine.Cursor.SetCursor(m_CursorTexture, m_CursorHotspot, CursorMode.Auto);
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[CustomCursor] Failed to set custom cursor texture: {ex.Message}. Using default cursor.");
                    // Fall back to default cursor
                    UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
            
            // Cache UIDocument reference to avoid repeated FindFirstObjectByType calls
            InitializeUIDocument();
        }

        void Update()
        {
            // Mouse input handled directly in code - no locking
        }

        void CreateLockNotificationUI()
        {
            // Lock notification UI disabled - mouse locking removed
        }

        void ShowLockNotification()
        {
            // Lock notification disabled - mouse locking removed
        }
        
        void InitializeUIDocument()
        {
            m_UIDocument = FindFirstObjectByType<UIDocument>();
            if (m_UIDocument == null)
            {
                UnityEngine.Debug.LogWarning("[CustomCursor] No UIDocument found in scene. Button highlighting disabled.");
                m_IsInitialized = false;
                return;
            }
            
            m_Root = m_UIDocument.rootVisualElement;
            if (m_Root == null)
            {
                UnityEngine.Debug.LogWarning("[CustomCursor] UIDocument has no root visual element. Button highlighting disabled.");
                m_IsInitialized = false;
                return;
            }
            
            m_IsInitialized = true;
        }
        
        void OnEnable()
        {
            if (!m_IsInitialized)
            {
                InitializeUIDocument();
            }
            if (!m_IsInitialized || m_Root == null) return;

            // Pointer-based highlighting (mouse/touch) without per-frame polling
            m_Root.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.NoTrickleDown);
            m_Root.RegisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
            m_Root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave, TrickleDown.TrickleDown);

            // Controller-driven focus highlighting: Buttons gain focus via keyboard/gamepad navigation
            m_Root.RegisterCallback<FocusInEvent>(OnFocusIn, TrickleDown.TrickleDown);
            m_Root.RegisterCallback<FocusOutEvent>(OnFocusOut, TrickleDown.TrickleDown);
        }
        
        // Pointer event handlers (mouse/touch)
        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!m_HighlightButtons) return;
            var element = evt.target as VisualElement;
            Button button = null;
            // If pointer is over a child, find nearest Button ancestor
            var current = element;
            while (current != null && button == null)
            {
                button = current as Button;
                current = current.parent;
            }

            if (button != null && m_CurrentHoveredElement != button)
            {
                UnhighlightCurrentElement();
                m_CurrentHoveredElement = button;
                HighlightElement(button);
                if (m_CursorHoverTexture != null && m_UseHardwareCursor)
                {
                    try
                    {
                        UnityEngine.Cursor.SetCursor(m_CursorHoverTexture, m_CursorHotspot, CursorMode.Auto);
                    }
                    catch
                    {
                        // Silently fail - use default cursor
                    }
                }
            }
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            // Optional: could set hover cursor here
        }

        void OnPointerLeave(PointerLeaveEvent evt)
        {
            if (m_CurrentHoveredElement != null)
            {
                UnhighlightCurrentElement();
                if (m_CursorTexture != null && m_UseHardwareCursor)
                {
                    try
                    {
                        UnityEngine.Cursor.SetCursor(m_CursorTexture, m_CursorHotspot, CursorMode.Auto);
                    }
                    catch
                    {
                        // Silently fail - use default cursor
                        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                }
            }
        }

        // Focus event handlers (keyboard/gamepad navigation)
        void OnFocusIn(FocusInEvent evt)
        {
            var button = evt.target as Button;
            if (button == null) return;
            // Highlight focused button
            UnhighlightCurrentElement();
            m_CurrentHoveredElement = button;
            HighlightElement(button);
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            var button = evt.target as Button;
            if (button == null) return;
            if (m_CurrentHoveredElement == button)
            {
                UnhighlightCurrentElement();
            }
        }
        
        void HighlightElement(VisualElement element)
        {
            if (element == null) return;
            
            // Store original style
            m_OriginalColor = element.resolvedStyle.backgroundColor;
            m_OriginalScale = element.resolvedStyle.scale;
            
            // Apply highlight
            element.style.backgroundColor = m_HighlightColor;
            element.style.scale = new StyleScale(new Scale(Vector3.one * m_HighlightScale));
        }
        
        void UnhighlightCurrentElement()
        {
            if (m_CurrentHoveredElement == null) return;
            
            // Restore original style
            m_CurrentHoveredElement.style.backgroundColor = m_OriginalColor;
            m_CurrentHoveredElement.style.scale = new StyleScale(m_OriginalScale);
            
            m_CurrentHoveredElement = null;
        }
        
        void OnDisable()
        {
            // Cleanup any highlighted elements when disabled
            UnhighlightCurrentElement();
            if (m_Root != null)
            {
                // Unregister event handlers
                m_Root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                m_Root.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
                m_Root.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
                m_Root.UnregisterCallback<FocusInEvent>(OnFocusIn);
                m_Root.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            }
        }
        
        void OnDestroy()
        {
            // Reset cursor to default
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
