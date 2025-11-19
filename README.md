# 9x9 - Cube Maze Survival Game

**A multiplayer survival puzzle game inspired by the 1997 film *Cube***

[![Unity](https://img.shields.io/badge/Unity-6000.2.10f1-black.svg)](https://unity.com/)
[![Netcode](https://img.shields.io/badge/Netcode-2.3.2-blue.svg)](https://docs-multiplayer.unity3d.com/netcode/current/about/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## 🎮 Game Overview

**9x9** is a procedurally-generated maze-escape game where players navigate through a deadly 9×9×9 cube grid filled with hazards, puzzles, and scarce resources. Compete in **Battle Royale** mode, cooperate in **Co-op** mode, or test your skills in smaller **3x3** and **5x5** arenas.

### Core Pillars
- 🧩 **Procedural Generation** - Every game features a unique 9×9×9 room layout
- 🎯 **Survival Mechanics** - Scavenge items, manage inventory, avoid deadly traps
- ⚔️ **PvP Combat** - Engage other players for resources and survival
- 🚪 **Room-Based Puzzles** - Solve environmental challenges to progress
- 👥 **Multiplayer Modes** - Battle Royale, Co-op, 3x3, 5x5, and Standard

---

## 🚀 Quick Start

### Prerequisites
- **Unity 6000.2.10f1** (LTS)
- **Visual Studio Code** with C# Dev Kit (recommended)
- **Git** (for version control)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/9x9.git
   cd 9x9
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add" → Select `9x9` folder
   - Open with Unity 6000.2.10f1

3. **Restore packages**
   - Unity will automatically restore packages from `Packages/manifest.json`
   - Wait for compilation to complete (check bottom-right progress bar)

4. **Open the main scene**
   - Navigate to `Assets/Scenes/MetagameScene.unity`
   - Press **Play** to test

### First Run

The first time you run the game:
1. **Main Menu** will appear with the "9x9" title
2. Click **Single Player** → **New Game**
3. The maze will generate (9×9×9 = 729 rooms, sparse mode ~218 rooms)
4. You'll spawn at a corner of the cube
5. Navigate to the **center room (4,4,4)** to find the exit

---

## 📚 Documentation

Comprehensive guides are available in the `Documentation/` folder:

### Setup & Configuration
- [**Quick Start Guide**](Documentation/Setup/QUICK_START.md) - Fast setup for experienced Unity developers
- [**Full Setup Guide**](Documentation/Setup/FULL_SETUP_GUIDE.md) - Complete walkthrough with troubleshooting
- [**Cube Game Setup**](Documentation/Setup/CUBE_GAME_SETUP.md) - How to configure CubeGameApplication
- [**Troubleshooting**](Documentation/Setup/TROUBLESHOOTING.md) - Common errors and solutions

### Development
- [**Build Plan**](Documentation/Development/BUILD_PLAN.md) - Build checklist and deployment guide
- [**Code Quality Report**](Documentation/Development/CODE_QUALITY_REPORT.md) - Build status and metrics
- [**API Reference**](Documentation/Development/API_REFERENCE.md) - CubeGame API documentation *(coming soon)*

### Game Design
- [**Game Overview**](Documentation/GameDesign/GAME_OVERVIEW.md) - Vision, mechanics, target audience
- [**Art Direction**](Documentation/GameDesign/ART_DIRECTION.md) - Visual style and UI design
- [**Mechanics**](Documentation/GameDesign/MECHANICS.md) - Gameplay systems and rules

### Reference Materials
- [**Original Demo Reference**](Documentation/References/README.md) - Unity template demo vs. 9x9 comparison
- [**Redundant Files Explained**](Documentation/REDUNDANT_FILES_EXPLAINED.md) - What was cleaned up and why

---

## 🎯 Game Modes

### 🏆 Battle Royale
- **Players**: 8 corners spawn (up to 8 players)
- **Objective**: Be the first to reach the center exit room
- **Time Limit**: 10 minutes
- **PvP**: Full combat enabled
- **Winner**: First player to exit OR last player alive

### 🤝 Co-op
- **Players**: 2-8 players
- **Objective**: All players must reach the exit together
- **Time Limit**: None
- **PvP**: Disabled (friendly fire off)
- **Winner**: Team victory when all players escape

### ⚡ 3x3 Mode
- **Grid Size**: 3×3×3 = 27 rooms
- **Players**: 2-4
- **Objective**: Fast-paced exit race
- **Time Limit**: 5 minutes
- **Winner**: First to exit

### 🎲 5x5 Mode
- **Grid Size**: 5×5×5 = 125 rooms
- **Players**: 2-6
- **Objective**: Medium-difficulty maze
- **Time Limit**: 8 minutes
- **Winner**: First to exit

### 🔧 Standard Mode
- **Grid Size**: Full 9×9×9
- **Players**: 1-8
- **Objective**: Practice and exploration
- **Time Limit**: None
- **PvP**: Optional

---

## 🏗️ Project Structure

```
9x9/
├── Assets/
│   ├── Scenes/
│   │   └── MetagameScene.unity          # Main menu and game loader
│   ├── Scripts/
│   │   ├── Runtime/
│   │   │   ├── Game/
│   │   │   │   ├── CubeGame/            # 9x9 game application
│   │   │   │   ├── Player/              # FirstPersonController
│   │   │   │   ├── Items/               # PickupItem
│   │   │   │   ├── Hazards/             # Spikes, Lasers
│   │   │   │   └── Rooms/               # DoorController, RoomData
│   │   │   ├── Metagame/                # Main menu, matchmaking
│   │   │   └── Shared/
│   │   │       ├── Procedural/          # RoomGenerator (9×9×9)
│   │   │       ├── Systems/             # InventorySystem, PlayerProfileManager
│   │   │       └── Data/                # PlayerProfile, GameMode
│   │   ├── Editor/                      # Build tools
│   │   └── Shared/                      # JSON utilities
│   ├── Prefabs/
│   │   ├── Game/                        # CubeGameApplication, Player
│   │   ├── Rooms/                       # Room templates
│   │   └── Shared/                      # NetworkManager
│   └── UIToolkit/                       # UXML/USS UI files
├── Documentation/                       # All project documentation
└── Packages/                            # Unity packages
```

---

## 🛠️ Technology Stack

### Core
- **Unity 6000.2.10f1** - Game engine (LTS release)
- **C# / .NET Standard 2.1** - Programming language
- **Universal Render Pipeline (URP)** - Graphics pipeline

### Networking
- **Netcode for GameObjects 2.3.2** - Unity's official multiplayer framework
- **Unity Transport 2.4.0** - Low-level networking layer
- **Unity Gaming Services** - Matchmaking, relay, authentication

### UI
- **UI Toolkit** - Modern declarative UI (UXML/USS)
- **TextMeshPro** - Advanced text rendering

### Input
- **Unity Input System 1.14.2** - New input system with gamepad support

---

## 🎨 Key Features

### ✨ Procedural 9×9×9 Maze Generation
- **729 total room positions** in perfect cube grid
- **Sparse generation** (30% density ≈ 218 rooms) for performance
- **Guaranteed pathfinding** from all 8 corners to center exit
- **Room rotation** - Rooms rotate 90° after player exits for disorientation
- **Hazard distribution** - Spikes, lasers, false doors

### 🎮 First-Person Controller
- **WASD movement** with sprint (Shift)
- **Mouse look** with adjustable sensitivity
- **Jump mechanics** with gravity
- **Health system** (0-100 HP)
- **Damage feedback** and death handling

### 📦 Inventory System
- **5 hotbar slots** for quick access
- **Item types**: Health potions, keys, weapons, tools
- **Pickup mechanics** with visual/audio feedback
- **Persistent storage** via PlayerProfile

### 👤 Player Profile System
- **Local JSON storage** (future: cloud sync)
- **Stats tracking**: Wins, kills, deaths, games played
- **Friends list** (planned: multiplayer integration)
- **Save/Continue** functionality

---

## 🏃 Running the Game

### Play Mode (Unity Editor)
1. Open `Assets/Scenes/MetagameScene.unity`
2. Press **Play** (Ctrl+P)
3. Navigate main menu:
   - **Single Player** → New Game (instant start)
   - **Multiplayer** → Battle Royale (requires matchmaking)
   - **Profile** → View stats and friends list

### Build & Run
```powershell
# Build client (Windows Standalone)
dotnet build 9x9.slnx -c Release

# Build dedicated server (Linux)
# See Documentation/Development/BUILD_PLAN.md
```

---

## 🐛 Troubleshooting

### Common Issues

**"StartupConfiguration.json not found"**
- **Fix**: ConfigurationManager auto-creates this file on first run
- **Manual fix**: Copy from `Assets/Resources/DefaultConfigurations/StartupConfiguration.json` to project root

**Black screen after "Single Player"**
- **Cause**: CubeGameApplication not assigned in NetworkManager
- **Fix**: Open `Assets/Prefabs/Shared/NetworkManager.prefab` → Set "Game App Prefab" to `CubeGameApplication`

**"Room templates not assigned"**
- **Fix**: Open RoomGenerator in Inspector → Assign room prefabs to "Room Templates" array

See [**Troubleshooting Guide**](Documentation/Setup/TROUBLESHOOTING.md) for complete list.

---

## 🤝 Contributing

This project is currently in active development. Contributions, bug reports, and feature requests are welcome!

### How to Contribute
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style
- Follow Unity C# coding conventions
- Use `PascalCase` for public members
- Use `camelCase` for private fields with `m_` prefix
- Add XML documentation comments for public APIs

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Unity Technologies** - Multiplayer Netcode for GameObjects template
- ***Cube* (1997)** - Film inspiration for cube grid concept
- **Godot Engine** - Original prototype development

---

## 📧 Contact

**Project Maintainer**: [Your Name]  
**Email**: [your.email@example.com]  
**Discord**: [Your Discord Server]  
**Twitter**: [@YourHandle]

---

## 🗺️ Roadmap

### ✅ Completed (November 2025)
- [x] Port from Godot to Unity
- [x] Implement 9×9×9 procedural generation
- [x] Create CubeGame application system
- [x] Build main menu with profile system
- [x] First-person controller with health
- [x] Inventory system
- [x] Hazards (spikes, lasers)
- [x] Room rotation mechanics

### 🚧 In Progress
- [ ] Create CubeGameApplication prefab in Unity
- [ ] Implement HUD (health bar, inventory display, minimap)
- [ ] Test multiplayer modes (Battle Royale, Co-op)
- [ ] Room prefab templates (Standard, Hazard, Puzzle, Exit, Spawn)

### 🔮 Planned Features
- [ ] VR support (Meta Quest)
- [ ] WebGL build for browser play
- [ ] Steam integration (achievements, leaderboards)
- [ ] Procedural puzzle generation
- [ ] Weapon system (melee/ranged)
- [ ] AI pathfinding for bot opponents
- [ ] Spectator mode
- [ ] Replay system
- [ ] Level editor (custom room creation)
- [ ] Mod support

---

**Built with ❤️ and Unity 6**

*Escape the cube. Survive the maze. Win the game.*
