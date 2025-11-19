# GitHub Copilot Instructions for 9x9 Unity Project

## Project Overview
This is a Unity 6000.2.10f1 **cube-inspired survival puzzle game** ported from Godot. The game features procedurally-generated maze-escape gameplay where players solve puzzles, scavenge items, and engage in PvP combat within interconnected cubic rooms (inspired by the 1997 film *Cube*).

**Project Type**: Multiplayer Survival Puzzle Game  
**Unity Version**: 6000.2.10f1  
**Rendering**: Universal Render Pipeline (URP)  
**Namespace**: `Unity.Template.Multiplayer.NGO`  
**Original Source**: Godot 4.1 project (ported to Unity for enhanced multiplayer and cross-platform support)

## Game Vision & Core Pillars
- **Exploration**: Navigate interconnected rooms with environmental hazards
- **Survival**: Inventory management and resource scavenging
- **PvP Combat**: Player vs. player encounters with weapons/abilities
- **Puzzle Solving**: Room-based mechanics that gate progression
- **Procedural Generation**: Randomized room layouts for replayability
- **Multi-Platform**: PC, Web, and potential VR deployment

## Architecture

### Application Structure
The project follows a Model-View-Controller (MVC) pattern with separate applications:
- **MetagameApplication**: Manages menus, matchmaking, and lobby
- **GameApplication**: Manages gameplay and game sessions
- Both inherit from `BaseApplication<TModel, TView, TController>`

### Key Packages
- `com.unity.netcode.gameobjects` (2.3.2) - Core networking
- `com.unity.multiplayer.tools` (2.2.6) - Multiplayer debugging/profiling
- `com.unity.services.multiplayer` (1.1.3) - Matchmaking services
- `com.unity.dedicated-server` (1.6.1) - Server builds
- `com.unity.render-pipelines.universal` (17.2.0) - URP rendering
- `com.unity.inputsystem` (1.14.2) - New Input System

## Code Style Guidelines

### Naming Conventions
- Use PascalCase for public members, classes, methods
- Use camelCase for private fields
- Prefix internal fields with `m_` when serialized
- Use descriptive names that indicate purpose

### Unity-Specific Patterns
```csharp
// Prefer Unity namespaces
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

// Use namespaces consistently
namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Your code here
}
```

### Network Code
- Use `NetworkBehaviour` for networked objects
- Mark network variables with `[NetworkVariable]`
- Use RPCs for client-server communication: `[ServerRpc]`, `[ClientRpc]`
- Check `IsServer`, `IsClient`, `IsHost` before network operations
- Handle dedicated server scenarios: `NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient`

### Coroutines and Async
- Use `CoroutinesHelper` for coroutine management
- Prefer async/await with Unity Services
- Always handle Task exceptions properly

### UI
- Project uses UI Toolkit (UIElements)
- Separate UI logic into View classes
- Keep controllers separate from visual elements

## Common Patterns

### Singleton Pattern
```csharp
internal new static GameApplication Instance { get; private set; }

protected override void Awake()
{
    base.Awake();
    Instance = this;
}
```

### Events System
- Use C# events for loose coupling
- Prefer `Action` and `Action<T>` delegates
- Clean up event subscriptions in `OnDestroy()`

### Service Initialization
```csharp
// Unity Services authentication and initialization
await UnityServicesAuthenticator.Initialize();
await MatchmakerTicketer.CreateTicket();
```

## Build Configuration

### Platform Targets
- Windows (Standalone)
- Linux (Dedicated Server)
- Additional platforms as needed

### Build Preprocessor
- Uses `BuildProcessor` for build customization
- Handles command line arguments for server builds
- Cloud build integration via `CloudBuildHelpers`

## Testing Guidelines
- Uses Unity Test Framework (1.6.0)
- Write unit tests for game logic
- Use Multiplayer Play Mode for testing multiplayer scenarios

## Performance Considerations
- Use object pooling for frequently spawned objects
- Minimize network traffic with NetworkVariable optimization
- Profile regularly with Multiplayer Tools Package
- Optimize rendering with URP settings

## Debugging Tips
- Use Multiplayer Tools Profiler for network debugging
- Enable Netcode debug logging: `NetworkManager.Singleton.LogLevel`
- Test with Multiplayer Play Mode before building
- Check Unity Services dashboard for matchmaking issues

## File Organization
```
Assets/
├── Scripts/
│   ├── Editor/          # Editor-only scripts
│   ├── Runtime/         # Runtime game code
│   │   ├── Game/       # Gameplay logic
│   │   ├── Metagame/   # Menus, matchmaking
│   │   ├── Shared/     # Shared utilities
│   │   └── UnityGameServices/ # UGS integration
│   └── Shared/          # Shared between editor and runtime
├── Prefabs/            # Game prefabs
├── Scenes/             # Unity scenes
├── Settings/           # Project settings
└── UIToolkit/          # UI Toolkit assets
```

## Important Notes
- Always test multiplayer code with multiple players
- Handle network disconnections gracefully
- Validate player inputs on server side
- Use NetworkVariable for state synchronization
- Dedicated server builds exclude client-only code
- Use command line arguments for server configuration
- Keep matchmaking logic separate from gameplay

## Resources
- [Unity Netcode Documentation](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [Unity Gaming Services](https://unity.com/solutions/gaming-services)
- [UI Toolkit Manual](https://docs.unity3d.com/Manual/UIElements.html)
- [URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)

---

# BEGINNER'S GUIDE: Godot to Unity Port

This section is designed for developers **completely new to Unity** who are familiar with Godot. It explains Unity concepts by comparing them to Godot equivalents.

## Table of Contents
1. [Fundamental Differences](#fundamental-differences)
2. [Core Ported Systems](#core-ported-systems)
3. [Scene Setup Guide](#scene-setup-guide)
4. [Input System Setup](#input-system-setup)
5. [Prefab Creation Guide](#prefab-creation-guide)
6. [Common Tasks & Patterns](#common-tasks--patterns)
7. [Troubleshooting](#troubleshooting)

---

## Fundamental Differences

### Godot vs Unity: Key Concepts

| Concept | Godot | Unity |
|---------|-------|-------|
| **Script Language** | GDScript | C# |
| **Scene Files** | `.tscn` (text-based) | Scenes (binary/YAML) |
| **Reusable Objects** | Scenes (inherited) | Prefabs (instantiated) |
| **Physics Objects** | CharacterBody3D, RigidBody3D | CharacterController, Rigidbody |
| **UI System** | Control nodes (Label, TextureProgress) | uGUI (Canvas, Text, Slider) |
| **Signals** | `signal name(params)` | C# events (`event Action<T>`) |
| **Autoload (Globals)** | AutoLoad singletons | Singleton pattern + DontDestroyOnLoad |
| **Node Children** | `$NodeName` or `get_node()` | `GetComponent<T>()` or `transform.Find()` |
| **Process Loops** | `_process()`, `_physics_process()` | `Update()`, `FixedUpdate()` |
| **Inspector Export** | `@export var speed: float` | `[SerializeField] private float m_Speed;` |
| **Random Numbers** | `RandomNumberGenerator` | `System.Random` or `UnityEngine.Random` |

### File Structure Comparison

**Godot Project:**
```
godot_project/
├── CharacterBody3D.gd      # Player script
├── player.tscn             # Player scene
├── systems/Inventory.gd    # Autoload singleton
├── tools/RoomGenerator.gd  # Generator script
└── rooms/hazard_spikes.gd  # Hazard script
```

**Unity Project:**
```
9x9/Assets/Scripts/
├── Runtime/
│   ├── Game/
│   │   ├── Player/FirstPersonController.cs    # Player script (ported)
│   │   ├── Items/PickupItem.cs                # Pickup script (ported)
│   │   ├── Hazards/HazardSpikes.cs            # Hazard script (ported)
│   │   ├── Hazards/HazardLaser.cs             # Hazard script (ported)
│   │   └── UI/HUDController.cs                # HUD script (ported)
│   └── Shared/
│       ├── Systems/InventorySystem.cs         # Singleton (ported)
│       └── Procedural/RoomGenerator.cs        # Generator (ported)
└── Prefabs/                                    # Reusable GameObjects
    ├── Player.prefab
    ├── Rooms/
    │   ├── RoomBase.prefab
    │   └── SimpleRoom.prefab
    └── Items/
        └── Pickup_HealthPotion.prefab
```

---

## Core Ported Systems

### 1. Player Controller (FirstPersonController.cs)

**Godot Source:** `CharacterBody3D.gd`  
**Unity Port:** `Assets/Scripts/Runtime/Game/Player/FirstPersonController.cs`

#### Key Translations:

| Godot Code | Unity Equivalent | Notes |
|------------|------------------|-------|
| `extends CharacterBody3D` | `[RequireComponent(typeof(CharacterController))]` | Unity uses component composition |
| `@export var SPEED: float = 5.0` | `[SerializeField] private float m_Speed = 5f;` | Private with serialization |
| `velocity.y -= gravity * delta` | `m_Velocity.y -= 9.81f * Time.fixedDeltaTime;` | Manual gravity application |
| `move_and_slide()` | `m_CharacterController.Move(m_Velocity * Time.fixedDeltaTime);` | Different API |
| `is_on_floor()` | `m_CharacterController.isGrounded` | Property vs method |
| `Input.get_vector(...)` | New Input System callbacks (`OnMove()`) | See Input Setup below |
| `signal health_changed(int)` | `public event Action<int> HealthChanged;` | C# events |
| `health_changed.emit(health)` | `HealthChanged?.Invoke(health);` | Null-conditional operator |

#### Setup Steps:

1. **Create Player GameObject:**
   - Right-click in Hierarchy → 3D Object → Capsule
   - Rename to "Player"
   - Add Component → Character Controller
   - Adjust CharacterController height/radius to match capsule

2. **Attach Scripts:**
   - Add Component → FirstPersonController
   - Create child GameObject named "Camera"
   - Add Component → Camera to child
   - Drag Camera into FirstPersonController's "Camera" field

3. **Configure Input:**
   - See [Input System Setup](#input-system-setup)

4. **Tag the Player:**
   - Select Player GameObject
   - Inspector → Tag → Player (required for collision detection)

---

### 2. Inventory System (InventorySystem.cs)

**Godot Source:** `systems/Inventory.gd` (AutoLoad singleton)  
**Unity Port:** `Assets/Scripts/Runtime/Shared/Systems/InventorySystem.cs`

#### Singleton Pattern Explanation:

**Godot AutoLoad:**
```gdscript
# Project Settings > AutoLoad > Inventory
extends Node

var items: Dictionary = {}

func add_item(item_id: String, quantity: int = 1):
    items[item_id] = items.get(item_id, 0) + quantity
```

**Unity Singleton:**
```csharp
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
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
    
    public void AddItem(string itemId, int quantity = 1) { /*...*/ }
}
```

#### Setup Steps:

1. **Create Inventory GameObject:**
   - Hierarchy → Create Empty
   - Rename to "InventorySystem"
   - Add Component → InventorySystem script

2. **Access from Other Scripts:**
   ```csharp
   // Godot: Inventory.add_item("health_potion", 1)
   // Unity:
   InventorySystem.Instance.AddItem("health_potion", 1);
   ```

3. **Event Subscription:**
   ```csharp
   // Godot: Inventory.inventory_changed.connect(_on_inventory_changed)
   // Unity:
   InventorySystem.Instance.InventoryChanged += OnInventoryChanged;
   
   // ALWAYS unsubscribe in OnDestroy!
   void OnDestroy()
   {
       if (InventorySystem.Instance != null)
           InventorySystem.Instance.InventoryChanged -= OnInventoryChanged;
   }
   ```

---

### 3. Room Generator (RoomGenerator.cs)

**Godot Source:** `tools/RoomGenerator.gd`  
**Unity Port:** `Assets/Scripts/Runtime/Shared/Procedural/RoomGenerator.cs`

#### NEW: 9x9x9 Cube Grid System

**Complete Redesign:** The room generator now creates a perfect **9×9×9 cube grid** (729 total room positions) inspired by the film *Cube* (1997).

**Key Features:**
- **Grid Size:** 9×9×9 = 729 possible room positions
- **Center Exit:** Room at position (4,4,4) contains the exit
- **8 Corner Spawns:** Players spawn at cube corners (0,0,0), (8,0,0), (0,0,8), (8,0,8), etc.
- **Guaranteed Paths:** Automatic pathfinding ensures all corners connect to center
- **Sparse or Dense:** Choose between full 729 rooms or optimized subset

#### Key Translations:

| Feature | Implementation |
|---------|----------------|
| **Grid Structure** | `GameObject[,,] m_RoomGrid` - 3D array storing all rooms |
| **Cell Position** | `Vector3Int` for grid coordinates (x,y,z) |
| **World Position** | `cell * m_RoomSize` (default 10 units per room) |
| **Pathfinding** | Manhattan distance algorithm for corner→center paths |
| **Room Selection** | Special prefabs for exit/spawn, random for standard rooms |

#### Setup Steps:

1. **Create Generator GameObject:**
   - Hierarchy → Create Empty
   - Rename to "RoomGenerator"
   - Add Component → RoomGenerator

2. **Configure in Inspector:**
   - **Grid Size:** 9 (creates 9×9×9 = 729 cells)
   - **Room Size:** 10 (world units per room)
   - **Room Templates:** Assign standard room prefabs (3-5 variants)
   - **Exit Room Prefab:** Special room for center (optional)
   - **Spawn Room Prefab:** Special room for corners (optional)
   - **Generate All Rooms:** OFF (use sparse generation)
   - **Sparse Density:** 0.3 (30% of 729 = ~218 rooms)
   - **Seed:** 0 for random, or specific number for reproducible layout

3. **Room Prefab Requirements:**
   - **Consistent Size:** All rooms must be exactly `m_RoomSize` units (10×10×10)
   - **No Door Anchors Needed:** Grid alignment is automatic!
   - **Optional:** Add "door_north", "door_south", etc. for visual doors

4. **Generation Modes:**
   - **Sparse (Recommended):** Generates subset with guaranteed paths
   - **Full (Performance Heavy):** All 729 rooms instantiated

5. **Access Room Data:**
   ```csharp
   // Get room at grid position
   GameObject room = roomGenerator.GetRoomAt(4, 4, 4); // Center room
   
   // Get random spawn position
   Vector3 spawnPos = roomGenerator.GetRandomSpawnPosition();
   
   // Check if room is special
   RoomData data = room.GetComponent<RoomData>();
   if (data.IsExitRoom) { /* Win condition */ }
   if (data.IsSpawnRoom) { /* Player spawn */ }
   ```

---

### 4. Pickup Items (PickupItem.cs)

**Godot Source:** `rooms/pickup_item.gd`  
**Unity Port:** `Assets/Scripts/Runtime/Game/Items/PickupItem.cs`

#### Trigger vs Collision:

**Godot:**
```gdscript
extends Area3D  # Non-physical trigger zone

func _on_body_entered(body: Node):
    Inventory.add_item(item_id, quantity)
    queue_free()
```

**Unity:**
```csharp
[RequireComponent(typeof(Collider))]  // Must have collider
public class PickupItem : MonoBehaviour
{
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;  // Set to trigger mode
    }
    
    void OnTriggerEnter(Collider other)  // Called on collision
    {
        if (other.CompareTag("Player"))
        {
            InventorySystem.Instance.AddItem(m_ItemId, m_Quantity);
            Destroy(gameObject);
        }
    }
}
```

#### Setup Steps:

1. **Create Pickup Prefab:**
   - Create 3D Object → Sphere (or any mesh)
   - Add Component → Sphere Collider (if not present)
   - **CHECK "Is Trigger"** in Collider component
   - Add Component → PickupItem script

2. **Configure Script:**
   - Set Item ID (e.g., "health_potion")
   - Set Quantity (e.g., 1)
   - Enable Rotate/Bob for visual effect

3. **Optional: Audio:**
   - Assign Audio Clip to "Pickup Sound"

4. **Save as Prefab:**
   - Drag GameObject from Hierarchy to Project window
   - Delete from scene (use prefab instances instead)

---

### 5. Hazards (HazardSpikes.cs, HazardLaser.cs)

**Godot Source:** `rooms/hazard_spikes.gd`, `rooms/hazard_laser.gd`  
**Unity Port:** `Assets/Scripts/Runtime/Game/Hazards/HazardSpikes.cs`, `HazardLaser.cs`

#### Instant vs Continuous Damage:

**Spikes (Instant with Cooldown):**
```csharp
// Godot: await get_tree().create_timer(cooldown).timeout
// Unity: Manual timer with Time.time

private bool m_IsCoolingDown = false;
private float m_CooldownEndTime = 0f;

void OnTriggerEnter(Collider other)
{
    if (m_IsCoolingDown) return;
    
    FirstPersonController player = other.GetComponent<FirstPersonController>();
    if (player != null)
    {
        player.TakeDamage(m_Damage);
        m_IsCoolingDown = true;
        m_CooldownEndTime = Time.time + m_Cooldown;
    }
}

void Update()
{
    if (m_IsCoolingDown && Time.time >= m_CooldownEndTime)
        m_IsCoolingDown = false;
}
```

**Laser (Continuous Damage):**
```csharp
// Godot: Tracked bodies in array, used _process() with timers
// Unity: Dictionary tracking each body independently

private Dictionary<GameObject, float> m_BodiesInArea = new Dictionary<GameObject, float>();

void OnTriggerEnter(Collider other)
{
    m_BodiesInArea[other.gameObject] = 0f;  // Add to tracking
}

void OnTriggerExit(Collider other)
{
    m_BodiesInArea.Remove(other.gameObject);  // Stop tracking
}

void Update()
{
    foreach (var kvp in m_BodiesInArea)
    {
        if (Time.time >= kvp.Value + m_DamageInterval)
        {
            // Deal damage and update timer
        }
    }
}
```

---

### 6. HUD Controller (HUDController.cs)

**Godot Source:** `ui/HUD.gd`  
**Unity Port:** `Assets/Scripts/Runtime/Game/UI/HUDController.cs`

#### UI System Comparison:

| Godot UI Toolkit | Unity uGUI |
|------------------|------------|
| `CanvasLayer` | `Canvas` |
| `TextureProgress` | `Slider` or `Image` (with fillAmount) |
| `Label` | `TextMeshProUGUI` |
| `GridContainer` | `GridLayoutGroup` |
| `label.text = "..."` | `textComponent.text = "...";` |
| `progress_bar.value = 75` | `slider.value = 75;` |

#### Setup Steps:

1. **Create Canvas:**
   - Hierarchy → UI → Canvas
   - Set Render Mode → Screen Space - Overlay

2. **Create Health Bar:**
   - Right-click Canvas → UI → Slider
   - Rename to "HealthBar"
   - Configure: Min Value = 0, Max Value = 100
   - Optionally add Text child for "75/100" display

3. **Create Hotbar:**
   - Right-click Canvas → UI → Panel (rename to "Hotbar")
   - Add Component → Grid Layout Group
   - Set Cell Size (e.g., 100x50)
   - Create 5 Text children (for 5 inventory slots)

4. **Create Crosshair:**
   - Right-click Canvas → UI → Image
   - Set Anchor to center (hold Alt+Shift, click center)
   - Assign crosshair texture

5. **Attach HUD Script:**
   - Select Canvas
   - Add Component → HUDController
   - Assign references in Inspector:
     - Health Bar → Slider component
     - Health Text → Text component
     - Slots Container → Hotbar Panel
     - Player → Drag Player GameObject from Hierarchy

---

## Scene Setup Guide

### Creating Your First Scene

1. **Create Main Scene:**
   - File → New Scene
   - Save as `MainGame.unity` in `Assets/Scenes/`

2. **Add Ground:**
   - 3D Object → Plane
   - Scale to 10x1x10
   - Add Material for visibility

3. **Add Lighting:**
   - GameObject → Light → Directional Light
   - Rotate to angle downward

4. **Add Player:**
   - Drag Player prefab into scene
   - Position at (0, 2, 0)

5. **Add InventorySystem:**
   - Create Empty → Rename to "InventorySystem"
   - Add InventorySystem script

6. **Add HUD Canvas:**
   - UI → Canvas
   - Add HUDController script
   - Link player reference

7. **Add RoomGenerator (Optional):**
   - Create Empty → Rename to "RoomGenerator"
   - Add RoomGenerator script
   - Assign room prefabs

---

## Input System Setup

Unity's New Input System requires configuration. Here's how to set it up:

### Step 1: Enable New Input System

1. Edit → Project Settings → Player
2. Scroll to "Active Input Handling"
3. Select "Input System Package (New)" or "Both"
4. Unity will restart

### Step 2: Create Input Actions Asset

1. Right-click in Project → Create → Input Actions
2. Rename to "PlayerInputActions"
3. Double-click to open

### Step 3: Configure Actions

Create these action maps:

**Player Actions:**
- **Move** (Value, Vector2)
  - Binding: WASD (Composite)
  - Binding: Arrow Keys (Composite)
  - Binding: Left Stick (Gamepad)
  
- **Look** (Value, Vector2)
  - Binding: Mouse Delta
  - Binding: Right Stick (Gamepad)
  
- **Jump** (Button)
  - Binding: Space
  - Binding: Gamepad Button South
  
- **Sprint** (Button)
  - Binding: Left Shift
  - Binding: Gamepad Left Stick Press

### Step 4: Generate C# Class

1. In Input Actions window, click "Generate C# Class"
2. Check "Auto-Save"
3. Apply

### Step 5: Attach to Player

1. Select Player GameObject
2. Add Component → Player Input
3. Assign "PlayerInputActions" asset
4. Set Behavior → "Invoke Unity Events"
5. Link events to FirstPersonController methods:
   - Move → FirstPersonController.OnMove
   - Look → FirstPersonController.OnLook
   - Jump → FirstPersonController.OnJump
   - Sprint → FirstPersonController.OnSprint

---

## Prefab Creation Guide

Prefabs are Unity's equivalent to Godot's inherited scenes.

### Room Prefab Structure

```
RoomBase (GameObject)
├── Walls (3D objects)
├── Floor (3D object)
├── Ceiling (3D object)
├── Bounds (Cube with MeshRenderer) - for overlap detection
├── door_north (Empty GameObject)
├── door_south (Empty GameObject)
├── door_east (Empty GameObject)
└── door_west (Empty GameObject)
```

#### Creating a Room Prefab:

1. **Build Room in Scene:**
   - Create Empty → Rename to "SimpleRoom"
   - Add floor (Cube scaled to 10x0.1x10)
   - Add walls (4 cubes scaled appropriately)
   - Add door anchors (Create Empty at each door position)

2. **Add Bounds:**
   - Create child Cube → Rename to "Bounds"
   - Scale to match room dimensions
   - Disable MeshRenderer (just needs the Renderer component for bounds calculation)
   - **DO NOT add collider** (RoomGenerator uses Renderer.bounds)

3. **Name Door Anchors:**
   - Position empty GameObjects at door locations
   - Name exactly: `door_north`, `door_south`, `door_east`, `door_west`
   - Ensure they face outward (blue Z-axis pointing out of room)

4. **Save as Prefab:**
   - Drag "SimpleRoom" from Hierarchy to `Assets/Prefabs/Rooms/`
   - Delete from scene

5. **Create Variants:**
   - Duplicate prefab in Project window
   - Open Prefab mode (double-click)
   - Add hazards, pickups, puzzles
   - Save

---

## Common Tasks & Patterns

### Task 1: Add New Collectible Item

**Godot Pattern:**
```gdscript
# Create pickup_item.tscn scene
# Attach pickup_item.gd
@export var item_id: String = "new_item"
```

**Unity Pattern:**

1. Create sphere GameObject
2. Add SphereCollider → Set "Is Trigger"
3. Add PickupItem script
4. Set Item ID = "new_item"
5. Save as Prefab

**Access in Code:**
```csharp
if (InventorySystem.Instance.HasItem("new_item"))
{
    Debug.Log("Player has the new item!");
}
```

---

### Task 2: Create Custom Hazard

**Example: Poison Gas Cloud**

```csharp
using UnityEngine;

public class HazardPoisonGas : MonoBehaviour
{
    [SerializeField] private int m_DamagePerSecond = 5;
    private float m_DamageTimer = 0f;
    
    void OnTriggerStay(Collider other)  // Called every frame while in trigger
    {
        m_DamageTimer += Time.deltaTime;
        
        if (m_DamageTimer >= 1f)
        {
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                player.TakeDamage(m_DamagePerSecond);
                m_DamageTimer = 0f;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        m_DamageTimer = 0f;  // Reset timer when leaving
    }
}
```

---

### Task 3: Implement Weapon System

**Basic Raycast Weapon:**

```csharp
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    [SerializeField] private int m_Damage = 25;
    [SerializeField] private float m_Range = 100f;
    [SerializeField] private Camera m_Camera;
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))  // Left mouse click
        {
            Shoot();
        }
    }
    
    void Shoot()
    {
        Ray ray = m_Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        
        if (Physics.Raycast(ray, out RaycastHit hit, m_Range))
        {
            // Check if we hit something damageable
            FirstPersonController enemy = hit.collider.GetComponent<FirstPersonController>();
            if (enemy != null)
            {
                enemy.TakeDamage(m_Damage);
                Debug.Log($"Hit {hit.collider.name} for {m_Damage} damage!");
            }
        }
    }
}
```

---

### Task 4: Save/Load Game State

**Using PlayerPrefs (Simple):**

```csharp
// Save inventory
public void SaveGame()
{
    int index = 0;
    foreach (var kvp in InventorySystem.Instance.Items)
    {
        PlayerPrefs.SetString($"item_{index}_id", kvp.Key);
        PlayerPrefs.SetInt($"item_{index}_count", kvp.Value);
        index++;
    }
    PlayerPrefs.SetInt("item_count", index);
    PlayerPrefs.Save();
}

// Load inventory
public void LoadGame()
{
    InventorySystem.Instance.Clear();
    
    int itemCount = PlayerPrefs.GetInt("item_count", 0);
    for (int i = 0; i < itemCount; i++)
    {
        string id = PlayerPrefs.GetString($"item_{i}_id");
        int count = PlayerPrefs.GetInt($"item_{i}_count");
        InventorySystem.Instance.AddItem(id, count);
    }
}
```

---

### Task 5: Add Sound Effects

**Playing Audio on Events:**

```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip m_PickupSound;
    [SerializeField] private AudioClip m_DamageSound;
    [SerializeField] private AudioSource m_AudioSource;
    
    void Start()
    {
        // Subscribe to events
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.InventoryChanged += OnItemPickup;
        
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.HealthChanged += OnHealthChanged;
    }
    
    void OnItemPickup()
    {
        m_AudioSource.PlayOneShot(m_PickupSound);
    }
    
    void OnHealthChanged(int newHealth)
    {
        m_AudioSource.PlayOneShot(m_DamageSound);
    }
}
```

---

## Troubleshooting

### Problem: Player Falls Through Floor

**Cause:** Missing colliders or incorrect layer settings

**Solution:**
1. Ensure floor has Collider component (BoxCollider, MeshCollider)
2. Player needs CharacterController component
3. Check Layer Collision Matrix: Edit → Project Settings → Physics

---

### Problem: Input Not Working

**Cause:** New Input System not configured

**Solution:**
1. Check Project Settings → Player → Active Input Handling = "Input System Package"
2. Ensure PlayerInput component is on Player GameObject
3. Verify Input Actions asset is assigned
4. Check that callbacks (OnMove, OnJump, etc.) are linked in Inspector

---

### Problem: Inventory Not Updating UI

**Cause:** Event not subscribed or InventorySystem missing

**Solution:**
1. Check InventorySystem GameObject exists in scene
2. Verify HUDController has subscribed: `InventorySystem.Instance.InventoryChanged += OnInventoryChanged;`
3. Check for unsubscribe errors in OnDestroy
4. Debug with: `Debug.Log("Inventory changed!");` in event handler

---

### Problem: Pickup Items Not Collected

**Cause:** Collider not set to trigger, or player tag missing

**Solution:**
1. Select Pickup GameObject
2. Ensure Collider component has "Is Trigger" checked
3. Select Player GameObject → Tag must be "Player"
4. Check OnTriggerEnter is spelled correctly (case-sensitive!)

---

### Problem: Hazards Not Dealing Damage

**Cause:** Player doesn't have TakeDamage method or wrong component

**Solution:**
1. Ensure Player has FirstPersonController script attached
2. Verify TakeDamage() method exists and is public
3. Check hazard collider is set to trigger
4. Debug: Add `Debug.Log("Collided with " + other.name);` in OnTriggerEnter

---

### Problem: Room Generator Not Working

**Cause:** Missing Bounds or incorrect door naming

**Solution:**
1. Each room prefab must have child named "Bounds" (exact spelling)
2. Bounds must have MeshRenderer or Collider component
3. Door anchors must be named exactly: `door_north`, `door_south`, etc.
4. Check Console for errors during generation

---

### Problem: NullReferenceException

**Cause:** Accessing a component/object that doesn't exist

**Solution:**
1. Use null checks:
   ```csharp
   if (InventorySystem.Instance != null)
   {
       InventorySystem.Instance.AddItem("key", 1);
   }
   ```

2. Check Inspector: Are all references assigned?

3. Use `GetComponent` safely:
   ```csharp
   FirstPersonController player = other.GetComponent<FirstPersonController>();
   if (player != null)
   {
       player.TakeDamage(10);
   }
   ```

---

## Beginner Learning Path

### Week 1: Unity Basics
1. Complete Unity Learn tutorials: "Create with Code"
2. Understand GameObjects, Components, Transforms
3. Learn Inspector and Hierarchy windows

### Week 2: C# Fundamentals
1. Variables, methods, classes
2. Events vs direct method calls
3. Namespaces and using statements

### Week 3: Ported Systems
1. Study FirstPersonController.cs (compare to Godot version)
2. Understand InventorySystem singleton pattern
3. Test pickup items and hazards

### Week 4: Procedural Generation
1. Understand RoomGenerator.cs
2. Create custom room prefabs
3. Experiment with different layouts

### Week 5: UI & Polish
1. Build HUD from scratch
2. Add visual/audio feedback
3. Implement pause menu

### Week 6: Multiplayer (Advanced)
1. Learn Unity Netcode basics
2. Synchronize player movement
3. Implement networked inventory

---

## Quick Reference: Godot → Unity Cheat Sheet

```
# VARIABLES
Godot: var speed: float = 5.0
Unity: float speed = 5f;

Godot: @export var health: int = 100
Unity: [SerializeField] private int m_Health = 100;

# FUNCTIONS
Godot: func _ready():
Unity: void Start() {

Godot: func _process(delta: float):
Unity: void Update() {

Godot: func _physics_process(delta: float):
Unity: void FixedUpdate() {

# NODE ACCESS
Godot: $Camera or get_node("Camera")
Unity: GetComponent<Camera>() or transform.Find("Camera")

# SIGNALS/EVENTS
Godot: signal health_changed(new_health: int)
Unity: public event Action<int> HealthChanged;

Godot: health_changed.emit(health)
Unity: HealthChanged?.Invoke(health);

Godot: signal_name.connect(callable)
Unity: EventName += MethodName;

# SCENE/PREFAB
Godot: var scene = preload("res://player.tscn")
       var instance = scene.instantiate()
Unity: GameObject prefab = Resources.Load<GameObject>("Player");
       GameObject instance = Instantiate(prefab);

# PHYSICS
Godot: move_and_slide()
Unity: characterController.Move(velocity * Time.deltaTime);

Godot: is_on_floor()
Unity: characterController.isGrounded

# RANDOMNESS
Godot: var rng = RandomNumberGenerator.new()
       rng.seed = 123
       var value = rng.randi_range(1, 10)
Unity: var rng = new System.Random(123);
       int value = rng.Next(1, 11);

# TIMERS
Godot: await get_tree().create_timer(1.0).timeout
Unity: float timer = Time.time + 1f;
       // In Update(): if (Time.time >= timer) { ... }

# DESTROY OBJECT
Godot: queue_free()
Unity: Destroy(gameObject);

# FIND OBJECT
Godot: get_tree().get_root().find_child("Player", true, false)
Unity: GameObject.FindGameObjectWithTag("Player")
       FindObjectOfType<FirstPersonController>()
```

---

## Additional Resources

### Official Documentation
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Unity Scripting Reference](https://docs.unity3d.com/ScriptReference/)

### Tutorials
- [Unity Learn](https://learn.unity.com/) - Official Unity courses
- [Brackeys YouTube](https://www.youtube.com/@Brackeys) - Beginner-friendly tutorials
- [Sebastian Lague](https://www.youtube.com/@SebastianLague) - Procedural generation

### Community
- [Unity Forums](https://forum.unity.com/)
- [r/Unity3D](https://www.reddit.com/r/Unity3D/)
- [Unity Discord](https://discord.com/invite/unity)

---

**Last Updated:** November 13, 2025  
**Maintained By:** Development Team  
**For Questions:** Check troubleshooting section or ask in Unity forums
