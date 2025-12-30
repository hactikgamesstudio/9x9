using Unity.Template.Multiplayer.NGO.Core;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Main controller of the <see cref="MetagameApplication"></see>
    /// </summary>
    public class MetagameController : Controller<MetagameApplication>
    {
        void Awake()
        {
            AddListener<PlayerSignedIn>(OnPlayerSignedIn);
            AddListener<MatchEnteredEvent>(OnMatchEntered);
        }

        void OnDestroy()
        {
            RemoveListeners();
        }

        protected override void RemoveListeners()
        {
            RemoveListener<PlayerSignedIn>(OnPlayerSignedIn);
            RemoveListener<MatchEnteredEvent>(OnMatchEntered);
        }
        void OnPlayerSignedIn(Unity.Template.Multiplayer.NGO.Runtime.PlayerSignedIn evt)
        {
            if (evt.Success)
            {
                UnityEngine.Debug.Log($"Player signed in with id {evt.PlayerId}");
            }
            else
            {
                UnityEngine.Debug.Log("Player did not sign in");
            }
        }

        void OnMatchEntered(MatchEnteredEvent evt)
        {
            DisableViewsAndListeners();
        }

        void DisableViewsAndListeners()
        {
            for (int i = 0; i < App.View.transform.childCount; i++)
            {
                App.View.transform.GetChild(i).gameObject.SetActive(false);
            }
            App.OnReturnToMetagameAfterMatch -= OnReturnToMetagameAfterMatch;
            App.OnReturnToMetagameAfterMatch += OnReturnToMetagameAfterMatch;
            CustomNetworkManager.Singleton.ReturnToMetagame = App.CallOnReturnToMetagameAfterMatch;
        }

        void OnReturnToMetagameAfterMatch()
        {
            EnableViewsAndListener();
        }

        void EnableViewsAndListener()
        {
            for (int i = 0; i < App.View.transform.childCount; i++)
            {
                App.View.transform.GetChild(i).gameObject.SetActive(true);
            }
            App.View.Matchmaker.Hide();
            App.View.LoadingScreen.Hide();
            App.View.MainMenu.DisableControlsUnsupportedInAutoconnectMode();
        }
    }
}
