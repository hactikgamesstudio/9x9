# Development Roadmap & Milestones

## Project Vision

Transform the 9x9 Cube Maze Game into a complete multiplayer survival puzzle experience with full singleplayer support, procedural generation, and engaging combat/puzzle mechanics.

**Target Platforms:** PC (Windows/Linux), WebGL, Potential VR  
**Current Unity Version:** 6000.2.10f1  
**Current Status:** Pre-Alpha - Core systems in development

---

## Current Status (November 18, 2025)

### ✅ Completed Features

**Core Systems:**
- Procedural 9×9×9 cube grid room generation
- Player controller with first-person movement
- Inventory system (basic)
- Hazards (spikes, lasers) with damage
- Pickup items system
- Room data and door controllers
- HUD with health display

**Multiplayer Framework:**
- Unity Netcode for GameObjects integration
- Unity Gaming Services (UGS) authentication
- Matchmaker integration (requires configuration)
- Network manager and connection approval
- Dedicated server build support

**Project Infrastructure:**
- Git repository initialized
- Organized folder structure (Game/Core/Bots)
- Assembly definitions (Game, Core, Bots)
- Comprehensive documentation

**Singleplayer Features (NEW):**
- Bot AI controller with movement, combat, pathfinding
- Bot manager for spawning and lifecycle
- 8-corner spawn system matching multiplayer

### 🚧 In Progress

- Singleplayer mode implementation
- Bot system integration with game controllers
- Unity project compilation testing (reorganization)

### ❌ Not Started

- Singleplayer UI menu
- NavMesh pathfinding for bots
- Bot item pickup and puzzle solving
- Multiple game modes fully implemented
- Win/loss condition screens
- VR support

---

## Milestone 1: Singleplayer Foundation 🎯 CURRENT

**Target:** December 1, 2025  
**Goal:** Fully functional singleplayer mode with bot AI

### Tasks

#### Week 1 (Nov 18-24): Core Integration
- [x] Project reorganization (Game/Core/Bots structure)
- [x] Bot AI implementation (BotController, BotManager)
- [ ] Unity compilation testing and fixes
- [ ] Namespace updates for moved files
- [ ] Create Bot prefab from Player prefab
- [ ] CubeGameController refactor for singleplayer mode
  - [ ] Add IsSingleplayerMode() detection
  - [ ] Disable NetworkManager in singleplayer
  - [ ] Integrate BotManager spawning
  - [ ] Handle local-only game state

#### Week 2 (Nov 25-Dec 1): UI & Game Modes
- [ ] Singleplayer menu UI
  - [ ] "Singleplayer" button on main menu
  - [ ] Game mode selection (Battle Royale, Coop, Standard)
  - [ ] Bot count configuration
- [ ] Bot-hazard integration
  - [ ] Bots take damage from spikes
  - [ ] Bots take damage from lasers
  - [ ] Bots avoid hazards (basic pathfinding)
- [ ] Win/loss conditions
  - [ ] Player wins when all bots eliminated
  - [ ] Player loses when health reaches 0
  - [ ] End-game screen with stats
- [ ] Testing and polish
  - [ ] All game modes work with bots
  - [ ] No network errors in singleplayer
  - [ ] Performance acceptable with 7 bots

**Deliverables:**
- Playable singleplayer mode
- Battle Royale mode (player vs 7 bots)
- Basic win/loss screens
- No multiplayer dependencies in singleplayer

**Success Criteria:**
- Player can start singleplayer from menu
- 7 bots spawn and behave correctly
- Combat works (player vs bots)
- Win/loss conditions trigger correctly
- No compilation errors or warnings

---

## Milestone 2: Enhanced AI & Polish

**Target:** December 15, 2025  
**Goal:** Improved bot AI, NavMesh pathfinding, item interaction

### Tasks

#### NavMesh Integration
- [ ] Research dynamic NavMesh baking for procedural rooms
- [ ] Implement NavMesh generation in RoomGenerator
- [ ] Update BotController to use NavMeshAgent
- [ ] Handle dynamic obstacles (doors, hazards)
- [ ] Performance optimization for 9×9×9 grid

#### Bot Behaviors
- [ ] Item pickup system for bots
  - [ ] Detect nearby items
  - [ ] Pick up health potions
  - [ ] Use items strategically (low health → heal)
- [ ] Puzzle solving
  - [ ] Detect interactive objects (switches, levers)
  - [ ] Solve simple puzzles (open doors)
  - [ ] Pathfind to exit room
- [ ] Combat improvements
  - [ ] Dodge attacks
  - [ ] Use cover
  - [ ] Retreat when low health

#### Difficulty Levels
- [ ] Easy AI
  - [ ] Slower movement (50% speed)
  - [ ] Lower damage (5 per hit)
  - [ ] Longer attack cooldown (2s)
  - [ ] Reduced detection range (5 units)
- [ ] Medium AI (current implementation)
- [ ] Hard AI
  - [ ] Faster movement (150% speed)
  - [ ] Higher damage (20 per hit)
  - [ ] Shorter attack cooldown (0.5s)
  - [ ] Increased detection range (20 units)
  - [ ] Aggressive pursuit

**Deliverables:**
- NavMesh-based pathfinding
- Bots interact with items and puzzles
- 3 difficulty levels
- Improved combat AI

**Success Criteria:**
- Bots navigate complex room layouts
- Bots solve puzzles and reach exit
- Difficulty levels feel distinct
- Performance remains acceptable

---

## Milestone 3: Multiplayer Configuration

**Target:** January 15, 2026  
**Goal:** Working multiplayer with matchmaking

### Tasks

#### Unity Gaming Services Setup
- [ ] Create queue configuration in UGS dashboard
- [ ] Configure environment IDs
- [ ] Test matchmaking flow
- [ ] Handle connection errors gracefully

#### Multiplayer Testing
- [ ] Test 2-player matches
- [ ] Test 4-player matches
- [ ] Test 8-player matches (full lobby)
- [ ] Dedicated server builds
- [ ] Client/server synchronization

#### Game Modes Implementation
- [ ] Battle Royale (last player standing)
- [ ] Cooperative (all players vs environment)
- [ ] Team Deathmatch (4v4)
- [ ] Race to Exit (first to exit wins)

#### Networking Optimization
- [ ] Reduce network bandwidth
- [ ] Optimize NetworkVariable usage
- [ ] Client-side prediction for movement
- [ ] Lag compensation

**Deliverables:**
- Functional multiplayer matchmaking
- 4 game modes playable online
- Stable 8-player matches
- Dedicated server support

**Success Criteria:**
- Players can find matches via matchmaker
- All game modes work in multiplayer
- No critical network bugs
- Acceptable latency (<100ms for good connections)

---

## Milestone 4: Content Expansion

**Target:** February 28, 2026  
**Goal:** More rooms, hazards, items, puzzles

### Tasks

#### Room Variants
- [ ] 20+ unique room layouts
- [ ] Themed room sets (lab, dungeon, sci-fi)
- [ ] Special rooms (treasure, trap, boss)
- [ ] Room rarity system (common, rare, legendary)

#### Hazards
- [ ] Moving platforms
- [ ] Rotating blades
- [ ] Poison gas clouds
- [ ] Electrical fields
- [ ] Collapsing floors

#### Items & Weapons
- [ ] Health potions (small, medium, large)
- [ ] Armor/shields
- [ ] Speed boosts
- [ ] Melee weapons (sword, axe)
- [ ] Ranged weapons (bow, gun)
- [ ] Grenades/explosives

#### Puzzles
- [ ] Pressure plates
- [ ] Color-coded switches
- [ ] Pattern recognition
- [ ] Timed challenges
- [ ] Multi-step puzzles

**Deliverables:**
- 20+ room prefabs
- 5+ new hazard types
- 10+ items/weapons
- 5+ puzzle types

**Success Criteria:**
- Rooms feel varied and interesting
- Items add strategic depth
- Puzzles are solvable but challenging
- Content feels balanced

---

## Milestone 5: Polish & Release Prep

**Target:** March 31, 2026  
**Goal:** Beta-ready build

### Tasks

#### Visual Polish
- [ ] Art pass on all rooms
- [ ] Particle effects (explosions, pickups, death)
- [ ] UI/UX improvements
- [ ] Main menu polish
- [ ] End-game screens

#### Audio
- [ ] Background music
- [ ] Sound effects (footsteps, combat, doors, pickups)
- [ ] Ambient sounds (room atmosphere)
- [ ] Voice lines for bots (optional)

#### Gameplay Balance
- [ ] Weapon damage tuning
- [ ] Health/armor values
- [ ] Bot AI difficulty balancing
- [ ] Room generation fairness
- [ ] Match duration optimization

#### Performance Optimization
- [ ] Profiling and bottleneck identification
- [ ] Object pooling for frequent spawns
- [ ] Texture/mesh optimization
- [ ] Network bandwidth reduction
- [ ] Memory usage optimization

#### Testing
- [ ] Playtest sessions (10+ people)
- [ ] Bug fixing
- [ ] Balancing adjustments
- [ ] Performance testing (low-end PCs)

**Deliverables:**
- Polished visuals and audio
- Balanced gameplay
- Optimized performance
- Minimal bugs

**Success Criteria:**
- Game looks and sounds professional
- Runs at 60+ FPS on target hardware
- Playtesters report positive experience
- No critical bugs

---

## Milestone 6: Steam Release (Beta)

**Target:** April 30, 2026  
**Goal:** Early Access on Steam

### Tasks

#### Steam Integration
- [ ] Steam SDK integration
- [ ] Achievements
- [ ] Leaderboards
- [ ] Cloud saves
- [ ] Workshop support (custom rooms)

#### Release Preparation
- [ ] Store page creation
- [ ] Trailer video
- [ ] Screenshots
- [ ] Marketing materials
- [ ] Press kit

#### Post-Launch Support
- [ ] Community feedback monitoring
- [ ] Patch releases
- [ ] Content updates
- [ ] Bug fixes

**Deliverables:**
- Steam Early Access build
- Store page live
- Marketing campaign

**Success Criteria:**
- Successful Steam launch
- Positive initial reviews
- Active player base
- Clear roadmap communicated

---

## Future Considerations (Post-Launch)

### VR Support
- [ ] VR controller mapping
- [ ] Comfort options (teleport, smooth locomotion)
- [ ] VR-specific UI
- [ ] Performance optimization for VR

### Mobile Port
- [ ] Touch controls
- [ ] Simplified graphics for mobile
- [ ] Cloud save sync
- [ ] Cross-platform play

### Advanced Features
- [ ] Procedural room generation (runtime)
- [ ] User-generated content (room editor)
- [ ] Ranked matchmaking
- [ ] Seasonal events
- [ ] Battle pass / progression system

### Community Features
- [ ] Clan/guild system
- [ ] Friend lists
- [ ] Private matches
- [ ] Spectator mode
- [ ] Replay system

---

## Risk Management

### Technical Risks

**Risk:** NavMesh baking too slow for 9×9×9 grid  
**Mitigation:** Pre-bake common layouts, use simpler pathfinding, async baking

**Risk:** Network synchronization issues at 8 players  
**Mitigation:** Server-authoritative design, client prediction, bandwidth optimization

**Risk:** Performance issues with 7 AI bots + player  
**Mitigation:** Optimize bot update frequency, use object pooling, profile early

### Scope Risks

**Risk:** Feature creep delaying milestones  
**Mitigation:** Strict milestone focus, defer non-critical features

**Risk:** Underestimating multiplayer complexity  
**Mitigation:** Early multiplayer testing, dedicated server focus

### Resource Risks

**Risk:** Solo development bandwidth  
**Mitigation:** Use existing assets, focus on core loop first, community feedback

---

## Success Metrics

### Milestone 1 (Singleplayer)
- Player retention >30 minutes per session
- Bot AI feels challenging but fair
- No critical bugs

### Milestone 2 (Enhanced AI)
- Players prefer Hard difficulty bots
- NavMesh pathfinding feels natural
- Bots solve puzzles >80% success rate

### Milestone 3 (Multiplayer)
- Average matchmaking time <2 minutes
- Match completion rate >90%
- Multiplayer retention >60 minutes per session

### Milestone 6 (Launch)
- 1,000+ wishlists before launch
- 75%+ positive Steam reviews
- 100+ concurrent players in first week

---

## Development Principles

1. **Prototype First:** Get core loop working before polish
2. **Iterate Often:** Weekly playtests, rapid iteration
3. **Player Feedback:** Community-driven development
4. **Performance Matters:** 60 FPS minimum target
5. **Multiplayer-Ready:** Design systems for both SP and MP
6. **Modular Design:** Easy to add/remove features
7. **Documentation:** Keep docs updated with code
8. **Version Control:** Commit early, commit often

---

**Document Created:** November 18, 2025  
**Last Updated:** November 18, 2025  
**Status:** Living document - update after each milestone  
**Owner:** Development Team
