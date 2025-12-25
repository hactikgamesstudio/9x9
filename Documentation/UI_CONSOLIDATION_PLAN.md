# UI/Menu File Consolidation Audit & Plan

**Date**: December 12, 2025  
**Purpose**: Organize scattered menu/UI scripts into cohesive folder structure  
**Status**: Planning phase

---

## Current State: File Scatter

### 🔴 **Root Scripts** (Assets/Scripts/)
**Problem**: Menu/UI scripts at project root level

Located in `Assets/Scripts/`:
- `MainMenuPanel.cs` — main menu panel logic
- `MenuButton.cs` — reusable button component
- `MenuManager.cs` — menu state manager
- `SinglePlayerMenuPanel.cs` — singleplayer UI
- `UIMenuPanel.cs` — base UI panel class
- `NewGamePanel.cs` — new game setup UI

**Issue**: Mixed with core runtime structure; hard to find menu-specific code.

---

### 🟡 **Metagame Views/Controllers** (Assets/Scripts/Runtime/Metagame/)
**Status**: Organized but separate from root menu scripts

Structure:
```
Runtime/Metagame/
├── Views/
│   ├── MainMenuView.cs      ← NEW (uGUI version)
│   ├── MatchmakerView.cs
│   ├── LoadingScreenView.cs
│   └── MetagameView.cs
├── Controllers/
│   ├── MainMenuController.cs
│   ├── MatchmakerController.cs
│   └── MetagameController.cs
├── Models/
│   └── MetagameModel.cs
├── MetagameApplication.cs
└── MetagameEvents.cs
```

**Issue**: MainMenuView (uGUI) is here, but root Menu*.cs files do the same things. Duplication and confusion.

---

### 🟢 **Game UI** (Assets/Scripts/Runtime/Game/UI/)
**Status**: Organized correctly

```
Game/UI/
├── HUDController.cs      ← In-game HUD (health, inventory)
└── MatchRecapView.cs     ← End-game recap screen
```

**Good**: In-game UI separate from menus. No changes needed.

---

### 🟡 **Shared UI** (Assets/Scripts/Runtime/Shared/UI/)
**Status**: Minimal, only CustomCursor

```
Shared/UI/
└── CustomCursor.cs       ← Mouse cursor styling
```

**Note**: Could be better organized if more shared UI components added.

---

### 🔴 **Prefabs: Metagame** (Assets/Prefabs/Metagame/)
**Status**: Only one prefab

```
Prefabs/Metagame/
└── MetagameApplication.prefab    ← Root app + UI hierarchy
```

**Issue**: All menu UI lives inside MetagameApplication.prefab as children. Hard to isolate individual menu prefabs.

---

### 🟡 **Scenes** (Assets/Scenes/)
**Status**: Minimal

```
Scenes/
└── MetagameScene/
    ├── MetagameScene.unity       ← Main menu scene
    └── (deprecated MenuScene.unity reference exists)
```

**Issue**: Only one active scene; no separate menu/loading screens.

---

## Problem Statement

**Current Issues:**
1. **Duplication**: MenuButton, MainMenuPanel, MenuManager in root AND MainMenuView/MainMenuController in Metagame
2. **Inconsistency**: Mix of MVC pattern (Metagame) and non-pattern (root scripts)
3. **Discoverability**: New developers don't know where menu code lives
4. **Monolithic Prefab**: MetagameApplication is god object; hard to reuse menu pieces
5. **No Clear Hierarchy**: Menu panels, buttons, managers scattered across folders

---

## Proposed Solution: Consolidation into `Assets/Scripts/Runtime/UI/`

### New Structure:

```
Assets/Scripts/Runtime/UI/
├── Menus/                           ← All menu-related code
│   ├── MainMenu/
│   │   ├── MainMenuView.cs          (from Metagame/Views)
│   │   ├── MainMenuController.cs    (from Metagame/Controllers)
│   │   ├── MainMenuPanel.cs         (from root Scripts)
│   │   ├── MenuButton.cs            (from root Scripts)
│   │   ├── MenuManager.cs           (from root Scripts)
│   │   └── NewGamePanel.cs          (from root Scripts)
│   ├── Matchmaker/
│   │   ├── MatchmakerView.cs        (from Metagame/Views)
│   │   └── MatchmakerController.cs  (from Metagame/Controllers)
│   └── Loading/
│       ├── LoadingScreenView.cs     (from Metagame/Views)
│       └── LoadingScreenController.cs (future)
├── HUD/                             ← In-game HUD (keep separate)
│   ├── HUDController.cs             (from Game/UI)
│   └── MatchRecapView.cs            (from Game/UI)
├── Shared/                          ← Reusable UI components
│   ├── CustomCursor.cs              (from Shared/UI)
│   ├── UIMenuPanel.cs               (from root Scripts)
│   └── BaseUIView.cs                (future: shared base class)
└── UIEvents.cs                      ← Centralized UI event definitions
```

---

### Key Benefits:

1. **Single Source**: All UI code in one place (`Runtime/UI/`)
2. **Clear Hierarchy**: Menus, HUD, Shared are distinct concerns
3. **MVC Consistency**: Move away from root scripts toward MVC pattern
4. **Reusability**: Menu components become prefabable and reusable
5. **Easy Navigation**: "Where is menu code?" → "In Runtime/UI/Menus/"
6. **Scalability**: New menu systems (pause screen, settings, credits) fit naturally

---

## Implementation Steps

### Phase 1: Move & Consolidate Scripts
1. Create folder structure: `Assets/Scripts/Runtime/UI/{Menus, HUD, Shared}`
2. Move root `Menu*.cs` files → `Runtime/UI/Menus/MainMenu/`
3. Move `Metagame/Views/MainMenuView.cs` → `Runtime/UI/Menus/MainMenu/`
4. Move `Metagame/Controllers/MainMenuController.cs` → `Runtime/UI/Menus/MainMenu/`
5. Consolidate `UIMenuPanel.cs` → `Runtime/UI/Shared/`
6. Move `Game/UI/*.cs` → `Runtime/UI/HUD/`
7. Move `Shared/UI/CustomCursor.cs` → `Runtime/UI/Shared/`
8. Update all namespace declarations to match new locations

### Phase 2: Create Assembly Definition
- Create `Assets/Scripts/Runtime/UI/UI.asmdef`
- Dependencies: Core, Game, UnityEngine.UI, TextMeshPro
- Separates UI compilation from game logic

### Phase 3: Reorganize Prefabs
- Rename `MetagameApplication.prefab` → `MenuRoot.prefab` (clearer purpose)
- Extract individual menu panel prefabs from hierarchy:
  - `MainMenu.prefab` (buttons, layout)
  - `Matchmaker.prefab` (queue UI)
  - `LoadingScreen.prefab` (progress bar, tips)
- Store in `Assets/Prefabs/UI/Menus/`

### Phase 4: Update References
- Update `MetagameApplication.cs` to reference new script locations
- Update `MetagameController.cs` with new namespaces
- Verify all prefab assignments in scenes
- Search for any hardcoded path references

### Phase 5: Documentation
- Update [PROJECT_STRUCTURE.md](Documentation/PROJECT_STRUCTURE.md)
- Add UI architecture guide to docs
- Document new folder layout in README

---

## Risk Assessment

### 🟢 **Low Risk** (Simple moves)
- Menu button scripts (MainMenuPanel.cs, MenuButton.cs, MenuManager.cs)
- CustomCursor.cs
- UIMenuPanel.cs

### 🟡 **Medium Risk** (Namespace updates)
- MainMenuView.cs, MainMenuController.cs
- Matchmaker/Loading views and controllers
- References in MetagameApplication, MetagameController

### 🔴 **High Risk** (Prefab wiring)
- MetagameApplication.prefab child hierarchy
- Canvas reference assignments
- Scene instantiation logic

**Mitigation**: 
- Test in editor after each move
- Verify prefab references before committing
- Use "Find References" feature liberally

---

## Rollback Strategy

If issues arise:
1. Keep git history (commits are reversible)
2. Document exact folder structure before moves
3. Test scene loading after each phase
4. Small incremental commits (move 1-2 files per commit)

---

## Timeline Estimate

- Phase 1 (Scripts): ~30 min (6-8 files)
- Phase 2 (Namespace updates): ~20 min
- Phase 3 (Testing): ~30 min
- Phase 4 (Prefabs): ~20 min
- Phase 5 (References + Docs): ~15 min

**Total**: ~2 hours

---

## Next Steps

1. Approve folder structure
2. Create `Assets/Scripts/Runtime/UI/` folder structure
3. Begin Phase 1: Move scripts incrementally
4. Test compilation after each move
5. Update documentation

---

**Prepared By**: Keith Baker  
**Ready to Proceed**: Yes, awaiting approval
