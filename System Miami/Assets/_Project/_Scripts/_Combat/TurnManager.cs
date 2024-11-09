using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.AbilitySystem;
using System;

namespace SystemMiami
{
    /// <summary>
    /// The phases of a turn.
    /// </summary>
    public enum Phase
    {
        MovementPhase,
        ActionPhase
    }

    /// <summary>
    /// Manages turns and phases in the combat system.
    /// Handles switching between player and enemy turns,
    /// as well as movement and action phases.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        // Singleton instance of the TurnManager
        public static TurnManager Instance;

        // List of all player characters
        public List<Combatant> playerCharacters;
        // List of all enemy characters
        public List<Combatant> enemyCharacters;

        // The current phase (Movement or Action)
        public Phase currentPhase;

        public GameObject enemyPrefab;
        public GameObject bossPrefab; // TODO: Assign boss prefab

        public int numberOfEnemies = 3;

        // Flag indicating if it's the player's turn
        public bool isPlayerTurn = true;

        public Action<Combatant> BeginTurn;
        public Action<Phase> NewTurnPhase;

        #region Unity Methods
        //===============================

        private void Awake()
        {
            // Initialize singleton instance
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            // Initialize lists
            if (playerCharacters == null)
            {
                playerCharacters = new List<Combatant>();
            }
            if (enemyCharacters == null)
            {
                enemyCharacters = new List<Combatant>();
            }

            // Set starting phase
            currentPhase = Phase.MovementPhase;
        }

        private void Start()
        {
            SpawnEnemies();
            // Initialize turns, start with first player
            StartPlayerTurn();
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
            isPlayerTurn = true;
            currentPhase = Phase.MovementPhase;

            // Reset player actions and movement points
            foreach (Combatant character in playerCharacters)
            {
                character.ResetTurn();
            }

            // Actions for other scripts to use
            BeginTurn?.Invoke(playerCharacters[0]);
            NewTurnPhase?.Invoke(Phase.MovementPhase);

            Debug.Log("Player's turn started. Movement Phase.");
        }

        /// <summary>
        /// Starts the enemy's turn.
        /// Resets movement points and action flags for each enemy character.
        /// </summary>
        public void StartAllEnemyTurns()
        {
            isPlayerTurn = false;
            currentPhase = Phase.MovementPhase;

            // Reset enemy actions and movement points
            foreach (Combatant enemy in enemyCharacters)
            {
                enemy.ResetTurn();
            }

            Debug.Log("Enemy's turn started.");

            // Start enemy AI coroutine
            StartCoroutine(EnemyTurnSequence());
        }

        /// <summary>
        /// Coroutine for handling enemy turns.
        /// Each enemy takes their movement and action phases in sequence.
        /// </summary>
        private IEnumerator EnemyTurnSequence()
        {
            foreach (Combatant enemyCombatant in enemyCharacters)
            {
                EnemyController enemyController = enemyCombatant.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    yield return StartCoroutine(enemyController.TakeTurn());
                }
                else
                {
                    Debug.LogWarning("EnemyController not found on enemy " + enemyCombatant.name);
                }
            }

            // After enemies have taken their turns, start player turn
            StartPlayerTurn();
        }


        /// <summary>
        /// Called when the player has finished their turn.
        /// Starts the enemy turn.
        /// </summary>
        public void EndPlayerTurn()
        {
            Debug.Log("Player's turn ended.");

            // Reduce cooldowns and update status effects for player
            foreach (Combatant player in playerCharacters)
            {
                player.GetComponent<Abilities>().ReduceCooldowns();
                player.Stats.UpdateStatusEffects();
            }
            // After player turn ends, start enemy turn
            StartAllEnemyTurns();
        }

        /// <summary>
        /// Called when the player wants to end the movement phase.
        /// Switches to action phase.
        /// </summary>
        public void EndMovementPhase()
        {
            if (isPlayerTurn)
            {
                currentPhase = Phase.ActionPhase;
                NewTurnPhase?.Invoke(Phase.ActionPhase);
                Debug.Log("Player's Action Phase started.");
            }
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

                    // Ensure the enemy has an EnemyController
                    EnemyController enemyController = enemyGO.GetComponent<EnemyController>();
                    if (enemyController == null)
                    {
                        enemyController = enemyGO.AddComponent<EnemyController>();
                        // You can also initialize enemyController properties here if needed
                    }

                    // Set enemy ID
                    enemyCombatant.ID = i + 1;

                    // Position enemy on the tile
                    PositionCharacterOnTile(enemyCombatant, spawnTile);

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
        private OverlayTile GetRandomUnblockedTile()
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

        /// <summary>
        /// Positions a character on the specified tile.
        /// </summary>
        private void PositionCharacterOnTile(Combatant character, OverlayTile tile)
        {
            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            character.CurrentTile = tile;

            // Update tile's current character
            tile.currentCharacter = character;
        }

        //===============================
        #endregion // ^Character Positioning and Spawning^
    }
}
