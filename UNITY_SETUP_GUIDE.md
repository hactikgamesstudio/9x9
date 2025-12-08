# Unity Scene Setup Guide - Menu Integration
**Date**: December 8, 2025  
**Project**: 9x9 Multiplayer Cube Game  

## Purpose
This guide walks you through the Unity Editor configuration required to connect the imported menu system to the game architecture.

---

## Prerequisites

✅ All compilation errors resolved  
✅ Unity Editor closed (recommended before starting)  
✅ Files present:
- `Assets/Scripts/Runtime/Metagame/Views/MainMenuView.cs` (64 lines)
- `Assets/Scripts/MenuManager.cs`
- `Assets/Scripts/NewGamePanel.cs`
- `Assets/Prefab/MenuScene.unity`

---

## Step-by-Step Setup

### **Step 1: Open Unity and Load the Project**

1. Launch Unity Hub
2. Open the 9x9 project
3. Wait for compilation to complete
4. **Verify no errors** in the Console window

---

### **Step 2: Locate the Menu Scene**

1. In Project window, navigate to: `Assets/Prefab/`
2. Find `MenuScene.unity`
3. Double-click to open it

**What you should see**:
- MenuCanvas GameObject in Hierarchy
- MenuController (MenuManager component)
- Various panel GameObjects (MainMenuPanel, SinglePlayerMenuPanel, NewGamePanel)

---

### **Step 3: Verify MenuManager Configuration**

1. **Select "MenuController" GameObject** in Hierarchy
2. **In Inspector**, find the MenuManager component
3. **Verify these references are assigned**:
   - Main Menu Panel → MainMenuPanel GameObject
   - Single Player Menu Panel → SinglePlayerMenuPanel GameObject
   - New Game Panel → NewGamePanel GameObject

**If any are missing**:
- Drag the corresponding GameObject from Hierarchy into the field

---

### **Step 4: Add MainMenuView Component**

1. **Select "MenuController" GameObject** (or create new empty GameObject named "ImportedMenu")
2. Click **Add Component** button in Inspector
3. Search for "MainMenuView"
4. Click to add the component

**If MainMenuView doesn't appear**:
- Check Console for compilation errors
- Ensure `MainMenuView.cs` exists in `Assets/Scripts/Runtime/Metagame/Views/`

---

### **Step 5: Assign MainMenuView References**

With "MenuController" selected and MainMenuView component added:

1. **Menu Manager** field:
   - Drag the **MenuManager component** from the same GameObject
   - OR click the circle icon → select "MenuController" → choose MenuManager

2. **Menu Canvas** field:
   - Drag the **MenuCanvas GameObject** from Hierarchy
   - This is typically the root Canvas object in the scene

**Visual Check**:
- Both fields should show "MenuCanvas" and "MenuManager (MenuManager)"
- No "None (GameObject)" or "None (MenuManager)" warnings

---

### **Step 6: Open MetagameScene**

1. Navigate to: `Assets/Scenes/MetagameScene.unity`
2. Double-click to open

---

### **Step 7: Locate MetagameApplication Prefab**

In the Hierarchy window:

1. Look for **"MetagameApplication"** GameObject
2. **Select it**
3. If you don't see it:
   - Click in Hierarchy search bar
   - Type "metagame"
   - It may be under another GameObject

---

### **Step 8: Update MetagameView Reference**

With MetagameApplication selected:

1. **In Inspector**, find the **MetagameView** component
2. **Locate the "Main Menu View" field**
3. **Current value**: Likely references old UI Toolkit menu
4. **Click the field** to clear it

**Assign the new menu**:
- **Option A** (if MenuScene objects are in this scene):
  - Drag "MenuController" GameObject into this field
  
- **Option B** (if MenuScene is separate):
  - You'll need to instantiate MenuScene into MetagameScene first:
    1. Drag `MenuScene.unity` from Project into Hierarchy
    2. Find the MenuController GameObject
    3. Drag it into the "Main Menu View" field

---

### **Step 9: Save Changes**

1. **File → Save** (or Ctrl+S)
2. **Save the scene** when prompted
3. **Save the prefab** if you modified MetagameApplication

---

### **Step 10: Test the Integration**

1. **Press Play** in Unity Editor
2. **Watch Console** for:
   ```
   MenuManager started
   ShowMainMenu called
   Main Menu Panel activated: MainMenuPanel
   ```

3. **Test navigation**:
   - Click "Single Player" → should show Single Player menu
   - Click "New Game" → should show New Game panel
   - Select bot count and difficulty
   - Click "Start Game"

4. **Check Console** for event broadcasting:
   ```
   Starting game with X bots on Y difficulty
   Broadcasted StartSinglePlayerModeEvent with BotCount: X
   [MainMenuController] OnStartSinglePlayerMode called with GameMode: NewGame, BotCount: X
   ```

---

## Troubleshooting

### **Menu doesn't appear**
- Check MenuCanvas is active (checkbox in Inspector)
- Verify MainMenuView component is enabled
- Check Menu Manager reference is assigned

### **"Start Game" doesn't work**
- Open `NewGamePanel.cs` and verify `OnStartGameClicked()` broadcasts event
- Check MainMenuController is listening for StartSinglePlayerModeEvent
- Look for errors in Console

### **MetagameView can't find MainMenuView**
- Ensure the GameObject with MainMenuView is in the same scene as MetagameApplication
- OR use a prefab reference if menus are in separate scene

### **Compilation errors**
- Verify only ONE `MainMenuView.cs` exists in project
- Check for orphaned `.meta` files
- Reimport scripts: Right-click `Scripts` folder → Reimport

---

## Scene Hierarchy Reference

**Expected structure**:

```
MetagameScene
├── MetagameApplication
│   └── MetagameView (component)
│       └── Main Menu View: <ImportedMenu reference>
└── [Other game objects]

MenuScene (or instantiated in MetagameScene)
├── MenuCanvas
│   └── MenuController
│       ├── MenuManager (component)
│       ├── MainMenuView (component) ← THIS IS WHAT YOU CONFIGURED
│       ├── MainMenuPanel
│       ├── SinglePlayerMenuPanel
│       └── NewGamePanel
```

---

## Next Steps After Successful Test

✅ Menu displays and navigation works  
✅ Start Game broadcasts event correctly  
✅ Game initializes with bot count  

**Continue to**:
1. Test actual game start (verify network initialization)
2. Test with different bot counts (1-20)
3. Test difficulty settings (if implemented)
4. Add keyboard/gamepad navigation (optional enhancement)

---

## Questions or Issues?

Check the following documents:
- `MENU_INTEGRATION.md` - Technical integration details
- `.github/copilot-instructions.md` - Project overview
- Console logs - Always check for errors/warnings

**If you encounter errors**:
1. Copy the full error message from Console
2. Note which step you're on
3. Check file locations match the guide
4. Verify Unity version is 6000.2.10f1

---

**Setup Complete!** Your menu should now be fully integrated with the game architecture.
