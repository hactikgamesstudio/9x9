using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// End-of-match recap screen showing winner, stats, and menu options.
    /// </summary>
    public class MatchRecapView : MonoBehaviour
    {
        [SerializeField] private UIDocument m_UIDocument;
        private VisualElement m_Root;

        void Start()
        {
            if (m_UIDocument == null)
            {
                m_UIDocument = GetComponent<UIDocument>();
            }

            if (m_UIDocument != null)
            {
                m_Root = m_UIDocument.rootVisualElement;
            }
        }

        public void SetResult(string resultText)
        {
            var resultLabel = m_Root?.Q<Label>("resultLabel");
            if (resultLabel != null)
                resultLabel.text = resultText;
        }

        public void Show() { if (gameObject != null) gameObject.SetActive(true); }
        public void Hide() { if (gameObject != null) gameObject.SetActive(false); }
    }
}
