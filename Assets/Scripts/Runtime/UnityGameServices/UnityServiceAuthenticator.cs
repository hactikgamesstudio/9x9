using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Minimal authenticator shim for editor/offline mode
    internal static class UnityServiceAuthenticator
    {
        public static string PlayerId { get; private set; } = string.Empty;

        public static async Task<bool> TrySignInAsync(string environment, string profileName)
        {
            try
            {
                // In a full implementation, call Unity Services Authentication here.
                // For now, simulate an async sign-in and assign a stable id.
                await Task.Yield();
                PlayerId = SystemInfo.deviceUniqueIdentifier;
                if (string.IsNullOrEmpty(PlayerId))
                {
                    PlayerId = Guid.NewGuid().ToString("N");
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGS] Sign-in failed: {e.Message}");
                PlayerId = string.Empty;
                return false;
            }
        }
    }
}
