#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    internal static class ValidateDefaultAssemblies
    {
        private static readonly string[] AllowedTypeFullNames =
        {
            // Runtime shim
            "Unity.Template.Multiplayer.NGO.AssemblyCSharpShim",
            // Editor shim
            "Unity.Template.Multiplayer.NGO.Editor.EditorAssemblyShim"
        };

        [InitializeOnLoadMethod]
        private static void Run()
        {
            // Delay to run after domain reload/compilation completes.
            EditorApplication.delayCall += Check;
        }

        private static void Check()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                // Validate runtime default assembly
                ValidateAssembly(assemblies, "Assembly-CSharp");

                // Validate editor default assembly
                ValidateAssembly(assemblies, "Assembly-CSharp-Editor");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Default assembly validation failed: {ex.Message}");
            }
        }

        private static void ValidateAssembly(Assembly[] assemblies, string name)
        {
            var asm = assemblies.FirstOrDefault(a => a.GetName().Name == name);
            if (asm == null)
            {
                // If shims are present in the domain, warn; otherwise, allow no-shim setup.
                bool anyShimLoaded = AllowedTypeFullNames.Any(fullName =>
                    assemblies.Any(a => a.GetType(fullName) != null));
                if (anyShimLoaded)
                {
                    Debug.LogWarning($"{name} not found. Ensure shim files exist to keep Burst happy.");
                }
                return;
            }

            var nonShimTypes = asm.GetTypes()
                .Where(t => t.IsClass && !AllowedTypeFullNames.Contains(t.FullName))
                .ToList();

            if (nonShimTypes.Count > 0)
            {
                var names = string.Join(", ", nonShimTypes.Select(t => t.FullName));
                Debug.LogWarning($"{name} contains non-shim types: {names}.\n" +
                                 "Keep production code inside asmdefs. Default assemblies should remain empty except shims.");
            }
        }
    }
}
#endif
