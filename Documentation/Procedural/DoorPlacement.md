# Door Placement System

The room assembly system now automatically detects adjacent rooms and places doors accordingly.

## How It Works

1. **Two-Pass Generation:**
   - Pass 1: Instantiate all rooms in the grid
   - Pass 2: Detect neighbors, calculate door flags, assemble rooms with appropriate walls

2. **RoomDoorFlags Enum:**
   ```csharp
   [Flags]
   public enum RoomDoorFlags
   {
       None = 0,
       North = 1 << 0,  // +Z direction
       South = 1 << 1,  // -Z direction
       East = 1 << 2,   // +X direction
       West = 1 << 3,   // -X direction
       All = North | South | East | West
   }
   ```

3. **Automatic Detection:**
   - `RoomGenerator.CalculateDoorFlags(x, y, z)` checks all 4 horizontal neighbors
   - If a neighbor exists, that side gets a door flag
   - Door flags are passed to `RoomAssembler.Assemble(seed, doorFlags)`

4. **Wall Selection:**
   - Solid walls used when flag is `None` for that side
   - Door walls used when flag is set for that side
   - Falls back to `DoorWallsShared` if directional list is empty

## Creating Door Wall Prefabs

1. Duplicate your solid wall prefab
2. Add a door opening/archway mesh
3. Optionally add a door frame, collider cutout, or interactive door component
4. Add to the appropriate `DoorWalls*` list in your `RoomPieceSet`

## RoomPieceSet Configuration

- **WallsNorth/South/East/West:** Solid walls (no door)
- **DoorWallsNorth/South/East/West:** Walls with door openings
- **WallsShared:** Fallback for solid walls
- **DoorWallsShared:** Fallback for door walls

## Manual Override

You can manually specify door flags:
```csharp
var assembler = roomPrefab.GetComponent<RoomAssembler>();
assembler.Assemble(seed: 12345, doorFlags: RoomDoorFlags.North | RoomDoorFlags.South);
```

## Tips

- Keep door opening size consistent (e.g., 2m wide × 3m tall)
- Align door pivot to wall socket position for clean snapping
- Add NavMesh links or collision zones at door thresholds for AI pathfinding
- For locked/conditional doors, create variants and swap at runtime based on game state
