using System;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Minimal stub used by UnityServicesInitializer; can be expanded to use UGS Matchmaker
    internal class MatchmakerTicketer : MonoBehaviour
    {
        private string m_LastQueueName = string.Empty;

        public string LastQueueName => m_LastQueueName;

        /// <summary>
        /// Finds a match in the specified queue
        /// </summary>
        public void FindMatch(string queueName, Action<SessionError> onCompleted, Action<float> onUpdateTimer)
        {
            m_LastQueueName = queueName;
            Debug.Log($"[MatchmakerTicketer] Finding match in queue: {queueName}");
            // TODO: Integrate with Unity Gaming Services Matchmaker API
            // For now, simulate success after a delay
            onCompleted?.Invoke(SessionError.None);
        }

        /// <summary>
        /// Stops the current matchmaking search
        /// </summary>
        public async void StopSearch()
        {
            Debug.Log("[MatchmakerTicketer] Stopping matchmaking search");
            // TODO: Call UGS API to cancel ticket
            await System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// Leaves the current match session
        /// </summary>
        public async void LeaveSession()
        {
            Debug.Log("[MatchmakerTicketer] Leaving match session");
            // TODO: Call UGS API to leave session
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }

    /// <summary>
    /// Session error enum for matchmaking operations
    /// </summary>
    public enum SessionError
    {
        None,
        Unknown,
        MatchmakerAssignmentFailed,
        MatchmakerAssignmentTimeout,
        MatchmakerCancelled
    }
}
