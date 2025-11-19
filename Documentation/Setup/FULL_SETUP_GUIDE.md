# Complete Setup Guide — 9x9 Unity Project

**Last Updated:** November 14, 2025  
**Unity Version:** 6000.2.10f1  
**Project Type:** Multiplayer Survival Puzzle (URP + Netcode for GameObjects)

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Input System Setup](#input-system-setup)
3. [Main Scene Assembly](#main-scene-assembly)
4. [Room Generator Configuration](#room-generator-configuration)
5. [Player Setup](#player-setup)
6. [HUD & UI Setup](#hud--ui-setup)
7. [Inventory System](#inventory-system)
8. [Hazards & Pickups](#hazards--pickups)
9. [Testing & Debugging](#testing--debugging)
10. [Performance Tips](#performance-tips)
11. [Godot to Unity Reference](#godot-to-unity-reference)
12. [Troubleshooting](#troubleshooting)

---

## Project Overview

### What Is This Project?

A multiplayer survival puzzle game inspired by the 1997 film *Cube*. Players spawn at the corners of a 9×9×9 grid of rooms, navigate hazards, solve puzzles, scavenge items, and engage in PvP combat while trying to reach the exit at the center.

### Core Features

- **9×9×9 Room Grid:** 729 possible room positions with sparse generation
- **Procedural Generation:** Randomized layouts with guaranteed paths from corners to center
- **Room Rotation Mechanic:** Rooms rotate after doors close (Cube film mechanic)
- **Dynamic Doors:** Auto-open/close with optional false doors
- **Player Movement:** First-person controller with sprint, jump, FOV transitions
- **Health System:** Damage from hazards, healing from pickups
- **Inventory System:** Event-driven item management
- **Multiplayer Ready:** Built on Netcode for GameObjects

### Technology Stack

- **Unity 6000.2.10f1**
- **Universal Render Pipeline (URP)**
- **Unity Netcode for GameObjects (2.3.2)**
- **New Input System (1.14.2)**
- **C# (.NET Standard 2.1)**

---

## Input System Setup

### Step 1: Enable New Input System

1. **Open Project Settings:**
   - Edit → Project Settings → Player
   - Scroll to **"Active Input Handling"**
   - Select **"Input System Package (New)"** or **"Both"**
   - Unity will restart

### Step 2: Create Input Actions Asset

1. **Create Asset:**
   - Right-click in Project window → Create → Input Actions
   - Rename to `PlayerInputActions`
   - Save in `Assets/Settings/` (create folder if needed)

2. **Open Input Actions Editor:**
   - Double-click `PlayerInputActions`

### Step 3: Configure Action Maps

Create a **Player** action map with these actions:

#### Move (Value, Vector2)

- **Composite:** WASD
  - Up: W
  - Down: S
  - Left: A
  - Right: D
- **Composite:** Arrow Keys
  - Up: Up Arrow
  - Down: Down Arrow
  - Left: Left Arrow
  - Right: Right Arrow
- **Binding:** Left Stick (Gamepad)

#### Look (Value, Vector2)

- **Binding:** Mouse Delta
  - Processor: Scale (X: 0.1, Y: 0.1) — adjust sensitivity
- **Binding:** Right Stick (Gamepad)

#### Jump (Button)

- **Binding:** Space
- **Binding:** Gamepad Button South (A on Xbox)

#### Sprint (Button)

- **Binding:** Left Shift
- **Binding:** Gamepad Left Stick Press

#### Interact (Button)

- **Binding:** E
- **Binding:** Gamepad Button West (X on Xbox)

#### Fire (Button)

- **Binding:** Mouse Left Button
- **Binding:** Gamepad Right Trigger

### Step 4: Generate C# Class

1. In Input Actions window, check **"Generate C# Class"**
2. Check **"Auto-Save"**
3. Set **Namespace:** `Unity.Template.Multiplayer.NGO.Runtime`
4. Click **Apply**

### Step 5: Lock Cursor (Optional)

Add this script for FPS cursor lock:

```csharp
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    public class CursorLocker : MonoBehaviour
    {
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            // Press Escape to unlock cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
```

---

## Main Scene Assembly

### Scene Structure Overview

```text
MainGame (Scene)
├── Lighting
│   └── Directional Light
├── Environment
│   └── Ground Plane
├── Systems
│   ├── RoomGenerator
│   └── InventorySystem
├── Player
│   ├── Camera
│   └── CursorLocker
└── UI
    └── Canvas (HUD)
```

### Step 1: Create New Scene

1. File → New Scene
2. Save as `MainGame.unity` in `Assets/Scenes/`

### Step 2: Add Lighting

1. **Directional Light:**
   - Already present in new scenes
   - Rotate to angle downward (e.g., X: 50, Y: -30)
   - Set Intensity: 1.5

2. **Environment Lighting:**
   - Window → Rendering → Lighting
   - Environment tab → Skybox Material: Default-Skybox
   - Ambient Intensity: 1.0

### Step 3: Add Ground (Temporary)

For testing before room generation:

1. GameObject → 3D Object → Plane
2. Rename to "Ground"
3. Scale: 10, 1, 10
4. Add Material:
   - Create Material in `Assets/Materials/`
   - Assign to plane

### Step 4: Add System GameObjects

#### InventorySystem

1. Hierarchy → Create Empty
2. Rename to "InventorySystem"
3. Add Component → `InventorySystem` script
4. The script already implements singleton pattern with `DontDestroyOnLoad`

#### RoomGenerator

1. Hierarchy → Create Empty
2. Rename to "RoomGenerator"
3. Add Component → `RoomGenerator` script
4. Configure (see [Room Generator Configuration](#room-generator-configuration))

### Step 5: Position Camera

Before adding the player prefab, set up your scene view:

1. Position Scene view camera where you want player to spawn
2. GameObject → Align View to Selected (Ctrl+Shift+F)
3. This helps with initial testing

---

## Room Generator Configuration

### Inspector Settings Breakdown

#### Grid Settings

| Field | Value | Description |
|-------|-------|-------------|
| **Grid Size** | 9 | Creates 9×9×9 = 729 cells |
| **Room Size** | 10 | World units per room (default) |
| **Generate All Rooms** | OFF | Use sparse generation |
| **Sparse Density** | 0.3 | 30% of 729 = ~218 rooms |

#### Room Templates

| Field | Assignment | Notes |
|-------|-----------|-------|
| **Room Templates** | Array of 3-5 prefabs | Standard room variants |
| **Exit Room Prefab** | Special prefab (optional) | Placed at center (4,4,4) |
| **Spawn Room Prefab** | Special prefab (optional) | Placed at 8 corners |

#### Door Settings

| Field | Value | Description |
|-------|-------|-------------|
| **False Door Chance** | 0.0 - 1.0 | Probability of blocked doors |
| **Door Open Speed** | 2.0 | Units per second |
| **Door Close Speed** | 1.0 | Units per second |

#### Rotation Settings

| Field | Value | Description |
|-------|-------|-------------|
| **Enable Room Rotation** | TRUE | Rooms rotate after door close |
| **Rotation Cooldown** | 5.0 | Seconds between rotations |
| **Rotation Duration** | 2.0 | Seconds to complete rotation |

#### Randomization

| Field | Value | Description |
|-------|-------|-------------|
| **Seed** | 0 | 0 = random, or specific number |
| **Use Random Seed** | TRUE | Generates new seed each time |

### Room Prefab Requirements

All room prefabs must have:

1. **Consistent Size:** Exactly `Room Size` units (default 10×10×10)
2. **Bounds Object:** Child GameObject named "Bounds" with MeshRenderer
3. **Door Anchors:** Children named exactly:
   - `door_north`
   - `door_south`
   - `door_east`
   - `door_west`
   - `door_up` (optional)
   - `door_down` (optional)

4. **Door Setup (per anchor):**
   - Child GameObject named "DoorCube" (the visual door)
   - `DoorController` script on anchor
   - BoxCollider (Is Trigger = true) on anchor
   - DoorCube assigned in DoorController's "Door Object" field

### Generation Modes

#### Full Generation (Generate All Rooms = TRUE)

- Creates all 729 rooms
- Heavy on performance (10-15 seconds)
- Guaranteed complete cube
- Use for final builds or testing

#### Sparse Generation (Generate All Rooms = FALSE)

- Creates subset based on Sparse Density
- Faster (1-3 seconds)
- Guaranteed paths from corners to center
- Recommended for development

### Path Connectivity Algorithm

The generator uses Manhattan distance pathfinding:

1. **Identify Endpoints:** 8 corner positions + center (4,4,4)
2. **Generate Paths:** For each corner, step toward center
3. **Place Rooms:** Along each path, place rooms at intervals
4. **Fill Remaining:** Random positions up to density limit
5. **Connect Doors:** Enable doors between adjacent rooms

### Testing Generation

```csharp
// Manual generation trigger (add to RoomGenerator for testing)
void Update()
{
    if (Input.GetKeyDown(KeyCode.G))
    {
        ClearPrevious();
        Generate();
        Debug.Log("Rooms regenerated!");
    }
}
```

---

## Player Setup

### Using Existing Player Prefab

If you have a `Player.prefab`:

1. Drag into scene at spawn position (e.g., 0, 2, 0)
2. Verify components:
   - `FirstPersonController` script
   - `CharacterController` component
   - `PlayerInput` component
   - Child `Camera` GameObject

### Creating Player From Scratch

#### Step 1: Create GameObject

1. Hierarchy → 3D Object → Capsule
2. Rename to "Player"
3. Position: 0, 2, 0
4. Tag: Player (crucial for triggers!)

#### Step 2: Add CharacterController

1. Remove Capsule Collider
2. Add Component → Character Controller
3. Settings:
   - Height: 2
   - Radius: 0.5
   - Center: 0, 1, 0
   - Slope Limit: 45
   - Step Offset: 0.3

#### Step 3: Add Camera

1. Create child GameObject → Rename to "Camera"
2. Add Component → Camera
3. Position: 0, 0.6, 0 (eye level)
4. Tag: MainCamera

#### Step 4: Add FirstPersonController Script

1. Select Player GameObject
2. Add Component → `FirstPersonController`
3. Assign Camera reference in Inspector

#### Step 5: Add Player Input

1. Add Component → Player Input
2. Assign `PlayerInputActions` asset
3. Set **Behavior:** Invoke Unity Events
4. Link events:
   - **Player → Move** → `FirstPersonController.OnMove`
   - **Player → Look** → `FirstPersonController.OnLook`
   - **Player → Jump** → `FirstPersonController.OnJump`
   - **Player → Sprint** → `FirstPersonController.OnSprint`

#### Step 6: Configure FirstPersonController

| Field | Value | Notes |
|-------|-------|-------|
| **Walk Speed** | 5.0 | Base movement speed |
| **Sprint Multiplier** | 1.5 | Sprint = 7.5 units/sec |
| **Jump Force** | 5.0 | Initial jump velocity |
| **Gravity** | 9.81 | Matches real gravity |
| **Mouse Sensitivity** | 2.0 | Adjust to preference |
| **Look X Limit** | 89 | Max look up/down angle |
| **Normal FOV** | 60 | Default camera FOV |
| **Sprint FOV** | 70 | FOV when sprinting |
| **FOV Transition Speed** | 8.0 | Lerp speed |
| **Max Health** | 100 | Starting health |
| **Health** | 100 | Current health (runtime) |

#### Step 7: Add CursorLocker

1. Add Component → `CursorLocker` (see Input System section)

#### Step 8: Save as Prefab

1. Drag Player from Hierarchy to `Assets/Prefabs/`
2. This allows spawning at runtime

---

## HUD & UI Setup

### Canvas Structure

```text
Canvas
├── HealthBar
│   ├── Background
│   ├── Fill
│   └── HealthText
```
├── Hotbar
│   ├── Slot_0
│   ├── Slot_1
│   ├── Slot_2
│   ├── Slot_3
│   └── Slot_4
├── Crosshair
└── DebugInfo (optional)
```

### Step 1: Create Canvas

1. Hierarchy → UI → Canvas
2. Settings:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920×1080

### Step 2: Create Health Bar

1. Right-click Canvas → UI → Slider
2. Rename to "HealthBar"
3. Position: Top-left corner
4. Settings:
   - Min Value: 0
   - Max Value: 100
   - Whole Numbers: TRUE
   - Value: 100

5. **Customize Appearance:**
   - Background: Red tint
   - Fill Area → Fill: Green tint
   - Delete Handle (not needed)

6. **Add Health Text:**
   - Right-click HealthBar → UI → Text - TextMeshPro
   - Rename to "HealthText"
   - Text: "100 / 100"
   - Position: Center of slider
   - Alignment: Center

### Step 3: Create Hotbar

1. Right-click Canvas → UI → Panel
2. Rename to "Hotbar"
3. Position: Bottom-center
4. Add Component → Grid Layout Group
5. Settings:
   - Cell Size: 80, 80
   - Spacing: 10, 10
   - Child Alignment: Middle Center

6. **Create Slots:**
   - Right-click Hotbar → UI → Image (5 times)
   - Rename: Slot_0, Slot_1, Slot_2, Slot_3, Slot_4
   - Each slot:
     - Add child Text - TextMeshPro
     - Text: "Empty"
     - Alignment: Center

### Step 4: Create Crosshair

1. Right-click Canvas → UI → Image
2. Rename to "Crosshair"
3. Anchor: Center (hold Alt+Shift, click center)
4. Width/Height: 32, 32
5. Assign crosshair texture (or use simple white square)

### Step 5: Add HUDController Script

1. Select Canvas
2. Add Component → `HUDController`
3. Assign references:
   - **Health Bar:** Slider component
   - **Health Text:** TextMeshProUGUI component
   - **Slots Container:** Hotbar Panel
   - **Player:** Player GameObject from Hierarchy

### Step 6: Test HUD

Press Play and verify:
- Health bar shows 100/100
- Crosshair is centered
- Hotbar slots are visible

---

## Inventory System

### How It Works

The `InventorySystem` is a singleton that manages items and fires events when inventory changes.

### Setup (Already Done)

The system GameObject should exist in your scene:

1. Hierarchy → InventorySystem
2. Script: `InventorySystem.cs`
3. No Inspector configuration needed

### Using the Inventory

#### Adding Items (from code)

```csharp
InventorySystem.Instance.AddItem("health_potion", 1);
```

#### Removing Items

```csharp
InventorySystem.Instance.RemoveItem("medkit", 1);
```

#### Checking Items

```csharp
if (InventorySystem.Instance.HasItem("key"))
{
    Debug.Log("Player has the key!");
}
```

#### Subscribing to Events

```csharp
void Start()
{
    InventorySystem.Instance.InventoryChanged += OnInventoryChanged;
}

void OnDestroy()
{
    if (InventorySystem.Instance != null)
        InventorySystem.Instance.InventoryChanged -= OnInventoryChanged;
}

void OnInventoryChanged()
{
    Debug.Log("Inventory updated!");
    // Update UI here
}
```

### Item ID Conventions

Recommended naming:

- `health_potion`
- `medkit`
- `key_red`
- `key_blue`
- `ammo_pistol`
- `weapon_sword`

---

## Hazards & Pickups

### Pickup Items

#### Creating a Pickup Prefab

1. **Create Base Object:**
   - 3D Object → Sphere
   - Rename to "Pickup_HealthPotion"
   - Scale: 0.5, 0.5, 0.5

2. **Add Collider:**
   - Add Component → Sphere Collider
   - **Is Trigger:** TRUE
   - Radius: 0.5

3. **Add Script:**
   - Add Component → `PickupItem`
   - **Item ID:** "health_potion"
   - **Quantity:** 1
   - **Rotate:** TRUE
   - **Bob:** TRUE
   - **Pickup Sound:** Assign audio clip (optional)

4. **Save as Prefab:**
   - Drag to `Assets/Prefabs/Items/`

#### Testing Pickups

1. Place in scene near player
2. Press Play
3. Walk into pickup
4. Check Console: "Player picked up health_potion (x1)"
5. Check Inventory System

### Hazard: Spikes

#### Creating Spike Hazard

1. **Create Base Object:**
   - 3D Object → Cube
   - Rename to "Hazard_Spikes"
   - Scale: 2, 0.5, 2

2. **Add Trigger:**
   - Add Component → Box Collider
   - **Is Trigger:** TRUE
   - Center: 0, 0.5, 0
   - Size: 2, 1, 2

3. **Add Script:**
   - Add Component → `HazardSpikes`
   - **Damage:** 25
   - **Cooldown:** 2.0 (seconds between damage)
   - **Damage Sound:** Assign audio clip (optional)

4. **Visual Feedback (optional):**
   - Add Material with red tint
   - Add particle system for danger indicator

### Hazard: Laser

#### Creating Laser Hazard

1. **Create Base Object:**
   - 3D Object → Cube
   - Rename to "Hazard_Laser"
   - Scale: 10, 0.1, 0.1 (beam shape)

2. **Add Trigger:**
   - Add Component → Box Collider
   - **Is Trigger:** TRUE
   - Size: 10, 0.5, 0.5 (slightly larger than visual)

3. **Add Script:**
   - Add Component → `HazardLaser`
   - **Damage Per Second:** 10
   - **Damage Interval:** 0.5 (damage every 0.5s)
   - **Continuous Sound:** Assign looping audio (optional)

4. **Visual Feedback:**
   - Add emissive material (bright red/blue)
   - Add Line Renderer for beam effect
   - Add particle system at endpoints

### Damage Flow

```
Player enters trigger
    ↓
OnTriggerEnter/Stay called
    ↓
Hazard gets FirstPersonController component
    ↓
Calls player.TakeDamage(amount)
    ↓
Player health reduced
    ↓
HealthChanged event fired
    ↓
HUD updates health bar
```

---

## Testing & Debugging

### Basic Scene Test

1. **Open MainGame scene**
2. **Verify Objects:**
   - Player at spawn position
   - RoomGenerator configured
   - InventorySystem present
   - Canvas with HUD

3. **Press Play:**
   - WASD to move
   - Mouse to look
   - Space to jump
   - Left Shift to sprint

4. **Check Console:**
   - No errors (warnings OK)
   - Room generation messages

### Testing Room Generation

1. **Add Debug Key (temporary):**

```csharp
// In RoomGenerator.cs Update()
void Update()
{
    if (Input.GetKeyDown(KeyCode.G))
    {
        ClearPrevious();
        Generate();
        Debug.Log("Rooms regenerated!");
    }
}
```

2. **Press G in Play Mode** to regenerate

3. **Verify:**
   - Rooms appear in grid pattern
   - Center room at (4,4,4) is special
   - Corner rooms are spawn points

### Testing Doors

1. **Locate a Room with Doors**
2. **Approach Door:**
   - Door should slide up (open)
   - Hear open sound (if assigned)

3. **Walk Through Door**
4. **Walk Away:**
   - Door should slide down (close)
   - Hear close sound
   - Room may rotate (if enabled)

### Testing Inventory

1. **Place Pickup in Scene**
2. **Walk Into It:**
   - Pickup disappears
   - Console: "Player picked up..."
   - HUD hotbar updates

3. **Check InventorySystem:**
   - Select InventorySystem GameObject
   - Inspect Items dictionary (debug view)

### Common Test Scenarios

#### Test 1: Player Movement

- Walk, sprint, jump work
- Camera rotates smoothly
- FOV transitions during sprint
- No jittering or clipping

#### Test 2: Health System

- Walk into spike hazard
- Health bar decreases
- Health text updates
- Die() triggers at 0 health

#### Test 3: Door Mechanics

- All 4 doors (N/S/E/W) open/close
- Rotation triggers after close
- False doors stay closed (red tint)
- No collision issues

#### Test 4: Room Navigation

- Can walk between rooms
- Rooms connect properly
- No gaps or overlaps
- Exit room reachable from corners

---

## Performance Tips

### Optimization Checklist

#### Room Generation

- **Use Sparse Mode:** 30% density = ~218 rooms vs 729
- **Occlusion Culling:** Enable in Window → Rendering → Occlusion Culling
- **LOD Groups:** Add to distant room details
- **Static Batching:** Mark room prefabs as Static

#### Rendering

- **URP Settings:**
  - Shadows: Medium distance (50 units)
  - Anti-aliasing: FXAA or SMAA
  - Disable Depth Texture (if not needed)

- **Lighting:**
  - Use baked lighting for static rooms
  - Limit real-time lights (max 2-3 per room)

#### Physics

- **Colliders:**
  - Use simple shapes (Box/Sphere) over Mesh Colliders
  - Set Layer Collision Matrix (Edit → Project Settings → Physics)
  - Disable Player-Player collision if not needed

- **Fixed Timestep:**
  - Edit → Project Settings → Time
  - Fixed Timestep: 0.02 (50Hz) — balance physics vs performance

#### Scripts

- **Caching:**
```csharp
// BAD: Called every frame
void Update()
{
    GetComponent<Rigidbody>().velocity = Vector3.zero;
}

// GOOD: Cached in Start
private Rigidbody m_Rigidbody;

void Start()
{
    m_Rigidbody = GetComponent<Rigidbody>();
}

void Update()
{
    m_Rigidbody.velocity = Vector3.zero;
}
```

- **Object Pooling:**
  - Pool pickups, projectiles, particles
  - Avoid Instantiate/Destroy in gameplay loop

### Profiler Usage

1. **Window → Analysis → Profiler**
2. **Press Play**
3. **Check:**
   - CPU Usage: Scripts, Rendering, Physics
   - Memory: Allocations per frame
   - Rendering: Draw calls, batches

4. **Target Metrics:**
   - 60 FPS = 16.6ms frame time
   - < 500 draw calls
   - < 1MB allocations per frame

---

## Godot to Unity Reference

### Core Concepts

| Godot | Unity | Notes |
|-------|-------|-------|
| `Node` | `GameObject` | Base scene object |
| `@export var` | `[SerializeField]` | Inspector variables |
| `_ready()` | `Start()` | Initialization |
| `_process(delta)` | `Update()` | Per-frame update |
| `_physics_process(delta)` | `FixedUpdate()` | Physics update |
| `queue_free()` | `Destroy(gameObject)` | Delete object |
| `get_node("Path")` | `transform.Find("Path")` | Find child |
| `$NodeName` | N/A | Use GetComponent |
| `signal` | `event Action` | Events |
| `.emit()` | `.Invoke()` | Fire event |
| `extends` | `: MonoBehaviour` | Inheritance |

### Input System

| Godot | Unity | Notes |
|-------|-------|-------|
| `Input.is_action_pressed("move_forward")` | InputAction callback | New Input System |
| `Input.get_vector(...)` | `OnMove(CallbackContext)` | Read from context |
| `Input.get_mouse_mode()` | `Cursor.lockState` | Cursor control |

### Physics

| Godot | Unity | Notes |
|-------|-------|-------|
| `CharacterBody3D` | `CharacterController` | Player physics |
| `RigidBody3D` | `Rigidbody` | Dynamic objects |
| `move_and_slide()` | `Move(velocity * Time.deltaTime)` | Character movement |
| `is_on_floor()` | `isGrounded` | Ground check |
| `Area3D` | Collider (Is Trigger) | Trigger zones |

### Scenes & Prefabs

| Godot | Unity | Notes |
|-------|-------|-------|
| `.tscn` file | Prefab asset | Reusable objects |
| `instantiate()` | `Instantiate(prefab)` | Spawn instance |
| Inherited Scene | Prefab Variant | Modified copy |

### Scripting

| Godot | Unity | Notes |
|-------|-------|-------|
| GDScript | C# | Language |
| `var health: int = 100` | `int health = 100;` | Typed variable |
| `func _ready():` | `void Start() {` | Method syntax |
| `print("text")` | `Debug.Log("text");` | Console output |
| `await get_tree().create_timer(1.0).timeout` | Coroutine or Task | Async delay |

### AutoLoad / Singletons

| Godot | Unity | Notes |
|-------|-------|-------|
| AutoLoad singleton | Singleton pattern + DontDestroyOnLoad | Persistent object |

```csharp
// Unity Singleton Pattern
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

---

## Troubleshooting

### StartupConfiguration.json Not Found

**Symptoms:** Console error when entering Play Mode: "FileNotFoundException: StartupConfiguration.json not found..."

**Causes:**

- Configuration file missing from project root
- ConfigurationManager not set to auto-create file
- First time running project after cloning

**Solutions:**

1. **Automatic Fix (Recommended):** The ConfigurationManager now auto-creates the file on first run
2. **Manual Creation:** Window → Multiplayer → Bootstrapper → Create Configuration
3. **Copy Existing:** Copy `Assets/Resources/DefaultConfigurations/StartupConfiguration.json` to project root
4. **Multiplayer Play Mode:** If using virtual players, copy config from Main Editor to each virtual player's folder

**Default Configuration:**

```json
{
   "OverrideMultiplayerRole" : "True",
   "StartAsHost" : "True",
   "StartAsServer" : "False",
   "StartAsClient" : "False",
   "MaxPlayers" : "2",
   "Port" : "9797",
   "EnableBots" : "False",
   "AllowReconnection" : "False",
   "ServerIP" : "127.0.0.1",
   "AutoConnect" : "False"
}
```

**Note:** This file controls whether the game starts as Host/Server/Client and networking settings.

---

### Player Falls Through Floor

**Symptoms:** Player drops infinitely on Play

**Causes:**

- Missing colliders on floor/rooms
- Player not using CharacterController
- Incorrect layer collision settings

**Solutions:**

1. Verify floor has Collider component (BoxCollider, MeshCollider)
2. Check Player has CharacterController (not just Collider)
3. Edit → Project Settings → Physics → Layer Collision Matrix
4. Ensure Default layer collides with itself

---

### Input Not Working

**Symptoms:** WASD/mouse input does nothing

**Causes:**

- Input System not enabled
- PlayerInput component missing
- Events not linked

**Solutions:**

1. Edit → Project Settings → Player → Active Input Handling = "Input System Package"
2. Restart Unity
3. Player GameObject → Add Component → Player Input
4. Assign PlayerInputActions asset
5. Set Behavior = "Invoke Unity Events"
6. Link Player → Move to FirstPersonController.OnMove, etc.

---

### Script Not Found / Can't Add Component

**Symptoms:** "The associated script can not be loaded" or component search fails

**Causes:**

- Compile errors in project
- File name doesn't match class name
- Script in wrong folder

**Solutions:**

1. Check Console for errors (red messages)
2. Fix all compile errors first
3. Verify file name = class name (case-sensitive)
4. Ensure script is in `Assets/` folder, not `Library/`
5. Right-click Assets folder → Reimport

---

### Doors Not Opening

**Symptoms:** Walk up to door, nothing happens

**Causes:**

- Player not tagged "Player"
- Door collider not set to trigger
- DoorController script missing
- Door Object not assigned

**Solutions:**

1. Select Player → Tag = "Player" (top of Inspector)
2. Select door anchor → Collider → Is Trigger = TRUE
3. Ensure DoorController script is on door anchor (not child)
4. DoorController → Door Object field → Assign DoorCube child

---

### Room Rotation Not Working

**Symptoms:** Door closes but room doesn't rotate

**Causes:**

- Rotate Room On Close = false
- RoomGenerator not in scene
- Rotation cooldown active

**Solutions:**

1. DoorController → Rotate Room On Close = TRUE
2. Verify RoomGenerator GameObject exists
3. RoomGenerator → Enable Room Rotation = TRUE
4. Check rotation cooldown (default 5 seconds between rotations)

---

### Inventory Not Updating

**Symptoms:** Pick up item, HUD doesn't change

**Causes:**

- InventorySystem not in scene
- HUD not subscribed to event
- Player reference missing in HUD

**Solutions:**

1. Verify InventorySystem GameObject exists
2. Check HUDController Start() subscribes:

   ```csharp
   InventorySystem.Instance.InventoryChanged += OnInventoryChanged;
   ```

3. Canvas → HUDController → Player field → Assign Player GameObject
4. Check Console for NullReferenceException

---

### Health Bar Not Decreasing

**Symptoms:** Take damage, health bar stays full

**Causes:**

- HUD not subscribed to player health event
- Slider reference not assigned

**Solutions:**

1. HUDController subscribes:

   ```csharp
   m_Player.HealthChanged += OnHealthChanged;
   ```

2. Canvas → HUDController → Health Bar → Assign Slider component
3. Canvas → HUDController → Health Text → Assign TextMeshProUGUI
4. Check player TakeDamage() fires HealthChanged event

---

### Performance Issues / Low FPS

**Symptoms:** Game runs slow, stuttering

**Causes:**

- Too many rooms generated
- Expensive shaders
- Too many draw calls
- Real-time lighting

**Solutions:**

1. RoomGenerator → Generate All Rooms = FALSE
2. Use Sparse Density = 0.2-0.3
3. Window → Analysis → Profiler to identify bottleneck
4. Mark static objects as Static
5. Use baked lighting instead of real-time
6. Reduce shadow distance (Edit → Project Settings → Quality)

---

### NullReferenceException Errors

**Symptoms:** Console spam: "NullReferenceException: Object reference not set..."

**Causes:**

- Missing component references
- Destroyed objects still referenced
- Singleton accessed before initialization

**Solutions:**

1. Identify the line number in error message
2. Add null checks:

   ```csharp
   if (InventorySystem.Instance != null)
   {
       InventorySystem.Instance.AddItem("key", 1);
   }
   ```

3. Use `GetComponent<T>()` with validation:

   ```csharp
   FirstPersonController player = other.GetComponent<FirstPersonController>();
   if (player != null)
   {
       player.TakeDamage(10);
   }
   ```

4. Verify references assigned in Inspector (no "None" values)

---

### Rooms Overlapping or Gaps

**Symptoms:** Rooms too close or too far apart

**Causes:**

- Inconsistent room prefab sizes
- Bounds object missing/wrong size
- Room Size setting incorrect

**Solutions:**

1. All room prefabs must be exactly Room Size units (default 10×10×10)
2. Each room needs "Bounds" child with MeshRenderer
3. RoomGenerator → Room Size = 10 (must match prefab size)
4. Use Gizmos to visualize grid (OnDrawGizmos in RoomGenerator)

---

## Additional Resources

### Unity Documentation

- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [Scripting Reference](https://docs.unity3d.com/ScriptReference/)
- [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/index.html)
- [Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/)

### Learning Resources

- [Unity Learn](https://learn.unity.com/) — Official tutorials
- [Brackeys YouTube](https://www.youtube.com/@Brackeys) — Beginner guides
- [Sebastian Lague](https://www.youtube.com/@SebastianLague) — Procedural generation

### Community

- [Unity Forums](https://forum.unity.com/)
- [r/Unity3D](https://www.reddit.com/r/Unity3D/)
- [Unity Discord](https://discord.com/invite/unity)

---

## Quick Command Reference

### Build & Run

```powershell
# Build project (Debug)
dotnet build "9x9.slnx" -c Debug

# Build project (Release)
dotnet build "9x9.slnx" -c Release

# Filter for project warnings only
dotnet build "9x9.slnx" 2>&1 | Select-String "Assets\\Scripts"
```

### Unity from Command Line

```powershell
# Open project
Unity.exe -projectPath "C:\\Users\\kiidh\\9x9"

# Build standalone (Windows)
Unity.exe -batchmode -projectPath "C:\\Users\\kiidh\\9x9" -buildWindows64Player "Build/Game.exe"

# Run tests
Unity.exe -batchmode -projectPath "C:\\Users\\kiidh\\9x9" -runTests -testResults "TestResults.xml"
```

### Git Workflow

```powershell
# Status
git status

# Stage changes
git add Assets/Scripts/Runtime/Game/Rooms/DoorController.cs

# Commit
git commit -m "Fix: Add Door Object field to DoorController"

# Push
git push origin main
```

---

## Next Steps

1. **Complete Room Prefabs:**
   - Create 3-5 room variants from StandardRoom
   - Add hazards, pickups, puzzles
   - Test each variant in isolation

2. **Test Full Generation:**
   - Configure RoomGenerator with all prefabs
   - Generate sparse grid (30%)
   - Verify paths from corners to center
   - Test door rotation mechanic

3. **Implement Win Condition:**
   - Add ExitTrigger script to center room
   - Detect player entry
   - Display victory UI

4. **Add Weapons (Optional):**
   - Create raycast weapon script
   - Add shooting input action
   - Implement PvP damage

5. **Multiplayer Setup:**
   - Configure Netcode spawn objects
   - Test with Multiplayer Play Mode
   - Sync room rotations across clients

6. **Polish:**
   - Audio: footsteps, ambient, doors, hazards
   - VFX: particles for hazards, pickups
   - UI: pause menu, inventory screen
   - Tutorial: onboarding for new players

---

## End of Setup Guide

For session notes and ongoing development updates, see:

- `Documentation/SessionNotes/2025-11-13.md`
- `Documentation/SessionNotes/2025-11-14.md`

For quick reference, see:

- `SETUP_GUIDE.md` (condensed version)
