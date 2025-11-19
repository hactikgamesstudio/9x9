using NUnit.Framework;
using UnityEngine;
using Unity.Template.Multiplayer.NGO.Runtime;

public class CubeGameControllerServerLifecycleTests
{
    private GameObject _root;

    [SetUp]
    public void Setup()
    {
        _root = new GameObject("TestRoot_Controller");
    }

    [TearDown]
    public void Teardown()
    {
        if (_root != null)
        {
            Object.DestroyImmediate(_root);
        }

        foreach (var gen in Object.FindObjectsByType<RoomGenerator>(FindObjectsSortMode.None))
        {
            if (gen != null)
            {
                Object.DestroyImmediate(gen.gameObject);
            }
        }
    }

    [Test]
    public void DoesNotCreateGenerator_WhenNoNetworkManagerServer()
    {
        // Arrange: No NetworkManager in scene, ensure no generator exists
        var controllerGO = new GameObject("CubeGameController");
        controllerGO.transform.SetParent(_root.transform);
        var controller = controllerGO.AddComponent<CubeGameController>();

        // Assert: controller should not auto-create a generator without server context
        var found = Object.FindFirstObjectByType<RoomGenerator>();
        Assert.IsNull(found, "RoomGenerator should not be created without a server NetworkManager.");
    }

    [Test]
    public void CleansUp_RuntimeCreatedGenerator_OnDestroy()
    {
        // Arrange: Create controller with a runtime-created generator flag via public method if available; else set using reflection
        var controllerGO = new GameObject("CubeGameController");
        controllerGO.transform.SetParent(_root.transform);
        var controller = controllerGO.AddComponent<CubeGameController>();

        var genGO = new GameObject("RoomGenerator_Runtime");
        var gen = genGO.AddComponent<RoomGenerator>();

        // Try to set internal tracking flag via reflection for m_RoomGeneratorCreatedAtRuntime
        var field = typeof(CubeGameController).GetField("m_RoomGeneratorCreatedAtRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(controller, gen);
        }

        // Act
        Object.DestroyImmediate(controllerGO);

        // Assert: Generator should be destroyed
        Assert.IsTrue(gen == null || gen.Equals(null), "Runtime-created RoomGenerator should be destroyed with controller.");
    }
}
