using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.AbilitySystem;

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

        // Index of the current player character taking their turn
        private int currentPlayerIndex = 0;
        // Index of the current enemy character taking their turn
        private int currentEnemyIndex = 0;

        public GameObject enemyPrefab;

        public int numberOfEnemies = 3;

        // Flag indicating if it's the player's turn
        public bool isPlayerTurn = true;

        public int enemyDetectionRadius = 2;

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
            foreach (Combatant enemy in enemyCharacters)
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
                Combatant enemy = enemyCharacters[currentEnemyIndex];

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
                    // (layla note) This will happen the same frame as movement is called
                yield return StartCoroutine(EnemyAction(enemy));

                currentEnemyIndex++;
            }

            // After enemies have taken their turns, start player turn
            StartPlayerTurn();
        }

        /// <summary>
        /// Spawns enemies on the map.
        /// </summary>
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
                    Combatant enemy = enemyGO.GetComponent<Combatant>();

                    // Position enemy on the tile
                    PositionCharacterOnTile(enemy, spawnTile);

                    // Add to enemy list
                    enemyCharacters.Add(enemy);
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
                int index = Random.Range(0, unblockedTiles.Count);
                return unblockedTiles[index];
            }

            return null;
        }

        /// <summary>
        /// Coroutine for enemy movement phase.
        /// Placeholder 
        /// </summary>

        private IEnumerator EnemyMove(Combatant enemy)
        {
            // Check if any player is within the detection radius
            Combatant targetPlayer = FindNearestPlayerWithinRadius(enemy, enemyDetectionRadius);

            if (targetPlayer != null)
            {
                // Player is within detection radius, chase the player

                // Calculate path to the player
                PathFinder pathFinder = new PathFinder();
                List<OverlayTile> path = pathFinder.FindPath(enemy.CurrentTile, targetPlayer.CurrentTile);

                // Limit movement to enemy's movement points
                int movementPoints = (int)enemy.Speed.Get();
                if (path.Count > movementPoints)
                {
                    path = path.GetRange(0, movementPoints);
                }

                // Move along the path
                foreach (OverlayTile tile in path)
                {
                    if (tile.isBlocked || tile.currentCharacter != null)
                    {
                        // Cannot move to blocked or occupied tiles
                        break;
                    }

                    // Decrement speed
                    enemy.Speed.Lose(1);

                    // Update tiles' currentCharacter
                    enemy.CurrentTile.currentCharacter = null;
                    enemy.CurrentTile = tile;
                    tile.currentCharacter = enemy;

                    // Move enemy's position
                    enemy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
                    enemy.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

                    yield return new WaitForSeconds(0.2f); // Wait for movement simulation
                }
            }
            else
            {
                // No player within detection radius, move randomly
                yield return StartCoroutine(EnemyRandomMove(enemy));
            }

            yield return null;
        }

        /// <summary>
        /// Coroutine for enemy action phase.
        /// </summary>
        private IEnumerator EnemyAction(Combatant enemy)
        {
            // attack player if adjacent
            Combatant adjacentPlayer = FindAdjacentPlayer(enemy);

            if (adjacentPlayer != null)
            {
                // Use basic attack
                print($"{name} has attacked!");
                    enemy.HasActed = true;
                
            }

            yield return new WaitForSeconds(0.5f); // Wait for action simulation
        }

        /// <summary>
        /// Finds the nearest player character to the enemy.
        /// </summary>
        private Combatant FindNearestPlayer(Combatant enemy)
        {
            Combatant nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Combatant player in playerCharacters)
            {
                int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
                               Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        /// <summary>
        /// Finds the nearest player character within a given radius of the enemy.
        /// </summary>
        private Combatant FindNearestPlayerWithinRadius(Combatant enemy, int radius)
        {
            Combatant nearestPlayer = null;
            int shortestDistance = int.MaxValue;

            foreach (Combatant player in playerCharacters)
            {
                int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
                               Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

                if (distance <= radius && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlayer = player;
                }
            }

            return nearestPlayer;
        }

        /// <summary>
        /// Coroutine for enemy random movement when not chasing the player.
        /// </summary>
        private IEnumerator EnemyRandomMove(Combatant enemy)
        {
            while ((int)enemy.Speed.Get() > 0)
            {
                // Get walkable neighbor tiles
                List<OverlayTile> walkableTiles = GetWalkableNeighbourTiles(enemy.CurrentTile);

                if (walkableTiles.Count == 0)
                {
                    // No walkable tiles available
                    break;
                }

                // Decrement speed
                enemy.Speed.Lose(1);

                // Choose a random tile
                int index = Random.Range(0, walkableTiles.Count);
                OverlayTile tile = walkableTiles[index];

                // Update tiles' currentCharacter
                enemy.CurrentTile.currentCharacter = null;
                enemy.CurrentTile = tile;
                tile.currentCharacter = enemy;

                // Move enemy's position
                enemy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
                enemy.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

                yield return new WaitForSeconds(0.2f); // Wait for movement simulation
            }

            yield return null;
        }

        /// <summary>
        /// Gets walkable neighbor tiles for random movement.
        /// </summary>
        private List<OverlayTile> GetWalkableNeighbourTiles(OverlayTile currentTile)
        {
            PathFinder pathFinder = new PathFinder();
            List<OverlayTile> neighbours = pathFinder.GetNeighbourTiles(currentTile);
            List<OverlayTile> walkableTiles = new List<OverlayTile>();

            foreach (OverlayTile tile in neighbours)
            {
                if (!tile.isBlocked && tile.currentCharacter == null)
                {
                    walkableTiles.Add(tile);
                }
            }

            return walkableTiles;
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

        /// <summary>
        /// Positions a character on the specified tile.
        /// </summary>
        private void PositionCharacterOnTile(Combatant character, OverlayTile tile)
        {
            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            character.CurrentTile = tile;

            // Update tile's current character
            tile.currentCharacter = character;
        }

        private Combatant FindAdjacentPlayer(Combatant enemy)
        {
            PathFinder pathFinder = new PathFinder();
            List<OverlayTile> neighbours = pathFinder.GetNeighbourTiles(enemy.CurrentTile);

            foreach (OverlayTile tile in neighbours)
            {
                if (tile.currentCharacter != null && playerCharacters.Contains(tile.currentCharacter))
                {
                    return tile.currentCharacter;
                }
            }

            return null;
        }


    }
}
