// Authors: Johnny, Layla
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemMiami.Management;
using SystemMiami.CombatSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SystemMiami
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        #region SERIALIZED
        // ======================================

        [SerializeField] bool showDebug;

        [Header("Component Groups")]
        [Tooltip("Components active in all modes")]
        [SerializeField] private List<Component> sharedComponents = new List<Component>(); // Always enabled

        [Tooltip("Components active in neighborhood mode")]
        [SerializeField] private List<Component> neighborhoodComponents = new List<Component>();
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject interactionUI;

        [Tooltip("Components active in Dungeon mode")]
        [SerializeField] private List<Component> dungeonComponents = new List<Component>();

        [Header("Scene Names")]
        [SerializeField] private string neighborhoodSceneName = "Neighborhood"; // Name of the neighborhood scene
        [SerializeField] private string dungeonSceneName = "Dungeon"; // Name of the Dungeon scene

        // ======================================
        #endregion // SERIALIZED


        #region PRIVATE VARS
        // ======================================

        private bool beenToCombat;
        private Vector3 neighborhoodReturnPos;

        // ======================================
        #endregion // SERIALIZED


        #region UNITY METHODS
        // ======================================

        private void OnEnable()
        {
            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        private void OnDisable()
        {
            // Unsubscribe from scene loaded event
            SceneManager.sceneLoaded -= onSceneLoaded;
        }

        protected override void Awake()
        {
            if (MGR != null && TurnManager.MGR != null)
            {
                MGR.transform.position = transform.position;
                TurnManager.MGR.playerCharacter = MGR.GetComponent<Combatant>();
            }

            base.Awake();
        }

        private void Start()
        {
            // Ensure shared components are always enabled
            enableComponents(sharedComponents);
        }
        // ======================================
        #endregion


        #region PRIVATE METHODS
        // ======================================
        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Check the name of the loaded scene and adjust components accordingly
            if (scene.name.Contains(neighborhoodSceneName))
            {
                EnterNeighborhood();
            }
            else if (scene.name.Contains(dungeonSceneName))
            {
                EnterDungeon();
            }
        }

        // Disables all components in the provided list
        private void disableComponents(List<Component> componentList)
        {
            foreach (Component component in componentList)
            {
                // Null check
                if (component == null) { print("component was null"); continue; }

                // Component is not enableable, cast to Behaviour
                Behaviour behaviour = component as Behaviour;

                // If cast fails, skip this Component
                if (behaviour == null) { print($"cast to behaviour failed on {component}"); continue; }

                behaviour.enabled = false;
            }
        }

        // Enables all components in the provided list
        private void enableComponents(List<Component> componentList)
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

        private void returnToStoredPos()
        {
            if (!beenToCombat) { return; }

            if (showDebug)
            {
                Debug.Log($"{this} is returning to a previously stored position.\n" +
                    $"Will return to: {neighborhoodReturnPos}");
            }

            transform.position = neighborhoodReturnPos;
        }
        // ======================================
        #endregion


        #region PUBLIC METHODS
        // ======================================

        // Switches to Neighborhood Mode
        public void EnterNeighborhood()
        {
            Debug.Log("Entering Neighborhood Mode");

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerCamera.SetActive(true);
            interactionUI.SetActive(true);
            interactionUI.GetComponentInChildren<PromptBox>().Clear();
            
            disableComponents(dungeonComponents);
            enableComponents(neighborhoodComponents);

            returnToStoredPos();
        }

        // Switches to Dungeon Mode
        public void EnterDungeon()
        {
            Debug.Log("Entering Dungeon Mode");

            beenToCombat = true;

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerCamera.SetActive(false);
            interactionUI.SetActive(false);

            disableComponents(neighborhoodComponents);
            enableComponents(dungeonComponents);
        }

        public void StoreNeighborhoodPosition()
        {
            if (showDebug)
            {
                Debug.Log($"{this}'s position is being stored for Neighborhood re-entry.\n" +
                    $"Position is: {transform.position}");
            }

            neighborhoodReturnPos = transform.position;
        }
        // ======================================
        #endregion
    }
}