//Author: Johnny
// Old script do not use, keeping it in the studio just in case we use it in the future
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class TurnManager : MonoBehaviour
    {
        public List<Hero> allHeros;  // All player and enemy Heros
        private int currentHeroIndex = 0;
        private Hero playerHero;  // Reference to the player Hero with Rigidbody2D
        private void Start()
        {
            allHeros = new List<Hero>(FindObjectsOfType<Hero>()); // Automatically finds all Hero objects in the scene
            FindPlayerWithRigidbody();    // Find the player with Rigidbody2D at the start
        }



        private void Update()
        {
            HandleTurn();
        }

        // Method to find the player with Rigidbody2D
        private void FindPlayerWithRigidbody()
        {
            foreach (Hero hero in allHeros)
            {
                Rigidbody2D rb = hero.GetComponent<Rigidbody2D>();
                if (rb != null && hero.IsPlayerControlled)  // Check if the hero is player-controlled and has a Rigidbody2D
                {
                    playerHero = hero;
                    Debug.Log("Player hero found: " + playerHero.heroName);
                    return;
                }
            }

            Debug.LogError("No player with Rigidbody2D found!");
        }

        private void HandleTurn()
        {
            if (allHeros == null || allHeros.Count == 0)
            {
                Debug.LogError("allHeros list is null or empty!");
                return;
            }

            if (currentHeroIndex < 0 || currentHeroIndex >= allHeros.Count)
            {
                Debug.LogError("currentHeroIndex is out of bounds: " + currentHeroIndex);
                return;
            }

            Hero currentHero = allHeros[currentHeroIndex];

            if (currentHero == null)
            {
                Debug.LogError("Current hero is null!");
                return;
            }

            if (currentHero.IsPlayerControlled)
            {
                PlayerTurn(currentHero);
            }
            else
            {
                AITurn(currentHero);
            }

            // End turn and switch to the next hero
            if (currentHero.actionPoints <= 0)
            {
                currentHeroIndex = (currentHeroIndex + 1) % allHeros.Count;
            }
        }

        private int CalculateDistance(Vector2 start, Vector2 end)
        {
            return Mathf.Abs((int)(start.x - end.x)) + Mathf.Abs((int)(start.y - end.y));
        }

        // Expanded Player Turn Logic
        private void PlayerTurn(Hero hero)
        {
            // Display UI showing available actions like Move, Damage, End Turn
            if (hero.actionPoints > 0)
            {
                // Movement phase
                if (Input.GetMouseButtonDown(0)) // Assuming left-click for selecting a tile
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 clickedTile = WorldToGrid(mousePosition); // Convert mouse click to grid _gridPosition

                    if (IsTileValidForMovement(clickedTile, hero))  // Check if tile is within movement range
                    {
                        MoveHeroToTile(hero, clickedTile);
                        hero.actionPoints--; // Deduct action points after movement
                        return; // Exit after movement, wait for next player action
                    }
                }

                // Damage phase
                if (Input.GetMouseButtonDown(1)) // Assuming right-click for attack
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Hero targetHero = GetHeroAtPosition(mousePosition); // Get the hero under the cursor

                    if (targetHero != null && IsWithinAttackRange(hero, targetHero))
                    {
                        AttackHero(hero, targetHero);
                        hero.actionPoints--; // Deduct action points after attack
                        return; // Exit after attack, wait for next player action
                    }
                }
            }
            else
            {
                EndPlayerTurn(hero);
            }
        }

        // Movement Logic
        private bool IsTileValidForMovement(Vector2 targetTile, Hero hero)
        {
            int distance = CalculateDistance(hero.currentTile, targetTile);  // Using CalculateDistance here
            return distance <= hero.movementRange && IsTileWalkable(targetTile);
        }

        private void MoveHeroToTile(Hero hero, Vector2 targetTile)
        {
            Vector3 worldPosition = GridToWorld((int)targetTile.x, (int)targetTile.y);  // Cast to int
            StartCoroutine(MoveOverTime(hero, worldPosition)); // Coroutine for smooth movement
            hero.currentTile = targetTile;  // Update the hero's grid _gridPosition
        }

        private IEnumerator MoveOverTime(Hero hero, Vector3 targetPosition)
        {
            float moveSpeed = 5f;  // Adjust as necessary
            while (hero.transform.position != targetPosition)
            {
                hero.transform.position = Vector3.MoveTowards(hero.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Damage Logic
        private bool IsWithinAttackRange(Hero attacker, Hero target)
        {
            int distance = CalculateDistance(attacker.currentTile, target.currentTile);
            return distance <= attacker.attackRange;
        }

        private void AttackHero(Hero attacker, Hero target)
        {
            // Use attackPower in the damage calculation
            int damage = Mathf.Max(0, attacker.attackPower - target.defense); // Simple damage calculation
            target.health -= damage;

            // Optionally trigger attack animation and effects
            TriggerAttackAnimation(attacker, target);
            Debug.Log(attacker.heroName + " attacked " + target.heroName + " for " + damage + " damage.");

            if (target.health <= 0)
            {
                KillHero(target);  // Remove target if health drops to zero
            }
        }


        // End Player Turn
        private void EndPlayerTurn(Hero hero)
        {
            // Reset hero's action points for the next turn
            hero.actionPoints = hero.maxActionPoints;
            Debug.Log("Player turn ended for: " + hero.heroName);
            NextTurn(); // Switch to the next hero in the turn order
        }

        // AI Turn (Simple Example)
        private void AITurn(Hero hero)
        {
            // AI logic here
            Debug.Log("AI is taking its turn");
            // End AI turn immediately for now
            hero.actionPoints = 0;
        }

        // Helper Functions

        // Convert World MapPosition to Grid Coordinates
        private Vector2 WorldToGrid(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x); // Example of conversion
            int y = Mathf.FloorToInt(worldPosition.y);
            return new Vector2(x, y);
        }

        // Convert Grid Coordinates to World MapPosition
        private Vector3 GridToWorld(int x, int y)
        {
            float worldX = (x - y) * 0.5f;  // Cast to float
            float worldY = (x + y) * 0.25f; // Cast to float
            return new Vector3(worldX, worldY, 0);  // Return a Vector3 (float values)
        }

        // Get Hero Under Mouse
        private Hero GetHeroAtPosition(Vector3 worldPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                Hero hero = hit.collider.GetComponent<Hero>();
                return hero;
            }
            return null;
        }

        // Tile Walkability Check
        private bool IsTileWalkable(Vector2 tile)
        {
            // Check if the tile is walkable (e.g. no obstacles, not occupied by another hero)
            return true; // For now, assume all tiles are walkable
        }

        // Trigger Damage Animation (Placeholder)
        private void TriggerAttackAnimation(Hero attacker, Hero target)
        {
            // Play attack animation here
        }

        // Kill Hero Logic
        private void KillHero(Hero target)
        {
            Debug.Log(target.heroName + " has been defeated.");
            allHeros.Remove(target);
            Destroy(target.gameObject); // Remove the hero from the game
        }

        // Switch to Next Turn
        private void NextTurn()
        {
            currentHeroIndex = (currentHeroIndex + 1) % allHeros.Count;
        }
    }

    // Example Hero Class
    public class Hero : MonoBehaviour
    {
        public string heroName;
        public int health;
        public int attackPower;
        public int defense;
        public int movementRange;
        public int attackRange;
        public int actionPoints;
        public int maxActionPoints;
        public Vector2 currentTile;
        public bool IsPlayerControlled;
    }
}
