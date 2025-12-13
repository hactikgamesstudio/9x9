using UnityEngine;
using Unity.Template.Multiplayer.NGO.Core;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.MainMenu;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.Matchmaker;
using Unity.Template.Multiplayer.NGO.Runtime.UI.Menus.Loading;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main view of the <see cref="MetagameApplication"></see>
    /// </summary>
    public class MetagameView : View<MetagameApplication>
    {
        public MainMenuView MainMenu => m_MainMenuView;

        [SerializeField]
        MainMenuView m_MainMenuView;

        public MatchmakerView Matchmaker => m_MatchmakerView;

        [SerializeField]
        MatchmakerView m_MatchmakerView;

        public LoadingScreenView LoadingScreen => m_LoadingScreenView;

        [SerializeField]
        LoadingScreenView m_LoadingScreenView;

        void Start()
        {
            if (App.IsDedicatedServer)
            {
                OnDedicatedServerDestroyViews();
            }
        }

        void OnDedicatedServerDestroyViews()
        {
            Destroy(gameObject);
        }
    }
}
