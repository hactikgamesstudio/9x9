# 9x9x9 Unity Setup Guide - Quick Start

## Installation Complete!

All ported scripts are now in your Unity project at `Assets/Scripts/Runtime/`.

### Ported Scripts

- **FirstPersonController.cs** - Player movement and health (replaces CharacterBody3D.gd)
- **InventorySystem.cs** - Global inventory manager (replaces Inventory.gd AutoLoad)
- **RoomGenerator.cs** - 9x9x9 cube grid generator (replaces RoomGenerator.gd)
- **DoorController.cs** - Automatic door system with rotation triggers
- **PickupItem.cs** - Collectible items (replaces pickup_item.gd)
- **HazardSpikes.cs** - Instant damage hazards (replaces hazard_spikes.gd)
- **HazardLaser.cs** - Continuous damage zones (replaces hazard_laser.gd)
- **HUDController.cs** - Health/inventory UI (replaces HUD.gd)

---

## CRITICAL: Creating Room Prefabs

This is where you are now. Follow these steps to create your room prefabs.

### Room Structure Template

Every room prefab must have this exact structure:

```
StandardRoom (GameObject)
├── Floor (Cube - 10x0.2x10)
├── Ceiling (Cube - 10x0.2x10)
├── WallNorth (Cube - 10x3x0.2)
├── WallSouth (Cube - 10x3x0.2)
├── WallEast (Cube - 0.2x3x10)
├── WallWest (Cube - 0.2x3x10)
├── Bounds (Cube - visual reference only, no collider)
├── door_north (GameObject with DoorController)
├── door_south (GameObject with DoorController)
├── door_east (GameObject with DoorController)
└── door_west (GameObject with DoorController)
```

### Room Requirements

- **Exact Size**: All rooms MUST be 10x10x10 units (matches RoomGenerator.m_RoomSize)
- **Centered at Origin**: Room should be centered at (0,0,0) in prefab mode
- **4 Doors**: North, South, East, West (Up/Down optional for vertical movement)
- **Automatic Doors**: Use DoorController component for sliding door mechanics
- **Room Rotation**: Enabled by default - rooms rotate 90° after player exits

---

## Step-by-Step: Create Base Room Prefab

### 1. Create Empty Room GameObject

1. Hierarchy -> Right-click -> Create Empty
2. Rename to "StandardRoom"
3. Reset Transform (Position 0,0,0, Rotation 0,0,0, Scale 1,1,1)

### 2. Add Floor and Ceiling

**Floor:**
1. Right-click StandardRoom -> 3D Object -> Cube
2. Rename to "Floor"
3. Transform:
   - Position: (0, -1.4, 0)
   - Rotation: (0, 0, 0)
   - Scale: (10, 0.2, 10)

**Ceiling:**
1. Right-click StandardRoom -> 3D Object -> Cube
2. Rename to "Ceiling"
3. Transform:
   - Position: (0, 1.4, 0)
   - Rotation: (0, 0, 0)
   - Scale: (10, 0.2, 10)

### 3. Add Walls

**WallNorth:**
- Position: (0, 0, 5)
- Scale: (10, 3, 0.2)

**WallSouth:**
- Position: (0, 0, -5)
- Scale: (10, 3, 0.2)

**WallEast:**
- Position: (5, 0, 0)
- Scale: (0.2, 3, 10)

**WallWest:**
- Position: (-5, 0, 0)
- Scale: (0.2, 3, 10)

### 4. Add Bounds (Reference Object)

1. Right-click StandardRoom -> 3D Object -> Cube
2. Rename to "Bounds"
3. Scale: (10, 10, 10)
4. **Disable MeshRenderer** (uncheck in Inspector)
5. **Do NOT add collider** (RoomGenerator uses Renderer.bounds for overlap detection)

### 5. Create Door GameObjects

You'll create 4 doors. Each door has:
- A cube that slides up/down
- A BoxCollider (Is Trigger = true) to detect player
- A DoorController script

#### Door North

1. Right-click StandardRoom -> Create Empty
2. Rename to "door_north"
3. Position: (0, 0, 4.9)

**Create Door Cube:**
1. Right-click door_north -> 3D Object -> Cube
2. Rename to "DoorCube"
3. Transform:
   - Position: (0, 0, 0) - relative to parent
   - Rotation: (0, 0, 0)
   - Scale: (3, 3, 0.2)

**Add BoxCollider Trigger:**
1. Select door_north
2. Add Component -> Box Collider
3. Check "Is Trigger"
4. Set Size: (3, 3, 1.5)
5. Set Center: (0, 0, 0.75) - extends forward from door

**Add DoorController:**
1. Select door_north
2. Add Component -> DoorController
3. Settings:
   - Door Object: Drag "DoorCube" from Hierarchy
   - Open Distance: 3.2
   - Close Distance: 0
   - Open Speed: 2
   - Close Speed: 2
   - Trigger Room Rotation: true (checked)

#### Door South

1. Right-click StandardRoom -> Create Empty
2. Rename to "door_south"
3. Position: (0, 0, -4.9)
4. Repeat "Create Door Cube" steps above
5. Add BoxCollider: Size (3, 3, 1.5), Center (0, 0, -0.75) - extends backward
6. Add DoorController with same settings

#### Door East

1. Right-click StandardRoom -> Create Empty
2. Rename to "door_east"
3. Position: (4.9, 0, 0)
4. Rotation: (0, 90, 0) - rotated to face correct direction
5. Create Door Cube (same as above)
6. Add BoxCollider: Size (3, 3, 1.5), Center (0, 0, 0.75)
7. Add DoorController with same settings

#### Door West

1. Right-click StandardRoom -> Create Empty
2. Rename to "door_west"
3. Position: (-4.9, 0, 0)
4. Rotation: (0, -90, 0) - rotated to face correct direction
5. Create Door Cube (same as above)
6. Add BoxCollider: Size (3, 3, 1.5), Center (0, 0, 0.75)
7. Add DoorController with same settings

### 6. Save as Prefab

1. Drag "StandardRoom" from Hierarchy -> `Assets/Prefabs/Rooms/` folder
2. If folder doesn't exist: Right-click Assets -> Create -> Folder -> name it "Prefabs", then create "Rooms" inside
3. Delete "StandardRoom" from scene (you'll use the prefab)

---

## Create Room Variants

Now create different room types by duplicating the base prefab:

### Method 1: Duplicate in Project Window

1. Select `StandardRoom.prefab` in Project
2. Ctrl+D to duplicate
3. Rename to variant type (e.g., "Room_Variant1", "Room_Hazard", "Room_Loot")

### Method 2: Edit Prefab Mode

1. Double-click prefab in Project window
2. Add/remove/modify objects
3. Click "< Prefab" at top to save and exit

### Suggested Variants

**Variant 1 - Pillars:**
- Add 4 cylinder pillars at corners for visual variety
- Position at (±3, 0, ±3)
- Scale: (0.5, 3, 0.5)

**Variant 2 - Hazard Room:**
- Add HazardSpikes or HazardLaser in center
- Add warning lights (red point lights)

**Variant 3 - Loot Room:**
- Add 3-5 PickupItem objects
- Position randomly but not blocking doors

**Variant 4 - Maze:**
- Add internal wall sections (cubes)
- Create winding path from entrance to exit

**Variant 5 - Empty:**
- Just the base room - good for corridors

### Special Room: Exit Room

1. Duplicate StandardRoom -> Rename "ExitRoom"
2. Add large glowing cube in center (emission material)
3. Add point light (green, intensity 5)
4. Optionally add win trigger script

### Special Room: Spawn Room

1. Duplicate StandardRoom -> Rename "SpawnRoom"
2. Add spawn marker (empty GameObject at center)
3. Optionally add starting items/tutorial text

---

## Quick Reference: Door Settings

Copy-paste these exact values for each door:

**door_north:**
- Position: (0, 0, 4.9)
- Rotation: (0, 0, 0)
- BoxCollider: Is Trigger = true, Size (3, 3, 1.5), Center (0, 0, 0.75)
- DoorController: Open Distance 3.2, Speeds 2, Rotation enabled

**door_south:**
- Position: (0, 0, -4.9)
- Rotation: (0, 0, 0)
- BoxCollider: Is Trigger = true, Size (3, 3, 1.5), Center (0, 0, -0.75)
- DoorController: Open Distance 3.2, Speeds 2, Rotation enabled

**door_east:**
- Position: (4.9, 0, 0)
- Rotation: (0, 90, 0)
- BoxCollider: Is Trigger = true, Size (3, 3, 1.5), Center (0, 0, 0.75)
- DoorController: Open Distance 3.2, Speeds 2, Rotation enabled

**door_west:**
- Position: (-4.9, 0, 0)
- Rotation: (0, -90, 0)
- BoxCollider: Is Trigger = true, Size (3, 3, 1.5), Center (0, 0, 0.75)
- DoorController: Open Distance 3.2, Speeds 2, Rotation enabled

---

## Next Steps After Room Creation

Once you have 3-5 room prefabs:

1. **Configure RoomGenerator** (see SETUP_GUIDE_FULL.md)
2. **Setup Player Input Actions** (see SETUP_GUIDE_FULL.md)
3. **Create Main Scene** (see SETUP_GUIDE_FULL.md)
4. **Test in Play Mode**

---

**Quick Status:** You are currently at Step 5 (Creating Room Variants). Once done, move to RoomGenerator configuration.

**Need Full Guide?** See `SETUP_GUIDE_FULL.md` for complete documentation including Input System, Scene Setup, Troubleshooting, and Godot comparison guide.
