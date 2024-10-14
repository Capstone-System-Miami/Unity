using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        public List<CharacterInfo> playerCharacters;
        // List of all enemy characters
        public List<CharacterInfo> enemyCharacters;

        // The current phase (Movement or Action)
        public Phase currentPhase;

        // Index of the current player character taking their turn
        private int currentPlayerIndex = 0;
        // Index of the current enemy character taking their turn
        private int currentEnemyIndex = 0;

        // Flag indicating if it's the player's turn
        public bool isPlayerTurn = true;

        private void Awake()
        {
            // Initialize singleton instance
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            // Initialize lists
            playerCharacters = new List<CharacterInfo>();
            enemyCharacters = new List<CharacterInfo>();

            // Set starting phase
            currentPhase = Phase.MovementPhase;
        }

        private void Start()
        {
            // Initialize turns, start with first player
            StartPlayerTurn();
        }

        /// <summary>
        /// Starts the player's turn.
        /// Resets movement points and action flags for each player character.
        /// </summary>
        public void StartPlayerTurn()
        {
            isPlayerTurn = true;
            currentPhase = Phase.MovementPhase;

            // Reset player actions and movement points
            foreach (CharacterInfo character in playerCharacters)
            {
                character.ResetTurn();
            }

            currentPlayerIndex = 0;

            Debug.Log("Player's turn started. Movement Phase.");
        }

        /// <summary>
        /// Starts the enemy's turn.
        /// Resets movement points and action flags for each enemy character.
        /// </summary>
        public void StartEnemyTurn()
        {
            isPlayerTurn = false;
            currentPhase = Phase.MovementPhase;

            // Reset enemy actions and movement points
            foreach (CharacterInfo enemy in enemyCharacters)
            {
                enemy.ResetTurn();
            }

            currentEnemyIndex = 0;

            Debug.Log("Enemy's turn started.");

            // Start enemy AI coroutine
            StartCoroutine(EnemyTurn());
        }

        /// <summary>
        /// Coroutine for handling enemy turns.
        /// Each enemy takes their movement and action phases in sequence.
        /// </summary>
        private IEnumerator EnemyTurn()
        {
            while (currentEnemyIndex < enemyCharacters.Count)
            {
                CharacterInfo enemy = enemyCharacters[currentEnemyIndex];

                // Enemy movement phase
                currentPhase = Phase.MovementPhase;

                Debug.Log("Enemy " + currentEnemyIndex + " Movement Phase.");

                // Enemy AI for movement
                // Placeholder: Move randomly for now
                yield return StartCoroutine(EnemyMove(enemy));

                // Enemy action phase
                currentPhase = Phase.ActionPhase;

                Debug.Log("Enemy " + currentEnemyIndex + " Action Phase.");

                // Enemy AI for action
                // Placeholder: Do nothing for now
                yield return StartCoroutine(EnemyAction(enemy));

                currentEnemyIndex++;
            }

            // After enemies have taken their turns, start player turn
            StartPlayerTurn();
        }

        /// <summary>
        /// Coroutine for enemy movement phase.
        /// Placeholder 
        /// </summary>
        private IEnumerator EnemyMove(CharacterInfo enemy)
        {
            // Placeholder for enemy movement AI
            // For now, wait a second to simulate movement
            Debug.Log("Enemy moving...");
            yield return new WaitForSeconds(1f);
        }

        /// <summary>
        /// Coroutine for enemy action phase.
        /// Placeholder
        /// </summary>
        private IEnumerator EnemyAction(CharacterInfo enemy)
        {
            // Placeholder for enemy action AI
            // just waits a sec for now
            Debug.Log("Enemy acting...");
            yield return new WaitForSeconds(1f);
        }

        /// <summary>
        /// Called when the player has finished their turn.
        /// Starts the enemy turn.
        /// </summary>
        public void EndPlayerTurn()
        {
            Debug.Log("Player's turn ended.");
            // After player turn ends, start enemy turn
            StartEnemyTurn();
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
                Debug.Log("Player's Action Phase started.");
                // Update UI or allow player to perform actions TODO
            }
            else
            {
                // For enemies, this is called in the EnemyTurn coroutine
            }
        }
    }
}
