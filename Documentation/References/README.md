# Reference Materials - 9x9 Project

This folder contains reference implementations and documentation from the original Unity Multiplayer NGO template. These files are preserved for learning purposes and architectural reference.

## Folder Structure

### OriginalDemo/
Contains the original "Press to Win" demo game files from the Unity template. These files demonstrate the MVC pattern and multiplayer architecture but are **not used in the 9x9 cube maze game**.

**Purpose**: Reference material for understanding Unity Netcode patterns, MVC architecture, and multiplayer event systems.

**Files**:
- `GameApplication.cs` - Original demo application (replaced by CubeGameApplication)
- `GameController.cs` - Original demo controller (replaced by CubeGameController)
- `MatchController.cs` - Original match controller (replaced by CubeGameController)
- `MatchRecapController.cs` - Original recap controller (reusable)
- `MatchView.cs` - Original match view (replaced by CubeGameView)
- `MatchDataSynchronizer.cs` - Original NetworkBehaviour example (replaced by MazeDataSynchronizer)
- `Bot.prefab` - AI bot prefab from demo (not used in 9x9)
- `GameApplication.prefab` - Original demo prefab (replaced by CubeGameApplication.prefab)
- `MatchDataSynchronizer.prefab` - Original synchronizer (replaced)

## What's Currently Active in 9x9

### Active Game Systems
The 9x9 cube maze game uses these files instead:

- `CubeGameApplication.cs` - Main game application
- `CubeGameController.cs` - Maze generation, corner spawning, win/loss logic
- `CubeGameModel.cs` - Maze game state management
- `CubeGameView.cs` - In-game UI (HUD, victory/defeat screens)
- `CubeGameEvents.cs` - PlayerReachedExitEvent, PlayerDiedEvent, MazeDataSynchronizer
- `RoomGenerator.cs` - 9×9×9 procedural maze generator
- `FirstPersonController.cs` - Player movement and health
- `InventorySystem.cs` - Item collection and storage

### Shared Systems (Still Used)
These components work for both demo and 9x9:

- `Player.cs` - Network player object (enhanced with IsAlive, Kills, Deaths)
- `MetagameApplication.cs` - Main menu and matchmaking
- `CustomNetworkManager.cs` - Network bootstrapper
- `MatchRecapView.cs` - End-game recap screen (reusable)
- `MatchRecapController.cs` - Recap logic (reusable)

## How to Use These References

### Learning MVC Pattern
Study `GameApplication.cs`, `GameController.cs`, `GameModel.cs`, `GameView.cs` to understand:
- BaseApplication<TModel, TView, TController> inheritance
- Event-driven architecture (AppEvent system)
- Server/Client separation logic
- NetworkBehaviour integration

### Comparing Implementations
Compare original demo files with CubeGame equivalents:

| Original Demo | 9x9 Replacement | Key Differences |
|---------------|-----------------|-----------------|
| GameApplication.cs | CubeGameApplication.cs | Identical structure, just renamed |
| GameController.cs | CubeGameController.cs | Added maze generation, corner spawning, multiple game modes |
| MatchDataSynchronizer.cs | MazeDataSynchronizer.cs | Tracks maze generation instead of countdown |
| MatchView.cs | CubeGameView.cs | Added HUD integration, victory/defeat screens |
| GameModel.cs | CubeGameModel.cs | Added PlayersAlive, CurrentGameMode, MazeGenerated flags |

### Reusing Code Patterns
Useful patterns to extract from demo files:

1. **Countdown Timer** (GameController.cs):
   ```csharp
   IEnumerator CountdownRoutine(int seconds) {
       while (seconds > 0) {
           yield return new WaitForSeconds(1f);
           seconds--;
       }
   }
   ```

2. **Player Disconnection Handling** (GameController.cs):
   ```csharp
   void OnServerPlayerDisconnected(PlayerDisconnected evt) {
       if (!Model.AllowReconnection) {
           // End match if no reconnection
       }
   }
   ```

3. **Winner Determination** (GameController.cs):
   ```csharp
   Player winner = firstClientStillConnected?.PlayerObject.GetComponent<Player>();
   ```

4. **NetworkVariable Callbacks** (MatchDataSynchronizer.cs):
   ```csharp
   m_Countdown.OnValueChanged += OnCountdownChanged;
   ```

## Files NOT Needed for 9x9

These files can be deleted after moving to References:

### Delete from Active Project
- [ ] `Assets/Prefabs/Game/Bot.prefab` - AI bot not used
- [ ] `Assets/Prefabs/Game/GameApplication.prefab` - Replaced by CubeGameApplication
- [ ] `Assets/Prefabs/Game/MatchDataSynchronizer.prefab` - Replaced
- [ ] `Assets/Scenes/TestScene.unity` - Demo scene not needed

### Keep as Reference Only
- `GameApplication.cs` - Good MVC reference
- `GameController.cs` - Countdown and disconnection patterns
- `MatchController.cs` - Simple event handling example
- `MatchDataSynchronizer.cs` - NetworkVariable usage example

## When to Delete Reference Files

**DON'T DELETE** if:
- You're still learning Unity Netcode
- You want to understand MVC architecture
- You might add similar features (AI bots, countdown timers)
- Team members are new to the codebase

**SAFE TO DELETE** when:
- All team members understand the architecture
- 9x9 game is fully implemented and tested
- You've extracted all useful patterns
- Project is production-ready

---

**Last Updated**: November 17, 2025  
**Maintained By**: Development Team
