# Controller Setup Guide

This guide explains how to enable and configure controller support for **Nintendo Switch**, **PlayStation**, **Xbox**, and **generic controllers** in the 9x9 Unity project.

---

## Overview

The project uses Unity's **New Input System** with automatic controller detection and support for:
- **Xbox Controllers** (Xbox One, Xbox Series X|S, Xbox 360)
- **PlayStation Controllers** (DualShock 4, DualSense)
- **Nintendo Switch Controllers** (Pro Controller, Joy-Cons)
- **Generic USB/Bluetooth Controllers**

---

## Quick Setup

### 1. Enable Input System Package

The Input System package is already included. Verify it's active:

1. **Edit → Project Settings → Player**
2. **Active Input Handling**: Set to **"Input System Package (New)"** or **"Both"**
3. Unity will restart if you change this setting

### 2. Input Actions Configuration

The `FirstPersonController` is already configured to work with controllers via `PlayerInput` callbacks.

**Supported inputs:**
- **Movement**: Left Stick (all controllers)
- **Camera Look**: Right Stick (all controllers)
- **Jump**: A/Cross/B button (context-aware)
- **Sprint**: Left Stick Press (L3)

### 3. Controller-Specific Button Mappings

| Action | Xbox | PlayStation | Switch | Generic |
|--------|------|-------------|--------|---------|
| **Jump** | A | Cross (✕) | B | Button 0 |
| **Sprint** | Left Stick Click | L3 | Left Stick Click | Button 8 |
| **Menu/Pause** | Start | Options | + | Button 9 |
| **Back** | B | Circle (◯) | A | Button 1 |

---

## Creating Input Actions Asset

If you need to create or modify the Input Actions:

### Step 1: Create Asset

1. Right-click in **Project** window
2. **Create → Input Actions**
3. Name it `PlayerInputActions`
4. Double-click to open

### Step 2: Define Action Map

Create a **Player** action map with these actions:

#### **Move** (Value, Vector2)
- **Keyboard**: WASD Composite
- **Keyboard**: Arrow Keys Composite
- **Gamepad**: Left Stick
  - Path: `<Gamepad>/leftStick`
  - Processors: `Stick Deadzone (0.125)`

#### **Look** (Value, Vector2)
- **Mouse**: Delta
  - Path: `<Mouse>/delta`
  - Processors: `Scale (0.05)`
- **Gamepad**: Right Stick
  - Path: `<Gamepad>/rightStick`
  - Processors: `Stick Deadzone (0.125), Scale Vector2 (x: 300, y: 300)`

#### **Jump** (Button)
- **Keyboard**: Space
- **Gamepad**: Button South
  - Path: `<Gamepad>/buttonSouth`
  - Notes: Auto-detects A (Xbox), Cross (PS), B (Switch)

#### **Sprint** (Button)
- **Keyboard**: Left Shift
- **Gamepad**: Left Stick Press
  - Path: `<Gamepad>/leftStickPress`

### Step 3: Generate C# Class

1. In Input Actions window, check **"Auto-Save"**
2. Click **"Generate C# Class"**
3. Set Class Name: `PlayerInputActions`
4. Set Namespace: `Unity.Template.Multiplayer.NGO.Runtime`
5. Click **Generate**

---

## Wiring PlayerInput to Player Prefab

### Step 1: Add PlayerInput Component

1. Select **Player** prefab (or GameObject in scene)
2. **Add Component → Player Input**

### Step 2: Configure PlayerInput

- **Actions**: Assign `PlayerInputActions` asset
- **Default Map**: `Player`
- **Behavior**: `Invoke Unity Events`
- **Camera**: (Optional) Assign main camera

### Step 3: Link Events

Expand **Events → Player** in Inspector:

- **Move** → `FirstPersonController.OnMove`
- **Look** → `FirstPersonController.OnLook`
- **Jump** → `FirstPersonController.OnJump`
- **Sprint** → `FirstPersonController.OnSprint`

---

## Testing Controllers

### Windows

**Xbox Controllers:**
- Plug-and-play via USB or Xbox Wireless Adapter
- No additional drivers needed

**PlayStation Controllers:**
- USB: Works natively
- Bluetooth: Use DS4Windows for DualShock 4, or native support for DualSense on Windows 10+

**Switch Pro Controller:**
- USB: Works natively
- Bluetooth: Pair via Windows Bluetooth settings

**Generic Controllers:**
- Most USB/Bluetooth controllers work automatically
- Test with Unity Input Debugger: **Window → Analysis → Input Debugger**

### Testing in Unity Editor

1. **Window → Analysis → Input Debugger**
2. Connect controller
3. Press buttons to see input events
4. Verify all axes and buttons register

### Testing at Runtime

1. Enter Play Mode
2. Use controller to move/look/jump
3. Check Console for input logs (if debug enabled)

---

## Advanced: Multi-Controller Support

To support **multiple players** with different controllers:

### Step 1: Enable Multiplayer Mode

In `PlayerInput` component:
- **Control Scheme**: Create schemes for Keyboard/Mouse and Gamepad
- **Join Behavior**: `Join Players When Button Is Pressed`

### Step 2: Define Control Schemes

In Input Actions asset:

**Keyboard&Mouse:**
- Devices: Keyboard, Mouse
- Required: Both

**Gamepad:**
- Devices: Gamepad
- Required: Gamepad

### Step 3: Player Joining

Players join by pressing a button on their controller:
- **Keyboard**: Press Space or W/A/S/D
- **Gamepad**: Press A/Cross/B

---

## Troubleshooting

### Controller Not Detected

**Solution:**
1. Open **Input Debugger**: Window → Analysis → Input Debugger
2. Check if device appears in list
3. If not, reconnect controller or restart Unity

### Wrong Button Mappings

**Xbox/PS/Switch controllers use different button layouts:**
- Use `<Gamepad>/buttonSouth` for Jump (auto-maps to A/Cross/B)
- Use `<Gamepad>/buttonEast` for Back (auto-maps to B/Circle/A)

**For explicit controllers:**
- `<XInputController>/buttonSouth` (Xbox only)
- `<DualShockGamepad>/buttonSouth` (PlayStation only)
- `<SwitchProControllerHID>/buttonSouth` (Switch only)

### Stick Drift / Dead Zone Issues

**Solution:**
1. Open Input Actions
2. Select **Move** or **Look** action
3. Add Processor: **Stick Deadzone**
4. Set **Min** to `0.125` and **Max** to `0.95`

### Sprint Not Working on Controller

**Check:**
- Action is bound to `<Gamepad>/leftStickPress` (L3)
- `FirstPersonController.OnSprint` receives `context.performed` (not `context.started`)

---

## Platform-Specific Notes

### Nintendo Switch

- **Pro Controller**: Full support via USB/Bluetooth
- **Joy-Cons**: Detected as separate controllers; use **both** for full control
- **Combine Joy-Cons**: Pair as single controller in system settings

### PlayStation 5 (DualSense)

- **Haptics/Adaptive Triggers**: Not supported in Unity Input System (use DualSense API for PS5 builds)
- **Touchpad**: Detected as mouse input; can be bound separately

### Xbox Controllers

- **Elite Controller**: All buttons/paddles supported
- **Adaptive Controller**: Full support

---

## Example: Custom Control Scheme

To add a custom "Racing Wheel" control scheme:

```csharp
// In Input Actions:
// 1. Add new Control Scheme: "Wheel"
// 2. Required Device: <SteeringWheel>
// 3. Bind Steering to <SteeringWheel>/steeringWheel
// 4. Bind Accelerate to <SteeringWheel>/accelerator
```

---

## Resources

- [Unity Input System Docs](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)
- [Input Action Assets](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/ActionAssets.html)
- [PlayerInput Component](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/Components.html)
- [Gamepad Support](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/Gamepad.html)

---

**Last Updated:** November 30, 2025  
**Maintained By:** Development Team
