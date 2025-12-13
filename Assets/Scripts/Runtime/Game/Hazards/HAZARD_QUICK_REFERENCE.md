# Hazard System Quick Reference

## Available Hazards

| Hazard | Type | Damage | Effect | Use Case |
|--------|------|--------|--------|----------|
| **Spikes** | Instant+Cooldown | 25 | Single hit per 1.5s | Floor/wall traps |
| **Laser** | Continuous | 10/s | Beam damage | Moving/static beams |
| **Gas** | Continuous | 5/0.5s | Damage + Slow | Area denial |
| **Acid** | Continuous | 8/0.3s | Damage + Push | Liquid hazards |
| **Lava** | Continuous | 20/0.5s | High damage + Push | Death zones |
| **Walls** | Timed | 50 | Crush on 2 sides | Room mechanics |
| **Electric** | Pulse | 15/0.8s | Area damage + Push | Center hazards |

## Damage per Second (DPS)

- Spikes: 16.7 DPS (assuming hits every 1.5s)
- Laser: 10 DPS
- Gas: 10 DPS
- Acid: 26.7 DPS
- Lava: 40 DPS
- Electric: 18.75 DPS

## Configuration Quick Edit

File: `HazardConfig.cs`

```csharp
// Damage multiplier (all types)
config.Damage *= 1.5f;  // 50% more damage

// Intervals (continuous hazards)
GasConfig.DamageInterval = 0.5f;      // Faster ticks
LaserConfig.DamageInterval = 2f;      // Slower damage

// Speed (movement/knockback)
LavaConfig.Speed = 10f;               // Stronger push
CollapsingWallsConfig.Speed = 5f;     // Faster collapse

// Cooldown (spikes)
SpikesConfig.Cooldown = 2f;           // Harder to avoid
SpikesConfig.Cooldown = 1f;           // Easier to avoid
```

## Testing Scenarios

### 1. Damage Output Test
```
Player health: 100
Place player in Lava for 5 seconds
Expected: 100 - (20 × 5) = 0 (dead)
Actual: ___
```

### 2. Knockback Test
```
Place player in Acid/Lava
Expected: Player pushed away
Actual: ___
```

### 3. Multiple Hazards
```
Room with: Spikes + Gas
Expected: Both damage simultaneously
Actual: ___
```

### 4. Collapsing Walls
```
Enter room with collapsing walls
Expected: 2 random walls collapse after 8s
Actual: ___
```

## Recommended Balancing

### Easy Mode
- Spike Cooldown: 3s
- Gas Damage: 2/1s
- Lava Knockback: 4
- Collapse Cooldown: 15s

### Normal Mode (Default)
- Spike Cooldown: 1.5s
- Gas Damage: 5/0.5s
- Lava Knockback: 8
- Collapse Cooldown: 8s

### Hard Mode
- Spike Cooldown: 0.5s
- Gas Damage: 10/0.3s
- Lava Knockback: 12
- Collapse Cooldown: 4s

## Debug Commands

```csharp
// Log all hazard configs
HazardConfig.LogAllConfigs();

// Test damage
GetComponent<FirstPersonController>().TakeDamage(25);

// Check hazard in area
HazardBase hazard = GetComponent<HazardBase>();
Debug.Log($"Bodies in hazard: {hazard.GetBodiesInArea().Count}");
```

## Inspector Setup Checklists

### Trigger Hazard (Spikes, Laser, Gas, Acid, Lava)
- [ ] GameObject created
- [ ] Collider added
- [ ] "Is Trigger" checked
- [ ] Hazard script added
- [ ] Color indicator applied
- [ ] Tested with player

### Collapsing Walls
- [ ] 4 walls created (North, South, East, West)
- [ ] Walls positioned at room edges
- [ ] CollapseController GameObject created
- [ ] Script attached to controller
- [ ] All 4 walls assigned in inspector
- [ ] Cooldown/Duration tested

### Electric Field
- [ ] Sphere/Cube created
- [ ] Collider added
- [ ] "Is Trigger" checked
- [ ] Script attached
- [ ] Pulse Radius set correctly
- [ ] Visual effects applied

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| No damage taken | Collider not a trigger | Check "Is Trigger" in collider |
| Bot not damaged | Bot script not found | Ensure BotController exists |
| Walls don't collapse | Walls not assigned | Drag walls into inspector fields |
| Continuous damage too slow | Interval too long | Reduce DamageInterval |
| Knockback too weak | Speed value too low | Increase Speed parameter |
| Electric field not pulsing | Script not running | Check if component enabled |

## Class Activities

### Activity 1: Balance Tuning
1. Create test room with 1 hazard
2. Record how fast player dies
3. Adjust config values
4. Document DPS changes
5. Present findings

### Activity 2: Hazard Combinations
1. Create room with 2-3 hazards
2. Test synergies (e.g., Spikes + Gas)
3. Determine if balanced
4. Modify config to improve

### Activity 3: Custom Hazard
1. Copy HazardBase template
2. Create new hazard type
3. Add to HazardConfig.cs
4. Test in room
5. Present to class

---

**Quick Links:**
- Full Setup Guide: [HAZARD_SETUP.md](HAZARD_SETUP.md)
- Config File: `Assets/Scripts/Runtime/Game/Hazards/HazardConfig.cs`
- Base Class: `Assets/Scripts/Runtime/Game/Hazards/HazardBase.cs`
