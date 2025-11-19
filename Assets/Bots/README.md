# Bots System

## Overview
The Bots system provides AI-controlled players for singleplayer mode in the 9x9 Cube Maze Game. Bots mimic player behavior including movement, combat, puzzle solving, and item interaction.

## Architecture

### BotController.cs
**Purpose:** Individual bot AI controller  
**Namespace:** `Unity.Template.Multiplayer.NGO.Bots`

**Key Features:**
- Autonomous movement and pathfinding
- Player detection and pursuit within range
- Combat with configurable damage and cooldown
- Stuck detection and recovery
- Health management and death handling

**Configuration:**
- `Bot Name` - Display name for the bot
- `Bot ID` - Unique identifier
- `Move Speed` - Base movement speed (default: 3)
- `Rotation Speed` - How fast bot turns (default: 5)
- `Attack Range` - Distance at which bot can attack (default: 2)
- `Detection Range` - Distance at which bot detects players (default: 10)
- `Damage` - Damage per attack (default: 10)
- `Attack Cooldown` - Time between attacks (default: 1s)

**Behavior:**
1. **Idle/Roaming:** Moves to random waypoints when no player detected
2. **Detection:** Scans for players within detection range
3. **Pursuit:** Follows detected player, updating path periodically
4. **Combat:** Attacks player when in attack range, respects cooldown
5. **Stuck Recovery:** Detects if stuck and finds new path

**Public Methods:**
```csharp
void Initialize(string botName, int botID, Vector3 spawnPosition)
void TakeDamage(int damage)
```

**Properties:**
```csharp
string BotName { get; }
int BotID { get; }
int Health { get; }
bool IsDead { get; }
```

### BotManager.cs
**Purpose:** Manages bot lifecycle and spawning  
**Namespace:** `Unity.Template.Multiplayer.NGO.Bots`

**Key Features:**
- Spawns bots at corner spawn positions (8 corners of 9×9×9 cube)
- Tracks active bots and their state
- Handles bot death events
- Provides win condition checking (all bots eliminated)

**Configuration:**
- `Bot Prefab` - Prefab with BotController component
- `Max Bots` - Maximum number of bots (default: 7, for 8 total players)
- `Spawn Bots At Start` - Auto-spawn on Start()
- `Bot Name Prefix` - Prefix for bot names (default: "Bot")

**Public Methods:**
```csharp
void SpawnBots() // Spawn all bots at corner positions
BotController SpawnBot(Vector3 position) // Spawn single bot
void OnBotDied(BotController bot) // Called when bot dies
void ClearAllBots() // Remove all bots
BotController GetBotByID(int botID) // Get bot by ID
```

**Properties:**
```csharp
List<BotController> ActiveBots { get; }
int ActiveBotCount { get; }
int MaxBots { get; }
```

## Usage

### Setup in Unity Editor

1. **Create Bot Prefab:**
   - Duplicate Player prefab
   - Rename to "BotPlayer"
   - Remove `FirstPersonController` component (if present)
   - Add `BotController` component
   - Ensure prefab has `CharacterController` component
   - Configure bot settings in Inspector

2. **Add BotManager to Scene:**
   - Create empty GameObject, rename to "BotManager"
   - Add `BotManager` component
   - Assign Bot Prefab to `m_BotPrefab` field
   - Set `Max Bots` (default 7 for 8 total players)
   - Enable `Spawn Bots At Start` for automatic spawning

3. **Room Generator Integration:**
   - BotManager automatically finds `RoomGenerator` in scene
   - Uses `RoomGenerator.GetSpawnPositions()` for corner spawns
   - Falls back to hardcoded positions if RoomGenerator not found

### Code Integration

**Singleplayer Mode Detection:**
```csharp
// In CubeGameController or similar
if (IsSingleplayerMode())
{
    // Spawn BotManager
    GameObject botManagerObj = new GameObject("BotManager");
    BotManager botManager = botManagerObj.AddComponent<BotManager>();
    botManager.SpawnBots();
}
```

**Manual Bot Spawning:**
```csharp
BotManager botManager = FindObjectOfType<BotManager>();
Vector3 spawnPos = new Vector3(10, 1, 10);
BotController bot = botManager.SpawnBot(spawnPos);
```

**Listening for Bot Events:**
```csharp
// In game controller
BotManager botManager = FindObjectOfType<BotManager>();
if (botManager.ActiveBotCount == 0)
{
    Debug.Log("Player wins! All bots eliminated.");
}
```

## AI Behavior Details

### Pathfinding
- **Current Implementation:** Simple direct-line movement to waypoints
- **Stuck Detection:** Checks if bot moved less than threshold in 2 seconds
- **Recovery:** Sets new random target when stuck
- **Future:** Integrate Unity NavMesh for advanced pathfinding

### Combat
- **Target Selection:** Nearest player within detection range
- **Attack Pattern:** Melee attacks when in attack range
- **Cooldown System:** Prevents spam attacks
- **Damage Handling:** Supports taking damage from player/hazards

### Movement
- **Speed:** Configurable move speed (default slower than player)
- **Rotation:** Smooth rotation towards target
- **Gravity:** Applied via CharacterController
- **Ground Detection:** Uses CharacterController.isGrounded

## Game Mode Support

### Battle Royale
- Spawn 7 bots at corner positions
- Last player/bot standing wins
- Bots can fight each other (future feature)

### Cooperative
- Spawn bots as allies (future feature)
- Bots help player reach exit
- Shared win condition

### Standard Maze Escape
- Bots race to exit room
- First to reach exit wins

## Future Enhancements

### Planned Features
1. **NavMesh Integration:** Use Unity NavMesh for advanced pathfinding
2. **Difficulty Levels:** Easy/Medium/Hard bot AI
3. **Item Pickup:** Bots collect health potions, weapons
4. **Puzzle Solving:** Bots interact with switches, doors
5. **Team Behavior:** Bots form alliances or factions
6. **Personality Traits:** Aggressive, defensive, explorer archetypes
7. **Animation:** Walk, run, attack, death animations
8. **Voice/Sound:** Audio cues for bot actions

### Known Limitations
- No NavMesh pathfinding (direct-line movement only)
- No item/pickup interaction yet
- No puzzle solving capability
- No bot-vs-bot combat
- Basic stuck detection (no advanced obstacle avoidance)

## Testing

### Test Scenarios
1. **Basic Spawning:** 7 bots spawn at corners without errors
2. **Movement:** Bots move to random waypoints and pursue player
3. **Combat:** Bot attacks player when in range, deals damage
4. **Death:** Bot dies when health reaches 0, triggers OnBotDied
5. **Win Condition:** Game detects when all bots eliminated
6. **Stuck Recovery:** Bot escapes when stuck behind obstacle

### Debug Tools
- **Gizmos:** BotController draws detection/attack ranges in Scene view
- **Logs:** BotManager logs spawn, death, and win events
- **Inspector:** Watch bot health, target position, state in real-time

## Dependencies

### Required Scripts
- `Unity.Template.Multiplayer.NGO.Game.Player.FirstPersonController` - For player targeting
- `Unity.Template.Multiplayer.NGO.Core.Procedural.RoomGenerator` - For spawn positions

### Unity Components
- `CharacterController` - Required on bot prefab
- `Transform` - Standard Unity component

### Assembly References
- `Game.asmdef` - For FirstPersonController access
- `Core.asmdef` - For RoomGenerator access
- `Unity.Netcode.Runtime` - For future multiplayer bot sync
- `Unity.InputSystem` - Future input simulation

---

**Created:** November 18, 2025  
**Namespace:** `Unity.Template.Multiplayer.NGO.Bots`  
**Status:** Initial implementation for singleplayer mode
