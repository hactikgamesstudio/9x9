using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// In-game HUD controller for health, inventory, minimap display.
    /// Uses UI Toolkit for flexible, performant runtime UI.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private UIDocument m_UIDocument;
        private VisualElement m_Root;
        private Label m_HealthLabel;
        private Label m_InventoryLabel;

        void Start()
        {
            if (m_UIDocument == null)
            {
                m_UIDocument = GetComponent<UIDocument>();
            }

            if (m_UIDocument != null)
            {
                m_Root = m_UIDocument.rootVisualElement;
                m_HealthLabel = m_Root?.Q<Label>("healthLabel");
                m_InventoryLabel = m_Root?.Q<Label>("inventoryLabel");
            }
        }

        public void UpdateHealth(int current, int max)
        {
            if (m_HealthLabel != null)
                m_HealthLabel.text = $"Health: {current}/{max}";
        }

        public void UpdateInventory(string itemList)
        {
            if (m_InventoryLabel != null)
                m_InventoryLabel.text = $"Inventory: {itemList}";
        }

        public void Show() { if (gameObject != null) gameObject.SetActive(true); }
        public void Hide() { if (gameObject != null) gameObject.SetActive(false); }
    }
}
