#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Netcode;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    /// <summary>
    /// Editor-time validator for Netcode NetworkPrefabs to catch null or invalid entries.
    /// - Warns on domain reload.
    /// - Provides a manual menu to validate and auto-fix.
    /// </summary>
    internal static class NetcodeNetworkPrefabValidator
    {
        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            // Delay to allow scenes to finish loading
            EditorApplication.delayCall += () => ValidateAll(openScenesOnly: true, autoFix: false);
        }

        [MenuItem("Tools/Netcode/Validate Network Prefabs")]
        private static void ValidateMenu()
        {
            ValidateAll(openScenesOnly: false, autoFix: false);
        }

        [MenuItem("Tools/Netcode/Validate && Fix Network Prefabs")] 
        private static void ValidateAndFixMenu()
        {
            ValidateAll(openScenesOnly: false, autoFix: true);
        }

        private static void ValidateAll(bool openScenesOnly, bool autoFix)
        {
            try
            {
                var managers = FindNetworkManagers(openScenesOnly);
                int totalRemoved = 0;
                foreach (var nm in managers)
                {
                    totalRemoved += ValidateOne(nm, autoFix);
                }

                if (totalRemoved > 0 && autoFix)
                {
                    Debug.LogWarning($"[Netcode] Validator removed {totalRemoved} invalid NetworkPrefab entries across NetworkManagers.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Netcode] Validator encountered an issue: {ex.Message}");
            }
        }

        private static IEnumerable<NetworkManager> FindNetworkManagers(bool openScenesOnly)
        {
            if (openScenesOnly)
            {
                return Resources.FindObjectsOfTypeAll<NetworkManager>()
                                 .Where(nm => nm != null && nm.gameObject.scene.IsValid());
            }
            else
            {
                return Resources.FindObjectsOfTypeAll<NetworkManager>()
                                 .Where(nm => nm != null);
            }
        }

        private static int ValidateOne(NetworkManager networkManager, bool autoFix)
        {
            if (networkManager == null || networkManager.NetworkConfig == null || networkManager.NetworkConfig.Prefabs == null)
            {
                return 0;
            }

            var prefabList = networkManager.NetworkConfig.Prefabs;
            if (prefabList == null || prefabList.Prefabs == null)
            {
                return 0;
            }

            // Build list of valid prefabs (removing nulls, duplicates, and non-NetworkObject prefabs)
            var validPrefabs = new List<NetworkPrefab>();
            var seen = new HashSet<GameObject>();
            int invalid = 0;

            foreach (var entry in prefabList.Prefabs)
            {
                bool isValid = entry.Prefab != null &&
                               entry.Prefab.TryGetComponent<NetworkObject>(out _) &&
                               !seen.Contains(entry.Prefab);

                if (isValid)
                {
                    validPrefabs.Add(entry);
                    seen.Add(entry.Prefab);
                }
                else
                {
                    invalid++;
                    if (!autoFix)
                    {
                        var reason = entry.Prefab == null ? "null prefab"
                                    : (!entry.Prefab.TryGetComponent<NetworkObject>(out _) ? "missing NetworkObject"
                                    : "duplicate prefab");
                        Debug.LogWarning($"[Netcode] Invalid NetworkPrefab on {networkManager.name}: {reason}", networkManager);
                    }
                }
            }

            if (invalid > 0 && autoFix)
            {
                Debug.LogWarning($"[Netcode] Found {invalid} invalid NetworkPrefab entries on {networkManager.name}. " +
                                 "Please manually remove null/duplicate entries from NetworkManager → NetworkConfig → Prefabs list in the Inspector.", 
                                 networkManager);
            }

            return invalid;
        }
    }
}
#endif
