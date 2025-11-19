# Singleplayer Setup Instructions

## 1. Remove Network Play in Singleplayer

- Detect singleplayer mode (no matchmaking, no network session).
- Disable all Netcode/NetworkManager logic for singleplayer sessions.
- Ensure player spawning, game state, and win/loss logic do not depend on networked objects or RPCs.
- Remove/disable any calls to NetworkManager.Singleton, NetworkBehaviour, or network variables in singleplayer code paths.
- UI: Hide or disable multiplayer-specific UI elements (player list, network status, etc.) in singleplayer.

## 2. Add Bots for Singleplayer

- Implement a BotController class that mimics player movement, puzzle solving, and combat.
- On singleplayer start, spawn bots at the same positions as multiplayer spawns (corners, etc.).
- Bots should use the same Player prefab, but be flagged as AI-controlled.
- Bot logic should include:
	- Pathfinding through the maze
	- Item pickup and inventory management
	- Combat with player and other bots
	- Puzzle solving (if applicable)
- Ensure bots interact with hazards, pickups, and win/loss conditions like real players.

## 3. Support All Game Modes in Singleplayer

- Allow singleplayer to select any game mode (Battle Royale, Coop, Standard, etc.).
- For modes requiring multiple players (Coop, Battle Royale), fill remaining slots with bots.
- Game logic should treat bots as players for win/loss, scoring, and progression.
- UI: Show bots in player list, stats, and end-game screens.

## 4. Setup Steps

- Add a "Singleplayer" option to the main menu.
- On singleplayer start:
	- Bypass matchmaking and network initialization.
	- Spawn player and bots locally.
	- Initialize game state for selected mode.
- Refactor CubeGameController and related systems to support both networked and local-only play.
- Add a BotManager to handle bot spawning, updates, and cleanup.

## 5. Testing

- Test each game mode in singleplayer with bots:
	- Player can win/lose as normal
	- Bots behave correctly (navigate, fight, solve puzzles)
	- No network errors or warnings
- Validate UI and stats for bots and player.

---

**Next Steps:**

- Refactor CubeGameController to separate networked and local logic.
- Implement BotController and BotManager.
- Update UI and game state logic for singleplayer support.
- Add tests for singleplayer and bot behavior.
