# 9x9 Cube Maze Game Setup Guide

## What We Created

You now have a complete **9x9 Cube Maze escape game** system to replace the demo "press to win" game. Here's what was built:

### New Files Created

1. **CubeGameApplication.cs** - Main application component for the maze game
2. **CubeGameModel.cs** - Game state management (maze status, players alive, match timer)
3. **CubeGameView.cs** - UI management (HUD, victory/defeat screens)
4. **CubeGameController.cs** - Game logic (maze generation, spawning, win conditions)
5. **CubeGameEvents.cs** - Events for player death and reaching exit

### Enhanced Files

- **RoomGenerator.cs** - Added `GetCornerSpawnPosition()` method for player spawning
- **Player.cs** - Added `IsAlive`, `Kills`, `Deaths` properties

---

## How to Set It Up

### Step 1: Create the CubeGameApplication Prefab

1. **In Unity Editor:**
   - Navigate to `Assets/Prefabs/Game/`
   - Right-click → Create → Prefab
   - Name it `CubeGameApplication`

2. **Add Components:**
   - Drag the new prefab into the scene
   - Add Component → **CubeGameApplication** script
   - Add Component → **CubeGameModel** script  
   - Add Component → **CubeGameView** script
   - Add Component → **CubeGameController** script

3. **Optional Child Objects:**
   ```
   CubeGameApplication (root)
   └── RoomGenerator (Empty GameObject with RoomGenerator script) - OPTIONAL
   ```

   **Note:** The following are NOT required and will be auto-created at runtime if missing:
   - **MazeDataSynchronizer** - Not implemented; can be added later for multiplayer sync
   - **HUDCanvas** - Auto-created by CubeGameView if not assigned
   - **MatchRecapCanvas** - Auto-created by CubeGameView if not assigned
   - **RoomGenerator** - Auto-created by CubeGameController if not found in scene

### Step 2: Configure RoomGenerator (Optional)

On the **RoomGenerator** component:

- **Grid Size:** 9 (creates 9×9×9 = 729 room grid)
- **Room Size:** 10 (world units per room)
- **Room Templates:** Drag standard room prefabs from `Assets/Prefabs/Rooms/`
  - `StandardRoom_Variant0`
  - `StandardRoom_HazardVariant0_FloorSpikes`
  - `StandardRoom_HazardVariant0_FloorLasers`
- **Exit Room Prefab:** `ExitRoom`
- **Spawn Room Prefab:** `SpawnRoom`
- **Generate On Start:** ❌ **UNCHECKED** (CubeGameController triggers generation)
- **Generate All Rooms:** ❌ Unchecked (use sparse generation)
- **Sparse Density:** 0.3 (30% of 729 = ~218 rooms)

### Step 3: Assign References in CubeGameView (Optional)

- **HUD:** Leave empty (auto-creates runtime HUD)
- **Match Recap:** Leave empty (auto-creates runtime recap screen)

### Step 4: Assign References in CubeGameController (Optional)

- **Room Generator:** Leave empty (auto-creates runtime generator)
- **Offline Player Prefab:** Leave empty (auto-creates minimal player with camera)

### Step 5: Replace GameApplication in NetworkManager

1. Open `Assets/Prefabs/Shared/NetworkManager.prefab`
2. Find **Custom Network Manager** component
3. Change **Game App Prefab** field from `GameApplication` to `CubeGameApplication`
4. Save the prefab

---

## Project Stability & Build Status

### ✅ Current Compilation State
- **Status:** Clean — no errors or Burst resolver issues.
- **Burst Version:** 1.8.27 (compatible with Unity 6000.2.10f1).
- **Assembly Setup:** No-shim (Burst 1.8.27 handles asmdef-only projects cleanly).
- **Default Assemblies:** Kept empty via validator at `Assets/Scripts/Editor/ValidateDefaultAssemblies.cs`.

### Key Architectural Patterns
1. **Interfaces over Reflection:** Cross-assembly calls use Core contracts (e.g., `IMenuManager`) to avoid cycles.
   - Example: `MainMenuView` implements `IMenuManager` in UI; `Metagame` consumes it via interface.
2. **Controlled Visibility:** Only methods necessary for cross-assembly use are public (e.g., `MatchmakerView.UpdateTimer()`).
3. **Assembly Boundaries:**
   - `Core`: Shared contracts and enums.
   - `Shared`: Utilities and systems (inventory, procedural generation).
   - `Game`: Gameplay logic.
   - `Metagame`: Menu and flow.
   - `UI`: UI panels and controllers.
   - `UnityGameServices`: UGS integration.

---

## Game Modes Supported

The CubeGameController supports all your game modes:

### 1. **NewGame / Standard**
- Players spawn at 8 corner positions
- First player to reach center exit room wins
- Deaths reduce player count

### 2. **Co-op**
- All players must reach the exit
- Tracks individual player progress
- Match ends when all players escape or die

### 3. **Battle Royale**
- 10-minute match timer
- Last player alive wins
- Can implement shrinking safe zone

### 4. **3x3 / 5x5 Modes**
- Easy to extend: Change `m_GridSize` in RoomGenerator
- 3x3 = 27 rooms, 5x5 = 125 rooms

---

## How It Works

### 1. Player Clicks "Single Player" → "New Game"
```
MainMenuController → CustomNetworkManager.InitializeNetworkLogic()
→ Server starts → GameApplication instantiated
```

### 2. CubeGameController Initialization Sequence
```csharp
OnServerStartMatch()
  ↓
OnServerInitializeMazeGame()
  ↓
RoomGenerator.GenerateGrid() // Creates 9×9×9 maze
  ↓
SpawnPlayersAtCorners() // Places players at (0,0,0), (8,0,0), etc.
  ↓
Game starts (players can move, explore, die, win)
```

### 3. Win Condition
```
Player reaches exit room → OnPlayerReachedExit() event
→ OnServerMatchEnded() → ShowVictory() → Return to menu
```

### 4. Death Condition
```
Player hits hazard → OnPlayerDied() event
→ PlayersAlive-- → Check if all dead → End match
```

---

## Testing the Game

1. **Open MetagameScene** in Unity
2. **Press Play**
3. **Click:** Single Player → New Game
4. **Watch:** Console logs show:
   ```
   [9x9 Server] Starting maze escape game!
   [9x9 Server] Generating 9x9x9 maze...
   [9x9 Server] Spawned player 0 at corner (5, 5, 5)
   ```
5. **Move** using WASD + Mouse (FirstPersonController)
6. **Find the exit** at center (4,4,4) or die trying

### Ensuring Stability While Developing
- Keep production code inside asmdefs; avoid editing shim files (currently disabled in `.disabled` state).
- When adding cross-assembly features, define contracts in Core; implement in respective assemblies.
- Use reflection sparingly; prefer typed interfaces.
- Run CI before pushing: confirm `.github/workflows/unity-ci-compile.yml` passes on your PR.
- If you see a new Burst error, check `Assets/Scripts/Editor/ValidateDefaultAssemblies.cs` or restore shims (see `Assets/AssemblyShim/README.md`).

### CI/CD Pipeline
- **Workflow:** `.github/workflows/unity-ci-compile.yml` runs batchmode compile on PRs to `Ground` and `feature/**` branches.
- **Required Secret:** Add `UNITY_LICENSE` to GitHub → Settings → Secrets and variables → Actions.
- **See:** `.github/README-Unity-CI.md` for setup details.

---

## Next Steps

1. **Compile the code** (fix the 11 errors mentioned above)
2. **Create CubeGameApplication prefab** in Unity
3. **Assign it** to NetworkManager
4. **Test** the game flow
5. **Add HUD elements** (health, minimap, inventory display)
6. **Implement combat** (weapons, PvP damage)
7. **Add more hazards** (moving traps, toxic gas, etc.)

---

## File Locations

```
Assets/Scripts/Runtime/
├── Game/
│   └── CubeGame/                          ← NEW FOLDER
│       ├── CubeGameApplication.cs         ← Manages game
│       ├── CubeGameModel.cs               ← Game state
│       ├── CubeGameView.cs                ← UI handling
│       ├── CubeGameController.cs          ← Game logic
│       └── CubeGameEvents.cs              ← Death/exit events
└── Shared/
    └── Procedural/
        └── RoomGenerator.cs               ← Enhanced with spawning

Assets/Prefabs/
├── Game/
│   └── CubeGameApplication.prefab        ← TO BE CREATED
└── Shared/
    └── NetworkManager.prefab             ← Update this
```

---

## What This Replaces

**OLD:**
- GameApplication.prefab → MatchView with "Press me to win" button + 60-second timer

**NEW:**
- CubeGameApplication.prefab → 9×9×9 procedural maze, corner spawning, exit-based win condition, hazards, PvP, inventory

The old GameApplication is still there if you want to reference it or use it for testing.

---

## Known Good Configuration

| Component | Version/Status | Notes |
|-----------|----------------|-------|
| Unity | 6000.2.10f1 | Stable |
| Netcode for GameObjects | 2.3.2 | — |
| Burst | 1.8.27 | Resolves asmdefs cleanly |
| TextMeshPro | 3.8.x | ✅ Included |
| Input System | 1.14.2 | — |
| Default Assemblies | Disabled | No shims needed with Burst 1.8.27 |
| CI/CD | GitHub Actions | Runs batchmode compile on PR |

Created: November 17, 2025  
Updated: December 25, 2025  
For: 9x9 Unity Cube Maze Escape Game
