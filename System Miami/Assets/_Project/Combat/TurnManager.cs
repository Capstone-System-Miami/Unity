// Author: Lee St Louis, Layla Hoey
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using SystemMiami.ui;
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
    /// Spawns enemies.
    /// </summary>
    public class TurnManager : Singleton<TurnManager>
    {
        public Combatant playerCharacter;

        // List of all enemy characters
        public List<Combatant> enemyCharacters = new List<Combatant>();

        // List of all Combatants
        public List<Combatant> combatants = new List<Combatant>();

        [Header("Testing")]
        public GameObject enemyPrefab;
        public GameObject bossPrefab; // TODO: Assign boss prefab
        public List<GameObject> enemyPrefabs = new();
        public int numberOfEnemies = 3;

        [SerializeField] private GoToNeighborhood goToNeighborhood;

        [SerializeField, ReadOnly] private int enemiesRemaining;

        public bool IsPlayerTurn
        {
            get
            {
                if (CurrentTurnOwner == null) { return false; }

                return CurrentTurnOwner is PlayerCombatant;
            }
        }

        public Combatant CurrentTurnOwner { get; private set; }

        public bool IsGameOver
        {
            get
            {
                if (playerCharacter == null) { return true; }
                if (enemyCharacters.Count == 0) { return true; }

                return false;
            }
        }

        private Dictionary<object, Action> dungeonClearedActions = new();
        public Action<Phase> NewTurnPhase;

        #region Unity Methods
        //===============================
        private void OnEnable()
        {
            GAME.MGR.CombatantDeath += OnCombatantDeath;
        }
        private void Start()
        {
            if (playerCharacter != null)
            {
                Vector3Int charTilePos = Coordinates.ScreenToIso(playerCharacter.transform.position, 0);

                if (!MapManager.MGR.map.TryGetValue((Vector2Int)charTilePos, out OverlayTile charTile))
                {
                    if (!MapManager.MGR.map.TryGetValue((Vector2Int.zero), out charTile))
                    {
                        Debug.LogError(
                            $"{this} failed to find a tile " +
                            $"to place the player on.");
                        return;
                    }
                }

                if (!MapManager.MGR.TryPlaceOnTile(playerCharacter, charTile))
                {
                    Debug.LogError(
                        $"{this} tried to place {playerCharacter} " +
                        $"through the MapManager, but it failed.");
                    return;
                }

                playerCharacter.InitAll();
            }

            combatants.Add(playerCharacter);
            if (GAME.MGR.TryGetEnemies(out enemyPrefabs))
            {
                SpawnEnemies(enemyPrefabs);
            }
            else
            {
                SpawnEnemies();
            }

            combatants.AddRange(enemyCharacters);

            StartCoroutine(TurnSequence());
        }

        private void Update()
        {
            if (CurrentTurnOwner == null)
            { return; }


            //Debug.Log($"Current Turn Owner: {CurrentTurnOwner.name}");
        }

        private void OnDisable()
        {
            GAME.MGR.CombatantDeath -= OnCombatantDeath;
        }
        //===============================
        #endregion // ^Unity Methods^

        #region Turn Management
        //===============================

        /// <summary>
        /// Coroutine for handling enemy turns.
        /// Each enemy takes their movement and action phases in sequence.
        /// </summary>
        private IEnumerator TurnSequence()
        {
            bool combatantsReady = true;

            do
            {
                combatantsReady = true;
                foreach (Combatant combatant in combatants)
                {
                    if (!combatant.ReadyToStart)
                    {
                        combatantsReady = false;
                        break;
                    }
                }
                Debug.Log("IN");
                yield return null;
            } while (!combatantsReady);

            Debug.Log("OUT");
            enemiesRemaining = enemyCharacters.Count;
            while (!IsGameOver)
            {
                // Remove null items.
                // These nulls can occur when
                // combatants die.
                combatants.RemoveAll(combatant => combatant == null);

                foreach (Combatant combatant in combatants)
                {
                    if (combatant == null) { continue; }

                    // TODO this is a werid way of
                    // 'forcing' a state switch, as
                    // states should control their own
                    // transitions. Consider converting
                    // this to a request or something.
                    combatant.CurrentState.SwitchState(combatant.Factory.TurnStart());

                    CurrentTurnOwner = combatant;
                    yield return new WaitForEndOfFrame();
                    yield return new WaitUntil(() => !combatant.IsMyTurn);
                }

                yield return null;
            }
        }

        //===============================
        #endregion // ^Turn Management^

        #region Spawning
        //===============================
        private void SpawnEnemies()
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                // Find a random unblocked tile to place the enemy
                OverlayTile spawnTile = MapManager.MGR.GetRandomValidTile();

                if (spawnTile == null)
                {
                    Debug.LogWarning("No unblocked tiles available for spawning enemies.");
                }

                SpawnEnemy(spawnTile, enemyPrefab, (i + 1));
            }
        }

        private void SpawnEnemies(List<GameObject> prefabs)
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                OverlayTile spawnTile = MapManager.MGR.GetRandomValidTile();

                if (spawnTile == null)
                {
                    Debug.LogWarning("No unblocked tiles available for spawning enemies.");
                }

                SpawnEnemy(spawnTile, prefabs[i], (i + 1));
            }
        }

        private void SpawnEnemy(OverlayTile spawnTile, GameObject prefab, int id)
        {
            // Instantiate enemy
            GameObject enemyGO = Instantiate(prefab);
            Combatant enemyCombatant = enemyGO.GetComponent<Combatant>();


            if (enemyCombatant == null)
            {
                enemyCombatant = enemyGO.AddComponent<Combatant>();
            }


            // Set enemy ID
            enemyCombatant.ID = id;

            // Set enemy name
            string newName = enemyCombatant.name;
            newName = newName.Replace("(Clone)", "");
            newName += $" {enemyCombatant.ID}";
            enemyCombatant.name = newName;

            // Position enemy on the tile
            if (!MapManager.MGR.TryPlaceOnTile(enemyCombatant, spawnTile))
            {
                Debug.LogError($"" +
                    $"{this} couldn't place " +
                    $"{enemyCombatant.gameObject} " +
                    $"on {spawnTile.gameObject}");
            }

            // Add to enemy list
            enemyCharacters.Add(enemyCombatant);

            Debug.Log($"Spawning {enemyCombatant}");
        }

        public void OnCombatantDeath(Combatant combatant)
        {
            Debug.Log("Combatant Death called" + combatant.name + " has died");

            if (combatant is EnemyCombatant && --enemiesRemaining == 0)
            {
                OnDungeonCleared();
            }
            else if (combatant is PlayerCombatant)
            {
                GAME.MGR.GoToCharacterSelect();
            }
        }

        public void AddDungeonClearedAction(object client, Action action)
        {
            dungeonClearedActions[client] = action;
        }

        protected void OnDungeonCleared()
        {
            List<object> nulls = new();

            foreach (object client in dungeonClearedActions.Keys)
            {
                if (client == null
                    || (client is GameObject go && !go.activeSelf)
                    || (client is MonoBehaviour mono && !mono.enabled))
                {
                    Debug.LogError($"{name} adding a null to null list");
                    continue;
                }

            }
            foreach (object client in nulls)
            {
                Debug.LogError($"{name} removing a null from dict");
                dungeonClearedActions.Remove(client);
            }

            foreach (object client in dungeonClearedActions.Keys)
            {
                dungeonClearedActions[client].Invoke();
            }

            goToNeighborhood.Go(false);
        }
        //===============================
        #endregion // ^Spawning^
    }
}
