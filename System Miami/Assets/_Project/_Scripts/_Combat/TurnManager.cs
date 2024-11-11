using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    /// <summary>
    /// The phases of a turn.
    /// </summary>
    public enum Phase
    {
        Movement,
        Action,
        None
    }

    /// <summary>
    /// Manages turns and phases in the combat system.
    /// Handles switching between player and enemy turns,
    /// as well as movement and action phases.
    /// </summary>
    public class TurnManager : Singleton<TurnManager>
    {
        public Combatant playerCharacter;

        // List of all enemy characters
        public List<Combatant> enemyCharacters = new List<Combatant>();

        // List of all Combatants
        public List<Combatant> combatants = new List<Combatant>();

        public GameObject enemyPrefab;
        public GameObject bossPrefab; // TODO: Assign boss prefab

        public int numberOfEnemies = 3;

        public bool IsPlayerTurn
        {
            get
            {
                return CurrentTurnOwner.Controller is PlayerController;
            }
        }

        public Action<Combatant> BeginTurn;
        public Action<Phase> NewTurnPhase;
        public Action<Combatant> EndTurn;

        public Combatant CurrentTurnOwner { get; private set; }

        public bool IsGameOver = false;

        #region Unity Methods
        //===============================

        private void Start()
        {
            SpawnEnemies();

            combatants.Add(playerCharacter);
            combatants.AddRange(enemyCharacters);

            //// Initialize turns, start with first player
            //StartPlayerTurn();

            StartCoroutine(TurnSequence());
        }


        private void Update()
        {
            if (CurrentTurnOwner == null)
                { return; }

            Debug.Log($"Current Turn Owner: {CurrentTurnOwner.name}");
            if (!CurrentTurnOwner.Controller.IsMyTurn)
            {

            }
        }
        //===============================
        #endregion // ^Unity Methods^

        #region Turn Management
        //===============================

        /// <summary>
        /// Starts the player's turn.
        /// Resets movement points and action flags for each player character.
        /// </summary>
        public void StartPlayerTurn()
        {
            playerCharacter.Controller.StartTurn();
            CurrentTurnOwner = playerCharacter;

            // Actions for other scripts to use
            BeginTurn?.Invoke(playerCharacter);
            NewTurnPhase?.Invoke(Phase.Movement);

            Debug.Log("Player's turn started. Movement Phase.");
        }

        /// <summary>
        /// Starts the enemy's turn.
        /// Resets movement points and action flags for each enemy character.
        /// </summary>
        public void StartAllEnemyTurns()
        {
            // Start enemy AI coroutine
            StartCoroutine(TurnSequence());
        }

        /// <summary>
        /// Coroutine for handling enemy turns.
        /// Each enemy takes their movement and action phases in sequence.
        /// </summary>
        private IEnumerator TurnSequence()
        {
            while (!IsGameOver)
            {
                foreach (Combatant combatant in combatants)
                {
                    if (combatant == null)
                    { continue; }

                    if (combatant.Controller == null)
                    {
                        Debug.LogWarning($"CombatantController not found in {combatant} on {combatant.name}");
                        continue;
                    }

                    CurrentTurnOwner = combatant;
                    combatant.Controller.StartTurn();

                    yield return new WaitForEndOfFrame();
                    yield return new WaitUntil(() => !combatant.Controller.IsMyTurn);

                    //yield return StartCoroutine(controller.TakeTurn()); 
                }

                yield return null;
            }
        }


        /// <summary>
        /// Called when the player has finished their turn.
        /// Starts the enemy turn.
        /// </summary>
        public void EndPlayerTurn()
        {
            Debug.Log("Player's turn ended.");

            // Reduce cooldowns and update status effects for player
            playerCharacter.GetComponent<Abilities>().ReduceCooldowns();
            playerCharacter.Stats.UpdateStatusEffects();
            // After player turn ends, start enemy turn
            StartAllEnemyTurns();
        }

        /// <summary>
        /// Called when the player wants to end the movement phase.
        /// Switches to action phase.
        /// </summary>
        public void EndMovementPhase()
        {
            CurrentTurnOwner.Controller.TryNextPhase();
            Debug.Log($"{CurrentTurnOwner.name}'s Action Phase started.");
        }

        //===============================
        #endregion // ^Turn Management^

        #region Character Positioning and Spawning
        //===============================

        private void SpawnEnemies()
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                // Find a random unblocked tile to place the enemy
                OverlayTile spawnTile = GetRandomUnblockedTile();

                if (spawnTile != null)
                {
                    // Instantiate enemy
                    GameObject enemyGO = Instantiate(enemyPrefab);
                    Combatant enemyCombatant = enemyGO.GetComponent<Combatant>();
                    if (enemyCombatant == null)
                    {
                        enemyCombatant = enemyGO.AddComponent<Combatant>();
                    }

                    //// Ensure the enemy has an EnemyController
                    //EnemyController enemyController = enemyGO.GetComponent<EnemyController>();
                    //if (enemyController == null)
                    //{
                    //    enemyController = enemyGO.AddComponent<EnemyController>();
                    //    // You can also initialize enemyController properties here if needed
                    //}

                    // Set enemy ID
                    enemyCombatant.ID = i + 1;

                    // Position enemy on the tile
                    MapManager.MGR.PositionCharacterOnTile(enemyCombatant, spawnTile);

                    // Add to enemy list
                    enemyCharacters.Add(enemyCombatant);

                    Debug.Log($"Spawning {enemyCombatant}");
                }
                else
                {
                    Debug.LogWarning("No unblocked tiles available for spawning enemies.");
                }
            }
        }


        /// <summary>
        /// Finds a random unblocked tile on the map.
        /// </summary>
        /// <returns>An unblocked OverlayTile or null if none are available.</returns>
        public OverlayTile GetRandomUnblockedTile()
        {
            // Get all unblocked tiles
            List<OverlayTile> unblockedTiles = new List<OverlayTile>();

            foreach (var tile in MapManager.MGR.map.Values)
            {
                if (!tile.isBlocked && tile.currentCharacter == null)
                {
                    unblockedTiles.Add(tile);
                }
            }

            if (unblockedTiles.Count > 0)
            {
                // Select a random tile
                int index = UnityEngine.Random.Range(0, unblockedTiles.Count);
                return unblockedTiles[index];
            }

            return null;
        }

        //===============================
        #endregion // ^Character Positioning and Spawning^
    }
}
