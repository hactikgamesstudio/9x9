using UnityEngine.UIElements;
using Unity.Template.Multiplayer.NGO.Core;

namespace Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.Loading
{
    public class LoadingScreenView : View<MetagameApplication>
    {
        Button m_QuitButton;
        VisualElement m_Root;
        UIDocument m_UIDocument;

        void Awake()
        {
            m_UIDocument = GetComponent<UIDocument>();
        }

        void OnEnable()
        {
            m_Root = m_UIDocument.rootVisualElement;
            m_QuitButton = m_Root.Q<Button>("quitButton");
            m_QuitButton.RegisterCallback<ClickEvent>(OnClickQuit);
        }

        void OnDisable()
        {
            m_QuitButton.UnregisterCallback<ClickEvent>(OnClickQuit);
        }

        void OnClickQuit(ClickEvent evt)
        {
            Broadcast(new ExitMatchLoadingEvent());
        }
    }
}
