using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject neighborhoodPlayerPrefab;
    public GameObject dungeonPlayerPrefab;

    private GameObject currentPlayer;

    void Awake()
    {
        // Ensure this persists across scenes (if required)
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize the player for the current scene
        SwitchPlayerForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchPlayerForScene(scene.name);
    }

    private void SwitchPlayerForScene(string sceneName)
    {
        if (currentPlayer != null)
            Destroy(currentPlayer); // Clean up the previous player instance

        // Choose the correct prefab based on the scene name
        if (sceneName == "Neighborhood")
        {
            currentPlayer = Instantiate(neighborhoodPlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else if (sceneName == "Dungeon")
        {
            currentPlayer = Instantiate(dungeonPlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"No player prefab assigned for scene: {sceneName}");
        }

        if (currentPlayer != null)
        {
            // Set Sorting Layer to "Base" and Order in Layer to 1
            SpriteRenderer spriteRenderer = currentPlayer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Default";
                spriteRenderer.sortingOrder = 1;
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found on the player prefab!");
            }

            // Optional: Set up any shared properties or references
            SetupPlayer(currentPlayer);
        }
    }



    private void SetupPlayer(GameObject player)
    {
        // Example: Linking player to game systems
        Debug.Log($"Player setup complete for {player.name}");

        // Inspect components on the player
        InspectComponents(player);
    }

    private void InspectComponents(GameObject player)
    {
        Debug.Log($"Inspecting components for {player.name}:");

        // List all attached components
        Component[] components = player.GetComponents<Component>();
        foreach (Component component in components)
        {
            Debug.Log($" - {component.GetType().Name}");
        }
    }

    public void CreatePrefabFromPlayer(GameObject player, string prefabName)
    {
        // Create a prefab from the current player setup
        string path = $"Assets/Prefabs/{prefabName}.prefab";

#if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(player, path);
        Debug.Log($"Prefab '{prefabName}' saved at {path}");
#else
        Debug.LogError("Prefab creation is only available in the Unity Editor.");
#endif
    }

    public void SwitchToPrefab(GameObject newPlayerPrefab)
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(newPlayerPrefab, Vector3.zero, Quaternion.identity);
        SetupPlayer(currentPlayer);
    }
}
