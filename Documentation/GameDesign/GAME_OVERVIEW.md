# 9x9 Game Overview

## Game Type
Multiplayer/Singleplayer survival puzzle FPS with procedural rooms and optional bots.

## Core Concept
Escape a deadly 9×9×9 cube of interconnected rooms by solving room mechanics, scavenging items, and surviving hazards and PvP/bot encounters. Paths from each corner lead to the center exit; every run is freshly generated.

## Genre
- **Primary Genre**: Survival Puzzle
- **Sub-Genre**: First-Person, Procedural Maze, Light PvP/PvE
- **Platforms**: PC (Windows/Linux), WebGL target; VR potential later

## Player Count
- **Multiplayer**: 2–8 (designed for 4–8)
- **Singleplayer**: 1 player + up to 7 bots

## Game Modes
1. **Battle Royale (default)** — Last player/bot standing wins in the 9×9×9 cube.
2. **Co-op Escape** — Players cooperate to reach the center exit; shared win/loss.
3. **Story / Solo** — Singleplayer run with bots filling other slots; narrative-light.
4. **Race to Exit (future)** — First to reach center exit wins; PvP enabled.

## Core Mechanics
- Procedural 9×9×9 grid with guaranteed corner→center paths
- Room-specific hazards (spikes, lasers) and puzzles gating progression
- Scavenging pickups (health, utility) with lightweight inventory
- First-person movement, sprint, jump; optional combat interactions
- Spawn at 8 cube corners; center room is the exit/win condition
- Bots mirror player rules; simple combat/survival behaviors

## Inspirations / Similar Games
- *Cube* (1997 film) — lethal modular rooms and tension
- *Left 4 Dead* (co-op pacing) — survive-to-extract flow
- *Apex Legends* (BR pacing) — shrinking tension via hazards/paths (lightweight)

## Unique Selling Points (USP)
1. Perfect 9×9×9 cube layout with deterministic corner→center connectivity
2. Blend of survival puzzles + PvP/PvE in compact, replayable runs
3. Singleplayer parity with bots using the same systems as multiplayer

## Target Audience
- **Age**: 13+ (teen-friendly violence, hazard themes)
- **Player Types**: Co-op squads, light competitive BR fans, roguelite/puzzle enthusiasts, solo players who want bots

## Setting / Theme
Industrial/sci-fi modular cube complex: cold metal rooms, hazard fixtures, minimal diegetic UI. Audio and lighting emphasize tension and navigation clarity.

## Technical Overview
- **Engine**: Unity 6000.2.10f1 (URP)
- **Networking**: Netcode for GameObjects; dedicated server builds supported
- **Architecture**: MVC split into Metagame (menus/matchmaking) and GameApplication (gameplay)
- **Procedural**: RoomGenerator builds the 9×9×9 grid with spawn/exit rules
- **Input**: Unity Input System; UI via uGUI (menus) and HUD
- **Services**: Unity Gaming Services (Matchmaker, Auth), Multiplayer Tools

## Development Status
**Phase**: Pre-Alpha (singleplayer/bots in progress, multiplayer framework present)  
**Version**: 0.1.0  
**Last Updated**: December 12, 2025

---

## Notes
- HUD documentation pending; add once implemented.
- NavMesh-based bot navigation is planned for procedural rooms (Milestone 2).
- VR is exploratory; not in scope for current milestones.
