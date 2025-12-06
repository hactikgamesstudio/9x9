# Procedural Room Assembly (From Pieces)

This system builds a room from individual prefab pieces (floors, ceilings, walls) so each room instance can have different variants.

## Components

- `RoomPieceSet` (ScriptableObject)
  - Lists of variants: Floors, Ceilings, WallsShared, WallsNorth/South/East/West.
  - Create via: Right-click → Create → 9x9 → Procedural → Room Piece Set.

- `RoomAssembler` (MonoBehaviour)
  - Add to a base room prefab (a simple empty "shell").
  - Looks for sockets by name or serialized references:
    - `socket_floor`, `socket_ceiling`, `socket_north`, `socket_south`, `socket_east`, `socket_west`
  - At runtime or in editor (Awake/Validate), it instantiates random piece variants under each socket.

## Setup Steps

1. Create a base room prefab (e.g., `RoomShell.prefab`).
   - Add empty child objects positioned where pieces should go:
     - `socket_floor` – center of the floor
     - `socket_ceiling` – center of the ceiling
     - `socket_north` – centered on north wall, forward points outward
     - `socket_south` – centered on south wall, forward points outward
     - `socket_east` – centered on east wall, forward points outward
     - `socket_west` – centered on west wall, forward points outward

2. Prepare piece prefabs:
   - Floor/ceiling meshes aligned to the socket position/rotation.
   - Wall meshes with their forward (blue Z axis) pointing outward.
   - Keep consistent dimensions (e.g., `10×10×10` if that’s your room size).

3. Create a `RoomPieceSet` asset and assign your variants.

4. Add `RoomAssembler` to `RoomShell.prefab` and assign the `RoomPieceSet`.
   - Enable "Assemble On Awake" for automatic building.
   - Optional: set a `Seed Override` for deterministic results.

## Seeding / Determinism

- `RoomAssembler.Assemble(int? seed)` lets you control randomness.
- For grid-based generators, derive a seed from cell coordinates and a global seed so rooms are stable across runs:

```csharp
// Example integration
var assembler = roomInstance.GetComponent<RoomAssembler>();
if (assembler != null)
{
    int cellSeed = globalSeed ^ (x * 73856093) ^ (y * 19349663) ^ (z * 83492791);
    assembler.Assemble(cellSeed);
}
```

## Integrating with `RoomGenerator`

- If your `RoomGenerator` instantiates room prefabs, simply use the `RoomShell.prefab` and let `RoomAssembler` run on Awake.
- For deterministic layouts, call `Assemble(cellSeed)` after instantiation.

## Tips

- Use `WallsShared` when all four sides can use the same pool; override with directional lists as needed.
- For cube-to-cube connections via doors:
  - Create wall variants with door openings.
  - After room generation, detect adjacent rooms and swap solid walls for door variants.
  - Or use logic in `RoomGenerator` to pass door flags when calling `Assemble()`.
- Keep pivot/origin consistent so pieces snap cleanly to sockets.

## Creating Sockets Quickly

Use the editor tool: **Tools → 9x9 → Create Room Sockets**
- Select your room prefab or GameObject.
- Configure room size (default 10).
- Click "Create Sockets on Selected" to auto-generate all 6 sockets.
- Sockets are positioned and rotated correctly (forward points outward for walls).

