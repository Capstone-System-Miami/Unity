using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class EnemyController_v2 : MonoBehaviour/*CombatantController*/
    {
        //public EnemyType enemyType;
        //public List<Ability> abilities = new List<Ability>();

        //private Combatant enemy;
        //public int enemyDetectionRadius = 2;

        //private void Awake()
        //{
        //    enemy = GetComponent<Combatant>();

        //    // Initialize abilities
        //    for (int i = 0; i < abilities.Count; i++)
        //    {
        //        if (abilities[i] != null)
        //        {
        //            // Instantiate a new instance of the ability to avoid modifying the original asset
        //            Ability abilityInstance = Instantiate(abilities[i]);
        //            abilityInstance.Init(enemy);
        //            abilities[i] = abilityInstance;
        //        }
        //    }
        //}

        //public IEnumerator TakeTurn()
        //{
        //    // Movement Phase
        //    TurnManager.Instance.currentPhase = Phase.MovementPhase;
        //    TurnManager.Instance.BeginTurn?.Invoke(enemy);
        //    TurnManager.Instance.NewTurnPhase?.Invoke(Phase.MovementPhase);

        //    Debug.Log($"{enemy.name}'s Movement Phase.");
        //    yield return StartCoroutine(MovementPhase());

        //    // Action Phase
        //    TurnManager.Instance.currentPhase = Phase.ActionPhase;
        //    TurnManager.Instance.NewTurnPhase?.Invoke(Phase.ActionPhase);

        //    Debug.Log($"{enemy.name}'s Action Phase.");
        //    yield return StartCoroutine(ActionPhase());
        //}

        //private IEnumerator MovementPhase()
        //{
        //    // Check if any player is within the detection radius
        //    Combatant targetPlayer = FindNearestPlayerWithinRadius(enemyDetectionRadius);

        //    // Check if any player is within ability range
        //    Ability selectedAbility = SelectAbility();

        //    if (selectedAbility != null)
        //    {
        //        // Player is within ability range; no need to move
        //        yield return null;
        //    }
        //    else if (targetPlayer != null)
        //    {
        //        // Player is within detection radius, chase the player

        //        // Calculate path to the player
        //        PathFinder pathFinder = new PathFinder();
        //        List<OverlayTile> path = pathFinder.FindPath(enemy.CurrentTile, targetPlayer.CurrentTile);

        //        // Limit movement to enemy's movement points
        //        int movementPoints = (int)enemy.Speed.Get();
        //        if (path.Count > movementPoints)
        //        {
        //            path = path.GetRange(0, movementPoints);
        //        }

        //        // Move along the path
        //        foreach (OverlayTile tile in path)
        //        {
        //            if (tile.isBlocked || tile.currentCharacter != null)
        //            {
        //                // Cannot move to blocked or occupied tiles
        //                break;
        //            }

        //            // Decrement speed
        //            enemy.Speed.Lose(1);

        //            // Update tiles' currentCharacter
        //            enemy.CurrentTile.currentCharacter = null;
        //            enemy.CurrentTile = tile;
        //            tile.currentCharacter = enemy;

        //            // Move enemy's position
        //            enemy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        //            enemy.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

        //            yield return new WaitForSeconds(0.2f); // Wait for movement simulation
        //        }
        //    }
        //    else
        //    {
        //        // No player within detection radius, move randomly
        //        yield return StartCoroutine(EnemyRandomMove());
        //    }

        //    yield return null;
        //}

        //private IEnumerator ActionPhase()
        //{
        //    if (enemy.HasActed)
        //    {
        //        yield break; // Enemy has already acted
        //    }

        //    // Select an ability
        //    Ability selectedAbility = SelectAbility();

        //    if (selectedAbility != null)
        //    {
        //        // Use the selected ability
        //        yield return StartCoroutine(UseEnemyAbility(selectedAbility));
        //        enemy.HasActed = true;
        //        Debug.Log($"{enemy.name} used {selectedAbility.name}.");
        //    }
        //    else
        //    {
        //        // No ability can be used, so end action phase
        //        enemy.HasActed = true;
        //        Debug.Log($"{enemy.name} can't use any abilities.");
        //    }

        //    yield return new WaitForSeconds(0.5f);
        //}

        ///// <summary>
        ///// Executes the enemy's ability.
        ///// </summary>
        //private IEnumerator UseEnemyAbility(Ability ability)
        //{
        //    // Target the nearest player
        //    Combatant targetPlayer = FindNearestPlayer();

        //    if (targetPlayer != null)
        //    {
        //        // Set enemy's facing direction towards the player
        //        DirectionalInfo directionInfo = new DirectionalInfo(
        //            (Vector2Int)enemy.CurrentTile.gridLocation,
        //            (Vector2Int)targetPlayer.CurrentTile.gridLocation
        //        );

        //        enemy.setDirection(directionInfo);

        //        // Set targets for each action in the ability
        //        foreach (CombatAction action in ability.Actions)
        //        {
        //            action.TargetingPattern.SetTargets(directionInfo);
        //            action.TargetingPattern.LockTargets(); // Lock the targets so they don't change
        //        }

        //        // Use the ability
        //        yield return StartCoroutine(ability.Use());
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"{enemy.name} could not find a target to use {ability.name}.");
        //    }

        //    yield return null;
        //}


        ///// <summary>
        ///// Selects an ability for the enemy.
        ///// </summary>
        //private Ability SelectAbility()
        //{
        //    // Select the first ability that can be used
        //    foreach (Ability ability in abilities)
        //    {
        //        if (!ability.isOnCooldown)
        //        {
        //            // Check if any target is in range
        //            if (IsPlayerInAbilityRange(ability))
        //            {
        //                return ability;
        //            }
        //        }
        //    }
        //    return null; // No ability can be used
        //}

        ///// <summary>
        ///// Checks if any player is within the ability's range.
        ///// </summary>
        //private bool IsPlayerInAbilityRange(Ability ability)
        //{
        //    int maxRange = 2; // You can adjust this or use ability.range if available

        //    foreach (Combatant player in TurnManager.Instance.playerCharacters)
        //    {
        //        int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //                       Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //        if (distance <= maxRange)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Coroutine for enemy random movement when not chasing the player.
        ///// </summary>
        //private IEnumerator EnemyRandomMove()
        //{
        //    while ((int)enemy.Speed.Get() > 0)
        //    {
        //        // Get walkable neighbor tiles
        //        List<OverlayTile> walkableTiles = GetWalkableNeighbourTiles();

        //        if (walkableTiles.Count == 0)
        //        {
        //            // No walkable tiles available
        //            break;
        //        }

        //        // Decrement speed
        //        enemy.Speed.Lose(1);

        //        // Choose a random tile
        //        int index = UnityEngine.Random.Range(0, walkableTiles.Count);
        //        OverlayTile tile = walkableTiles[index];

        //        // Update tiles' currentCharacter
        //        enemy.CurrentTile.currentCharacter = null;
        //        enemy.CurrentTile = tile;
        //        tile.currentCharacter = enemy;

        //        // Move enemy's position
        //        enemy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        //        enemy.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

        //        yield return new WaitForSeconds(0.2f); // Wait for movement simulation
        //    }

        //    yield return null;
        //}

        ///// <summary>
        ///// Gets walkable neighbor tiles for random movement.
        ///// </summary>
        //private List<OverlayTile> GetWalkableNeighbourTiles()
        //{
        //    PathFinder pathFinder = new PathFinder();
        //    List<OverlayTile> neighbours = pathFinder.GetNeighbourTiles(enemy.CurrentTile);
        //    List<OverlayTile> walkableTiles = new List<OverlayTile>();

        //    foreach (OverlayTile tile in neighbours)
        //    {
        //        if (!tile.isBlocked && tile.currentCharacter == null)
        //        {
        //            walkableTiles.Add(tile);
        //        }
        //    }

        //    return walkableTiles;
        //}

        ///// <summary>
        ///// Finds the nearest player character to the enemy.
        ///// </summary>
        //private Combatant FindNearestPlayer()
        //{
        //    Combatant nearestPlayer = null;
        //    int shortestDistance = int.MaxValue;

        //    foreach (Combatant player in TurnManager.Instance.playerCharacters)
        //    {
        //        int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //                       Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //        if (distance < shortestDistance)
        //        {
        //            shortestDistance = distance;
        //            nearestPlayer = player;
        //        }
        //    }

        //    return nearestPlayer;
        //}

        ///// <summary>
        ///// Finds the nearest player character within a given radius of the enemy.
        ///// </summary>
        //private Combatant FindNearestPlayerWithinRadius(int radius)
        //{
        //    Combatant nearestPlayer = null;
        //    int shortestDistance = int.MaxValue;

        //    foreach (Combatant player in TurnManager.Instance.playerCharacters)
        //    {
        //        int distance = Mathf.Abs(enemy.CurrentTile.gridLocation.x - player.CurrentTile.gridLocation.x) +
        //                       Mathf.Abs(enemy.CurrentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        //        if (distance <= radius && distance < shortestDistance)
        //        {
        //            shortestDistance = distance;
        //            nearestPlayer = player;
        //        }
        //    }

        //    return nearestPlayer;
        //}

        //private void OnDestroy()
        //{
        //    // Remove the enemy from the TurnManager's list when destroyed
        //    TurnManager.Instance.enemyCharacters.Remove(enemy);
        //}
    }
}
