using NUnit.Framework;
using UnityEngine;
using Unity.Template.Multiplayer.NGO.Runtime;

public class RoomGeneratorConfigTests
{
    private GameObject _root;

    [SetUp]
    public void Setup()
    {
        _root = new GameObject("TestRoot");
    }

    [TearDown]
    public void Teardown()
    {
        if (_root != null)
        {
            Object.DestroyImmediate(_root);
        }

        foreach (var go in Object.FindObjectsOfType<GameObject>())
        {
            if (go != null && go.name.StartsWith("Room_"))
            {
                Object.DestroyImmediate(go);
            }
        }
    }

    [Test]
    public void ApplyConfig_FillOnly_FillsMissingTemplates_And_GeneratesCenterRoom()
    {
        // Arrange
        var genGO = new GameObject("RoomGenerator");
        genGO.transform.SetParent(_root.transform);
        var generator = genGO.AddComponent<RoomGenerator>();

        var config = ScriptableObject.CreateInstance<RoomGenConfig>();
        var dummyRoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dummyRoom.name = "DummyRoom";
        dummyRoom.transform.SetParent(_root.transform);
        var exitRoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exitRoom.name = "ExitRoom";
        exitRoom.transform.SetParent(_root.transform);
        var spawnRoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        spawnRoom.name = "SpawnRoom";
        spawnRoom.transform.SetParent(_root.transform);

        config.RoomTemplates = new GameObject[] { dummyRoom };
        config.ExitRoomPrefab = exitRoom;
        config.SpawnRoomPrefab = spawnRoom;
        config.GridSize = 5; // small grid for test
        config.RoomSize = 2f;
        config.GenerateAllRooms = false;
        config.SparseDensity = 0.5f;
        config.Seed = 1234;

        // Act: fillOnly should populate missing refs; scalars remain default unless generator uses them later
        generator.ApplyConfig(config, fillOnly: true, applyScalarSettings: false);
        generator.Generate();

        int center = config.GridSize / 2; // center index for odd grid size
        var centerRoom = generator.GetRoomAt(center, center, center);

        // Assert: center room was generated
        Assert.NotNull(centerRoom, "Center room should exist after generation with config templates.");
    }

    [Test]
    public void ApplyConfig_Full_OverwritesScalars_And_Generates_3x3_Center()
    {
        // Arrange
        var genGO = new GameObject("RoomGenerator");
        genGO.transform.SetParent(_root.transform);
        var generator = genGO.AddComponent<RoomGenerator>();

        var config = ScriptableObject.CreateInstance<RoomGenConfig>();
        var dummy2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dummy2.name = "DummyRoom2";
        dummy2.transform.SetParent(_root.transform);
        var exit2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exit2.name = "ExitRoom2";
        exit2.transform.SetParent(_root.transform);
        var spawn2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        spawn2.name = "SpawnRoom2";
        spawn2.transform.SetParent(_root.transform);

        config.RoomTemplates = new GameObject[] { dummy2 };
        config.ExitRoomPrefab = exit2;
        config.SpawnRoomPrefab = spawn2;
        config.GridSize = 3; // enforce small grid
        config.RoomSize = 1f;
        config.GenerateAllRooms = false;
        config.SparseDensity = 1f;
        config.Seed = 42;

        // Act: full overwrite
        generator.ApplyConfig(config, fillOnly: false, applyScalarSettings: true);
        generator.Generate();

        // Center cell for 3x3 grid should be 1,1,1
        var centerRoom = generator.GetRoomAt(1, 1, 1);

        // Assert
        Assert.NotNull(centerRoom, "Center room for 3x3 grid should exist at (1,1,1).");
    }
}
