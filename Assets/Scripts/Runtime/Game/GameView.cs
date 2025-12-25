using Unity.Template.Multiplayer.NGO.Core;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main View of the <see cref="GameApplication"></see>
    /// </summary>
    public class GameView : View<GameApplication>
    {
        internal MonoBehaviour Match => m_MatchView;

        [SerializeField]
        MonoBehaviour m_MatchView;

        internal MonoBehaviour MatchRecap => m_MatchRecapView;

        [SerializeField]
        MonoBehaviour m_MatchRecapView;

        void Awake()
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
