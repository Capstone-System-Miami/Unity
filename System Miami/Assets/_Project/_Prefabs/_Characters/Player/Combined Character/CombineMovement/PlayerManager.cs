// Authors: Johnny
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    // Singleton Instance
    public static PlayerManager Instance { get; private set; }

    [Header("Component Groups")]
    [Tooltip("Components active in all modes")]
    public List<MonoBehaviour> sharedComponents; // Always enabled
    [Tooltip("Components active in neighborhood mode")]
    public List<MonoBehaviour> neighborhoodComponents;
    [Tooltip("Components active in Dungeon mode")]
    public List<MonoBehaviour> dungeonComponents;

    [Header("Scene Names")]
    public string neighborhoodSceneName = "Neighborhood"; // Name of the neighborhood scene
    public string dungeonSceneName = "Dungeon"; // Name of the Dungeon scene

    private void Awake()
    {
        // Enforce Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Ensure shared components are always enabled
        EnableComponents(sharedComponents);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check the name of the loaded scene and adjust components accordingly
        if (scene.name == neighborhoodSceneName)
        {
            EnterNeighborhood();
        }
        else if (scene.name == dungeonSceneName)
        {
            EnterDungeon();
        }
    }

    // Disables all components in the provided list
    private void DisableComponents(List<MonoBehaviour> componentList)
    {
        foreach (var component in componentList)
        {
            if (component != null)
                component.enabled = false;
        }
    }

    // Enables all components in the provided list
    private void EnableComponents(List<MonoBehaviour> componentList)
    {
        foreach (var component in componentList)
        {
            if (component != null)
                component.enabled = true;
        }
    }

    // Switches to Neighborhood Mode
    public void EnterNeighborhood()
    {
        Debug.Log("Entering Neighborhood Mode");
        DisableComponents(dungeonComponents);
        EnableComponents(neighborhoodComponents);
    }

    // Switches to Dungeon Mode
    public void EnterDungeon()
    {
        Debug.Log("Entering Dungeon Mode");
        DisableComponents(neighborhoodComponents);
        EnableComponents(dungeonComponents);
    }
}
