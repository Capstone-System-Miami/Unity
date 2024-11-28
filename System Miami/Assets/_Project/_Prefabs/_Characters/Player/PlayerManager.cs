// Authors: Johnny, Layla
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemMiami.Management;

namespace SystemMiami
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("Component Groups")]
        [Tooltip("Components active in all modes")]
        public List<Component> sharedComponents; // Always enabled

        [Tooltip("Components active in neighborhood mode")]
        public List<Component> neighborhoodComponents;

        [Tooltip("Components active in Dungeon mode")]
        public List<Component> dungeonComponents;

        [Header("Scene Names")]
        public string neighborhoodSceneName = "Neighborhood"; // Name of the neighborhood scene
        public string dungeonSceneName = "Dungeon"; // Name of the Dungeon scene

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
        private void DisableComponents(List<Component> componentList)
        {
            foreach (Component component in componentList)
            {
                // Null check
                if (component == null) { continue; }

                // Component is not enableable, cast to Behaviour
                Behaviour behaviour = component as Behaviour;

                // If cast fails, skip this Component
                if (behaviour == null) { continue; }

                behaviour.enabled = false;
            }
        }

        // Enables all components in the provided list
        private void EnableComponents(List<Component> componentList)
        {
            foreach (Component component in componentList)
            {
                // Null check
                if (component == null) { continue; }

                // Component is not enableable, cast to Behaviour
                Behaviour behaviour = component as Behaviour;

                // If cast fails, skip this Component
                if (behaviour == null) { continue; }

                behaviour.enabled = true;
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
}