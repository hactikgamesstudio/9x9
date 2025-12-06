# Creating Required ScriptableObject Assets

This guide shows you how to create the missing ScriptableObject assets needed for the 9x9 Cube Game to work.

---

## **What You Need to Create**

### **1. RoomGenConfig (Default Fallback Config)**
- **Purpose:** Provides default room templates when profiles are incomplete
- **Location:** `Assets/ScriptableObjects/RoomGenConfig.asset`
- **Quantity:** 1 (shared across all modes)

### **2. RoomGenProfile Assets (Mode-Specific Configs)**
- **Purpose:** Define generation parameters for each game mode
- **Location:** `Assets/ScriptableObjects/Profiles/`
- **Quantity:** 5 (one per game mode)

---

## **Step-by-Step Instructions**

### **Step 1: Create Folder Structure**

In Unity's Project window:

1. Navigate to `Assets/`
2. Right-click → Create → Folder → Name it **"ScriptableObjects"**
3. Inside ScriptableObjects folder, create another folder named **"Profiles"**

**Final structure:**
```
Assets/
└── ScriptableObjects/
    └── Profiles/
```

---

### **Step 2: Create RoomGenConfig**

1. **Navigate** to `Assets/ScriptableObjects/`
2. **Right-click** → Create → 9x9 → **Room Generation Config**
3. **Rename** it to: `DefaultRoomGenConfig`
4. **Select** the asset and configure in Inspector:

#### DefaultRoomGenConfig Settings:
```
Grid Size: 9
Room Size: 10
Generate All Rooms: ☐ (unchecked)
Sparse Density: 0.3
False Door Chance: 0.15
Enable Room Rotation: ☑ (checked)
Rotation Cooldown: 10
Seed: 0
```

**Room Templates Array:**
- Expand "Room Templates"
- Set **Size: 3** (or however many room prefabs you have)
- Drag your room prefabs from `Assets/Prefabs/Rooms/` into the array slots

**Exit Room Prefab:**
- Drag your exit room prefab (if you have one)

**Spawn Room Prefab:**
- Drag your spawn room prefab (if you have one)

---

### **Step 3: Create RoomGenProfile Assets**

You need to create **5 profiles**, one for each game mode.

#### **Profile 1: Battle Royale**

1. Navigate to `Assets/ScriptableObjects/Profiles/`
2. Right-click → Create → 9x9 → **Room Generation Profile**
3. Rename to: `BattleRoyaleProfile`
4. Configure:

```
Grid Size: 9
Room Size: 10
Generate All Rooms: ☐
Sparse Density: 0.35
Seed: 0 (random each time)
Guarantee Corner To Center Paths: ☑
```

- **Room Templates:** Assign 3-5 room prefabs
- **Exit Room Prefab:** (optional) Assign special exit room
- **Spawn Room Prefab:** (optional) Assign special spawn room

---

#### **Profile 2: 5×5 Mode**

1. Navigate to `Assets/ScriptableObjects/Profiles/`
2. Right-click → Create → 9x9 → **Room Generation Profile**
3. Rename to: `5x5Profile`
4. Configure:

```
Grid Size: 5
Room Size: 10
Generate All Rooms: ☐
Sparse Density: 0.4
Seed: 0 (random)
Guarantee Corner To Center Paths: ☑
```

- **Room Templates:** Assign 3-5 room prefabs
- **Exit Room Prefab:** (optional)
- **Spawn Room Prefab:** (optional)

---

#### **Profile 3: 3×3 Mode**

1. Navigate to `Assets/ScriptableObjects/Profiles/`
2. Right-click → Create → 9x9 → **Room Generation Profile**
3. Rename to: `3x3Profile`
4. Configure:

```
Grid Size: 3
Room Size: 10
Generate All Rooms: ☑ (checked - small grid, generate all)
Sparse Density: 1.0 (irrelevant when Generate All is checked)
Seed: 0 (random)
Guarantee Corner To Center Paths: ☑
```

- **Room Templates:** Assign 3-5 room prefabs
- **Exit Room Prefab:** (optional)
- **Spawn Room Prefab:** (optional)

---

#### **Profile 4: Story Mode**

1. Navigate to `Assets/ScriptableObjects/Profiles/`
2. Right-click → Create → 9x9 → **Room Generation Profile**
3. Rename to: `StoryModeProfile`
4. Configure:

```
Grid Size: 9
Room Size: 10
Generate All Rooms: ☐
Sparse Density: 0.3
Seed: 42 (deterministic - same layout every time)
Guarantee Corner To Center Paths: ☑
```

- **Room Templates:** Assign story-specific room prefabs if you have them
- **Exit Room Prefab:** (optional)
- **Spawn Room Prefab:** (optional)

---

#### **Profile 5: Co-op Mode**

1. Navigate to `Assets/ScriptableObjects/Profiles/`
2. Right-click → Create → 9x9 → **Room Generation Profile**
3. Rename to: `CoopModeProfile`
4. Configure:

```
Grid Size: 9
Room Size: 10
Generate All Rooms: ☐
Sparse Density: 0.35
Seed: 123 (deterministic - same layout for all players)
Guarantee Corner To Center Paths: ☑
```

- **Room Templates:** Assign 3-5 room prefabs
- **Exit Room Prefab:** (optional)
- **Spawn Room Prefab:** (optional)

---

## **Step 4: Assign to CubeGameController**

Now you need to assign these assets to the CubeGameController.

1. **Open** `Assets/Prefabs/Game/CubeGameApplication.prefab`
2. **Select** the root GameObject
3. **Find** the `CubeGameController` component in Inspector
4. **Assign** the ScriptableObjects:

### Assignments:
```
Default Room Gen Config: → DefaultRoomGenConfig
Story Mode Profile: → StoryModeProfile
Coop Mode Profile: → CoopModeProfile
Battle Royale Profile: → BattleRoyaleProfile
5x5 Profile: → 5x5Profile
3x3 Profile: → 3x3Profile
```

5. **Save** the prefab (Ctrl+S or File → Save)

---

## **Step 5: Create Offline Player Prefab (If Missing)**

If you don't have an offline player prefab assigned:

### Option A: Create from Scratch

1. **Create** new GameObject in scene: GameObject → Create Empty
2. **Rename** to: `OfflinePlayer`
3. **Add Components:**
   - Character Controller
     - Height: 2
     - Radius: 0.5
     - Center: (0, 1, 0)
   - First Person Controller (your script)
     - Configure speed, jump, mouse sensitivity

4. **Create Camera Child:**
   - Right-click OfflinePlayer → Create Empty
   - Rename to: `Camera`
   - Add Component → Camera
   - Set Tag → MainCamera
   - Position: (0, 1.6, 0)

5. **Save as Prefab:**
   - Drag `OfflinePlayer` from Hierarchy to `Assets/Prefabs/Player/`
   - Delete from scene

6. **Assign to CubeGameController:**
   - Open CubeGameApplication prefab
   - Find `Offline Player Prefab` field
   - Drag your new OfflinePlayer prefab into it

### Option B: Use Existing Networked Player Prefab

If you already have a player prefab for multiplayer:

1. **Duplicate** your existing player prefab
2. **Rename** to: `OfflinePlayer`
3. **Remove** networking components:
   - Remove `NetworkObject`
   - Remove any `NetworkBehaviour` scripts
4. **Ensure** it has:
   - CharacterController
   - FirstPersonController
   - Camera child tagged "MainCamera"
5. **Assign** to CubeGameController's `Offline Player Prefab` field

---

## **Step 6: Create Room Prefabs (If Missing)**

If you don't have room prefabs yet, here's a minimal setup:

### Simple Room Prefab:

1. **Create** new GameObject: Create Empty → Name it `SimpleRoom`

2. **Add Bounds Child:**
   - Right-click SimpleRoom → 3D Object → Cube
   - Rename to: `Bounds`
   - Scale: (10, 10, 10)
   - Disable MeshRenderer if you don't want it visible

3. **Add Floor:**
   - Right-click SimpleRoom → 3D Object → Cube
   - Rename to: `Floor`
   - Scale: (10, 0.1, 10)
   - Position: (0, 0, 0)

4. **Add Walls:**
   - Create 4 cubes for walls (North, South, East, West)
   - Scale each: (10, 10, 0.1) or (0.1, 10, 10)
   - Position at room edges

5. **Save as Prefab:**
   - Drag to `Assets/Prefabs/Rooms/SimpleRoom`

6. **Create Variants:**
   - Duplicate SimpleRoom prefab
   - Rename to SimpleRoom_Variant1, SimpleRoom_Variant2, etc.
   - Add different decorations, hazards, or pickups

7. **Assign to Profiles:**
   - Add these room prefabs to the Room Templates arrays in each profile

---

## **Verification Checklist**

After creating all assets, verify:

- [ ] `DefaultRoomGenConfig` exists in `Assets/ScriptableObjects/`
- [ ] All 5 profile assets exist in `Assets/ScriptableObjects/Profiles/`:
  - [ ] BattleRoyaleProfile
  - [ ] 5x5Profile
  - [ ] 3x3Profile
  - [ ] StoryModeProfile
  - [ ] CoopModeProfile
- [ ] Each profile has at least 1 room prefab in Room Templates array
- [ ] All profiles are assigned to CubeGameController in CubeGameApplication prefab
- [ ] DefaultRoomGenConfig is assigned to CubeGameController
- [ ] OfflinePlayer prefab exists and is assigned to CubeGameController
- [ ] OfflinePlayer has CharacterController, FirstPersonController, and Camera child
- [ ] Camera is tagged "MainCamera"

---

## **Quick Test**

After setup, test the generation:

1. **Enter Play Mode**
2. **Navigate to Main Menu**
3. **Click "New Game"**
4. **Watch Console** for these messages:
   ```
   [9x9] CubeGameApplication initialized
   [9x9] Offline singleplayer initialized
   [9x9] Broadcast StartMatchEvent for offline game initialization
   [9x9 Server] Applied NewGame profile: BattleRoyaleProfile
   [9x9 Server] Generating 9x9x9 maze...
   [9x9 Offline] Spawned offline player at (...)
   ```

5. **Verify** you see rooms generating in the Scene/Game view
6. **Check Hierarchy** for room clones and OfflinePlayer instance

---

## **Troubleshooting**

### "Profile missing room templates"
- Open the profile asset
- Expand Room Templates array
- Drag in at least 1 room prefab

### "m_OfflinePlayerPrefab not assigned"
- Create OfflinePlayer prefab (see Step 5)
- Assign to CubeGameController

### "RoomGenerator not found!"
- CubeGameController auto-creates one
- But it won't have templates unless profiles are configured

### Nothing renders
- See `Documentation/Troubleshooting/OFFLINE_SINGLEPLAYER_CHECKLIST.md`

---

**Last Updated:** November 29, 2025  
**Created By:** GitHub Copilot  
**For Unity Version:** 6000.2.10f1
