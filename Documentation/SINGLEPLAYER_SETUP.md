# Singleplayer Setup Instructions

## Progress Tracker

| Task | Status | Progress | Details |
|------|--------|----------|---------|
| Remove Network Play | ✅ Partial | 60% | `CustomNetworkManager.StartClientAsSinglePlayer()` exists; needs full bypass integration |
| Add Bots | ⚠️ Partial | 60% | **DONE**: BotController, BotManager created & integrated into CubeGameController |
| Game Mode Support | ⚠️ Partial | 50% | Bot spawning respects game modes (Coop, BattleRoyale, NewGame) |
| Setup Steps | ✅ Partial | 75% | Menu option exists; bot spawning implemented; cleanup added |
| Testing | ❌ Not Started | 0% | No test suite for singleplayer/bots |
| **Overall** | ⚠️ In Progress | **50%** | Bot system scaffolding complete; AI behavior & pathfinding next |

---

## 1. Remove Network Play in Singleplayer

**Status**: ✅ 60% Complete

**Implemented**:
- ✅ `CustomNetworkManager.StartClientAsSinglePlayer()` - bypasses matchmaking
- ✅ Singleplayer detection in `MainMenuController.OnStartSinglePlayerMode()`
- ✅ `ConfigurationManager.k_EnableBots` config key

**TODO**:
- [ ] Ensure `CubeGameController` disables network RPCs in singleplayer
- [ ] Hide multiplayer UI elements when in singleplayer
- [ ] Fully test network bypass with all game states

## 2. Add Bots for Singleplayer

**Status**: ⚠️ 60% Complete

**Implemented**:
- ✅ `BotController.cs` - AI state machine (Idle, Patrolling, Chasing, Combat, PuzzleSolving, CollectingItem)
- ✅ `BotManager.cs` - Bot spawning, lifecycle, cleanup
- ✅ Integration in `CubeGameController.OnServerInitializeMazeGame()`
- ✅ Game mode-aware bot spawning (Coop spawns fewer bots, BattleRoyale spawns max)
- ✅ Bot cleanup in `CubeGameController.OnDestroy()`

**TODO**:
- [ ] Implement pathfinding algorithm (A* or simple maze navigation)
- [ ] Add bot combat behavior (target player, dodge, attack)
- [ ] Implement puzzle-solving state
- [ ] Bot item collection logic
- [ ] Bot vs bot combat

## 3. Support All Game Modes in Singleplayer

**Status**: ⚠️ 40% Complete

**Implemented**:
- ✅ `GameMode` enum (Continue, Coop, NewGame)
- ✅ `CubeGameController` mode switching logic
- ✅ Game mode parameter passed to network initialization

**TODO**:
- [ ] Bot behavior changes per game mode (Coop = cooperative, Standard = solo)
- [ ] Coop mode: track all players reaching exit
- [ ] Battle Royale: implement bot elimination logic
- [ ] UI: Show game mode indicator in singleplayer
- [ ] Ensure scoring/progression works for all modes

## 4. Setup Steps

**Status**: ✅ 75% Complete

**Implemented**:
- ✅ Singleplayer menu option (detect in `MainMenuController`)
- ✅ Bypass matchmaking (`CustomNetworkManager.StartClientAsSinglePlayer()`)
- ✅ Network initialization bypass path
- ✅ `BotManager` created and spawns bots at startup
- ✅ Bots spawn at remaining corner positions (auto-calculated per mode)
- ✅ Game session lifecycle includes bot management (spawn → play → cleanup)

**TODO**:
- [ ] Player always spawns at specific starting corner (configurable)
- [ ] Bots spawn at remaining 7 corners (currently uses auto-positioning)
- [ ] Verify local game state initialization (no NetworkManager dependencies for bots)

## 5. Testing

**Status**: ❌ 0% Complete

**Test Cases to Implement**:
- [ ] Singleplayer mode starts without network errors
- [ ] Player and bots spawn at correct positions
- [ ] Bots navigate maze without errors
- [ ] Bots pick up items and manage inventory
- [ ] Combat works between player and bots
- [ ] Win/loss conditions trigger correctly
- [ ] UI shows bots in player list and stats
- [ ] All game modes (Standard, Coop, Continue) work with bots
- [ ] No network warnings/errors in Console

**Test Coverage**:
```
Singleplayer Tests:
├── Network Bypass
│   ├── Matchmaking disabled
│   ├── NetworkManager configured for local-only
│   └── No Netcode warnings
├── Bot System
│   ├── Spawn N bots correctly
│   ├── Bots navigate maze
│   └── Bots participate in win/loss
└── Game Modes
    ├── Standard (first to exit wins)
    ├── Coop (all must exit)
    └── Battle Royale (last bot standing)
```

---

## Implementation Priority

### Phase 1: Critical (Blocking) ✅ COMPLETE
1. ✅ **BotController.cs** - Core AI logic class
2. ✅ **BotManager.cs** - Bot lifecycle management
3. ✅ Bot spawning in `CubeGameController`

### Phase 2: Important (Next)
4. **Bot pathfinding algorithm** - A* or simple maze navigation
5. **Bot combat behavior** - target player, dodge, attack
6. **Game mode-specific bot logic** - Coop cooperation, BattleRoyale elimination

### Phase 3: Polish
7. Bot UI (name tags, health bars)
8. Bot audio/animation feedback
9. Comprehensive testing

---

**Last Updated**: December 4, 2025  
**Current Focus**: Bot system integration complete; Phase 2 ready  
**Next Action**: Implement bot pathfinding & combat behavior
