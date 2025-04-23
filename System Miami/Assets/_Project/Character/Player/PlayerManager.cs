// Authors: Johnny, Layla
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemMiami.Management;
using SystemMiami.CombatSystem;
using SystemMiami.InventorySystem;
using SystemMiami.Utilities;
using SystemMiami.Drivers;
using SystemMiami.Dungeons;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SystemMiami
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        #region SERIALIZED
        // ======================================
        [Header("Debug")]
        [SerializeField] private dbug log = new();

        [Header("Component Groups")]
        [Tooltip("Components active in all modes")]
        [SerializeField] private List<Component> sharedComponents = new(); // Always enabled
 
        [Tooltip("Components active in neighborhood mode")]
        [SerializeField] private List<Component> neighborhoodComponents = new();
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject interactionUI;

        [Tooltip("Components active in Dungeon mode")]
        [SerializeField] private List<Component> dungeonComponents = new();

        [Header("Scene Names")]
        [SerializeField] private string neighborhoodSceneName = "Neighborhood"; // Name of the neighborhood scene
        [SerializeField] private string dungeonSceneName = "Dungeon"; // Name of the Dungeon scene

        [Header("Other")]
        [SerializeField] private PlayerSprites playerSprites;
        public Sprite PlayerSprite
        {
            get
            {
                CharacterClassType charClass = GetComponent<Attributes>()._characterClass;
                return playerSprites.GetClassPFP(charClass);
            }
        }

        // ======================================
        #endregion // SERIALIZED


        #region PRIVATE VARS
        // ======================================

        private PlayerLevel level;
        private Attributes attributes;
        public Inventory inventory;

        private bool beenToCombat;
        private Vector3 neighborhoodReturnPos;

        // ======================================
        #endregion // SERIALIZED

        #region PROPERTIES

        public int CurrentLevel => level.CurrentLevel;
        public int CurrentCredits => inventory.Credits;
        public CharacterClassType CharClass => attributes._characterClass;
        public CharClassAnimationDriver AnimDriver;
        #endregion


        #region UNITY METHODS
        // ======================================
        private void OnEnable()
        {
            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            // Unsubscribe from scene loaded event
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        protected override void Awake()
        {
            // WARN: Not sure what this is, but I'm too scared to change it now
            // Just want to flag this as a potential source of bugs
            if (MGR != null && TurnManager.MGR != null)
            {
                MGR.transform.position = transform.position;
                TurnManager.MGR.playerCharacter = MGR.GetComponent<Combatant>();
            }

            base.Awake();
            
            if (!TryGetComponent(out level))
            {
                log.error($"{name}'s {this} didn't find a PlayerLevel");
            }

            if (!TryGetComponent(out inventory))
            {
                log.error($"{name}'s {this} didn't find an Inventory");
            }
        }

        private void Start()
        {
            // Ensure shared components are always enabled
            EnableComponents(sharedComponents);
        }
        // ======================================
        #endregion


        #region PRIVATE METHODS
        // ======================================
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
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
            else
            {
                DisableComponents(neighborhoodComponents);
                DisableComponents(dungeonComponents);

                // NOTE: 04.16 added this
                //
                EnableComponents(sharedComponents);
            }
        }

        // Disables all components in the provided list
        private void DisableComponents(List<Component> componentList)
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

        private void ReturnToStoredPos()
        {
            if (!beenToCombat) { return; }

            log.print($"{this} is returning to a previously stored position.\n" +
                $"Will return to: {neighborhoodReturnPos}");

            transform.position = neighborhoodReturnPos;
        }
        // ======================================
        #endregion


        #region PUBLIC METHODS
        // ======================================

        // Switches to Neighborhood Mode
        public void EnterNeighborhood()
        {
            log.print("Entering Neighborhood Mode");

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerCamera.SetActive(true);
            interactionUI.SetActive(true);
            interactionUI.GetComponentInChildren<PromptBox>().Clear();

            DisableComponents(dungeonComponents);
            EnableComponents(neighborhoodComponents);

            ReturnToStoredPos();
        }

        // Switches to Dungeon Mode
        public void EnterDungeon()
        {
            log.print("Entering Dungeon Mode");

            beenToCombat = true;

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerCamera.SetActive(false);
            interactionUI.SetActive(false);

            DisableComponents(neighborhoodComponents);
            EnableComponents(dungeonComponents);
        }

        public void StoreNeighborhoodPosition()
        {
            log.print($"{this}'s position is being stored for Neighborhood re-entry.\n" +
                $"Position is: {transform.position}");

            neighborhoodReturnPos = transform.position;
        }
        // ======================================
        #endregion
    }
}
