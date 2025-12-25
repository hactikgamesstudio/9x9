# Flashlight System Setup Guide

Date: November 26, 2025  
For: 9x9 Unity Cube Maze Escape Game

---

## Overview

Two flashlight variants:
1. **Character Body Flashlight** - Attached to player's head/camera, always follows look direction
2. **Weapon Flashlight** - Attached to gun, toggleable tactical light

---

## Part 1: Character Body Flashlight

### Step 1: Create Flashlight Script

**File:** `Assets/Scripts/Runtime/Game/Player/Flashlight.cs`

```csharp
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Character-mounted flashlight that follows camera look direction.
    /// Toggle on/off with F key.
    /// </summary>
    public class Flashlight : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private Light m_Light;
        [SerializeField] private float m_Range = 15f;
        [SerializeField] private float m_SpotAngle = 60f;
        [SerializeField] private Color m_LightColor = Color.white;
        [SerializeField] private float m_Intensity = 2f;

        [Header("Battery System (Optional)")]
        [SerializeField] private bool m_UseBattery = false;
        [SerializeField] private float m_MaxBatteryLife = 300f; // 5 minutes
        private float m_CurrentBattery;

        [Header("Controls")]
        [SerializeField] private KeyCode m_ToggleKey = KeyCode.F;

        private bool m_IsOn = true;

        void Awake()
        {
            // Auto-create light if not assigned
            if (m_Light == null)
            {
                var lightGO = new GameObject("FlashlightSpot");
                lightGO.transform.SetParent(transform);
                lightGO.transform.localPosition = Vector3.zero;
                lightGO.transform.localRotation = Quaternion.identity;
                
                m_Light = lightGO.AddComponent<Light>();
            }

            ConfigureLight();
            m_CurrentBattery = m_MaxBatteryLife;
        }

        void ConfigureLight()
        {
            m_Light.type = LightType.Spot;
            m_Light.range = m_Range;
            m_Light.spotAngle = m_SpotAngle;
            m_Light.color = m_LightColor;
            m_Light.intensity = m_Intensity;
            m_Light.shadows = LightShadows.Soft; // Or Hard for performance
            m_Light.enabled = m_IsOn;
        }

        void Update()
        {
            // Toggle flashlight
            if (Input.GetKeyDown(m_ToggleKey))
            {
                m_IsOn = !m_IsOn;
                m_Light.enabled = m_IsOn && m_CurrentBattery > 0;
            }

            // Battery drain (optional)
            if (m_UseBattery && m_IsOn && m_CurrentBattery > 0)
            {
                m_CurrentBattery -= Time.deltaTime;
                if (m_CurrentBattery <= 0)
                {
                    m_CurrentBattery = 0;
                    m_Light.enabled = false;
                    Debug.Log("Flashlight battery depleted!");
                }
            }
        }

        /// <summary>
        /// Recharge battery (e.g., from pickup item)
        /// </summary>
        public void Recharge(float amount)
        {
            m_CurrentBattery = Mathf.Min(m_CurrentBattery + amount, m_MaxBatteryLife);
        }

        public float BatteryPercent => m_CurrentBattery / m_MaxBatteryLife;
    }
}
```

### Step 2: Attach to Player

**In CubeGameApplication Prefab or Offline Player Prefab:**

1. Find the **Camera** child object (or create one if using fallback player)
2. Add Component → **Flashlight**
3. Leave **Light** field empty (auto-creates)
4. Configure settings:
   - Range: `15` (adjust for maze size)
   - Spot Angle: `60` (wider = more coverage, dimmer edges)
   - Intensity: `2` (brightness)
   - Use Battery: `☐` unchecked for infinite (or `☑` for realism)

### Step 3: Test

- Play the game
- Press **F** to toggle flashlight on/off
- Adjust Range/Intensity in Inspector while playing to tune

---

## Part 2: Weapon-Mounted Flashlight

### Step 1: Create Weapon Flashlight Script

**File:** `Assets/Scripts/Runtime/Game/Weapons/WeaponFlashlight.cs`

```csharp
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Tactical flashlight attached to weapon.
    /// Toggle on/off independently from character flashlight.
    /// </summary>
    public class WeaponFlashlight : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private Light m_Light;
        [SerializeField] private float m_Range = 20f;
        [SerializeField] private float m_SpotAngle = 30f; // Narrower for tactical
        [SerializeField] private Color m_LightColor = new Color(1f, 0.95f, 0.8f); // Slightly warm
        [SerializeField] private float m_Intensity = 3f; // Brighter than body light

        [Header("Controls")]
        [SerializeField] private KeyCode m_ToggleKey = KeyCode.T; // T for Tactical

        [Header("Visual (Optional)")]
        [SerializeField] private MeshRenderer m_LightGlowMesh; // Optional glow mesh
        [SerializeField] private Material m_OnMaterial;
        [SerializeField] private Material m_OffMaterial;

        private bool m_IsOn = false; // Default off

        void Awake()
        {
            if (m_Light == null)
            {
                var lightGO = new GameObject("WeaponLight");
                lightGO.transform.SetParent(transform);
                lightGO.transform.localPosition = new Vector3(0, 0, 0.5f); // Forward of weapon
                lightGO.transform.localRotation = Quaternion.identity;
                
                m_Light = lightGO.AddComponent<Light>();
            }

            ConfigureLight();
        }

        void ConfigureLight()
        {
            m_Light.type = LightType.Spot;
            m_Light.range = m_Range;
            m_Light.spotAngle = m_SpotAngle;
            m_Light.color = m_LightColor;
            m_Light.intensity = m_Intensity;
            m_Light.shadows = LightShadows.Hard; // Performance
            m_Light.enabled = m_IsOn;

            UpdateVisuals();
        }

        void Update()
        {
            if (Input.GetKeyDown(m_ToggleKey))
            {
                m_IsOn = !m_IsOn;
                m_Light.enabled = m_IsOn;
                UpdateVisuals();
            }
        }

        void UpdateVisuals()
        {
            if (m_LightGlowMesh != null && m_OnMaterial != null && m_OffMaterial != null)
            {
                m_LightGlowMesh.material = m_IsOn ? m_OnMaterial : m_OffMaterial;
            }
        }

        /// <summary>
        /// Force flashlight state (useful for weapon switching)
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            m_IsOn = enabled;
            m_Light.enabled = m_IsOn;
            UpdateVisuals();
        }
    }
}
```

### Step 2: Add to Weapon Prefab

**If you have a weapon prefab:**

1. Open weapon prefab (e.g., `Assets/Prefabs/Weapons/Rifle.prefab`)
2. Create child GameObject → Name it `TacticalLight`
3. Position it at the barrel/rail mount point (usually forward and slightly below)
4. Add Component → **Weapon Flashlight**
5. Configure:
   - Range: `20` (longer than body light)
   - Spot Angle: `30` (focused beam)
   - Intensity: `3`

**If you don't have weapons yet:**

Skip this for now; when you create weapon prefabs, add the `WeaponFlashlight` component to each that should support it.

### Step 3: Optional Visual Glow

**Add a simple glow mesh to show when light is on:**

1. In weapon prefab, under `TacticalLight`, add a child **Cube**
2. Scale it small: `(0.05, 0.05, 0.1)` (thin rectangle)
3. Position at light source
4. Create two materials:
   - `Assets/Materials/FlashlightOn.mat` → Emission enabled, bright yellow
   - `Assets/Materials/FlashlightOff.mat` → No emission, dark gray
5. Assign materials to `WeaponFlashlight` component:
   - Light Glow Mesh → Cube's MeshRenderer
   - On Material → FlashlightOn
   - Off Material → FlashlightOff

---

## Part 3: Performance Optimization

### Shadow Settings

For maze performance, tune light shadows:

**Character Flashlight:**
- Shadows: `Soft` (better quality, medium cost)
- Shadow Resolution: `Medium`
- Shadow Bias: `0.05`

**Weapon Flashlight:**
- Shadows: `Hard` (faster, sharper edges)
- Shadow Resolution: `Low` (tactical lights are secondary)
- Shadow Bias: `0.1`

### Light Culling

Add a simple distance check to disable lights when not needed:

```csharp
void Update()
{
    // Disable light if player is too far from any enemy/hazard
    float distanceToNearestThreat = GetNearestThreatDistance();
    if (distanceToNearestThreat > 50f)
    {
        m_Light.enabled = false;
    }
}
```

### URP Light Layers (Optional)

If using URP light layers:
- Character Flashlight → Layer "Player"
- Weapon Flashlight → Layer "Weapons"
- Configure which objects receive which lights

---

## Part 4: Integration with Existing Systems

### Inventory/Pickup Integration

**Battery Pickup Item:**

```csharp
// In PickupItem.cs or new BatteryPickup.cs
void OnTriggerEnter(Collider other)
{
    var flashlight = other.GetComponentInChildren<Flashlight>();
    if (flashlight != null)
    {
        flashlight.Recharge(60f); // 1 minute of battery
        Destroy(gameObject);
    }
}
```

### HUD Integration

**Show battery level in HUD:**

```csharp
// In HUDController.cs
void Update()
{
    var flashlight = m_Player.GetComponentInChildren<Flashlight>();
    if (flashlight != null)
    {
        m_BatterySlider.value = flashlight.BatteryPercent * 100f;
    }
}
```

---

## Quick Setup Checklist

### Character Flashlight (Immediate - Do This First)
- [ ] Create `Flashlight.cs` script
- [ ] Add to Camera in Player prefab
- [ ] Test toggle with F key
- [ ] Tune Range/Intensity for maze visibility

### Weapon Flashlight (Later - When Adding Weapons)
- [ ] Create `WeaponFlashlight.cs` script
- [ ] Add to weapon prefabs at barrel/rail position
- [ ] Add optional glow mesh + materials
- [ ] Test toggle with T key

### Optional Enhancements
- [ ] Battery system with pickups
- [ ] HUD battery indicator
- [ ] Flickering effect when battery low
- [ ] Sound effects (click on/off, battery warning beep)

---

## Troubleshooting

**Flashlight doesn't illuminate anything:**
- Check light Range (increase it)
- Ensure light Shadows are not set to None (use Soft or Hard)
- Verify light is child of Camera (for body) or weapon (for tactical)

**Performance issues:**
- Reduce Shadow Resolution to Low
- Use Hard shadows instead of Soft
- Disable one flashlight (keep only body or weapon active)
- Reduce light Range

**Light doesn't rotate with camera:**
- Ensure Flashlight component is on Camera GameObject, not Player root
- Check transform hierarchy: Player → Camera → Flashlight

**Toggle key doesn't work:**
- Verify Input System is not blocking legacy Input (use New Input System callbacks if needed)
- Check for key conflicts (F might be bound elsewhere)

---

**Next Steps After Setup:**
1. Add ambient maze lighting (dim Directional Light at intensity 0.1–0.2)
2. Add point lights in key rooms (exit, spawn) for landmarks
3. Add emissive materials to hazards for visibility without light
4. Test battery depletion for horror/tension gameplay

Created: November 26, 2025  
Maintained by: Development Team
