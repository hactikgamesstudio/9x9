using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    [CreateAssetMenu(fileName = "RoomPieceSet", menuName = "9x9/Procedural/Room Piece Set", order = 0)]
    public class RoomPieceSet : ScriptableObject
    {
        [Header("Floors & Ceilings")]
        public List<GameObject> Floors = new List<GameObject>();
        public List<GameObject> Ceilings = new List<GameObject>();

        [Header("Walls (Directional)")]
        public List<GameObject> WallsShared = new List<GameObject>();
        public List<GameObject> WallsNorth = new List<GameObject>();
        public List<GameObject> WallsSouth = new List<GameObject>();
        public List<GameObject> WallsEast = new List<GameObject>();
        public List<GameObject> WallsWest = new List<GameObject>();

        [Header("Door Walls (Directional)")]
        public List<GameObject> DoorWallsShared = new List<GameObject>();
        public List<GameObject> DoorWallsNorth = new List<GameObject>();
        public List<GameObject> DoorWallsSouth = new List<GameObject>();
        public List<GameObject> DoorWallsEast = new List<GameObject>();
        public List<GameObject> DoorWallsWest = new List<GameObject>();

        public GameObject GetRandomFloor(System.Random rng)
            => Pick(Floors, rng);

        public GameObject GetRandomCeiling(System.Random rng)
            => Pick(Ceilings, rng);

        public GameObject GetRandomWallNorth(System.Random rng)
            => PickDirectional(WallsNorth, rng);

        public GameObject GetRandomWallSouth(System.Random rng)
            => PickDirectional(WallsSouth, rng);

        public GameObject GetRandomWallEast(System.Random rng)
            => PickDirectional(WallsEast, rng);

        public GameObject GetRandomWallWest(System.Random rng)
            => PickDirectional(WallsWest, rng);

        public GameObject GetRandomDoorWallNorth(System.Random rng)
            => PickDirectional(DoorWallsNorth, rng, DoorWallsShared);

        public GameObject GetRandomDoorWallSouth(System.Random rng)
            => PickDirectional(DoorWallsSouth, rng, DoorWallsShared);

        public GameObject GetRandomDoorWallEast(System.Random rng)
            => PickDirectional(DoorWallsEast, rng, DoorWallsShared);

        public GameObject GetRandomDoorWallWest(System.Random rng)
            => PickDirectional(DoorWallsWest, rng, DoorWallsShared);

        private GameObject PickDirectional(List<GameObject> list, System.Random rng)
            => list != null && list.Count > 0 ? Pick(list, rng) : Pick(WallsShared, rng);

        private GameObject PickDirectional(List<GameObject> list, System.Random rng, List<GameObject> fallback)
            => list != null && list.Count > 0 ? Pick(list, rng) : Pick(fallback, rng);

        private static GameObject Pick(List<GameObject> list, System.Random rng)
        {
            if (list == null || list.Count == 0) return null;
            int i = rng.Next(0, list.Count);
            return list[i];
        }
    }
}