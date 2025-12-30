using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using System.Linq;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    /// <summary>
    /// Displays assembly dependency information to verify no circular dependencies.
    /// Run from menu: Tools > Show Assembly Dependencies
    /// </summary>
    public static class AssemblyDependencyViewer
    {
        [MenuItem("Tools/Show Assembly Dependencies")]
        public static void ShowDependencies()
        {
            Debug.Log("=== Assembly Dependency Report ===\n");
            
            var assemblies = CompilationPipeline.GetAssemblies();
            
            // Filter to our assemblies
            var ourAssemblies = assemblies
                .Where(a => a.name.Contains("Unity.Template.Multiplayer.NGO.Runtime"))
                .OrderBy(a => a.name)
                .ToArray();
            
            foreach (var assembly in ourAssemblies)
            {
                Debug.Log($"<b>{assembly.name}</b>");
                Debug.Log($"  Output: {assembly.outputPath}");
                
                // Show references to our other assemblies
                var refs = assembly.assemblyReferences
                    .Where(r => r.name.Contains("Unity.Template.Multiplayer.NGO.Runtime"))
                    .ToArray();
                
                if (refs.Length > 0)
                {
                    Debug.Log("  References:");
                    foreach (var r in refs)
                    {
                        Debug.Log($"    → {r.name}");
                    }
                }
                else
                {
                    Debug.Log("  References: (none to our assemblies)");
                }
                
                Debug.Log("");
            }
            
            // Check for circular dependencies
            Debug.Log("=== Checking for Circular Dependencies ===\n");
            
            bool foundCircular = false;
            foreach (var assembly in ourAssemblies)
            {
                var refs = assembly.assemblyReferences
                    .Where(r => r.name.Contains("Unity.Template.Multiplayer.NGO.Runtime"))
                    .ToArray();
                
                foreach (var refAsm in refs)
                {
                    // Check if the referenced assembly also references us
                    var backRef = assemblies
                        .FirstOrDefault(a => a.name == refAsm.name);
                    
                    if (backRef != null)
                    {
                        var backRefs = backRef.assemblyReferences
                            .Where(r => r.name == assembly.name)
                            .ToArray();
                        
                        if (backRefs.Length > 0)
                        {
                            Debug.LogError($"⚠️ CIRCULAR: {assembly.name} ↔ {refAsm.name}");
                            foundCircular = true;
                        }
                    }
                }
            }
            
            if (!foundCircular)
            {
                Debug.Log("✓ No circular dependencies found!");
            }
            
            Debug.Log("\n=== Expected Structure ===");
            Debug.Log("Core (base)");
            Debug.Log("Shared → Core");
            Debug.Log("Game → Core, Shared");
            Debug.Log("Metagame → Core, Shared, Game");
            Debug.Log("UI → Core, Shared, Game, Metagame");
            Debug.Log("\n=== End Report ===");
        }
    }
}
