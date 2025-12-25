using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ProjectCleanupTools : EditorWindow
{
    private static bool s_DryRun = true;
    private static readonly string[] s_DefaultWhitelistFolders = new[]
    {
        "Assets/Scenes",
        "Assets/Materials",
        "Assets/Prefabs",
        "Assets/Scripts",
        "Assets/Textures",
        "Assets/Models",
        "Assets/Audio",
        "Assets/Animations",
        "Assets/Settings",
        "Assets/UIToolkit",
        "Assets/ThirdParty",
        "Assets/Resources"
    };
    private static HashSet<string> s_ActiveWhitelist = new HashSet<string>(s_DefaultWhitelistFolders);

    // Preview caches
    private static readonly List<string> s_PreviewEmptyFolders = new List<string>();
    private static readonly List<(string source, string dest)> s_PreviewUnusedAssetMoves = new List<(string, string)>();
    private static readonly List<(string source, string dest)> s_PreviewVendorMoves = new List<(string, string)>();
    private static bool s_HavePreview;
    private Vector2 _scroll;
    [MenuItem("Tools/Project Cleanup Tools")]
    public static void ShowWindow()
    {
        GetWindow<ProjectCleanupTools>("Project Cleanup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cleanup & Organization", EditorStyles.boldLabel);
        s_DryRun = EditorGUILayout.ToggleLeft("Dry Run (preview only)", s_DryRun);
        if (GUILayout.Button("Generate Preview (All Tasks)"))
        {
            GeneratePreview();
        }
        if (s_HavePreview && !s_DryRun)
        {
            if (GUILayout.Button("Apply Previewed Changes")) ApplyPreviewedChanges();
        }
        EditorGUILayout.Space();
        DrawPreviewSection();
        EditorGUILayout.Space();
        DrawWhitelistEditor();
        EditorGUILayout.Space();
        GUILayout.Label("Individual Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Delete Empty Folders (Direct)")) DeleteEmptyFolders();
        if (GUILayout.Button("Move Unused Assets to _Unused (Direct)")) MoveUnusedAssets();
        if (GUILayout.Button("Organize Third-Party Assets (Direct)")) OrganizeThirdPartyAssets();
        if (GUILayout.Button("Organize Third-Party by Type")) OrganizeThirdPartyByType();
        if (GUILayout.Button("Scan for Missing Scripts")) ScanMissingScripts();
        if (GUILayout.Button("Generate Cleanup Report")) GenerateCleanupReport();
    }

    private void DrawPreviewSection()
    {
        GUILayout.Label("Preview Summary", EditorStyles.boldLabel);
        if (!s_HavePreview)
        {
            EditorGUILayout.HelpBox("No preview generated yet.", MessageType.Info);
            return;
        }
        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(180));
        GUILayout.Label($"Empty Folders: {s_PreviewEmptyFolders.Count}");
        foreach (var f in s_PreviewEmptyFolders.Take(20)) GUILayout.Label(" • " + f);
        if (s_PreviewEmptyFolders.Count > 20) GUILayout.Label(" … (truncated)");
        GUILayout.Space(6);
        GUILayout.Label($"Unused Asset Moves: {s_PreviewUnusedAssetMoves.Count}");
        foreach (var m in s_PreviewUnusedAssetMoves.Take(10)) GUILayout.Label($" • {m.source} -> {m.dest}");
        if (s_PreviewUnusedAssetMoves.Count > 10) GUILayout.Label(" … (truncated)");
        GUILayout.Space(6);
        GUILayout.Label($"Vendor Folder Moves: {s_PreviewVendorMoves.Count}");
        foreach (var v in s_PreviewVendorMoves.Take(10)) GUILayout.Label($" • {v.source} -> {v.dest}");
        if (s_PreviewVendorMoves.Count > 10) GUILayout.Label(" … (truncated)");
        EditorGUILayout.EndScrollView();
    }

    private void DrawWhitelistEditor()
    {
        GUILayout.Label("Whitelist Folders", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Whitelisted folders are never deleted or moved.", MessageType.None);
        foreach (var entry in s_DefaultWhitelistFolders)
        {
            bool active = s_ActiveWhitelist.Contains(entry);
            bool toggled = EditorGUILayout.ToggleLeft(entry, active);
            if (toggled && !active) s_ActiveWhitelist.Add(entry);
            else if (!toggled && active) s_ActiveWhitelist.Remove(entry);
        }
        EditorGUILayout.Space();
        GUILayout.Label("Add Custom Whitelist Folder (relative to Assets)");
        string newFolder = EditorGUILayout.TextField("New Folder", _newWhitelistCandidate);
        if (newFolder != _newWhitelistCandidate)
            _newWhitelistCandidate = newFolder;
        if (GUILayout.Button("Add Folder to Whitelist"))
        {
            if (!string.IsNullOrWhiteSpace(_newWhitelistCandidate))
            {
                string norm = NormalizeAssetPath(_newWhitelistCandidate);
                if (!s_ActiveWhitelist.Contains(norm)) s_ActiveWhitelist.Add(norm);
                _newWhitelistCandidate = string.Empty;
            }
        }
    }

    private string _newWhitelistCandidate = string.Empty;

    private static void DeleteEmptyFolders()
    {
        string assetsPath = Application.dataPath;
        var dirs = Directory.GetDirectories(assetsPath, "*", SearchOption.AllDirectories).ToList();
        int deleted = 0;
        foreach (var dir in dirs)
        {
            // Skip special folders
            if (dir.Replace('\\','/').Contains("/Library") || dir.Replace('\\','/').Contains("/Temp")) continue;
            // Whitelist: never delete top-level whitelisted folders
            string dirAssetPath = "Assets" + dir.Replace(Application.dataPath, string.Empty).Replace('\\','/');
            if (IsWhitelisted(dirAssetPath)) continue;
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
            {
                if (!s_DryRun)
                {
                    FileUtil.DeleteFileOrDirectory(dir);
                }
                deleted++;
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Cleanup", s_DryRun ? $"Would delete {deleted} empty folders (Dry Run)." : $"Deleted {deleted} empty folders.", "OK");
    }

    private static void MoveUnusedAssets()
    {
        string unusedFolder = "Assets/_Unused";
        if (!AssetDatabase.IsValidFolder(unusedFolder) && !s_DryRun) AssetDatabase.CreateFolder("Assets", "_Unused");

        string[] assetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/") && !p.StartsWith(unusedFolder) && !AssetDatabase.IsValidFolder(p))
            .ToArray();

        // Consider anything referenced by scenes, prefabs, or scriptable objects as used
        string[] rootCandidates = AssetDatabase.FindAssets("t:Scene t:Prefab t:ScriptableObject");
        var used = new HashSet<string>();
        foreach (var guid in rootCandidates)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            foreach (var dep in AssetDatabase.GetDependencies(path, true)) used.Add(dep);
        }

        int moved = 0;
        foreach (var asset in assetPaths)
        {
            if (IsWhitelisted(asset)) continue; // never move whitelisted
            if (used.Contains(asset)) continue;
            string fileName = Path.GetFileName(asset);
            string dest = AssetDatabase.GenerateUniqueAssetPath($"{unusedFolder}/{fileName}");
            if (!s_DryRun)
            {
                var result = AssetDatabase.MoveAsset(asset, dest);
                if (string.IsNullOrEmpty(result)) moved++;
            }
            else
            {
                moved++;
                Debug.Log($"[Dry Run] Would move: {asset} -> {dest}");
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Cleanup", s_DryRun ? $"Would move {moved} unused assets to _Unused (Dry Run)." : $"Moved {moved} unused assets to _Unused.", "OK");
    }

    private static void OrganizeThirdPartyAssets()
    {
        // Move vendor folders to Assets/ThirdParty/<Vendor>
        string thirdPartyRoot = "Assets/ThirdParty";
        if (!AssetDatabase.IsValidFolder(thirdPartyRoot) && !s_DryRun) AssetDatabase.CreateFolder("Assets", "ThirdParty");

        // Heuristics: folders that contain README, LICENSE, or have vendor names
        string[] vendorHints = new[] {"Plugins", "ThirdParty", "External", "Odin", "DOTween", "TextMeshPro", "Cinemachine", "EZ", "FMOD"};
        var assetDirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.TopDirectoryOnly);
        int moved = 0;
        foreach (var dir in assetDirs)
        {
            var dirName = Path.GetFileName(dir);
            if (dirName == "ThirdParty" || dirName.StartsWith("_")) continue;
            string dirPathAsset = $"Assets/{dirName}";
            if (vendorHints.Any(h => dirName.IndexOf(h, System.StringComparison.OrdinalIgnoreCase) >= 0))
            {
                string destFolder = $"{thirdPartyRoot}/{dirName}";
                if (!s_DryRun)
                {
                    EnsureFolder(thirdPartyRoot, dirName);
                    var result = AssetDatabase.MoveAsset(dirPathAsset, destFolder);
                    if (string.IsNullOrEmpty(result)) moved++;
                }
                else
                {
                    moved++;
                    Debug.Log($"[Dry Run] Would move folder: {dirPathAsset} -> {destFolder}");
                }
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Third-Party", s_DryRun ? $"Would organize {moved} vendor folders into Assets/ThirdParty (Dry Run)." : $"Organized {moved} vendor folders into Assets/ThirdParty.", "OK");
    }

    private static void OrganizeThirdPartyByType()
    {
        // Move meshes/models, textures, prefabs, and documentation from ThirdParty to centralized folders
        string thirdPartyRoot = "Assets/ThirdParty";
        if (!AssetDatabase.IsValidFolder(thirdPartyRoot))
        {
            EditorUtility.DisplayDialog("Third-Party", "No ThirdParty folder found.", "OK");
            return;
        }

        // Ensure destination folders exist
        EnsureFolder("Assets", "Models");
        EnsureFolder("Assets/Models", "ThirdParty");
        EnsureFolder("Assets", "Textures");
        EnsureFolder("Assets/Textures", "ThirdParty");
        EnsureFolder("Assets/Prefabs", "ThirdParty");
        string docDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Documentation", "ThirdParty");
        Directory.CreateDirectory(docDir);

        int moved = 0;

        // Find all folders in ThirdParty recursively
        string thirdPartyPath = Path.Combine(Application.dataPath, "ThirdParty");
        if (!Directory.Exists(thirdPartyPath)) return;

        var allDirs = Directory.GetDirectories(thirdPartyPath, "*", SearchOption.AllDirectories);
        foreach (var dir in allDirs)
        {
            string folderName = Path.GetFileName(dir).ToLower();
            string assetPath = "Assets" + dir.Replace(Application.dataPath, string.Empty).Replace('\\', '/');

            // Skip meta files and already processed
            if (folderName.EndsWith(".meta")) continue;

            string destFolder = null;
            string destRoot = null;

            // Determine destination based on folder name
            if (folderName.Contains("mesh") || folderName.Contains("model"))
            {
                destRoot = "Assets/Models/ThirdParty";
                destFolder = $"{destRoot}/{GetVendorName(assetPath)}";
            }
            else if (folderName.Contains("texture"))
            {
                destRoot = "Assets/Textures/ThirdParty";
                destFolder = $"{destRoot}/{GetVendorName(assetPath)}";
            }
            else if (folderName.Contains("prefab"))
            {
                destRoot = "Assets/Prefabs/ThirdParty";
                destFolder = $"{destRoot}/{GetVendorName(assetPath)}";
            }
            else if (folderName.Contains("doc"))
            {
                // Move to Documentation/ThirdParty (outside Assets)
                string vendorName = GetVendorName(assetPath);
                string targetPath = Path.Combine(docDir, vendorName);
                if (!s_DryRun)
                {
                    Directory.CreateDirectory(targetPath);
                    // Copy files (Unity can't move outside Assets)
                    foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        if (file.EndsWith(".meta")) continue;
                        string relPath = file.Replace(dir, string.Empty).TrimStart('\\', '/');
                        string targetFile = Path.Combine(targetPath, relPath);
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                        File.Copy(file, targetFile, true);
                    }
                    Debug.Log($"Copied documentation: {assetPath} -> Documentation/ThirdParty/{vendorName}");
                }
                else
                {
                    Debug.Log($"[Dry Run] Would copy documentation: {assetPath} -> Documentation/ThirdParty/{vendorName}");
                }
                moved++;
                continue;
            }

            if (destFolder != null && !string.IsNullOrEmpty(destRoot))
            {
                if (!s_DryRun)
                {
                    EnsureFolderPath(destFolder);
                    string finalDest = AssetDatabase.GenerateUniqueAssetPath($"{destFolder}/{Path.GetFileName(assetPath)}");
                    var result = AssetDatabase.MoveAsset(assetPath, finalDest);
                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.Log($"Moved: {assetPath} -> {finalDest}");
                        moved++;
                    }
                }
                else
                {
                    Debug.Log($"[Dry Run] Would move: {assetPath} -> {destFolder}");
                    moved++;
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Third-Party Organization", s_DryRun ? $"Would organize {moved} third-party folders by type (Dry Run)." : $"Organized {moved} third-party folders by type.", "OK");
    }

    private static string GetVendorName(string assetPath)
    {
        // Extract vendor name from path like Assets/ThirdParty/VendorName/...
        var parts = assetPath.Split('/');
        if (parts.Length > 2 && parts[0] == "Assets" && parts[1] == "ThirdParty")
            return parts[2];
        return "Unknown";
    }

    private static void EnsureFolderPath(string path)
    {
        var parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }

    private static void GeneratePreview()
    {
        s_PreviewEmptyFolders.Clear();
        s_PreviewUnusedAssetMoves.Clear();
        s_PreviewVendorMoves.Clear();

        // Empty folders preview
        string assetsPath = Application.dataPath;
        var dirs = Directory.GetDirectories(assetsPath, "*", SearchOption.AllDirectories).ToList();
        foreach (var dir in dirs)
        {
            if (dir.Replace('\\','/').Contains("/Library") || dir.Replace('\\','/').Contains("/Temp")) continue;
            string dirAssetPath = "Assets" + dir.Replace(Application.dataPath, string.Empty).Replace('\\','/');
            if (IsWhitelisted(dirAssetPath)) continue;
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
                s_PreviewEmptyFolders.Add(dirAssetPath);
        }

        // Unused asset moves preview
        string unusedFolder = "Assets/_Unused";
        string[] assetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/") && !p.StartsWith(unusedFolder) && !AssetDatabase.IsValidFolder(p))
            .ToArray();
        string[] rootCandidates = AssetDatabase.FindAssets("t:Scene t:Prefab t:ScriptableObject");
        var used = new HashSet<string>();
        foreach (var guid in rootCandidates)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            foreach (var dep in AssetDatabase.GetDependencies(path, true)) used.Add(dep);
        }
        foreach (var asset in assetPaths)
        {
            if (IsWhitelisted(asset)) continue;
            if (used.Contains(asset)) continue;
            string fileName = Path.GetFileName(asset);
            string dest = AssetDatabase.GenerateUniqueAssetPath($"{unusedFolder}/{fileName}");
            s_PreviewUnusedAssetMoves.Add((asset, dest));
        }

        // Vendor folder moves preview
        string thirdPartyRoot = "Assets/ThirdParty";
        string[] vendorHints = new[] {"Plugins", "ThirdParty", "External", "Odin", "DOTween", "TextMeshPro", "Cinemachine", "EZ", "FMOD"};
        var assetDirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in assetDirs)
        {
            var dirName = Path.GetFileName(dir);
            if (dirName == "ThirdParty" || dirName.StartsWith("_")) continue;
            string dirPathAsset = $"Assets/{dirName}";
            if (vendorHints.Any(h => dirName.IndexOf(h, System.StringComparison.OrdinalIgnoreCase) >= 0))
            {
                string destFolder = $"{thirdPartyRoot}/{dirName}";
                if (!IsWhitelisted(dirPathAsset))
                    s_PreviewVendorMoves.Add((dirPathAsset, destFolder));
            }
        }

        s_HavePreview = true;
    }

    private static void ApplyPreviewedChanges()
    {
        if (!s_HavePreview) return;
        // Empty folders
        foreach (var folder in s_PreviewEmptyFolders)
        {
            string fullPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, folder.Substring("Assets/".Length));
            if (Directory.Exists(fullPath)) FileUtil.DeleteFileOrDirectory(fullPath);
        }
        // Unused assets
        foreach (var move in s_PreviewUnusedAssetMoves)
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Unused")) AssetDatabase.CreateFolder("Assets", "_Unused");
            AssetDatabase.MoveAsset(move.source, move.dest);
        }
        // Vendor folders
        foreach (var vend in s_PreviewVendorMoves)
        {
            string parent = "Assets/ThirdParty";
            if (!AssetDatabase.IsValidFolder(parent)) AssetDatabase.CreateFolder("Assets", "ThirdParty");
            EnsureFolder(parent, Path.GetFileName(vend.source));
            AssetDatabase.MoveAsset(vend.source, vend.dest);
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Apply", "Previewed changes applied.", "OK");
        s_HavePreview = false;
        s_PreviewEmptyFolders.Clear();
        s_PreviewUnusedAssetMoves.Clear();
        s_PreviewVendorMoves.Clear();
    }

    private static void ScanMissingScripts()
    {
        // Find prefabs with missing scripts
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;
        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            var gameObject = obj as GameObject;
            if (gameObject == null) continue;
            var comps = gameObject.GetComponentsInChildren<Component>(true);
            if (comps.Any(c => c == null))
            {
                Debug.LogWarning($"Missing scripts detected in prefab: {path}");
                count++;
            }
        }
        EditorUtility.DisplayDialog("Missing Scripts", $"Detected {count} prefabs with missing scripts (see Console).", "OK");
    }

    private static void GenerateCleanupReport()
    {
        string docDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Documentation", "Cleanup");
        Directory.CreateDirectory(docDir);
        string reportPath = Path.Combine(docDir, "AssetCleanupReport.md");

        var lines = new List<string>();
        lines.Add("# Asset Cleanup Report");
        lines.Add("");

        // Large textures
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (var guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null && (tex.width * tex.height) > (2048 * 2048))
            {
                lines.Add($"- Large texture: `{path}` ({tex.width}x{tex.height})");
            }
        }

        // Missing scripts
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            var go = obj as GameObject;
            if (go == null) continue;
            var comps = go.GetComponentsInChildren<Component>(true);
            if (comps.Any(c => c == null))
            {
                lines.Add($"- Missing scripts in prefab: `{path}`");
            }
        }

        File.WriteAllLines(reportPath, lines);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Report", $"Generated cleanup report at {reportPath}.", "OK");
    }

    private static void EnsureFolder(string parent, string child)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static bool IsWhitelisted(string assetPath)
    {
        var normalized = assetPath.Replace('\\','/');
        return s_ActiveWhitelist.Any(w => normalized.StartsWith(w));
    }

    private static string NormalizeAssetPath(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate)) return string.Empty;
        var p = candidate.Replace('\\','/');
        if (!p.StartsWith("Assets/")) p = "Assets/" + p.TrimStart('/');
        return p.TrimEnd('/');
    }
}
