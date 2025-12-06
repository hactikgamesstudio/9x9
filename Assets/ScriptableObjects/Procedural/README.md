# Default Room Piece Set

This is a placeholder `RoomPieceSet` asset for testing and reference.

## Usage

1. Create your floor/ceiling/wall prefabs.
2. Open this asset in the Inspector.
3. Drag prefabs into the appropriate lists:
   - **Floors**: Floor mesh variants
   - **Ceilings**: Ceiling mesh variants
   - **WallsShared**: Wall variants used for all directions (if no directional override)
   - **WallsNorth/South/East/West**: Direction-specific wall variants (optional)

4. Assign this `RoomPieceSet` to your `RoomShell.prefab`'s `RoomAssembler` component.

## Creating More Sets

Right-click in Project → Create → 9x9 → Procedural → Room Piece Set

This lets you have themed sets (e.g., "Industrial", "Sci-Fi", "Horror") and swap them at runtime or per-room.
