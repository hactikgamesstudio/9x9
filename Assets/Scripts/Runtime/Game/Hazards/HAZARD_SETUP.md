# Hazard System Setup Guide

## Overview
The hazard system provides a flexible framework for creating environmental dangers in the 9x9 cube maze. All hazards are configurable through `HazardConfig.cs` and use a base class system for consistency.

## Configuration

### HazardConfig.cs
Located at: `Assets/Scripts/Runtime/Game/Hazards/HazardConfig.cs`

This static class centralizes all hazard settings. Modify these values to adjust hazard behavior:

```csharp
HazardConfig.SpikesConfig.Damage = 25;
HazardConfig.LaserConfig.DamageInterval = 1f;
HazardConfig.GasConfig.Damage = 5;
// etc...
```

**Quick Config Changes:**
```
HazardConfig.SpikesConfig.Cooldown = 2f;  // Spike hit cooldown
HazardConfig.LaserConfig.Damage = 15;     // Laser damage per tick
HazardConfig.LavaConfig.Speed = 8f;       // Knockback strength
```

Debug all configs:
```csharp
HazardConfig.LogAllConfigs();  // Prints all hazard settings
```

---

## Hazard Types

### 1. Spikes (HazardSpikes.cs)
**Type:** Instant damage with cooldown

**Setup:**
1. Create a 3D Cube
2. Scale to spike shape (e.g., 0.5x3x0.5)
3. Add Component → Sphere Collider
4. Check "Is Trigger"
5. Add Component → HazardSpikes
6. Adjust Damage and Cooldown in Inspector

**Config Values:**
- Damage: 25
- Cooldown: 1.5s

**Behavior:**
- Deals damage on first contact
- Waits for cooldown before next damage
- Perfect for floor/wall spikes

---

### 2. Laser (HazardLaser.cs)
**Type:** Continuous damage while in contact

**Setup:**
1. Create a 3D Cube (thin, long shape for laser)
2. Scale appropriately (e.g., 0.2x0.2x10)
3. Add Component → Box Collider
4. Check "Is Trigger"
5. Add Component → HazardLaser

**Config Values:**
- Damage: 10
- Damage Interval: 1s

**Behavior:**
- Deals damage repeatedly while target is touching
- Good for beam/barrier hazards

---

### 3. Gas Cloud (HazardGas.cs)
**Type:** Continuous damage + movement slowdown

**Setup:**
1. Create a 3D Sphere or Cube
2. Add Component → Sphere/Box Collider
3. Check "Is Trigger"
4. Add Component → HazardGas
5. Adjust Movement Slow Multiplier (0.7 = 30% slower)

**Config Values:**
- Damage: 5
- Damage Interval: 0.5s
- Visual Color: Green/yellow

**Behavior:**
- Continuous damage ticks
- Visual indicator (green)
- Optional: Slows player movement

**Tip:** Tint the renderer green to match gas cloud appearance

---

### 4. Acid Pool (HazardAcidPool.cs)
**Type:** Continuous damage + knockback

**Setup:**
1. Create a 3D Plane or Cube (flat pool shape)
2. Add Component → Box Collider
3. Check "Is Trigger"
4. Add Component → HazardAcidPool
5. Adjust Knockback Force (2-5 typical)

**Config Values:**
- Damage: 8
- Damage Interval: 0.3s
- Movement Slow: 50%

**Behavior:**
- Rapid damage ticks
- Pushes targets away on contact
- Slows movement while inside

**Tip:** Use green color with transparency for acid effect

---

### 5. Lava Pool (HazardLava.cs)
**Type:** High damage + strong knockback

**Setup:**
1. Create a 3D Plane (lava surface)
2. Add Component → Box Collider
3. Check "Is Trigger"
4. Add Component → HazardLava
5. Adjust Knockback Force (6-10 for strong push)

**Config Values:**
- Damage: 20
- Damage Interval: 0.5s
- Knockback: 8 (strong)

**Behavior:**
- High damage per tick
- Constant knockback to push out
- Very dangerous zone

**Tip:** Use orange/red color with glow effect

---

### 6. Collapsing Walls (HazardCollapsingWalls.cs)
**Type:** Timed trap with periodic crushes

**Setup:**
1. Create room with 4 walls (North, South, East, West)
   - Create 4 Cubes positioned at room edges
   - Name them appropriately
2. Create empty GameObject called "CollapseController"
3. Add Component → HazardCollapsingWalls
4. Drag the 4 walls into the inspector fields
5. Adjust collapse parameters

**Key Parameters:**
- Collapse Speed: 3 (units/sec)
- Collapse Duration: 2 (seconds)
- Collapse Cooldown: 8 (seconds between collapses)
- Crush Damage: 50

**Behavior:**
- Selects 2 random walls every 8 seconds
- Walls slide inward over 2 seconds
- Deals 50 damage to anything trapped
- Walls reset to original position

**Advanced:**
- Increase Collapse Speed for faster danger
- Adjust Cooldown for frequency
- Higher Crush Damage = more lethal

---

### 7. Electric Field (HazardElectricField.cs)
**Type:** Pulsing hazard with area effect

**Setup:**
1. Create a Cube or Sphere (field area)
2. Add Component → Sphere Collider
3. Check "Is Trigger" (optional - pulses hit everything in radius)
4. Add Component → HazardElectricField
5. Adjust Pulse Radius and Push Force

**Key Parameters:**
- Pulse Radius: 5 (detection area)
- Push Force: 4 (knockback strength)
- Damage Interval: 0.8s (pulse frequency)

**Behavior:**
- Pulses every 0.8 seconds
- Damages and pushes all targets in radius
- Great for center-of-room hazards

**Tip:** Add a glowing sphere effect for visual feedback

---

## Testing Checklist

### Individual Hazard Tests
- [ ] Spikes deal damage and have cooldown
- [ ] Laser continuous damage works
- [ ] Gas slows movement
- [ ] Acid pushes player away
- [ ] Lava deals high damage
- [ ] Collapsing walls select random sides
- [ ] Electric field pulses properly

### Integration Tests
- [ ] Player takes damage from all hazard types
- [ ] Bots take damage from all hazard types
- [ ] Multiple hazards in same room work
- [ ] Damage doesn't exceed MaxHealth cap
- [ ] Player death by hazard triggers defeat

### Class Activity
1. **Create a test room** with 3-4 different hazards
2. **Record videos** of hazard behavior
3. **Adjust configs** in HazardConfig.cs
4. **Document behavior** changes
5. **Demo to class** with different settings

---

## Creating Custom Hazards

### Template

```csharp
public class HazardCustom : HazardBase
{
    protected override void Start()
    {
        m_HazardType = "CustomName";
        base.Start();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        m_BodiesInArea.Add(other);
        // Custom behavior
        DamageTarget(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        // Continuous effects
    }

    protected override void OnTriggerExit(Collider other)
    {
        m_BodiesInArea.Remove(other);
    }
}
```

### Key Methods
- `DamageTarget(Collider)` - Damages player or bot
- `GetBodiesInArea()` - Get all entities in hazard
- `m_Config` - Access settings from HazardConfig

---

## Adding New Hazard Type to Config

1. Add new `HazardSettings` to `HazardConfig.cs`:
```csharp
public static HazardSettings CustomHazardConfig = new HazardSettings
{
    Damage = 15,
    DamageInterval = 0.5f,
    VisualColor = new Color(1f, 0f, 1f, 1f)  // Magenta
};
```

2. Update `GetConfig()` switch statement:
```csharp
public static HazardSettings GetConfig(string hazardType)
{
    return hazardType switch
    {
        // ... existing cases ...
        "CustomHazard" => CustomHazardConfig,
        _ => SpikesConfig
    };
}
```

3. Create new script inheriting `HazardBase`
4. Set `m_HazardType = "CustomHazard"` in Start()

---

## Console Debugging

Enable debug logs to track hazard behavior:

```csharp
// In any hazard or test script
Debug.Log($"[HazardName] Target damaged for {damage}");

// Log all configs
HazardConfig.LogAllConfigs();

// Check if entity is in hazard
if (myHazard.GetBodiesInArea().Count > 0)
    Debug.Log("Entities in hazard!");
```

---

## Quick Customization Tips

1. **Make spikes more deadly:** `SpikesConfig.Damage = 50`
2. **Make gas less annoying:** `GasConfig.DamageInterval = 2f`
3. **Make lava escape easier:** `LavaConfig.Speed = 15f` (stronger knockback)
4. **Faster collapsing walls:** `CollapsingWallsConfig.Speed = 6f`
5. **Make electric field bigger:** Increase Pulse Radius in inspector

---

## File Structure

```
Assets/Scripts/Runtime/Game/Hazards/
├── HazardConfig.cs              # Config storage
├── HazardBase.cs                # Base class
├── HazardSpikes.cs              # Spike trap
├── HazardLaser.cs               # Laser beam
├── HazardGas.cs                 # Gas cloud
├── HazardAcidPool.cs            # Acid/water
├── HazardLava.cs                # Lava pool
├── HazardCollapsingWalls.cs     # Crushing walls
├── HazardElectricField.cs       # Electric pulses
└── HAZARD_SETUP.md              # This file
```

---

## Next Steps

1. ✅ Review existing hazard implementations
2. ✅ Test each hazard in a dedicated test room
3. ✅ Adjust HazardConfig values for balance
4. ✅ Add hazards to procedural room generation
5. ✅ Create hazard visual effects (colors, particles)
6. ✅ Document balance decisions

---

**Last Updated:** December 12, 2025
**Version:** 1.0 - Initial Hazard System
