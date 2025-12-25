using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Centralized configuration for all hazard types in the game
    /// Easy to modify and test different hazard values
    /// </summary>
    public static class HazardConfig
    {
        public class HazardSettings
        {
            public int Damage;
            public float DamageInterval;  // Time between damage ticks (for continuous hazards)
            public float Duration;        // For time-limited hazards like gas clouds
            public float Cooldown;        // For cooldown-based hazards like spikes
            public float Speed;           // For moving hazards
            public Color VisualColor;
        }

        /// <summary>
        /// Spike Trap - Instant damage with cooldown
        /// </summary>
        public static HazardSettings SpikesConfig = new HazardSettings
        {
            Damage = 25,
            Cooldown = 1.5f,
            VisualColor = new Color(0.8f, 0.2f, 0.2f, 1f)  // Red
        };

        /// <summary>
        /// Laser Beam - Continuous damage while in contact
        /// </summary>
        public static HazardSettings LaserConfig = new HazardSettings
        {
            Damage = 10,
            DamageInterval = 1f,
            VisualColor = new Color(1f, 0f, 0f, 1f)  // Bright red
        };

        /// <summary>
        /// Gas Cloud - Continuous damage over time
        /// </summary>
        public static HazardSettings GasConfig = new HazardSettings
        {
            Damage = 5,
            DamageInterval = 0.5f,
            Duration = 0f,  // 0 = permanent
            VisualColor = new Color(0.5f, 0.8f, 0.2f, 0.6f)  // Green/yellow with alpha
        };

        /// <summary>
        /// Water/Acid Pool - Continuous damage, slows movement
        /// </summary>
        public static HazardSettings AcidPoolConfig = new HazardSettings
        {
            Damage = 8,
            DamageInterval = 0.3f,
            VisualColor = new Color(0.2f, 0.8f, 0.2f, 0.7f)  // Green with alpha
        };

        /// <summary>
        /// Lava Pool - High damage, pushes player away
        /// </summary>
        public static HazardSettings LavaConfig = new HazardSettings
        {
            Damage = 20,
            DamageInterval = 0.5f,
            Speed = 5f,  // Knockback force
            VisualColor = new Color(1f, 0.5f, 0f, 0.8f)  // Orange with alpha
        };

        /// <summary>
        /// Collapsing Walls - Crushes player if caught, damages on impact
        /// </summary>
        public static HazardSettings CollapsingWallsConfig = new HazardSettings
        {
            Damage = 50,
            Speed = 3f,  // Wall collapse speed
            Duration = 2f,  // How long collapse takes
            VisualColor = new Color(0.5f, 0.5f, 0.5f, 1f)  // Gray
        };

        /// <summary>
        /// Electric Field - Damage in pulses, pushes away
        /// </summary>
        public static HazardSettings ElectricFieldConfig = new HazardSettings
        {
            Damage = 15,
            DamageInterval = 0.8f,
            Speed = 3f,  // Push force
            VisualColor = new Color(0f, 0.8f, 1f, 0.6f)  // Cyan with alpha
        };

        /// <summary>
        /// Falling Object - Single damage on impact, then disappears
        /// </summary>
        public static HazardSettings FallingObjectConfig = new HazardSettings
        {
            Damage = 30,
            Speed = 10f,  // Fall speed
            VisualColor = new Color(0.4f, 0.3f, 0.2f, 1f)  // Brown
        };

        /// <summary>
        /// Get config by hazard type name
        /// </summary>
        public static HazardSettings GetConfig(string hazardType)
        {
            return hazardType switch
            {
                "Spikes" => SpikesConfig,
                "Laser" => LaserConfig,
                "Gas" => GasConfig,
                "Acid" => AcidPoolConfig,
                "Lava" => LavaConfig,
                "CollapsingWalls" => CollapsingWallsConfig,
                "Electric" => ElectricFieldConfig,
                "FallingObject" => FallingObjectConfig,
                _ => SpikesConfig  // Default fallback
            };
        }

        /// <summary>
        /// Print all hazard configs for debugging
        /// </summary>
        public static void LogAllConfigs()
        {
            Debug.Log("[HazardConfig] ===== HAZARD CONFIGURATIONS =====");
            Debug.Log($"[HazardConfig] Spikes: Damage={SpikesConfig.Damage}, Cooldown={SpikesConfig.Cooldown}");
            Debug.Log($"[HazardConfig] Laser: Damage={LaserConfig.Damage}, Interval={LaserConfig.DamageInterval}");
            Debug.Log($"[HazardConfig] Gas: Damage={GasConfig.Damage}, Interval={GasConfig.DamageInterval}");
            Debug.Log($"[HazardConfig] Acid: Damage={AcidPoolConfig.Damage}, Interval={AcidPoolConfig.DamageInterval}");
            Debug.Log($"[HazardConfig] Lava: Damage={LavaConfig.Damage}, Knockback={LavaConfig.Speed}");
            Debug.Log($"[HazardConfig] Collapsing Walls: Damage={CollapsingWallsConfig.Damage}, Speed={CollapsingWallsConfig.Speed}");
            Debug.Log($"[HazardConfig] Electric Field: Damage={ElectricFieldConfig.Damage}, Push={ElectricFieldConfig.Speed}");
            Debug.Log($"[HazardConfig] Falling Object: Damage={FallingObjectConfig.Damage}, Fall Speed={FallingObjectConfig.Speed}");
            Debug.Log("[HazardConfig] =====================================");
        }
    }
}
