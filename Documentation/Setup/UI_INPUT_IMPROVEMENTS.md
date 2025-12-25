# UI & Input Improvements - Setup Guide

This guide covers the new custom cursor, bot count dropdown, and controller support implemented on November 30, 2025.

---

## 1. Custom Cursor Setup

**What Changed:**
- Cursor now **unlocked by default** for menu navigation
- Custom cursor texture support with button highlighting
- Hover effects on UI buttons (color + scale)

**Setup Steps:**

### Option A: Add to Metagame Scene

1. Open `MetagameApplication` scene
2. Create new GameObject: `CustomCursor`
3. Add Component → `CustomCursor`
4. Configure in Inspector:
   - **Cursor Texture**: (Optional) Assign custom cursor image
   - **Cursor Hover Texture**: (Optional) Assign hover state image
   - **Highlight Buttons**: ✓ Enabled
   - **Highlight Color**: Yellow-ish (default: `#FFFF80`)
   - **Highlight Scale**: `1.1` (10% larger on hover)

### Option B: Custom Cursor Textures

Create cursor textures:
- **Size**: 32x32 or 64x64 pixels
- **Format**: PNG with transparency
- **Import Settings**:
  - Texture Type: Cursor
  - Read/Write Enabled: ✓

---

## 2. Bot Count Dropdown

**What Changed:**
- **Replaced**: SliderInt (0-7 bots)
- **New**: DropdownField (0-8 bots)
- Clearer selection with text labels

**Dropdown Options:**
- "0 Bots" (solo maze run)
- "1 Bot"
- "2 Bots"
- "3 Bots" ← **Default**
- "4 Bots"
- "5 Bots"
- "6 Bots"
- "7 Bots"
- "8 Bots" (maximum)

**Backend Changes:**
- `MainMenuView.cs`: Uses `DropdownField` instead of `SliderInt`
- `OnClickNewGame` / `OnClickCoop`: Reads `m_BotCountDropdown.index` (0-8)
- `MetagameEvents.cs`: `StartSinglePlayerModeEvent.BotCount` still accepts `int`

**No UXML changes required!** The code creates the dropdown dynamically if not present in UXML.

---

## 3. Controller Support

**Supported Controllers:**
- ✅ Xbox (One, Series X|S, 360)
- ✅ PlayStation (DualShock 4, DualSense)
- ✅ Nintendo Switch (Pro Controller, Joy-Cons)
- ✅ Generic USB/Bluetooth controllers

**What Changed:**
- `FirstPersonController.cs` now supports all controller types via Input System
- Conditional compilation: Input System when available, legacy fallback otherwise
- Player can use **keyboard/mouse OR controller** seamlessly

**Quick Test:**
1. Connect any controller
2. Enter Play Mode
3. Use **Left Stick** to move
4. Use **Right Stick** to look
5. Press **A/Cross/B** to jump
6. Press **Left Stick (L3)** to sprint

**Full Setup:**
See `Documentation/Setup/CONTROLLER_SETUP.md` for:
- Input Actions configuration
- PlayerInput component wiring
- Multi-controller (local multiplayer)
- Platform-specific notes

---

## 4. Cursor Behavior Changes

### Before:
- Cursor **locked** on game start (FPS mode)
- Player couldn't navigate menus without unlocking

### After:
- Cursor **unlocked** by default (menu mode)
- Players can click buttons immediately
- Lock cursor when entering gameplay (click to lock)

**FirstPersonController Changes:**
```csharp
// OLD: Awake()
Cursor.lockState = CursorLockMode.Locked;
Cursor.visible = false;

// NEW: Awake()
Cursor.lockState = CursorLockMode.None;
Cursor.visible = true;
```

**When to Lock:**
- Player clicks "Play" or enters game scene
- Player clicks left mouse button in-game (re-lock)
- Press **Escape** to unlock for pause menu

---

## 5. Testing Checklist

### Custom Cursor:
- [ ] Cursor visible in main menu
- [ ] Buttons highlight on hover (yellow glow + scale)
- [ ] Hover texture changes (if assigned)
- [ ] Click events still work

### Bot Dropdown:
- [ ] Dropdown appears in Single Player menu
- [ ] Default selection: "3 Bots"
- [ ] Can select 0-8 bots
- [ ] "New Game" and "Co-op" buttons use selected value
- [ ] Bot count logged in Console

### Controller Support:
- [ ] Xbox controller detected (Window → Analysis → Input Debugger)
- [ ] PlayStation controller works (USB or Bluetooth)
- [ ] Switch Pro Controller works
- [ ] Movement (Left Stick), Look (Right Stick), Jump (A/Cross/B), Sprint (L3)
- [ ] Can switch between keyboard and controller mid-game

---

## 6. Known Issues & Limitations

### Custom Cursor:
- **Limitation**: Only highlights buttons in UI Toolkit (not legacy uGUI)
- **Workaround**: Use `EventTrigger` for uGUI elements

### Bot Dropdown:
- **Note**: Changing bot count mid-game has no effect (only applies to new games)
- **Multiplayer**: Bot count is ignored in online multiplayer modes

### Controller Support:
- **No vibration/haptics**: Unity Input System doesn't support PS5 haptics yet
- **Joy-Con pairing**: Pair both Joy-Cons as single controller in system settings
- **Stick drift**: Adjust dead zone in Input Actions (default: 0.125)

---

## 7. Optional: UI Toolkit UXML Setup

If you want to define the dropdown in UXML instead of runtime code:

```xml
<!-- In your main menu UXML -->
<ui:VisualElement name="singlePlayerMenu">
    <ui:Button name="continueButton" text="Continue" />
    <ui:DropdownField name="botCountDropdown" label="Number of Bots" />
    <ui:Label name="botCountLabel" text="Select bot count for singleplayer/co-op modes" />
    <ui:Button name="newGameButton" text="New Game" />
    <ui:Button name="coopButton" text="Co-op" />
    <ui:Button name="spBackButton" text="Back" />
</ui:VisualElement>
```

---

## 8. Future Enhancements

### Cursor:
- [ ] Animated cursor (sprite sheet)
- [ ] Sound effects on button hover/click
- [ ] Custom cursor per menu (combat, inventory, etc.)

### Bots:
- [ ] Bot difficulty slider (Easy/Medium/Hard)
- [ ] Bot AI customization (aggressive, defensive, balanced)
- [ ] Save bot count preference

### Controllers:
- [ ] Rebindable controls UI
- [ ] Controller vibration on damage/events
- [ ] Split-screen local multiplayer

---

## 9. Files Modified

**New Files:**
- `Assets/Scripts/Runtime/Shared/UI/CustomCursor.cs`
- `Documentation/Setup/CONTROLLER_SETUP.md`

**Modified Files:**
- `Assets/Scripts/Runtime/Metagame/Views/MainMenuView.cs`
  - Replaced `SliderInt` with `DropdownField`
  - Updated event handlers
- `Assets/Scripts/Runtime/Game/Player/FirstPersonController.cs`
  - Unlocked cursor by default
  - Added PlayerInput callbacks
  - Controller support via Input System

**No Breaking Changes:**
- Existing save games still work
- Multiplayer compatibility maintained
- Legacy Input Manager still supported (fallback)

---

## 10. Quick Commands (Windows PowerShell)

```powershell
# Test controller detection
# 1. Open Unity
# 2. Window → Analysis → Input Debugger
# 3. Connect controller
# 4. Press buttons to verify

# Create custom cursor textures (example with ImageMagick)
magick -size 32x32 xc:none -fill white -draw "circle 16,16 16,4" cursor.png
magick cursor.png -fill yellow -draw "circle 16,16 16,6" cursor_hover.png

# Import cursors
# Drag cursor.png and cursor_hover.png into Unity Assets/Textures/UI/
# Select each → Inspector → Texture Type: Cursor
```

---

**Last Updated:** November 30, 2025  
**Implemented By:** GitHub Copilot  
**For Questions:** Check CONTROLLER_SETUP.md or Unity Input System docs
