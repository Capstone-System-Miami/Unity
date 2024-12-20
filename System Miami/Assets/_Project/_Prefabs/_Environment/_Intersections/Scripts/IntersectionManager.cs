using System.Collections.Generic;
using SystemMiami;
using SystemMiami.Management;
using UnityEngine;
using UnityEngine.Events;

//Made By Antony

// The main class responsible for generating and managing streets in the game.
public class IntersectionManager : Singleton<IntersectionManager>
{
    // Serialized fields allow these private variables to be set in the Unity Editor.
    [Header("Street Generation Settings")]
    [SerializeField] private IntersectionPool[] streetPools; // Array of StreetPools, which contain prefabs for different street types.
    [SerializeField] private int maxStreets; // Maximum number of streets to generate.
    [SerializeField] private int minStreets; // Minimum number of streets to generate.
    [SerializeField] private int streetWidth; // The width of each street in world units.
    [SerializeField] private int streetHeight; // The height of each street in world units.
    [SerializeField] private int gridSizeX; // The width of the street grid.
    [SerializeField] private int gridSizeY; // The height of the street grid.
    [SerializeField] private int maxExits = 4; // Maximum number of exits per street.
    [SerializeField] private int maxStreetsToConnect = 4; // Maximum number of streets that can be connected from one street.
    
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab; // The player prefab to instantiate in the scene.
    
    // List to keep track of instantiated street GameObjects.
    private List<GameObject> streetObjects = new List<GameObject>();
    // Queue used for breadth-first street generation.
    private Queue<Vector2Int> streetQueue = new Queue<Vector2Int>();
    // Dictionary mapping StreetTypes to their corresponding StreetPools.
    private Dictionary<IntersectionType, IntersectionPool> streetPoolDictionary;
    // 2D array representing the grid of streets.
    private StreetData[,] streetGrid;
    // Counter for the number of streets generated.
    private int streetCount;
    // Flag to indicate if the street generation is complete.
    private bool generationComplete = false;

    // Possible directions to check for neighboring streets (up, down, right, left).
    private Vector2Int[] directions = {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(0, -1), // Down
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0)  // Left
    };

    // Mapping from moveDirection vectors to ExitDirection enums.
    private Dictionary<Vector2Int, ExitDirection> dirToExit = new Dictionary<Vector2Int, ExitDirection> {
        { new Vector2Int(0, 1), ExitDirection.North }, // Up corresponds to North exit.
        { new Vector2Int(0, -1), ExitDirection.South }, // Down corresponds to South exit.
        { new Vector2Int(1, 0), ExitDirection.East }, // Right corresponds to East exit.
        { new Vector2Int(-1, 0), ExitDirection.West } // Left corresponds to West exit.
    };
    
    // Mapping from moveDirection vectors to the opposite ExitDirection enums.
    private Dictionary<Vector2Int, ExitDirection> dirToOppositeExit = new Dictionary<Vector2Int, ExitDirection> {
        { new Vector2Int(0, 1), ExitDirection.South }, // Up's opposite is South.
        { new Vector2Int(0, -1), ExitDirection.North }, // Down's opposite is North.
        { new Vector2Int(1, 0), ExitDirection.West }, // Right's opposite is West.
        { new Vector2Int(-1, 0), ExitDirection.East } // Left's opposite is East.
    };
    
    // Mapping from ExitDirection enums to moveDirection vectors.
    private Dictionary<ExitDirection, Vector2Int> exitToDir = new Dictionary<ExitDirection, Vector2Int> {
        { ExitDirection.North, new Vector2Int(0, 1) }, // North exit corresponds to Up.
        { ExitDirection.South, new Vector2Int(0, -1) }, // South exit corresponds to Down.
        { ExitDirection.East, new Vector2Int(1, 0) }, // East exit corresponds to Right.
        { ExitDirection.West, new Vector2Int(-1, 0) } // West exit corresponds to Left.
    };

    // The index in the grid where the street generation starts.
    private Vector2Int startingStreetIndex;
    // Reference to the first StreetData object generated.
    private StreetData firstStreetGenerated;
    // Reference to the last StreetData object generated.
    private StreetData lastStreetGenerated;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    

    // Unity's Start method is called before the first frame update.
    void Start()
    {
        playerPrefab = GameObject.Find("Player");
        InitializeStreetPoolDictionary(); // Prepare the dictionary mapping StreetTypes to StreetPools.
        InitializeStreetGrid(); // SetAll up the grid data structure for street generation.
        StartStreetGenerationFromStreet(new Vector2Int(gridSizeX / 2, gridSizeY / 2)); // Begin street generation from the center of the grid.
    }

    // Unity's Update method is called once per frame.
    void Update()
    {
        // If there are streets in the queue and generation is not complete, continue generating streets.
        if (streetQueue.Count > 0 && !generationComplete)
        {
            // Dequeue a street index from the queue and attempt to generate a street there.
            TryGenerateStreet(streetQueue.Dequeue());
        }
        // If the street count is below the minimum required and generation is not complete.
        else if (streetCount < minStreets && !generationComplete)
        {
            // Regenerate the streets to try and meet the minimum street requirement.
            RegenerateStreets();
        }
        // If generation is not complete but no more streets are in the queue and the street count meets the minimum.
        else if (!generationComplete)
        {
            generationComplete = true;
            CleanupExits(); // Adjust exits to ensure consistency between connected streets.
            LogGenerationResults(); // Output generation statistics to the console.
        }
    }

    // Initialize the dictionary that maps StreetTypes to their corresponding StreetPools.
    private void InitializeStreetPoolDictionary()
    {
        streetPoolDictionary = new Dictionary<IntersectionType, IntersectionPool>();
        foreach (var pool in streetPools)
        {
            // Check if the IntersectionType is already in the dictionary to avoid duplicates.
            if (!streetPoolDictionary.ContainsKey(pool.streetType))
            {
                streetPoolDictionary.Add(pool.streetType, pool); // Add the IntersectionPool to the dictionary.
            }
            else
            {
                // Log a warning if a duplicate IntersectionType is detected.
                Debug.LogWarning("Duplicate IntersectionType detected in StreetPools");
            }
        }
    }

    // SetAll up the 2D array representing the grid, and initialize each cell with a new StreetData instance.
    private void InitializeStreetGrid()
    {
        streetGrid = new StreetData[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Create a new StreetData object for each grid cell with its grid index.
                streetGrid[x, y] = new StreetData { gridIndex = new Vector2Int(x, y) };
            }
        }
    }

    // Begin the street generation process starting from the specified grid index.
    private void StartStreetGenerationFromStreet(Vector2Int streetIndex)
    {
        // Add the starting street index to the queue for processing.
        streetQueue.Enqueue(streetIndex);
        // Mark the street as enqueued to prevent it from being enqueued again.
        streetGrid[streetIndex.x, streetIndex.y].enqueued = true;
        
        // Spawn the player at the center of the grid.
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        // Instantiate the player prefab at the center of the grid.
        Vector3 playerPosition = GetPositionFromGridIndex(new Vector2Int(gridSizeX / 2, gridSizeY / 2));
        playerPrefab.transform.position =  playerPosition;
    }
    
    // Attempt to generate a street at the specified grid index.
    private void TryGenerateStreet(Vector2Int streetIndex)
    {
        int x = streetIndex.x;
        int y = streetIndex.y;
    
        // Check if the grid index is within the bounds of the grid.
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY) return;
    
        StreetData currentStreet = streetGrid[x, y];
        if (currentStreet.hasStreet) return; // If a street already exists at this index, do nothing.
    
        // Mark this grid cell as having a street.
        currentStreet.hasStreet = true;
        // List of directions where new streets can potentially be generated.
        List<Vector2Int> availableDirs = new List<Vector2Int>();

        // Check each possible moveDirection to determine where we can generate new streets.
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            // Skip if neighbor index is out of bounds.
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;
    
            StreetData neighborStreet = streetGrid[nx, ny];
            if (neighborStreet.hasStreet && neighborStreet.exits.Contains(dirToOppositeExit[dir]))
            {
                // If the neighboring cell has a street and its exits include the opposite moveDirection,
                // then we need to add an exit in the current street in this moveDirection to connect them.
                currentStreet.exits.Add(dirToExit[dir]);
            }
            else if (!neighborStreet.hasStreet)
            {
                // If the neighboring cell doesn't have a street, it's an available moveDirection to expand into.
                availableDirs.Add(dir);
            }
        }

        // Determine the maximum number of exits that can be created from this street.
        int maxExits = Mathf.Min(maxStreets - streetCount, this.maxExits); // Limit exits by remaining streets to generate and the configured maxExits.
        // Decide randomly how many exits to create, at least one, up to maxExits, but not exceeding available directions.
        int exitsToCreate = Mathf.Min(Random.Range(1, maxExits + 1), availableDirs.Count);
        // Randomize the order of available directions to add randomness to street generation.
        ShuffleList(availableDirs);
    
        // Count how many streets are adjacent to this street already.
        int adjacentStreets = CountAdjacentStreets(streetIndex);
        // Limit the number of streets we can connect to based on maxStreetsToConnect and existing adjacent streets.
        int maxStreetsToConnect = Mathf.Min(this.maxStreetsToConnect - adjacentStreets, exitsToCreate);
        exitsToCreate = Mathf.Min(exitsToCreate, maxStreetsToConnect);
    
        int exitsCreated = 0;
        // Loop through available directions and create exits, enqueueing new streets for generation.
        for (int i = 0; i < availableDirs.Count && exitsCreated < exitsToCreate; i++)
        {
            Vector2Int dir = availableDirs[i];
            int nx = x + dir.x;
            int ny = y + dir.y;
            // Skip if neighbor index is out of bounds.
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;
    
            StreetData neighborStreet = streetGrid[nx, ny];
            // Check if we have not exceeded the maximum number of streets to generate.
            if (streetCount + streetQueue.Count < maxStreets)
            {
                // Add an exit in the current street in this moveDirection.
                currentStreet.exits.Add(dirToExit[dir]);
                // Add an exit in the neighboring street in the opposite moveDirection.
                neighborStreet.exits.Add(dirToOppositeExit[dir]);
                // If the neighbor has not been enqueued yet, enqueue it for generation.
                if (!neighborStreet.enqueued)
                {
                    neighborStreet.enqueued = true;
                    streetQueue.Enqueue(new Vector2Int(nx, ny));
                }
                exitsCreated++;
            }
        }

        // Instantiate the street GameObject in the scene.
        InstantiateStreet(currentStreet, streetIndex); // Instantiate the street
        streetCount++; // Increment the total street count.
        // If this is the first street generated, record it.
        if (firstStreetGenerated == null) firstStreetGenerated = currentStreet;
        // Update the last street generated.
        lastStreetGenerated = currentStreet;
    }

    // Randomly shuffle the elements in the list to randomize the order of directions.
    private void ShuffleList(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count); // Pick a random index from i to the end of the list.
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Instantiate a street GameObject at the specified grid index.
    private void InstantiateStreet(StreetData currentStreet, Vector2Int streetIndex)
    {
        // Determine the IntersectionType based on the exits of the current street.
        IntersectionType streetType = GetStreetTypeFromExits(currentStreet.exits);
        if (streetPoolDictionary.TryGetValue(streetType, out IntersectionPool pool))
        {
            // Randomly select a prefab from the pool of the determined IntersectionType.
            GameObject prefabToInstantiate = pool.streetPrefabs[Random.Range(0, pool.streetPrefabs.Length)];
            // Calculate the world position for this grid index.
            Vector3 position = GetPositionFromGridIndex(streetIndex);
            // Instantiate the street prefab at the calculated position with no rotation and under this manager's transform.
            GameObject streetInstance = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
            
            
            // Name the street instance based on its index in the grid for easier identification.
            streetInstance.name = $"Street [{streetIndex.x}, {streetIndex.y}]";
            // Add the instantiated street to the list of street objects.
            streetObjects.Add(streetInstance);
            // Keep a reference to the street instance in the StreetData.
            currentStreet.streetInstance = streetInstance;

            
            
            DungeonEntrance[] dungeonEntrances = streetInstance.GetComponentsInChildren<DungeonEntrance>();

            foreach (DungeonEntrance dungeonEntrance in dungeonEntrances)
            {
                if (dungeonEntrance == null)
                {
                    Debug.LogError($"DungeonEntrance component not found in {streetInstance.name}");
                    continue;
                }

                DifficultyLevel difficulty = GetRandomDifficulty();
                dungeonEntrance.SetDifficulty(difficulty);
                Debug.Log($"Set difficulty of {streetIndex} to {difficulty}");
                currentStreet.dungeonEntranceDifficulties.Add(difficulty);

            }

        }
        else
        {
            // Log a warning if no IntersectionPool is found for the determined IntersectionType.
            Debug.LogWarning($"No IntersectionPool found for IntersectionType: {streetType}");
        }
    }
    
    private DifficultyLevel GetRandomDifficulty()
    {
        float randomValue = Random.value;

        if (randomValue < 0.5f)
        {
            return DifficultyLevel.EASY; // 50% chance
        }
        else if (randomValue < 0.8f)
        {
            return DifficultyLevel.MEDIUM; // 30% chance
        }
        else
        {
            return DifficultyLevel.HARD; // 20% chance
        }
    }

    // After generation is complete, ensure that exits are consistent between connected streets.
    private void CleanupExits()
    {
        foreach (var streetData in streetGrid)
        {
            if (streetData.hasStreet)
            {
                // Create a new set to hold exits that are valid (i.e., actually connected to neighboring streets).
                HashSet<ExitDirection> validExits = new HashSet<ExitDirection>();
                int x = streetData.gridIndex.x;
                int y = streetData.gridIndex.y;
    
                // Check each exit moveDirection to see if it connects to a valid street.
                foreach (ExitDirection exit in streetData.exits)
                {
                    Vector2Int dir = exitToDir[exit];
                    int nx = x + dir.x;
                    int ny = y + dir.y;
                    // Skip if neighbor index is out of bounds.
                    if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;
    
                    StreetData neighborStreet = streetGrid[nx, ny];
                    // If the neighbor has a street and its exits include the opposite moveDirection, the connection is valid.
                    if (neighborStreet.hasStreet && neighborStreet.exits.Contains(dirToOppositeExit[dir]))
                    {
                        validExits.Add(exit);
                    }
                }
    
                // Update the exits of the street to include only valid exits.
                streetData.exits = validExits;
                UpdateStreetPrefab(streetData); // Update the street prefab to reflect valid exits
            }
        }
    }

    // Update the street GameObject to match the updated exits after cleanup.
    private void UpdateStreetPrefab(StreetData streetData)
    {
        // Determine the new IntersectionType based on the updated exits.
        IntersectionType streetType = GetStreetTypeFromExits(streetData.exits);
        if (streetData.streetInstance != null)
        {
            // Destroy the old street instance.
            Destroy(streetData.streetInstance);
            // Try to get a new prefab from the IntersectionPool corresponding to the new IntersectionType.
            if (streetPoolDictionary.TryGetValue(streetType, out IntersectionPool pool))
            {
                // Randomly select a prefab from the pool.
                GameObject prefabToInstantiate = pool.streetPrefabs[Random.Range(0, pool.streetPrefabs.Length)];
                // Calculate the position in the world for this grid index.
                Vector3 position = GetPositionFromGridIndex(streetData.gridIndex);
                // Instantiate the new street prefab.
                GameObject streetInstance = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                
                // Name the street instance based on its index in the grid.
                streetInstance.name = $"Street [{streetData.gridIndex.x}, {streetData.gridIndex.y}]";
                // Update the street instance reference in the StreetData.
                streetData.streetInstance = streetInstance;
                
                DungeonEntrance[] dungeonEntrances = streetInstance.GetComponentsInChildren<DungeonEntrance>();

                if (dungeonEntrances.Length != streetData.dungeonEntranceDifficulties.Count)
                {
                    Debug.LogWarning("Number of DungeonEntrances has changed");
                }

                for (int i = 0; i < dungeonEntrances.Length; i++)
                {
                    DungeonEntrance dungeonEntrance = dungeonEntrances[i];
                    if (dungeonEntrance == null)
                    {
                        Debug.LogError($"DungeonEntrance component not found in {streetInstance.name}");
                        continue;
                    }
                    DifficultyLevel difficulty;
                    if (i < streetData.dungeonEntranceDifficulties.Count)
                    {
                        difficulty = streetData.dungeonEntranceDifficulties[i];
                    }
                    else
                    {
                        difficulty = GetRandomDifficulty();
                        streetData.dungeonEntranceDifficulties.Add(difficulty);
                    }
                    dungeonEntrance.SetDifficulty(difficulty);
                    InteractionTrigger col = dungeonEntrances[i].GetComponentInChildren<InteractionTrigger>();
                    Debug.Log(col.name);
                    col.OnInteract.AddListener(GAME.MGR.GoToDungeon);
                }
            }
            else
            {
                // Log a warning if no IntersectionPool is found for the new IntersectionType.
                Debug.LogWarning($"No IntersectionPool found for IntersectionType: {streetType}");
            }
        }
    }

    // Destroy all generated streets and reset the grid to attempt generation again.
    private void RegenerateStreets()
    {
        // Destroy all instantiated street GameObjects.
        foreach (var obj in streetObjects)
        {
            Destroy(obj);
        }
        streetObjects.Clear(); // Clear the list of street objects.
    
        // SetDefault the street grid by creating new StreetData instances.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                streetGrid[x, y] = new StreetData { gridIndex = new Vector2Int(x, y) };
            }
        }
    
        streetQueue.Clear(); // Clear the street generation queue.
        streetCount = 0; // SetDefault the street count.
        generationComplete = false; // SetAll generation as not complete.
    
        // Restart street generation from the center of the grid.
        StartStreetGenerationFromStreet(new Vector2Int(gridSizeX / 2, gridSizeY / 2));
    }

    // Convert a grid index to an isometric world position.
    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        // Calculate the isometric X and Y coordinates.
        float isoX = (gridX - gridY) * (streetWidth / 2f);
        float isoY = (gridX + gridY) * (streetHeight / 4f);
        return new Vector3(isoX, isoY, 0);
    }

    // Draws the grid in the Unity Editor Scene view for visualization purposes.
    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f); // Cyan color with low opacity.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Gizmos.color = gizmoColor;
                // Draw a wireframe cube at each grid cell position to represent the grid.
                Gizmos.DrawWireCube(GetPositionFromGridIndex(new Vector2Int(x, y)), new Vector3(streetWidth, streetHeight, 0));
            }
        }
    }

    // Counts how many streets are adjacent (up, down, left, right) to the specified street index.
    private int CountAdjacentStreets(Vector2Int streetIndex)
    {
        int x = streetIndex.x;
        int y = streetIndex.y;
        int count = 0;
    
        // Check each moveDirection for neighboring streets.
        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            // Skip if neighbor index is out of bounds.
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;
            // If the neighbor has a street, increment the count.
            if (streetGrid[nx, ny].hasStreet) count++;
        }
    
        return count;
    }

    // Determines the IntersectionType based on the set of exits.
    private IntersectionType GetStreetTypeFromExits(HashSet<ExitDirection> exits)
    {
        ExitDirection combinedExits = ExitDirection.None;
        // Combine all exits into a single value using bitwise OR.
        foreach (ExitDirection exit in exits)
        {
            combinedExits |= exit;
        }
        // Cast the combined exits to a IntersectionType (assumes IntersectionType is defined to match combinations of ExitDirection).
        return (IntersectionType)combinedExits;
    }

    // Output statistics about the street generation process to the console.
    private void LogGenerationResults()
    {
        Debug.Log($"Generation Complete, {streetCount} streets generated");
        if (firstStreetGenerated != null)
        {
            Debug.Log($"First Street Generated: Index {firstStreetGenerated.gridIndex}");
        }
        if (lastStreetGenerated != null)
        {
            Debug.Log($"Last Street Generated: Index {lastStreetGenerated.gridIndex}");
        }
    }

    // Class to hold data about each street in the grid
    private class StreetData
    {
        public Vector2Int gridIndex; // The grid coordinates of this street.
        public bool hasStreet = false; // Whether a street has been generated at this grid location.
        public bool enqueued = false; // Whether this grid location has been added to the generation queue.
        public HashSet<ExitDirection> exits = new HashSet<ExitDirection>(); // The exits (directions) this street connects to.
        public GameObject streetInstance = null; // Reference to the instantiated street GameObject in the scene.
        public List<DifficultyLevel> dungeonEntranceDifficulties = new List<DifficultyLevel>(); // List of difficulties for DungeonEntrances.
    }
}
