using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    /// <summary>
    /// Helper to fix MetagameApplication prefab after assembly reorganization.
    /// Run this from menu: Tools > Fix Metagame Prefab
    /// </summary>
    public static class FixMetagamePrefab
    {
        [MenuItem("Tools/Fix Metagame Prefab")]
        public static void Fix()
        {
            string prefabPath = "Assets/Prefabs/Metagame/MetagameApplication.prefab";
            
            Debug.Log($"[FixMetagamePrefab] Loading prefab: {prefabPath}");
            
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[FixMetagamePrefab] Could not load prefab at {prefabPath}");
                return;
            }
            
            // Open prefab in isolation mode
            string prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabAssetPath);
            
            try
            {
                bool madeChanges = false;
                
                // Remove all missing scripts
                var allTransforms = prefabContents.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in allTransforms)
                {
                    int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                    if (removed > 0)
                    {
                        Debug.Log($"[FixMetagamePrefab] Removed {removed} missing script(s) from: {GetPath(t)}");
                        madeChanges = true;
                    }
                }
                
                // Verify key components exist
                var metaApp = prefabContents.GetComponent<Unity.Template.Multiplayer.NGO.Runtime.MetagameApplication>();
                if (metaApp == null)
                {
                    Debug.LogWarning("[FixMetagamePrefab] MetagameApplication component missing!");
                }
                else
                {
                    Debug.Log("[FixMetagamePrefab] ✓ MetagameApplication component found");
                }
                
                var metaView = prefabContents.GetComponent<Unity.Template.Multiplayer.NGO.Runtime.MetagameView>();
                if (metaView == null)
                {
                    Debug.LogWarning("[FixMetagamePrefab] MetagameView component missing!");
                }
                else
                {
                    Debug.Log("[FixMetagamePrefab] ✓ MetagameView component found");
                }
                
                var metaController = prefabContents.GetComponentInChildren<Unity.Template.Multiplayer.NGO.Runtime.MetagameController>();
                if (metaController == null)
                {
                    Debug.LogWarning("[FixMetagamePrefab] MetagameController component missing!");
                }
                else
                {
                    Debug.Log("[FixMetagamePrefab] ✓ MetagameController component found");
                }
                
                var metaModel = prefabContents.GetComponentInChildren<Unity.Template.Multiplayer.NGO.Runtime.MetagameModel>();
                if (metaModel == null)
                {
                    Debug.LogWarning("[FixMetagamePrefab] MetagameModel component missing!");
                }
                else
                {
                    Debug.Log("[FixMetagamePrefab] ✓ MetagameModel component found");
                }
                
                if (madeChanges)
                {
                    // Save changes back to prefab
                    PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabAssetPath);
                    Debug.Log("[FixMetagamePrefab] ✓ Prefab saved successfully!");
                }
                else
                {
                    Debug.Log("[FixMetagamePrefab] No missing scripts found. Prefab is clean!");
                }
            }
            finally
            {
                // Always unload prefab contents
                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
            
            AssetDatabase.Refresh();
            Debug.Log("[FixMetagamePrefab] Done!");
        }
        
        static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}
