using System.Collections.Generic;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// 9x9x9 Cube Grid Room Generator inspired by the film "Cube" (1997).
    /// Creates a perfect cubic grid of rooms with players spawning at corners and exit in center.
    /// 
    /// **FOR BEGINNERS - UNDERSTANDING THE 9x9x9 CUBE:**
    /// This generates a massive 3D grid of 729 rooms (9×9×9).
    /// 
    /// Grid Structure:
    /// - X axis: 0 to 8 (9 rooms wide)
    /// - Y axis: 0 to 8 (9 rooms tall)
    /// - Z axis: 0 to 8 (9 rooms deep)
    /// - Center room: (4, 4, 4) - Contains the exit
    /// - Corner spawn points: (0,0,0), (8,0,0), (0,0,8), (8,0,8), etc.
    /// 
    /// Room Placement:
    /// - Each room is exactly aligned to a grid cell
    /// - Room size determined by roomSize variable (default 10 units)
    /// - Doors automatically connect adjacent rooms
    /// 
    /// **PREFAB REQUIREMENTS:**
    /// Each room prefab must have:
    /// - Consistent size matching roomSize setting
    /// - Optional: "door_north", "door_south", "door_east", "door_west", "door_up", "door_down"
    /// - These are just markers, doors open to all adjacent rooms automatically
    /// </summary>
    public class RoomGenerator : MonoBehaviour
    {
        #region Inspector Variables
        
        [Header("Cube Grid Settings")]
        [Tooltip("Size of the cube grid (9 = 9x9x9 = 729 rooms)")]
        [SerializeField] private int m_GridSize = 9;
        
        [Tooltip("Physical size of each room in world units")]
        [SerializeField] private float m_RoomSize = 10f;
        
        [Header("Room Templates")]
        [Tooltip("Room prefabs to randomly choose from (standard rooms)")]
        [SerializeField] private GameObject[] m_RoomTemplates;
        
        [Tooltip("Special room prefab for the center exit room (optional)")]
        [SerializeField] private GameObject m_ExitRoomPrefab;
        
        [Tooltip("Special room prefab for corner spawn rooms (optional)")]
        [SerializeField] private GameObject m_SpawnRoomPrefab;
        
        [Header("Door Settings")]
        [Tooltip("Percentage of doors that are false/locked (0.0 to 0.5)")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float m_FalseDoorChance = 0.15f;
        
        [Tooltip("Enable room rotation after doors close (Cube film mechanic)")]
        [SerializeField] private bool m_EnableRoomRotation = true;
        
        [Tooltip("Time in seconds before rooms can rotate again")]
        [SerializeField] private float m_RotationCooldown = 10f;
        
        [Header("Generation Settings")]
        [Tooltip("Random seed for reproducible generation (0 = random)")]
        [SerializeField] private int m_Seed = 0;
        
        [Tooltip("Auto-generate on Start()? (Disable if using CubeGameController)")]
        [SerializeField] private bool m_GenerateOnStart = false;
        
        [Tooltip("Generate all 729 rooms or use sparse generation?")]
        [SerializeField] private bool m_GenerateAllRooms = false;
        
        [Tooltip("If sparse, what percentage of rooms to generate (0.1 = 10%)")]
        [SerializeField, Range(0.1f, 1f)]
        private float m_SparseDensity = 0.3f;
        
        #endregion
        
        #region Private Variables
        
        /// <summary>
        /// Random number generator for reproducible generation.
        /// </summary>
        private System.Random m_RNG;
        
        /// <summary>
        /// 3D array storing all room instances [x, y, z].
        /// Null entries = no room generated at that position.
        /// </summary>
        private GameObject[,,] m_RoomGrid;
        
        /// <summary>
        /// List of all placed room instances for easy iteration.
        /// </summary>
        private List<GameObject> m_PlacedRooms = new List<GameObject>();
        
        /// <summary>
        /// Center position of the grid (where exit room is placed).
        /// </summary>
        private Vector3Int m_CenterCell;
        
        /// <summary>
        /// Corner positions where players can spawn.
        /// </summary>
        private List<Vector3Int> m_CornerCells = new List<Vector3Int>();
        
        #endregion
        
        #region Unity Lifecycle
        
        void Start()
        {
            // Generate on start if enabled and we have at least one template (or special room prefabs)
            // This allows the generator to work even if only exit/spawn prefabs are assigned
            if (
                m_GenerateOnStart
                && (
                    m_RoomTemplates != null && m_RoomTemplates.Length > 0
                    || m_ExitRoomPrefab != null
                    || m_SpawnRoomPrefab != null
                )
            )
            {
                Generate();
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Apply configuration values from a ScriptableObject.
        /// Hybrid policy: by default only fills missing prefab references so
        /// inspector wiring remains authoritative. Scalars are optional.
        /// </summary>
        /// <param name="config">Room generation config</param>
        /// <param name="fillOnly">
        /// When true (default), only fills empty references for templates/exit/spawn.
        /// When false, overwrites all values from the config.
        /// </param>
        /// <param name="applyScalarSettings">
        /// When true, applies scalar settings (grid size, room size, density, etc.).
        /// Defaults to false so scene/inspector values remain primary.
        /// </param>
        public void ApplyConfig(
            RoomGenConfig config,
            bool fillOnly = true,
            bool applyScalarSettings = false
        )
        {
            if (config == null)
            {
                return;
            }

            // Prefab references
            if (!fillOnly || m_RoomTemplates == null || m_RoomTemplates.Length == 0)
            {
                m_RoomTemplates = config.RoomTemplates;
            }
            if (!fillOnly || m_ExitRoomPrefab == null)
            {
                m_ExitRoomPrefab = config.ExitRoomPrefab;
            }
            if (!fillOnly || m_SpawnRoomPrefab == null)
            {
                m_SpawnRoomPrefab = config.SpawnRoomPrefab;
            }

            // Scalar settings (optional)
            if (!fillOnly || applyScalarSettings)
            {
                m_GridSize = config.GridSize;
                m_RoomSize = config.RoomSize;
                m_GenerateAllRooms = config.GenerateAllRooms;
                m_SparseDensity = config.SparseDensity;
                m_FalseDoorChance = config.FalseDoorChance;
                m_EnableRoomRotation = config.EnableRoomRotation;
                m_RotationCooldown = config.RotationCooldown;
                m_Seed = config.Seed;
            }
        }

        /// <summary>
        /// Main generation function. Creates the 9x9x9 cube grid.
        /// </summary>
        [ContextMenu("Generate 9x9x9 Cube")]
        public void Generate()
        {
            // Initialize RNG with seed for reproducible generation
            // Seed = 0 means random generation each time
            if (m_Seed == 0)
            {
                m_RNG = new System.Random();
            }
            else
            {
                m_RNG = new System.Random(m_Seed);
            }
            
            // Clear previous generation to avoid duplicates
            ClearPrevious();
            
            // Initialize 3D array to store room references
            // Structure: [x, y, z] where null = no room at that position
            m_RoomGrid = new GameObject[m_GridSize, m_GridSize, m_GridSize];
            
            // Calculate special cells for exit and spawns
            // For 9x9x9 grid (0-8), center is at index 4
            int center = m_GridSize / 2;
            m_CenterCell = new Vector3Int(center, center, center);
            
            // Define corner spawn points (8 corners of the cube)
            // Players spawn at these corners and must navigate to center
            // For 9x9x9: (0,0,0), (8,0,0), (0,0,8), (8,0,8), (0,8,0), (8,8,0), (0,8,8), (8,8,8)
            m_CornerCells.Clear();
            m_CornerCells.Add(new Vector3Int(0, 0, 0)); // Bottom-front-left
            m_CornerCells.Add(new Vector3Int(m_GridSize - 1, 0, 0)); // Bottom-front-right
            m_CornerCells.Add(new Vector3Int(0, 0, m_GridSize - 1)); // Bottom-back-left
            m_CornerCells.Add(new Vector3Int(m_GridSize - 1, 0, m_GridSize - 1)); // Bottom-back-right
            m_CornerCells.Add(new Vector3Int(0, m_GridSize - 1, 0)); // Top-front-left
            m_CornerCells.Add(new Vector3Int(m_GridSize - 1, m_GridSize - 1, 0)); // Top-front-right
            m_CornerCells.Add(new Vector3Int(0, m_GridSize - 1, m_GridSize - 1)); // Top-back-left
            m_CornerCells.Add(new Vector3Int(m_GridSize - 1, m_GridSize - 1, m_GridSize - 1)); // Top-back-right

            Debug.Log(
                $"Starting 9x9x9 cube generation. Total possible rooms: {m_GridSize * m_GridSize * m_GridSize}"
            );

            // Generate rooms
            if (m_GenerateAllRooms)
            {
                GenerateAllRooms();
            }
            else
            {
                GenerateSparseRooms();
            }
            
            Debug.Log($"Generation complete. Placed {m_PlacedRooms.Count} rooms.");
        }
        
        /// <summary>
        /// Generates all 729 rooms in the grid.
        /// WARNING: This creates a lot of GameObjects and may impact performance!
        /// </summary>
        private void GenerateAllRooms()
        {
            for (int x = 0; x < m_GridSize; x++)
            {
                for (int y = 0; y < m_GridSize; y++)
                {
                    for (int z = 0; z < m_GridSize; z++)
                    {
                        Vector3Int cell = new Vector3Int(x, y, z);
                        PlaceRoomAtCell(cell);
                    }
                }
            }
        }
        
        /// <summary>
        /// Generates a subset of rooms using pathfinding to ensure connectivity.
        /// Creates paths from all corners to the center.
        /// </summary>
        private void GenerateSparseRooms()
        {
            // Step 1: Place the exit room at the center (guaranteed endpoint)
            PlaceRoomAtCell(m_CenterCell);
            
            // Step 2: Place spawn rooms at all 8 corners (guaranteed starting points)
            foreach (Vector3Int corner in m_CornerCells)
            {
                PlaceRoomAtCell(corner);
            }
            
            // Step 3: Create guaranteed paths from each corner to center
            // This ensures all spawns can reach the exit
            // Uses Manhattan distance pathfinding (moves on one axis at a time)
            foreach (Vector3Int corner in m_CornerCells)
            {
                CreatePathToCenter(corner);
            }
            
            // Step 4: Fill in additional random rooms to reach target density
            // Example: 30% density of 729 cells = ~218 rooms total
            int totalCells = m_GridSize * m_GridSize * m_GridSize;
            int targetRooms = Mathf.RoundToInt(totalCells * m_SparseDensity);
            
            while (m_PlacedRooms.Count < targetRooms)
            {
                // Pick random cell
                int x = m_RNG.Next(0, m_GridSize);
                int y = m_RNG.Next(0, m_GridSize);
                int z = m_RNG.Next(0, m_GridSize);
                Vector3Int cell = new Vector3Int(x, y, z);
                
                // Place if empty
                if (m_RoomGrid[x, y, z] == null)
                {
                    PlaceRoomAtCell(cell);
                }
            }
        }
        
        /// <summary>
        /// Creates a path from a corner to the center using Manhattan distance pathfinding.
        /// </summary>
        private void CreatePathToCenter(Vector3Int start)
        {
            Vector3Int current = start;
            
            // Manhattan distance pathfinding: move one step at a time toward center
            // Always moves on the axis with the largest remaining distance
            // This creates natural-looking paths without diagonal shortcuts
            while (current != m_CenterCell)
            {
                // Calculate difference on each axis
                Vector3Int diff = m_CenterCell - current;
                
                // Choose axis to move on (prioritize X, then Y, then Z)
                if (
                    Mathf.Abs(diff.x) >= Mathf.Abs(diff.y)
                    && Mathf.Abs(diff.x) >= Mathf.Abs(diff.z)
                )
                {
                    // Move one step along X axis toward center
                    current.x += (int)Mathf.Sign(diff.x);
                }
                else if (Mathf.Abs(diff.y) >= Mathf.Abs(diff.z))
                {
                    // Move one step along Y axis toward center
                    current.y += (int)Mathf.Sign(diff.y);
                }
                else
                {
                    // Move one step along Z axis toward center
                    current.z += (int)Mathf.Sign(diff.z);
                }
                
                // Place room at this step if not already placed
                // Allows multiple paths to share rooms
                if (m_RoomGrid[current.x, current.y, current.z] == null)
                {
                    PlaceRoomAtCell(current);
                }
            }
        }
        
        /// <summary>
        /// Places a room at the specified grid cell.
        /// Automatically selects appropriate room type (exit, spawn, or standard).
        /// </summary>
        private void PlaceRoomAtCell(Vector3Int cell)
        {
            // Skip if cell already has a room (prevents duplicates)
            if (m_RoomGrid[cell.x, cell.y, cell.z] != null)
            {
                return;
            }
            
            // Select appropriate prefab based on room type
            // Priority: Exit > Spawn > Random Standard
            GameObject prefab = null;
            string roomType = "Standard";
            
            // Center cell gets special exit room (if assigned)
            if (cell == m_CenterCell && m_ExitRoomPrefab != null)
            {
                prefab = m_ExitRoomPrefab;
                roomType = "EXIT";
            }
            // Corner cells get special spawn rooms (if assigned)
            else if (m_CornerCells.Contains(cell) && m_SpawnRoomPrefab != null)
            {
                prefab = m_SpawnRoomPrefab;
                roomType = "SPAWN";
            }
            // All other cells get random standard room from templates
            else if (m_RoomTemplates != null && m_RoomTemplates.Length > 0)
            {
                prefab = m_RoomTemplates[m_RNG.Next(0, m_RoomTemplates.Length)];
            }
            
            if (prefab == null)
            {
                Debug.LogWarning($"No room prefab available for cell {cell}");
                return;
            }
            
            // Calculate world position
            Vector3 worldPos = new Vector3(
                cell.x * m_RoomSize,
                cell.y * m_RoomSize,
                cell.z * m_RoomSize
            );
            
            // Instantiate room
            GameObject room = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            room.name = $"Room_{roomType}_{cell.x}_{cell.y}_{cell.z}";
            
            // Store in grid and list
            m_RoomGrid[cell.x, cell.y, cell.z] = room;
            m_PlacedRooms.Add(room);
            
            // Add room data component
            RoomData roomData = room.AddComponent<RoomData>();
            roomData.GridPosition = cell;
            roomData.IsExitRoom = (cell == m_CenterCell);
            roomData.IsSpawnRoom = m_CornerCells.Contains(cell);
            
            // Setup doors for this room
            SetupRoomDoors(room, roomData, cell);
        }
        
        /// <summary>
        /// Gets the room at a specific grid position.
        /// </summary>
        public GameObject GetRoomAt(int x, int y, int z)
        {
            if (x < 0 || x >= m_GridSize || y < 0 || y >= m_GridSize || z < 0 || z >= m_GridSize)
            {
                return null;
            }
            return m_RoomGrid[x, y, z];
        }
        
        /// <summary>
        /// Gets a random corner spawn position.
        /// </summary>
        public Vector3 GetRandomSpawnPosition()
        {
            Vector3Int corner = m_CornerCells[m_RNG.Next(0, m_CornerCells.Count)];
            return new Vector3(
                corner.x * m_RoomSize,
                corner.y * m_RoomSize + 1f, // Slightly above floor
                corner.z * m_RoomSize
            );
        }
        
        /// <summary>
        /// Setup doors for a room based on adjacent rooms and grid boundaries.
        /// Doors on outer perimeter are always blocked.
        /// Interior doors may be false (blocked) based on m_FalseDoorChance.
        /// </summary>
        private void SetupRoomDoors(GameObject room, RoomData roomData, Vector3Int cell)
        {
            // Six possible directions for 3D grid connectivity
            // Each direction maps to a cardinal direction or vertical axis
            Vector3Int[] directions = new Vector3Int[]
            {
                Vector3Int.right, // East (+X)
                Vector3Int.left, // West (-X)
                Vector3Int.up, // Top (+Y)
                Vector3Int.down, // Bottom (-Y)
                new Vector3Int(0, 0, 1), // North (+Z)
                new Vector3Int(0, 0, -1), // South (-Z)
            };
            
            // Expected door GameObject names in room prefabs
            string[] doorNames = new string[]
            {
                "door_east",
                "door_west",
                "door_up",
                "door_down",
                "door_north",
                "door_south",
            };

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3Int neighborCell = cell + directions[i];
                bool isOpen = true;
                
                // Determine if door should be open or blocked
                // Three conditions that block a door:
                
                // 1. Outer perimeter check: no doors leading outside the grid
                if (
                    neighborCell.x < 0
                    || neighborCell.x >= m_GridSize
                    || neighborCell.y < 0
                    || neighborCell.y >= m_GridSize
                    || neighborCell.z < 0
                    || neighborCell.z >= m_GridSize
                )
                {
                    isOpen = false; // Outer wall - always blocked
                }
                // 2. Neighbor room check: no door if no room exists in that direction
                else if (m_RoomGrid[neighborCell.x, neighborCell.y, neighborCell.z] == null)
                {
                    isOpen = false; // No adjacent room - blocked
                }
                // 3. False door mechanic: random chance to block passage (Cube film trap)
                else if (m_RNG.NextDouble() < m_FalseDoorChance)
                {
                    isOpen = false; // False door - looks open but blocked
                }
                
                // Store door state
                roomData.Doors[directions[i]] = isOpen;
                
                // Find and configure visual door GameObject if it exists
                Transform doorTransform = room.transform.Find(doorNames[i]);
                if (doorTransform != null)
                {
                    GameObject doorObj = doorTransform.gameObject;
                    
                    // Store reference based on direction
                    switch (i)
                    {
                        case 0:
                            roomData.DoorEast = doorObj;
                            break;
                        case 1:
                            roomData.DoorWest = doorObj;
                            break;
                        case 2:
                            roomData.DoorUp = doorObj;
                            break;
                        case 3:
                            roomData.DoorDown = doorObj;
                            break;
                        case 4:
                            roomData.DoorNorth = doorObj;
                            break;
                        case 5:
                            roomData.DoorSouth = doorObj;
                            break;
                    }

                    // Set door visual state (you can add DoorController script to handle animations)
                    doorObj.SetActive(true); // Door exists
                    
                    // Optional: Add a material/color to show if it's false
                    if (!isOpen)
                    {
                        // Could add red tint or "locked" visual here
                        Renderer renderer = doorObj.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.material.color = new Color(0.8f, 0.2f, 0.2f); // Red tint for false doors
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Trigger room rotation for all rooms (called periodically or when doors close).
        /// This is the classic Cube film mechanic.
        /// </summary>
        [ContextMenu("Trigger Room Rotation")]
        public void TriggerRoomRotation()
        {
            if (!m_EnableRoomRotation)
                return;

            foreach (GameObject roomObj in m_PlacedRooms)
            {
                if (roomObj == null)
                    continue;

                RoomData roomData = roomObj.GetComponent<RoomData>();
                if (roomData != null && !roomData.IsSpawnRoom) // Don't rotate spawn rooms
                {
                    roomData.RotateRoom(m_RotationCooldown);
                }
            }
        }
        
        /// <summary>
        /// Call this when all players have exited a room and doors close.
        /// Triggers rotation for that specific room.
        /// DoorController script calls this method after door closes.
        /// </summary>
        public void OnRoomExited(Vector3Int gridPosition)
        {
            // Skip if rotation is disabled
            if (!m_EnableRoomRotation)
                return;

            // Find the room at the specified grid position
            GameObject room = GetRoomAt(gridPosition.x, gridPosition.y, gridPosition.z);
            if (room != null)
            {
                // Get room data component and trigger rotation
                RoomData roomData = room.GetComponent<RoomData>();
                if (roomData != null)
                {
                    // Rotation respects cooldown to prevent spam
                    roomData.RotateRoom(m_RotationCooldown);
                }
            }
        }
        
        /// <summary>
        /// Clears all previously generated rooms.
        /// </summary>
        [ContextMenu("Clear Rooms")]
        public void ClearPrevious()
        {
            foreach (GameObject room in m_PlacedRooms)
            {
                if (room != null)
                {
                    Destroy(room);
                }
            }
            m_PlacedRooms.Clear();
            Debug.Log("Cleared all generated rooms");
        }
        
        #endregion
        
        #region Helper Components
        
        /// <summary>
        /// Data component attached to each generated room.
        /// Stores grid position, room type, door states, and rotation mechanics.
        /// </summary>
        public class RoomData : MonoBehaviour
        {
            public Vector3Int GridPosition;
            public bool IsExitRoom;
            public bool IsSpawnRoom;
            
            // Door management - tracks which directions have open passages
            // Key: Direction vector (e.g., Vector3Int.right = East)
            // Value: true = open, false = blocked/false door
            public Dictionary<Vector3Int, bool> Doors = new Dictionary<Vector3Int, bool>();
            
            // Rotation state
            public Quaternion OriginalRotation { get; private set; }
            public float LastRotationTime = -999f;
            public int RotationCount = 0;
            
            // Door GameObjects for visual opening/closing
            public GameObject DoorNorth;
            public GameObject DoorSouth;
            public GameObject DoorEast;
            public GameObject DoorWest;
            public GameObject DoorUp;
            public GameObject DoorDown;
            
            void Awake()
            {
                OriginalRotation = transform.rotation;
            }
            
            /// <summary>
            /// Check if door in given direction is open (not blocked/false).
            /// </summary>
            public bool IsDoorOpen(Vector3Int direction)
            {
                return Doors.ContainsKey(direction) && Doors[direction];
            }
            
            /// <summary>
            /// Rotate room 90 degrees on a random axis (Cube film mechanic).
            /// Called after all players exit and doors close.
            /// This disorients players and changes room connectivity.
            /// </summary>
            public void RotateRoom(float cooldown)
            {
                // Enforce cooldown to prevent rapid spinning
                if (Time.time - LastRotationTime < cooldown) return;
                
                // Choose random axis (X, Y, or Z) for 90-degree rotation
                // This mimics the film's rotating trap rooms
                int axis = Random.Range(0, 3);
                Vector3 rotationAxis =
                    axis == 0 ? Vector3.right : (axis == 1 ? Vector3.up : Vector3.forward);
                
                // Rotate 90 degrees in world space
                // World space ensures rotation is consistent regardless of parent transforms
                transform.Rotate(rotationAxis, 90f, Space.World);
                
                // Track rotation for debugging and gameplay logic
                LastRotationTime = Time.time;
                RotationCount++;
                
                Debug.Log($"Room at {GridPosition} rotated (total: {RotationCount} rotations)");
            }
        }
        
        #endregion
        
        #region Editor Visualization
        
        /// <summary>
        /// Draws the 9x9x9 grid in Scene view for debugging.
        /// Shows grid structure, spawn corners, and exit location.
        /// </summary>
        void OnDrawGizmos()
        {
            if (!Application.isPlaying || m_RoomGrid == null)
                return;
            
            // Draw grid cells
            for (int x = 0; x < m_GridSize; x++)
            {
                for (int y = 0; y < m_GridSize; y++)
                {
                    for (int z = 0; z < m_GridSize; z++)
                    {
                        Vector3 cellCenter = new Vector3(
                            x * m_RoomSize + m_RoomSize * 0.5f,
                            y * m_RoomSize + m_RoomSize * 0.5f,
                            z * m_RoomSize + m_RoomSize * 0.5f
                        );
                        
                        Vector3Int cell = new Vector3Int(x, y, z);
                        
                        // Color code: Exit = Green, Spawn = Blue, Filled = White, Empty = Red
                        if (cell == m_CenterCell)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(cellCenter, Vector3.one * m_RoomSize);
                        }
                        else if (m_CornerCells.Contains(cell))
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(cellCenter, Vector3.one * m_RoomSize);
                        }
                        else if (m_RoomGrid[x, y, z] != null)
                        {
                            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                            Gizmos.DrawWireCube(cellCenter, Vector3.one * m_RoomSize * 0.9f);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Draws selected gizmo with more detail.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            // Draw center axis lines
            Gizmos.color = Color.yellow;
            float gridExtent = m_GridSize * m_RoomSize;
            Gizmos.DrawLine(Vector3.zero, new Vector3(gridExtent, 0, 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, gridExtent, 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, gridExtent));
        }
        
        #endregion
        
        #region Public API for Game Controller
        
        /// <summary>
        /// Get spawn position for a player at one of the 8 corner positions.
        /// Corners are: (0,0,0), (8,0,0), (0,0,8), (8,0,8), (0,8,0), (8,8,0), (0,8,8), (8,8,8)
        /// </summary>
        /// <param name="playerIndex">Player index (0-7)</param>
        /// <returns>World position at the corner room center</returns>
        public Vector3 GetCornerSpawnPosition(int playerIndex)
        {
            if (m_CornerCells == null || m_CornerCells.Count == 0)
            {
                Debug.LogWarning("[RoomGenerator] Corner cells not initialized. Returning zero.");
                return Vector3.zero;
            }
            
            // Wrap player index to available corners
            playerIndex = playerIndex % m_CornerCells.Count;
            Vector3Int cornerCell = m_CornerCells[playerIndex];
            
            // Convert grid cell to world position (center of room)
            Vector3 worldPos = new Vector3(
                cornerCell.x * m_RoomSize + m_RoomSize / 2f,
                cornerCell.y * m_RoomSize + m_RoomSize / 2f,
                cornerCell.z * m_RoomSize + m_RoomSize / 2f
            );
            
            return worldPos;
        }
        
        #endregion
    }
}
