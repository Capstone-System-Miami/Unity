using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    // Serialized fields to set in the Unity Editor
    [SerializeField] private StreetPool[] streetPools;
    [SerializeField] private int maxStreets;
    [SerializeField] private int minStreets;
    [SerializeField] private int streetWidth;
    [SerializeField] private int streetHeight;
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    [SerializeField] private int maxExits = 4; // New serialized field for maxExits
    [SerializeField] private int maxStreetsToConnect = 4;
    
    // Private fields for managing streets
    private List<GameObject> streetObjects = new List<GameObject>();
    private Queue<Vector2Int> streetQueue = new Queue<Vector2Int>();
    private Dictionary<StreetType, StreetPool> streetPoolDictionary;
    private StreetData[,] streetGrid;
    private int streetCount;
    private bool generationComplete = false;

    // Directions for street generation
    private Vector2Int[] directions = {
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
    };

    // Mappings for exit directions
    private Dictionary<Vector2Int, ExitDirection> dirToExit = new Dictionary<Vector2Int, ExitDirection> {
        { new Vector2Int(0, 1), ExitDirection.North },
        { new Vector2Int(0, -1), ExitDirection.South },
        { new Vector2Int(1, 0), ExitDirection.East },
        { new Vector2Int(-1, 0), ExitDirection.West }
    };

    private Dictionary<Vector2Int, ExitDirection> dirToOppositeExit = new Dictionary<Vector2Int, ExitDirection> {
        { new Vector2Int(0, 1), ExitDirection.South },
        { new Vector2Int(0, -1), ExitDirection.North },
        { new Vector2Int(1, 0), ExitDirection.West },
        { new Vector2Int(-1, 0), ExitDirection.East }
    };

    private Dictionary<ExitDirection, Vector2Int> exitToDir = new Dictionary<ExitDirection, Vector2Int> {
        { ExitDirection.North, new Vector2Int(0, 1) },
        { ExitDirection.South, new Vector2Int(0, -1) },
        { ExitDirection.East, new Vector2Int(1, 0) },
        { ExitDirection.West, new Vector2Int(-1, 0) }
    };

    // Variables to track the first and last generated streets
    private Vector2Int startingStreetIndex;
    private StreetData firstStreetGenerated;
    private StreetData lastStreetGenerated;

    // Unity's Start method, called before the first frame update
    void Start()
    {
        InitializeStreetPoolDictionary(); // Initialize the street pool dictionary
        InitializeStreetGrid(); // Initialize the street grid
        StartStreetGenerationFromStreet(new Vector2Int(gridSizeX / 2, gridSizeY / 2)); // Start street generation from the center
    }

    // Unity's Update method, called once per frame
    void Update()
    {
        if (streetQueue.Count > 0 && !generationComplete)
        {
            TryGenerateStreet(streetQueue.Dequeue()); // Try to generate a street from the queue
        }
        else if (streetCount < minStreets && !generationComplete)
        {
            RegenerateStreets(); // Regenerate streets if the count is below the minimum
        }
        else if (!generationComplete)
        {
            generationComplete = true;
            CleanupExits(); // Cleanup exits after generation is complete
            LogGenerationResults(); // Log the results of the generation
        }
    }

    // Initialize the street pool dictionary
    private void InitializeStreetPoolDictionary()
    {
        streetPoolDictionary = new Dictionary<StreetType, StreetPool>();
        foreach (var pool in streetPools)
        {
            if (!streetPoolDictionary.ContainsKey(pool.streetType))
            {
                streetPoolDictionary.Add(pool.streetType, pool);
            }
            else
            {
                Debug.LogWarning("Duplicate StreetType detected in StreetPools");
            }
        }
    }

    // Initialize the street grid
    private void InitializeStreetGrid()
    {
        streetGrid = new StreetData[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                streetGrid[x, y] = new StreetData { gridIndex = new Vector2Int(x, y) };
            }
        }
    }

    // Start street generation from a specific street index
    private void StartStreetGenerationFromStreet(Vector2Int streetIndex)
    {
        streetQueue.Enqueue(streetIndex);
        streetGrid[streetIndex.x, streetIndex.y].enqueued = true;
    }

    // Try to generate a street at a specific index
    private void TryGenerateStreet(Vector2Int streetIndex)
    {
        int x = streetIndex.x;
        int y = streetIndex.y;

        // Check if the index is out of bounds
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY) return;

        StreetData currentStreet = streetGrid[x, y];
        if (currentStreet.hasStreet) return; // Return if the street already exists

        currentStreet.hasStreet = true;
        List<Vector2Int> availableDirs = new List<Vector2Int>();

        // Check available directions for street generation
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;

            StreetData neighborStreet = streetGrid[nx, ny];
            if (neighborStreet.hasStreet && neighborStreet.exits.Contains(dirToOppositeExit[dir]))
            {
                currentStreet.exits.Add(dirToExit[dir]);
            }
            else if (!neighborStreet.hasStreet)
            {
                availableDirs.Add(dir);
            }
        }

        // Determine the number of exits to create
        int maxExits = Mathf.Min(maxStreets - streetCount, this.maxExits);
        int exitsToCreate = Mathf.Min(Random.Range(1, maxExits + 1), availableDirs.Count);
        ShuffleList(availableDirs);

        int adjacentStreets = CountAdjacentStreets(streetIndex);
        int maxStreetsToConnect = Mathf.Min(this.maxStreetsToConnect - adjacentStreets, exitsToCreate);
        exitsToCreate = Mathf.Min(exitsToCreate, maxStreetsToConnect);

        int exitsCreated = 0;
        for (int i = 0; i < availableDirs.Count && exitsCreated < exitsToCreate; i++)
        {
            Vector2Int dir = availableDirs[i];
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;

            StreetData neighborStreet = streetGrid[nx, ny];
            if (streetCount + streetQueue.Count < maxStreets)
            {
                currentStreet.exits.Add(dirToExit[dir]);
                neighborStreet.exits.Add(dirToOppositeExit[dir]);
                if (!neighborStreet.enqueued)
                {
                    neighborStreet.enqueued = true;
                    streetQueue.Enqueue(new Vector2Int(nx, ny));
                }
                exitsCreated++;
            }
        }

        InstantiateStreet(currentStreet, streetIndex); // Instantiate the street
        streetCount++;
        if (firstStreetGenerated == null) firstStreetGenerated = currentStreet;
        lastStreetGenerated = currentStreet;
    }

    // Shuffle a list of directions
    private void ShuffleList(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Instantiate a street at a specific index
    private void InstantiateStreet(StreetData currentStreet, Vector2Int streetIndex)
    {
        StreetType streetType = GetStreetTypeFromExits(currentStreet.exits);
        if (streetPoolDictionary.TryGetValue(streetType, out StreetPool pool))
        {
            GameObject prefabToInstantiate = pool.streetPrefabs[Random.Range(0, pool.streetPrefabs.Length)];
            Vector3 position = GetPositionFromGridIndex(streetIndex);
            GameObject streetInstance = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
            
            
            // Name the street instance based on its index in the grid
            streetInstance.name = $"Street [{streetIndex.x}, {streetIndex.y}]";
            streetObjects.Add(streetInstance);
            currentStreet.streetInstance = streetInstance;
        }
        else
        {
            Debug.LogWarning($"No StreetPool found for StreetType: {streetType}");
        }
    }

    // Cleanup exits after generation is complete
    private void CleanupExits()
    {
        foreach (var streetData in streetGrid)
        {
            if (streetData.hasStreet)
            {
                HashSet<ExitDirection> validExits = new HashSet<ExitDirection>();
                int x = streetData.gridIndex.x;
                int y = streetData.gridIndex.y;

                foreach (ExitDirection exit in streetData.exits)
                {
                    Vector2Int dir = exitToDir[exit];
                    int nx = x + dir.x;
                    int ny = y + dir.y;
                    if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;

                    StreetData neighborStreet = streetGrid[nx, ny];
                    if (neighborStreet.hasStreet && neighborStreet.exits.Contains(dirToOppositeExit[dir]))
                    {
                        validExits.Add(exit);
                    }
                }

                streetData.exits = validExits;
                UpdateStreetPrefab(streetData); // Update the street prefab to reflect valid exits
            }
        }
    }

    // Update the street prefab based on the exits
    private void UpdateStreetPrefab(StreetData streetData)
    {
        StreetType streetType = GetStreetTypeFromExits(streetData.exits);
        if (streetData.streetInstance != null)
        {
            Destroy(streetData.streetInstance);
            if (streetPoolDictionary.TryGetValue(streetType, out StreetPool pool))
            {
                GameObject prefabToInstantiate = pool.streetPrefabs[Random.Range(0, pool.streetPrefabs.Length)];
                Vector3 position = GetPositionFromGridIndex(streetData.gridIndex);
                GameObject streetInstance = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                
                // Name the street instance based on its index in the grid
                streetInstance.name = $"Street [{streetData.gridIndex.x}, {streetData.gridIndex.y}]";
                streetData.streetInstance = streetInstance;
            }
            else
            {
                Debug.LogWarning($"No StreetPool found for StreetType: {streetType}");
            }
        }
    }

    // Regenerate all streets
    private void RegenerateStreets()
    {
        foreach (var obj in streetObjects)
        {
            Destroy(obj);
        }
        streetObjects.Clear();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                streetGrid[x, y] = new StreetData { gridIndex = new Vector2Int(x, y) };
            }
        }

        streetQueue.Clear();
        streetCount = 0;
        generationComplete = false;

        StartStreetGenerationFromStreet(new Vector2Int(gridSizeX / 2, gridSizeY / 2)); // Restart generation from the center
    }

    // Get the position in the world from the grid index
    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        float isoX = (gridX - gridY) * (streetWidth / 2f);
        float isoY = (gridX + gridY) * (streetHeight / 4f);
        return new Vector3(isoX, isoY, 0);
    }

    // Draw gizmos in the editor for visualization
    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireCube(GetPositionFromGridIndex(new Vector2Int(x, y)), new Vector3(streetWidth, streetHeight, 0));
            }
        }
    }

    // Count the number of adjacent streets
    private int CountAdjacentStreets(Vector2Int streetIndex)
    {
        int x = streetIndex.x;
        int y = streetIndex.y;
        int count = 0;

        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY) continue;
            if (streetGrid[nx, ny].hasStreet) count++;
        }

        return count;
    }

    // Get the street type based on the exits
    private StreetType GetStreetTypeFromExits(HashSet<ExitDirection> exits)
    {
        ExitDirection combinedExits = ExitDirection.None;
        foreach (ExitDirection exit in exits)
        {
            combinedExits |= exit;
        }
        return (StreetType)combinedExits;
    }

    // Log the results of the street generation
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
        public Vector2Int gridIndex;
        public bool hasStreet = false;
        public bool enqueued = false;
        public HashSet<ExitDirection> exits = new HashSet<ExitDirection>();
        public GameObject streetInstance = null;
    }
}