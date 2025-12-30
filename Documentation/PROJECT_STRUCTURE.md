# Project Structure (Reorganized)

## Overview
This document describes the reorganized project structure for the 9x9 Cube Maze Game. The structure has been simplified to separate core game logic from templates, networking, and metagame systems.

**Updated:** December 25, 2025 — Assembly architecture refined with no-shim setup (Burst 1.8.27).

## Assembly Boundaries

The project uses a clean asmdef-based architecture to prevent cycles and maintain clear dependencies:

| Assembly | Purpose | Dependencies |
|----------|---------|--------------|
| **Core** | Shared contracts, enums, interfaces | None |
| **Shared** | Utilities, systems (inventory, procedural) | Core |
| **Game** | Gameplay logic, player, hazards, rooms | Core, Shared |
| **Metagame** | Menu flow, application state | Core, Shared |
| **UI** | UI panels, controllers, menus | Core, Shared, Game, Metagame, UnityGameServices |
| **UnityGameServices** | UGS integration (matchmaker, auth) | Core, Shared, Metagame |

**Key Pattern:** Cross-assembly calls use interfaces defined in Core (e.g., `IMenuManager`); implementation details stay private to their assembly.

## Directory Structure

### `/Assets/Game/` - Core Gameplay
All gameplay-specific code and assets for the cube maze game.

- **`/Player/`** - Player controller, movement, health, inventory interaction
  - `FirstPersonController.cs` - Main player controller (ported from Godot)
  
- **`/Hazards/`** - Environmental hazards (spikes, lasers, etc.)
  - `HazardSpikes.cs` - Instant damage with cooldown
  - `HazardLaser.cs` - Continuous damage over time
  
- **`/Items/`** - Collectible items and pickups
  - `PickupItem.cs` - Generic pickup item component
  
- **`/Rooms/`** - Room-specific logic and data
  - `RoomData.cs` - Room metadata (is exit, is spawn, etc.)
  - `DoorController.cs` - Door behavior and transitions
  
- **`/UI/`** - In-game user interface
  - `HUDController.cs` - Health bar, inventory display, crosshair
  
- **`/Controllers/`** - Game mode controllers and application logic
  - `CubeGameApplication.cs` - Main game application
  - `CubeGameController.cs` - Game loop, room generation orchestration
  - `CubeGameModel.cs` - Game state data
  - `CubeGameView.cs` - Game view rendering
  - `CubeGameEvents.cs` - Game-specific events

### `/Assets/Bots/` - AI System (NEW)
Bot AI for singleplayer mode. Will contain:
- `BotController.cs` - Bot movement, combat, puzzle-solving
- `BotManager.cs` - Bot spawning, lifecycle management
- Bot behavior scripts (future)

### `/Assets/Core/` - Core Systems
Shared systems used across the entire game.

- **`/Procedural/`** - Procedural generation
  - `RoomGenerator.cs` - 9×9×9 cube grid generator
  - `RoomGenConfig.cs` - Configuration for room generation
  
- **`/Systems/`** - Singleton systems
  - `InventorySystem.cs` - Global inventory management
  - `PlayerProfileManager.cs` - Player profile/stats

### `/Assets/Scripts/Runtime/` - Template & Networking
Original multiplayer template code (kept for reference/networking).

- **`/Core/`** - Base MVC framework
  - `BaseApplication.cs`, `Controller.cs`, `Model.cs`, `View.cs`, `Element.cs`, `EventManager.cs`
  
- **`/Game/`** - Generic game application framework
  - `GameApplication.cs` - Base game application
  - `Controllers/`, `Models/`, `Views/` - Generic MVC structure
  - `Player.cs` - Network player wrapper (legacy)
  
- **`/Metagame/`** - Menu, matchmaking, lobby systems
  - Controllers, Models, Views for main menu and matchmaker
  
- **`/Shared/`** - Shared networking utilities
  - `CustomNetworkManager.cs`, `ConnectionApprovalHandler.cs`
  - `CommandLineArgumentsParser.cs`, `ConfigurationManager.cs`
  - `/Data/` - `GameMode.cs`, `PlayerProfile.cs`
  
- **`/UnityGameServices/`** - UGS integration
  - Matchmaker, authentication, initialization

### `/Assets/Prefabs/` - Reusable GameObjects
- **`/Game/`** - Game prefabs (Player, CubeGameApplication)
- **`/Rooms/`** - Room prefabs (ExitRoom, SpawnRoom, StandardRoom variants)
- **`/Metagame/`** - Menu prefabs
- **`/Shared/`** - NetworkManager, UnityServicesManager

### `/Assets/Scenes/` - Unity Scenes
- `MetagameScene.unity` - Main menu / matchmaking

### `/Documentation/` - Project Documentation
- Setup guides, game design docs, session notes
- `SINGLEPLAYER_SETUP.md` - Instructions for singleplayer implementation

### `/ProjectSettings/` - Unity Project Settings
Unity engine configuration (audio, physics, input, build settings, etc.)

### `/Packages/` - Unity Packages
Package manifest and lock files for dependencies

## Assembly Definitions

### `Game.asmdef` (NEW)
- **Location:** `/Assets/Game/`
- **Namespace:** `Unity.Template.Multiplayer.NGO.Game`
- **Dependencies:** Core, Unity.Netcode.Runtime, Unity.InputSystem, Unity.Template.Multiplayer.NGO.Runtime
- **Purpose:** All gameplay code (player, hazards, items, rooms, UI, controllers)

### `Core.asmdef` (NEW)
- **Location:** `/Assets/Core/`
- **Namespace:** `Unity.Template.Multiplayer.NGO.Core`
- **Dependencies:** Unity.Netcode.Runtime, Unity.Mathematics
- **Purpose:** Shared systems (procedural generation, inventory, profiles)

### `Bots.asmdef` (NEW)
- **Location:** `/Assets/Bots/`
- **Namespace:** `Unity.Template.Multiplayer.NGO.Bots`
- **Dependencies:** Game, Core, Unity.Netcode.Runtime, Unity.InputSystem, Unity.AI.Navigation
- **Purpose:** AI bot controllers for singleplayer mode

### `com.unity.template.multiplayer-ngo.runtime.asmdef` (LEGACY)
- **Location:** `/Assets/Scripts/Runtime/`
- **Purpose:** Template framework code (Core MVC, Metagame, Shared utilities)

### `com.unity.template.multiplayer-ngo.shared.asmdef` (LEGACY)
- **Location:** `/Assets/Scripts/Shared/`
- **Purpose:** JSON utilities, UI helpers

### `com.unity.template.multiplayer-ngo.editor.asmdef` (LEGACY)
- **Location:** `/Assets/Scripts/Editor/`
- **Purpose:** Editor-only scripts (bootstrapper, build processor)

## Naming Conventions

### Folders
- PascalCase (e.g., `Player/`, `Hazards/`)
- Descriptive, single-purpose names

### Scripts
- PascalCase with clear purpose (e.g., `FirstPersonController.cs`, `HazardLaser.cs`)
- Prefixes: `Bot*` for AI, `Hazard*` for hazards, `Pickup*` for items

### Namespaces
- Match folder structure: `Unity.Template.Multiplayer.NGO.Game.Player`
- Root namespaces defined in .asmdef files

## Migration Notes

### From Old Structure to New
- **Player scripts:** `Scripts/Runtime/Game/Player/` → `/Game/Player/`
- **Hazards:** `Scripts/Runtime/Game/Hazards/` → `/Game/Hazards/`
- **Items:** `Scripts/Runtime/Game/Items/` → `/Game/Items/`
- **Rooms:** `Scripts/Runtime/Game/Rooms/` → `/Game/Rooms/`
- **UI:** `Scripts/Runtime/Game/UI/` → `/Game/UI/`
- **CubeGame controllers:** `Scripts/Runtime/Game/CubeGame/` → `/Game/Controllers/`
- **Procedural:** `Scripts/Runtime/Shared/Procedural/` → `/Core/Procedural/`
- **Systems:** `Scripts/Runtime/Shared/Systems/` → `/Core/Systems/`

### Namespace Updates Required
After moving files, update namespace declarations:
- Old: `namespace Unity.Template.Multiplayer.NGO.Runtime`
- New (Game): `namespace Unity.Template.Multiplayer.NGO.Game` or `namespace Unity.Template.Multiplayer.NGO.Game.Player`
- New (Core): `namespace Unity.Template.Multiplayer.NGO.Core.Systems` or `namespace Unity.Template.Multiplayer.NGO.Core.Procedural`
- New (Bots): `namespace Unity.Template.Multiplayer.NGO.Bots`

## Benefits of New Structure

1. **Clear Separation:** Game logic separate from template/networking code
2. **Easier Navigation:** Related files grouped together
3. **Modular:** Bots, Core, and Game are independent modules
4. **Scalable:** Easy to add new systems without cluttering root
5. **Cleaner Git Diffs:** Changes to game logic don't touch template files

## Next Steps

1. ✅ Create new folder structure
2. ✅ Move files to organized locations
3. ✅ Create new assembly definitions
4. ⏳ Update namespaces in moved files
5. ⏳ Test Unity compilation
6. ⏳ Commit reorganization
7. ⏳ Implement bot system in `/Bots/`

---

**Last Updated:** November 18, 2025  
**Maintained By:** Development Team
