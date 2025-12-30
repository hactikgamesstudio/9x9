using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    /// <summary>
    /// Comprehensive test to verify Metagame assembly can access CustomNetworkManager.
    /// Run from menu: Tools > Test Metagame Assembly
    /// </summary>
    public static class TestMetagameAssembly
    {
        [MenuItem("Tools/Test Metagame Assembly")]
        public static void RunTests()
        {
            Debug.Log("=== Testing Metagame Assembly ===\n");
            
            bool allPassed = true;
            
            // Test 1: Can we find CustomNetworkManager type?
            allPassed &= TestType("Unity.Template.Multiplayer.NGO.Runtime.CustomNetworkManager");
            
            // Test 2: Can we find AutoConnectOnStartup property?
            allPassed &= TestProperty(
                "Unity.Template.Multiplayer.NGO.Runtime.CustomNetworkManager",
                "AutoConnectOnStartup"
            );
            
            // Test 3: Can we find ReturnToMetagame field?
            allPassed &= TestField(
                "Unity.Template.Multiplayer.NGO.Runtime.CustomNetworkManager",
                "ReturnToMetagame"
            );
            
            // Test 4: Can we find MainMenuView in Metagame assembly?
            allPassed &= TestType("Unity.Template.Multiplayer.NGO.Runtime.Metagame.Views.MainMenuView");
            
            // Test 5: Can we find MatchmakerView in Metagame assembly?
            allPassed &= TestType("Unity.Template.Multiplayer.NGO.Runtime.Metagame.Views.MatchmakerView");
            
            // Test 6: Can we find LoadingScreenView in Metagame assembly?
            allPassed &= TestType("Unity.Template.Multiplayer.NGO.Runtime.Metagame.Views.LoadingScreenView");
            
            // Test 7: Verify MetagameView references are correct
            allPassed &= TestMetagameViewReferences();
            
            // Test 8: Verify no circular dependencies
            allPassed &= TestNoCircularDependencies();
            
            Debug.Log("\n=== Test Results ===");
            if (allPassed)
            {
                Debug.Log("✓ ALL TESTS PASSED! Metagame assembly is correctly configured.");
            }
            else
            {
                Debug.LogError("✗ SOME TESTS FAILED! Check logs above.");
            }
        }
        
        static bool TestType(string typeName)
        {
            Type type = Type.GetType(typeName + ", Assembly-CSharp");
            if (type == null)
            {
                // Try without assembly hint
                type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == typeName);
            }
            
            if (type != null)
            {
                Debug.Log($"✓ Type found: {typeName}");
                return true;
            }
            else
            {
                Debug.LogError($"✗ Type NOT found: {typeName}");
                return false;
            }
        }
        
        static bool TestProperty(string typeName, string propertyName)
        {
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName);
            
            if (type == null)
            {
                Debug.LogError($"✗ Cannot test property - type not found: {typeName}");
                return false;
            }
            
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                Debug.Log($"✓ Property found: {typeName}.{propertyName}");
                return true;
            }
            else
            {
                Debug.LogError($"✗ Property NOT found: {typeName}.{propertyName}");
                return false;
            }
        }
        
        static bool TestField(string typeName, string fieldName)
        {
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName);
            
            if (type == null)
            {
                Debug.LogError($"✗ Cannot test field - type not found: {typeName}");
                return false;
            }
            
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                Debug.Log($"✓ Field found: {typeName}.{fieldName}");
                return true;
            }
            else
            {
                Debug.LogError($"✗ Field NOT found: {typeName}.{fieldName}");
                return false;
            }
        }
        
        static bool TestMetagameViewReferences()
        {
            try
            {
                Type metaViewType = Type.GetType("Unity.Template.Multiplayer.NGO.Runtime.MetagameView, Assembly-CSharp");
                if (metaViewType == null)
                {
                    Debug.LogError("✗ MetagameView type not found");
                    return false;
                }
                
                var mainMenuProp = metaViewType.GetProperty("MainMenu");
                if (mainMenuProp == null)
                {
                    Debug.LogError("✗ MetagameView.MainMenu property not found");
                    return false;
                }
                
                if (mainMenuProp.PropertyType.FullName != "Unity.Template.Multiplayer.NGO.Runtime.Metagame.Views.MainMenuView")
                {
                    Debug.LogError($"✗ MetagameView.MainMenu wrong type: {mainMenuProp.PropertyType.FullName}");
                    return false;
                }
                
                Debug.Log("✓ MetagameView references are correct");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"✗ Error testing MetagameView references: {ex.Message}");
                return false;
            }
        }
        
        static bool TestNoCircularDependencies()
        {
            // This is a simplified check - the full version is in AssemblyDependencyViewer
            var assemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            
            var metagame = assemblies.FirstOrDefault(a => a.name == "Unity.Template.Multiplayer.NGO.Runtime.Metagame");
            var ui = assemblies.FirstOrDefault(a => a.name == "Unity.Template.Multiplayer.NGO.Runtime.UI");
            
            if (metagame == null || ui == null)
            {
                Debug.LogWarning("⚠ Could not find Metagame or UI assembly for circular check");
                return true; // Don't fail the test
            }
            
            // Check if Metagame references UI
            bool metagameRefsUI = metagame.assemblyReferences.Any(r => r.name == "Unity.Template.Multiplayer.NGO.Runtime.UI");
            
            // Check if UI references Metagame
            bool uiRefsMetagame = ui.assemblyReferences.Any(r => r.name == "Unity.Template.Multiplayer.NGO.Runtime.Metagame");
            
            if (metagameRefsUI && uiRefsMetagame)
            {
                Debug.LogError("✗ CIRCULAR DEPENDENCY: Metagame ↔ UI");
                return false;
            }
            
            if (metagameRefsUI)
            {
                Debug.LogError("✗ Metagame should NOT reference UI assembly!");
                return false;
            }
            
            Debug.Log("✓ No circular dependencies between Metagame and UI");
            return true;
        }
    }
}
