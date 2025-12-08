# Menu Integration Documentation
**Date**: December 8, 2025  
**Branch**: feature/singleplayer-bots-simplify  
**Author**: Development Team  

## Overview
This document describes the integration of the traditional Unity UI menu system into the 9x9 multiplayer game project, replacing the original UI Toolkit-based menu.

## What Changed

### 1. Menu System Architecture
**Before**: UI Toolkit-based menu (UIElements, UXML)
- 638-line MainMenuView.cs with keyboard/gamepad navigation
- UI Toolkit elements (VisualElement, Button from UIElements)
- Input System callbacks for navigation

**After**: Traditional Unity UI menu (Canvas, uGUI)
- 64-line MainMenuView.cs wrapper
- MenuManager orchestrates panel navigation
- Traditional UI components (Canvas, Button from UnityEngine.UI)
- Event-based game start integration

### 2. Files Modified

#### **MainMenuView.cs** (Major Rewrite)
- **Location**: `Assets/Scripts/Runtime/Metagame/Views/MainMenuView.cs`
- **Before**: 638 lines, UI Toolkit implementation
- **After**: 64 lines, traditional UI wrapper
- **Purpose**: Bridges imported menu system with existing game architecture
- **Key Changes**:
  - Removed UI Toolkit dependencies (UIElements)
  - Added references to MenuManager and MenuCanvas
  - Simplified Show()/Hide() methods with `new` keyword to override base class
  - Maintained MultiplayerButton property for controller compatibility

#### **MenuManager.cs** (Enhanced)
- **Location**: `Assets/Scripts/MenuManager.cs`
- **Changes**: Added public property `MainMenuPanel` for external access
- **Purpose**: Allows MainMenuView to query button references

#### **NewGamePanel.cs** (Event Integration)
- **Location**: `Assets/Scripts/NewGamePanel.cs`
- **Changes**: Added event broadcasting in `OnStartGameClicked()`
- **Integration**: Now broadcasts `StartSinglePlayerModeEvent` with bot count
- **Event Flow**: NewGamePanel → AppEvent.Broadcast → MainMenuController

### 3. Imported Menu Components

The following files were imported as part of the new menu system:

- **MenuManager.cs**: Central menu orchestrator
- **MainMenuPanel.cs**: Main menu UI panel
- **SinglePlayerMenuPanel.cs**: Single player options panel
- **NewGamePanel.cs**: New game configuration panel (bot count, difficulty)
- **UIMenuPanel.cs**: Base class for all menu panels
- **MenuButton.cs**: Custom button component
- **MenuScene.unity**: Scene file containing the menu Canvas hierarchy

### 4. Files Removed

- **Old MainMenuView.cs**: 638-line UI Toolkit version (replaced)
- **MainMenuView_New.cs**: Temporary file during integration (renamed to MainMenuView.cs)
- **Orphaned meta files**: Cleaned up Unity metadata for deleted files

## Integration Architecture

### Event Flow
```
User clicks "Start Game" 
  ↓
NewGamePanel.OnStartGameClicked()
  ↓
AppEvent.Broadcast(StartSinglePlayerModeEvent)
  ↓
MainMenuController.OnStartSinglePlayerMode()
  ↓
CustomNetworkManager.InitializeNetworkLogic()
  ↓
Game starts with configured bot count
```

### Component Hierarchy
```
MetagameApplication (Prefab)
└── MetagameView
    └── MainMenuView (wrapper)
        └── MenuCanvas (imported)
            └── MenuManager
                ├── MainMenuPanel
                ├── SinglePlayerMenuPanel
                └── NewGamePanel
```

## Unity Editor Setup

### Required Configuration

1. **MenuCanvas GameObject**:
   - Must be assigned to MainMenuView.menuCanvas field
   - Contains all UI elements for the menu system

2. **MenuManager Component**:
   - Must be assigned to MainMenuView.menuManager field
   - Manages panel switching and navigation

3. **Panel References** (in MenuManager):
   - MainMenuPanel → main menu buttons
   - SinglePlayerMenuPanel → single player options
   - NewGamePanel → game configuration (bot count, difficulty)

4. **MetagameApplication Prefab**:
   - MetagameView component must reference the GameObject with MainMenuView
   - Typically named "ImportedMenu" or similar

## Testing Checklist

- [ ] Menu displays on game start
- [ ] Navigation between panels works (Main → Single Player → New Game)
- [ ] Bot count dropdown functional (1-20 bots)
- [ ] Difficulty selection works (Easy, Normal, Hard, Extreme)
- [ ] "Start Game" button broadcasts event correctly
- [ ] MainMenuController receives StartSinglePlayerModeEvent
- [ ] Game initializes with correct bot count
- [ ] Menu hides when game starts
- [ ] Back button navigation works correctly

## Event System Reference

### StartSinglePlayerModeEvent
```csharp
public class StartSinglePlayerModeEvent : AppEvent
{
    public GameMode GameMode { get; set; }
    public int BotCount { get; set; }
}
```

**GameMode Options**:
- `GameMode.NewGame`: Fresh single player game
- `GameMode.Continue`: Load saved game
- `GameMode.Coop`: Co-op mode with friends

## Troubleshooting

### Common Issues

**Issue**: Compilation errors about duplicate MainMenuView
- **Cause**: Orphaned meta files or duplicate class definitions
- **Solution**: Delete both .cs and .cs.meta files together, ensure only one MainMenuView.cs exists

**Issue**: Menu doesn't appear in game
- **Cause**: MainMenuView component not added or references not assigned
- **Solution**: Verify MenuManager and MenuCanvas are assigned in Inspector

**Issue**: Start Game button doesn't work
- **Cause**: Event not being broadcast or controller not listening
- **Solution**: Check console for "Broadcasted StartSinglePlayerModeEvent" message

**Issue**: CS0108 warnings about hiding inherited members
- **Cause**: Show()/Hide() methods override base class
- **Solution**: Add `new` keyword to method declarations (already fixed)

## Future Enhancements

### Potential Improvements
1. Add keyboard/gamepad navigation to imported menu
2. Implement visual feedback for button selection
3. Add menu transitions/animations
4. Create profile and options panel implementations
5. Add multiplayer menu integration

## Related Files

### Scripts
- `Assets/Scripts/Runtime/Metagame/Views/MainMenuView.cs` - Menu wrapper
- `Assets/Scripts/MenuManager.cs` - Menu orchestrator
- `Assets/Scripts/NewGamePanel.cs` - Game configuration
- `Assets/Scripts/Runtime/Metagame/Controllers/MainMenuController.cs` - Event handler
- `Assets/Scripts/Runtime/Metagame/MetagameEvents.cs` - Event definitions

### Scenes
- `Assets/Prefab/MenuScene.unity` - Imported menu scene
- `Assets/Scenes/MetagameScene.unity` - Main metagame scene

### Prefabs
- `Assets/Prefabs/Metagame/MetagameApplication.prefab` - Application root

## Commit History

### December 8, 2025
- Replaced UI Toolkit menu with traditional Unity UI
- Integrated MenuManager and panel system
- Added event broadcasting to NewGamePanel
- Fixed namespace conflicts and orphaned meta files
- Added `new` keyword to Show()/Hide() methods
- Created comprehensive documentation

---

**Note**: This integration maintains full compatibility with the existing game architecture while providing a cleaner, more familiar UI workflow using traditional Unity UI components.
