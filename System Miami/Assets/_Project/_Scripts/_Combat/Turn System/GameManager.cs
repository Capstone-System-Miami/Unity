//Johnny
using UnityEngine;

namespace CombatSystem.outdated
{


    public class GameManager : MonoBehaviour
    {
        public bool playerTurn = true; // Indicates whose turn it is
        public GameObject enemy; // Reference to the enemy GameObject
        public GameObject player; // Reference to the player GameObject

        void Update()
        {
            // Player turn logic
            if (playerTurn && Input.GetMouseButtonDown(0))
            {
                // Handle player actions (e.g., attacking the enemy)
                HandlePlayerActions();
            }

            // Check if it's time for the enemy's turn
            if (!playerTurn)
            {
                HandleEnemyTurn();
            }
        }

        void HandlePlayerActions()
        {
            // Implement player actions (e.g., attacking, movement)

            // After player actions, switch to enemy turn
            playerTurn = false;
        }

        void HandleEnemyTurn()
        {
            // Call the enemy's attack method
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
               
            }

            // After the enemy's attack, switch back to player turn
            playerTurn = true;
        }
    }
}
