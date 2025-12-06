# Integration Test Checklist & Code Path Reference

Date: 2025-11-21
Scope: Singleplayer bots, hazards, victory/defeat, event decoupling, procedural room generation.

---
## 1. Manual Integration Test (Unity Editor)
Scene: `Assets/Scenes/MetagameScene.unity`

### Startup
1. Open scene; enter Play Mode.
2. Confirm main menu UI renders (title + singleplayer option + bot count selector).

### Single Player Launch
3. Set bot count (test values: 0, 1, 3, 7).
4. Click New Game.
5. Verify maze generates (Player spawns at a corner, position roughly at one cube extremity).

### Bot Spawn Verification
6. In Hierarchy, verify number of bot GameObjects equals selected bot count (none when 0).
7. Observe bots begin movement toward player or chosen target.

### Hazard Interaction
8. Lead player into spike hazard: health decreases; UI updates.
9. Lure a bot into spikes / lasers: bot health decreases; elimination when <= 0.
10. Confirm no exceptions in Console during hazard damage.

### Victory Condition
11. Eliminate all bots (hazards or direct damage routine).
12. Verify win event triggers: victory screen or end state displayed.

### Defeat Condition
13. Allow player health to reach 0.
14. Confirm defeat screen appears and input is disabled (movement stops).

### Inventory Interaction
15. Pick up at least one item: hotbar slot updates; Console free of errors.

### Room Rotation
16. Exit a room to trigger any rotation mechanism; re-enter another room.
17. Ensure bots pathfinding / targeting not broken by rotated transforms.

### Center Exit
18. Navigate toward approximate center coordinates (grid cell 4,4,4 if accessible).
19. Verify no conflict between exit logic and victory condition (if both implemented).

### Profile Persistence
20. Return to main menu; open profile view; confirm session stats updated (wins/defeats).

### Batch Recompile Utility (Optional)
21. Exit Play Mode; run `Tools/UnityBatchRecompile.ps1` (after setting UNITY_EDITOR_PATH) to confirm clean compile.

Success Criteria:
- No runtime exceptions.
- Correct bot count spawn.
- Hazards damage player and bots consistently.
- Victory after last bot eliminated; defeat on player death.
- Inventory updates visually.
- Rotation does not break logic.

---
## 2. Key Code Paths (High-Level)

### Bot Spawning Flow
Menu -> Start event (includes bot count) -> `CustomNetworkManager` initialization -> `BotManager.SpawnBot(Vector3 position)` for each bot -> `BotController` Awake/Start.

### Bot Behavior / Targeting
`BotController` acquires target via nearest player lookup (reflection or tag search) or random selection fallback. Movement updates transform toward target; rotation via `Quaternion.LookRotation`.

### Bot Damage & Elimination
Hazards call `TakeDamage(int)` (reflection or component check). When health <= 0: bot invokes elimination sequence, notifies `BotManager`. Manager decrements active bot count and checks victory condition.

### Player Health & Death
`FirstPersonController.TakeDamage(int)` reduces health; at 0 calls `Die()`. `Die()` broadcasts defeat event via reflection into central event system. UI layer listens and switches to defeat screen.

### Hazard Interaction
`HazardSpikes` OnTriggerEnter applies instant damage with cooldown gating. `HazardLaser` maintains dictionary of entities with damage interval timers. Both differentiate player vs bot (tag/component/reflection) without direct assembly coupling.

### Victory Condition
`BotManager` tracks active bots. On each elimination, if remaining == 0 and player alive, broadcasts win event (reflection) to UI controller for victory screen transition.

### Procedural Room Generation
`RoomGenerator` builds sparse or full 9×9×9 grid (729 cells); ensures path connectivity from all corners to center (4,4,4). Provides spawn positions for player and bots (direct method or reflection access). Optional rotation post-room exit.

### Event Broadcasting (Decoupled)
Non-core scripts (bots/player) use reflection to invoke central AppEvent methods to avoid CS0433 type ambiguity. Pattern: locate event class type, fetch Broadcast/Invoke method with `BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic`, then invoke with payload.

### Inventory System
`InventorySystem.AddItem(id, qty)` updates internal dictionary and raises `InventoryChanged`. HUD listens to update hotbar slots. Persistence handled by profile manager on session end.

### Profile & Persistence
Stats (wins, deaths) updated after victory/defeat events. Stored via JSON or PlayerPrefs (depending on implementation) and reflected in profile menu.

### Batch Recompile Utility
Script: `Tools/UnityBatchRecompile.ps1`. Uses UNITY_EDITOR_PATH or known paths, runs Unity headless in batchmode, outputs `unity_recompile.log`. Exit code 0 => success.

---
## 3. Troubleshooting Signals

| Symptom | Likely Cause | Check |
|---------|--------------|-------|
| Bots not spawning | Start event not passing count | Verify menu controller event payload |
| Hazards ignore bots | Tag/component mismatch | Confirm bot has expected tag or reflected type |
| Victory never triggers | BotManager count not decrementing | Ensure elimination calls manager update |
| Player death no screen | Reflection broadcast failed | Check console for missing method errors |
| Rotation breaks movement | Using world vs local rotations incorrectly | Inspect transforms after rotation |
| Inventory not updating | HUD not subscribed to InventoryChanged | Confirm subscription in HUDController Start |

---
## 4. Suggested Future Automated Tests
- Spawn count parameterization (0..7).
- Health reduction on hazard contact edge cases.
- Victory path asserts when last bot removed.
- Defeat path asserts when health <= 0.
- Inventory add/remove event propagation.
- Room generation connectivity (corner -> center path existence).

---
## 5. Execution Order Dependencies
1. Menu initialization (defines bot count)
2. Game start event dispatch
3. Network/Gameplay application instantiation
4. Room generation (rooms available before bot spawn)
5. Bot spawn (requires spawn positions)
6. Player spawn + HUD binding
7. Hazard interactions (after all entities active)
8. Victory/Defeat evaluation

---
## 6. Environment Notes
- Ensure Unity version matches 6000.2.10f1 to avoid API drift.
- Batch recompile requires valid editor path; set `UNITY_EDITOR_PATH` variable if detection fails.
- Reflection-based event broadcasts rely on method names; renames must propagate.

---
## 7. Minimal Console Watchlist During Test
Look specifically for:
- NullReferenceException (missing component on bot/player)
- MissingMethodException (reflection broadcast target changed)
- InvalidOperationException (collection modified during hazard iteration)
- ArgumentException (Quaternion.LookRotation with zero vector)

---
## 8. Manual Cleanup After Test
- Stop Play Mode before running batch recompile.
- Clear Console; rerun steps if anomalies appear.
- Commit only if zero runtime exceptions.

---
Maintained by: Integration Automation Helper
Last Updated: 2025-11-21
