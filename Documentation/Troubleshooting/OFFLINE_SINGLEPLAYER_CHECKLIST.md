# Offline Single-Player Rendering Troubleshooting Checklist

This checklist helps debug issues where nothing renders when starting a new game from the single-player menu (black screen, no player, no camera, no maze).

---

## **Scene & Hierarchy Setup**

### **1. CustomNetworkManager Configuration**
- [ ] Open the scene containing `CustomNetworkManager` (usually the main menu scene)
- [ ] Select the `CustomNetworkManager` GameObject in Hierarchy
- [ ] In Inspector, verify **"Game App Prefab"** field is assigned to `CubeGameApplication` prefab
- [ ] Verify `ConfigurationManager` is assigned
- [ ] Verify `NetworkManager` component exists on the same GameObject

### **2. CubeGameApplication Prefab Setup**
- [ ] Navigate to the `CubeGameApplication` prefab location (likely in `Assets/Prefabs/`)
- [ ] Open prefab in Prefab Mode (double-click)
- [ ] Verify it has these components:
  - `CubeGameApplication`
  - `CubeGameModel`
  - `CubeGameView`
  - `CubeGameController`
- [ ] Select the `CubeGameController` component in Inspector for next steps

### **3. CubeGameController Configuration (CRITICAL)**

#### Room Generation Settings:
- [ ] **Room Generator Reference:** Check if `m_RoomGenerator` field is assigned
  - ⚠️ Can be null - it will auto-find or create one at runtime
  - ✅ Better to assign a pre-configured RoomGenerator prefab/GameObject

- [ ] **Default Room Gen Config:** Verify `m_DefaultRoomGenConfig` ScriptableObject is assigned
  - ⚠️ This provides fallback room templates if profiles are incomplete

#### Mode-Specific Profiles:
Verify these ScriptableObjects are assigned:
- [ ] `m_StoryModeProfile` (for Story mode)
- [ ] `m_CoopModeProfile` (for Co-op mode)
- [ ] `m_BattleRoyaleProfile` (for Battle Royale mode)
- [ ] `m_5x5Profile` (for 5×5 mode)
- [ ] `m_3x3Profile` (for 3×3 mode)

⚠️ **If profiles are missing, maze generation will fail!**

#### Offline Player Setup:
- [ ] **Offline Player Prefab:** **CRITICAL** - Verify `m_OfflinePlayerPrefab` is assigned
  - This prefab should contain:
    - `CharacterController` component
    - `FirstPersonController` component
    - Child GameObject with `Camera` component
  - ⚠️ If missing, a fallback player will be created, but might not have proper controls/settings

---

## **ScriptableObject Configuration**

### **4. RoomGenProfile ScriptableObjects**

For **each** profile (Battle Royale, 5×5, 3×3, Story, Co-op):

#### Essential Settings:
- [ ] Open the ScriptableObject asset in Inspector
- [ ] Verify **Room Templates** array has at least 1-3 room prefabs assigned
  - ❌ Empty array = no rooms will generate
- [ ] Check **Grid Size**:
  - 9 for full 9×9×9 cube (Battle Royale)
  - 5 for medium 5×5×5 (5×5 mode)
  - 3 for small 3×3×3 (3×3 mode)
- [ ] Check **Sparse Density**:
  - `1.0` = generate all rooms (full grid)
  - `0.3` = generate 30% of rooms (optimized/sparse)
  - `0.0` = generate only guaranteed path rooms

#### Optional But Recommended:
- [ ] Verify **Exit Room Prefab** is assigned (special room for center/exit)
- [ ] Verify **Spawn Room Prefab** is assigned (special room for corner spawns)
- [ ] Check **Room Size** matches your room prefab dimensions (default: 10 units)
- [ ] Verify **Seed** (0 = random, specific number = reproducible layout)

---

## **Room Prefab Requirements**

### **5. Room Template Prefabs**

Open one of your room template prefabs and verify:

#### Required Structure:
- [ ] Room prefab has a child GameObject named **"Bounds"** (exact name, case-sensitive)
- [ ] The "Bounds" child has a `MeshRenderer` or `Renderer` component
  - ℹ️ Used to calculate room dimensions for grid placement
  - ℹ️ Can disable the renderer if you don't want it visible

#### Visual Components:
- [ ] Room has floor mesh with material assigned
- [ ] Room has wall meshes with materials assigned
- [ ] Room has ceiling mesh with material assigned (optional)
- [ ] Check that room size matches `RoomGenerator.m_RoomSize` (default: 10×10×10 units)

#### Door Anchors (Optional):
- [ ] Room has empty GameObjects for doors: `door_north`, `door_south`, `door_east`, `door_west`
- [ ] Door anchors are positioned at room edges
- [ ] Door anchors face outward (blue Z-axis pointing out of room)

#### Materials & Rendering:
- [ ] Materials are assigned to all MeshRenderers
- [ ] Materials are not completely transparent
- [ ] Materials have proper shader (URP/Lit or URP/Unlit)

---

## **Offline Player Prefab Setup**

### **6. Offline Player Prefab Requirements**

If you have a custom offline player prefab assigned:

#### Required Components:
- [ ] Root GameObject has `CharacterController` component
  - Height: ~2.0
  - Center: (0, 1, 0)
  - Radius: ~0.5
- [ ] Root GameObject has `FirstPersonController` component
  - Speed, jump, mouse sensitivity configured

#### Camera Setup:
- [ ] Player has a child GameObject with `Camera` component
- [ ] Camera is tagged as **"MainCamera"**
  - ❌ If not tagged MainCamera, rendering will fail!
- [ ] Camera local position is around (0, 1.6, 0) relative to player (eye level)
- [ ] Camera has proper settings:
  - Clear Flags: Skybox or Solid Color
  - Culling Mask: includes Default layer (and other relevant layers)
  - Field of View: 60-90 degrees

#### Optional Components:
- [ ] Audio Listener (usually on camera)
- [ ] Player Input component (if using New Input System)
- [ ] Flashlight/weapon attachments

---

## **Runtime Debugging**

### **7. Console Output Check**

When you start a new game, look for these log messages **in this order**:

```
✅ [9x9] CubeGameApplication initialized - Maze escape game ready
✅ [9x9] Offline singleplayer initialized (local game + bots).
✅ [9x9] Broadcast StartMatchEvent for offline game initialization.
✅ [9x9 Server] Applied [ModeName] profile: [ProfileName]
✅ [9x9 Server] Generating 9x9x9 maze...
✅ [9x9 Offline] Spawned offline player at (x, y, z)
```

#### Missing Logs Indicate:
- ❌ No "CubeGameApplication initialized" = Game app didn't instantiate
- ❌ No "Broadcast StartMatchEvent" = Event system broken
- ❌ No "Applied profile" = Profile missing or not assigned
- ❌ No "Generating maze" = RoomGenerator failed to initialize
- ❌ No "Spawned offline player" = Player prefab missing or spawn failed

#### Common Errors to Check For:
- [ ] Check Console for `NullReferenceException`
- [ ] Check for `RoomGenerator not found!` error
- [ ] Check for `Profile missing room templates` warnings
- [ ] Check for `m_OfflinePlayerPrefab not assigned` warning
- [ ] Check for `GameApp prefab is not assigned!` error

---

### **8. Hierarchy Runtime Check**

While game is running (Play Mode):

#### Game Application Instance:
- [ ] Check Hierarchy for `CubeGameApplication(Clone)` GameObject
- [ ] Verify it has all MVC components attached
- [ ] Check that it's not disabled

#### Room Generation:
- [ ] Look for `RoomGenerator` GameObject (auto-created if missing)
- [ ] Look for multiple room clones in Hierarchy:
  - `SimpleRoom(Clone)`, `RoomBase(Clone)`, etc.
  - Should see dozens to hundreds depending on sparse density
- [ ] If NO rooms appear, generation failed

#### Player Instance:
- [ ] Look for `OfflinePlayer` GameObject in Hierarchy
  - OR `OfflinePlayer_Fallback` if prefab wasn't assigned
- [ ] Select the player and verify it's at a reasonable position (not at 0,0,0)
- [ ] Verify the player has a Camera child
- [ ] Verify the Camera child is tagged **"MainCamera"**

---

### **9. Camera & Rendering Check**

#### Game View:
- [ ] In Game View toolbar, check if "MainCamera" is shown at top-left corner
  - ❌ If it says "Display 1" or nothing, no main camera is active!

#### Camera Inspector (during Play Mode):
- [ ] Select the Camera in Hierarchy during Play Mode
- [ ] In Inspector, verify Camera component is **enabled** (checkbox checked)
- [ ] Check Camera's **Culling Mask**:
  - Should include "Default" layer at minimum
  - Should include any custom layers your rooms use
- [ ] Verify **Clear Flags** = Skybox or Solid Color (not "Depth only")
- [ ] Check **Near Clip Plane** = 0.1 to 0.3 (not too large)
- [ ] Check **Far Clip Plane** = 100 to 1000 (not too small)

#### Scene View Comparison:
- [ ] Open Scene View while in Play Mode
- [ ] Look for generated rooms/player
- [ ] If objects exist in Scene View but not Game View:
  - Camera position is wrong, OR
  - Camera culling mask excludes room layers, OR
  - Rooms are on wrong layer

---

### **10. Lighting Check**

#### Scene Lighting:
- [ ] Check if scene has a **Directional Light** in Hierarchy
- [ ] Verify Directional Light is enabled
- [ ] Check light intensity (1.0 is standard)
- [ ] Verify light is not pointing straight up (should angle downward)

#### Lighting Settings:
- [ ] Open: Window → Rendering → Lighting
- [ ] Check **Environment Lighting**:
  - Source: Skybox or Color
  - If Color, verify it's not pure black
  - Intensity Multiplier: 1.0 is standard
- [ ] Check **Realtime Lighting**:
  - Realtime Global Illumination: Enabled or Disabled (either works)
- [ ] If rooms appear completely black:
  - Increase Ambient Intensity temporarily for testing
  - Check if materials need emission
  - Verify materials are using proper URP shaders

#### Material Check:
- [ ] Select a room prefab
- [ ] Check materials on MeshRenderers
- [ ] Verify shader is URP-compatible:
  - Universal Render Pipeline/Lit
  - Universal Render Pipeline/Unlit
  - Universal Render Pipeline/Simple Lit
- [ ] Check Albedo color is not pure black

---

## **Quick Diagnostic Test**

### **11. Debug Cube Test**

If nothing renders and all settings look correct, add this temporary test to `CubeGameController.cs`.

**Location:** At the END of `OnServerInitializeMazeGame()` method (after all generation logic)

```csharp
// ============================================
// TEMPORARY DEBUG: Create visible test cube
// ============================================
var testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
testCube.transform.position = new Vector3(0, 2, 5); // In front of spawn
testCube.transform.localScale = Vector3.one * 2;
testCube.name = "DEBUG_VISIBLE_CUBE";

// Make it bright and obvious
var renderer = testCube.GetComponent<Renderer>();
if (renderer != null)
{
    renderer.material.color = Color.magenta; // Bright magenta
}

UnityEngine.Debug.LogWarning("DEBUG: Created bright magenta test cube at (0,2,5)");
UnityEngine.Debug.LogWarning("If you DON'T see the magenta cube, camera/rendering is broken");
UnityEngine.Debug.LogWarning("If you DO see the cube but no rooms, maze generation failed");
// ============================================
```

**Test Results:**
- ✅ **See magenta cube** → Camera works! Problem is maze generation.
- ❌ **Don't see cube** → Camera/rendering setup is broken.

---

## **Most Likely Issues (In Order of Frequency)**

### 🔥 **Critical Issues** (Will completely break rendering):

1. ❌ **`m_OfflinePlayerPrefab` not assigned** in CubeGameController
   - **Fix:** Assign player prefab in Inspector
   - **Workaround:** Fallback player will be created, but verify it appears

2. ❌ **Camera not tagged "MainCamera"**
   - **Fix:** Select camera → Tag dropdown → MainCamera

3. ❌ **Room generation profiles missing room template prefabs**
   - **Fix:** Assign at least 1 room prefab to each profile's Room Templates array

4. ❌ **GameApp prefab not assigned** in CustomNetworkManager
   - **Fix:** Assign CubeGameApplication prefab to "Game App Prefab" field

5. ❌ **StartMatchEvent not broadcast** (older builds)
   - **Fix:** Update CustomNetworkManager.cs per latest code

### ⚠️ **Common Issues** (Will cause partial failures):

6. ⚠️ **Room prefabs missing "Bounds" child**
   - **Symptom:** Rooms don't generate or overlap incorrectly
   - **Fix:** Add child GameObject named "Bounds" with Renderer component

7. ⚠️ **Materials using Built-In shaders instead of URP**
   - **Symptom:** Rooms appear magenta/pink
   - **Fix:** Edit → Render Pipeline → Universal Render Pipeline → Upgrade Project Materials

8. ⚠️ **Lighting too dark or missing**
   - **Symptom:** Rooms render black
   - **Fix:** Add Directional Light, increase ambient lighting

9. ⚠️ **RoomGenerator auto-created with no templates**
   - **Symptom:** Console shows "Created runtime RoomGenerator" warning
   - **Fix:** Pre-configure RoomGenerator in scene or on CubeGameApplication prefab

10. ⚠️ **Player spawns at (0,0,0) instead of corner**
    - **Symptom:** Player falls through floor or stuck in wall
    - **Fix:** Verify RoomGenerator has GetCornerSpawnPosition method

---

## **Verification Steps After Fixes**

### Final Checklist Before Testing:

- [ ] All profiles have room templates assigned
- [ ] CubeGameController has offline player prefab assigned
- [ ] Room prefabs have "Bounds" child with Renderer
- [ ] CustomNetworkManager has game app prefab assigned
- [ ] Camera exists and is tagged "MainCamera"
- [ ] Directional Light exists in scene
- [ ] Materials use URP shaders

### Test Procedure:

1. **Start Play Mode**
2. **Navigate to Main Menu**
3. **Click "New Game" (or equivalent single-player button)**
4. **Wait 2-3 seconds for generation**
5. **Check Console** for the expected log sequence
6. **Check Game View** for visible maze and player camera
7. **Check Hierarchy** for generated rooms and player instance

### Success Criteria:

✅ Console shows all expected log messages  
✅ Game View shows rendered maze rooms  
✅ Player camera is active and can look around  
✅ Movement controls work (WASD/arrow keys)  
✅ No errors in Console  

---

## **Advanced Debugging**

### Profiler Check:
- [ ] Open: Window → Analysis → Profiler
- [ ] Check CPU usage during generation
- [ ] Look for performance spikes or hangs

### Frame Debugger:
- [ ] Open: Window → Analysis → Frame Debugger
- [ ] Click "Enable" while in Play Mode
- [ ] Step through draw calls to see what's rendering
- [ ] Verify rooms/player are being submitted to GPU

### Network Manager State:
- [ ] Select CustomNetworkManager in Hierarchy (Play Mode)
- [ ] Check if `IsServer`, `IsClient`, `IsHost` are all false (expected for offline)
- [ ] Verify `m_GameApp` field shows CubeGameApplication instance

---

## **Getting Help**

If you've checked everything and it still doesn't work:

### Information to Provide:

1. **Console Output:** Copy all log messages from startup to failure
2. **Error Messages:** Copy full stack trace of any errors
3. **Inspector Screenshots:**
   - CubeGameController configuration
   - One RoomGenProfile configuration
   - CustomNetworkManager configuration
4. **Hierarchy Screenshot:** During Play Mode showing what objects exist
5. **Unity Version:** Exact version number (e.g., 6000.2.10f1)
6. **Platform:** Windows/Mac/Linux

### Quick Diagnostic Export:

Run this in Unity Console (bottom-left `>_` icon) during Play Mode:

```csharp
Debug.Log($"GameApp: {CubeGameApplication.Instance != null}");
Debug.Log($"NetworkMgr: {NetworkManager.Singleton != null}");
Debug.Log($"Camera: {Camera.main != null}");
Debug.Log($"Rooms: {FindObjectsByType<RoomData>(FindObjectsSortMode.None).Length}");
```

Share the output along with the information above.

---

**Last Updated:** November 29, 2025  
**Applies To:** Unity 6000.2.10f1, 9x9 Cube Maze Project  
**Related Files:**
- `CustomNetworkManager.cs`
- `CubeGameController.cs`
- `CubeGameApplication.cs`
- `RoomGenerator.cs`
- `RoomGenProfile.cs`
