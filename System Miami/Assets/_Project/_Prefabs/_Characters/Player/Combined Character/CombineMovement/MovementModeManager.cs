using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class MovementModeManager : MonoBehaviour
{
    // References to the components for Neighborhood and dungeon modes
    [Header("Neighborhood Components")]
    public MonoBehaviour NeighborhoodMovement; // WASD movement script
    public GameObject NeighborhoodUI;          // Any UI specific to Neighborhood

    [Header("Dungeon Components")]
    public MonoBehaviour dungeonMovement;   // Tile-based movement script
    public GameObject dungeonUI;            // Any UI specific to dungeon

    private void Start()
    {
        // Automatically detect and set the movement mode based on the current scene
        CheckSceneAndSetMode();
    }

    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene load events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneAndSetMode();
    }

    // Check the active scene and set the appropriate movement mode
    private void CheckSceneAndSetMode()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Neighborhood") // Replace with your Neighborhood scene name
        {
            EnterNeighborhood();
        }
        else if (currentSceneName == "Dungeon") // Replace with your dungeon scene name
        {
            EnterDungeon();
        }
        else
        {
            Debug.LogWarning("Unrecognized scene: " + currentSceneName);
        }
    }

    // Method to enable Neighborhood mode
    public void EnterNeighborhood()
    {
        // Enable Neighborhood components
        NeighborhoodMovement.enabled = true;
        if (NeighborhoodUI != null) NeighborhoodUI.SetActive(true);

        // Disable dungeon components
        dungeonMovement.enabled = false;
        if (dungeonUI != null) dungeonUI.SetActive(false);

        Debug.Log("Entered Neighborhood Mode");
    }

    // Method to enable dungeon mode
    public void EnterDungeon()
    {
        // Enable dungeon components
        dungeonMovement.enabled = true;
        if (dungeonUI != null) dungeonUI.SetActive(true);

        // Disable Neighborhood components
        NeighborhoodMovement.enabled = false;
        if (NeighborhoodUI != null) NeighborhoodUI.SetActive(false);

        Debug.Log("Entered Dungeon Mode");
    }
}
