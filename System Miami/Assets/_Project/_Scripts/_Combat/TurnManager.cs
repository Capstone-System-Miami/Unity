// Author: Lee St Louis, Layla Hoey
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
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
    /// Spawns enemies.
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
                if (CurrentTurnOwner == null)
                    { return false; }

                if (CurrentTurnOwner.Controller == null)
                    { return false; }

                return CurrentTurnOwner.Controller is PlayerController;
            }
        }

        public Action<Phase> NewTurnPhase;

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

        #region Unity Methods
        //===============================

        private void Start()
        {
            if (playerCharacter != null)
            {
                Vector3Int charTilePos = Coordinates.ScreenToIso(playerCharacter.transform.position, 0);
                MapManager.MGR.map.TryGetValue((Vector2Int)charTilePos, out OverlayTile charTile);
                charTile.PlaceCombatant(playerCharacter);
            }

            SpawnEnemies();

            combatants.Add(playerCharacter);
            combatants.AddRange(enemyCharacters);

            StartCoroutine(TurnSequence());
        }


        private void Update()
        {
            if (CurrentTurnOwner == null)
                { return; }

            Debug.Log($"Current Turn Owner: {CurrentTurnOwner.name}");
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
                OverlayTile spawnTile = MapManager.MGR.GetRandomUnblockedTile();

                if (spawnTile != null)
                {
                    // Instantiate enemy
                    GameObject enemyGO = Instantiate(enemyPrefab);
                    Combatant enemyCombatant = enemyGO.GetComponent<Combatant>();
                    if (enemyCombatant == null)
                    {
                        enemyCombatant = enemyGO.AddComponent<Combatant>();
                    }

                    // Set enemy ID
                    enemyCombatant.ID = i + 1;

                    // Set enemy name
                    string newName = enemyCombatant.name;
                    newName = newName.Replace("(Clone)", "");
                    newName += $"{enemyCombatant.ID}";
                    enemyCombatant.name = newName;

                    // Position enemy on the tile
                    spawnTile.PlaceCombatant(enemyCombatant);

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
        //===============================
        #endregion // ^Spawning^
    }
}
