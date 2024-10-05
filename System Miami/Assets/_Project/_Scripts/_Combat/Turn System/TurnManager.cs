//Author: Johnny
using System.Collections;
using System.Collections.Generic;  // Required for List<>
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<Unit> allUnits;  // All player and enemy units
    private int currentUnitIndex = 0;

    private void Update()
    {
        HandleTurn();
    }

    private void HandleTurn()
    {
        if (allUnits == null || allUnits.Count == 0)
        {
            Debug.LogError("allUnits list is null or empty!");
            return;
        }

        if (currentUnitIndex < 0 || currentUnitIndex >= allUnits.Count)
        {
            Debug.LogError("currentUnitIndex is out of bounds: " + currentUnitIndex);
            return;
        }

        Unit currentUnit = allUnits[currentUnitIndex];

        if (currentUnit == null)
        {
            Debug.LogError("Current unit is null!");
            return;
        }

        if (currentUnit.IsPlayerControlled)
        {
            PlayerTurn(currentUnit);
        }
        else
        {
            AITurn(currentUnit);
        }

        // End turn and switch to the next unit
        if (currentUnit.actionPoints <= 0)
        {
            currentUnitIndex = (currentUnitIndex + 1) % allUnits.Count;
        }
    }


    private int CalculateDistance(Vector2 start, Vector2 end)
    {
        return Mathf.Abs((int)(start.x - end.x)) + Mathf.Abs((int)(start.y - end.y));
    }

    // Expanded Player Turn Logic
    private void PlayerTurn(Unit unit)
    {
        // Display UI showing available actions like Move, Attack, End Turn
        if (unit.actionPoints > 0)
        {
            // Movement phase
            if (Input.GetMouseButtonDown(0)) // Assuming left-click for selecting a tile
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 clickedTile = WorldToGrid(mousePosition); // Convert mouse click to grid position

                if (IsTileValidForMovement(clickedTile, unit))  // Check if tile is within movement range
                {
                    MoveUnitToTile(unit, clickedTile);
                    unit.actionPoints--; // Deduct action points after movement
                    return; // Exit after movement, wait for next player action
                }
            }

            // Attack phase
            if (Input.GetMouseButtonDown(1)) // Assuming right-click for attack
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Unit targetUnit = GetUnitAtPosition(mousePosition); // Get the unit under the cursor

                if (targetUnit != null && IsWithinAttackRange(unit, targetUnit))
                {
                    AttackUnit(unit, targetUnit);
                    unit.actionPoints--; // Deduct action points after attack
                    return; // Exit after attack, wait for next player action
                }
            }
        }
        else
        {
            EndPlayerTurn(unit);
        }
    }

    // Movement Logic
    private bool IsTileValidForMovement(Vector2 targetTile, Unit unit)
    {
        int distance = CalculateDistance(unit.currentTile, targetTile);  // Using CalculateDistance here
        return distance <= unit.movementRange && IsTileWalkable(targetTile);
    }


    private void MoveUnitToTile(Unit unit, Vector2 targetTile)
    {
        Vector3 worldPosition = GridToWorld((int)targetTile.x, (int)targetTile.y);  // Cast to int
        StartCoroutine(MoveOverTime(unit, worldPosition)); // Coroutine for smooth movement
        unit.currentTile = targetTile;  // Update the unit's grid position
    }


    private IEnumerator MoveOverTime(Unit unit, Vector3 targetPosition)
    {
        float moveSpeed = 5f;  // Adjust as necessary
        while (unit.transform.position != targetPosition)
        {
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Attack Logic
    private bool IsWithinAttackRange(Unit attacker, Unit target)
    {
        int distance = CalculateDistance(attacker.currentTile, target.currentTile);
        return distance <= attacker.attackRange;
    }

    private void AttackUnit(Unit attacker, Unit target)
    {
        int damage = Mathf.Max(0, attacker.attackPower - target.defense); // Simple damage calculation
        target.health -= damage;

        // Optionally trigger attack animation and effects
        TriggerAttackAnimation(attacker, target);
        Debug.Log(attacker.name + " attacked " + target.name + " for " + damage + " damage.");

        if (target.health <= 0)
        {
            KillUnit(target);  // Remove target if health drops to zero
        }
    }

    // End Player Turn
    private void EndPlayerTurn(Unit unit)
    {
        // Reset unit's action points for the next turn
        unit.actionPoints = unit.maxActionPoints;
        Debug.Log("Player turn ended for: " + unit.name);
        NextTurn(); // Switch to the next unit in the turn order
    }

    // AI Turn (Simple Example)
    private void AITurn(Unit unit)
    {
        // AI logic here
        Debug.Log("AI is taking its turn");
        // End AI turn immediately for now
        unit.actionPoints = 0;
    }

    // Helper Functions

    // Distance Calculation
    private Vector3 GridToWorld(int x, int y)
    {
        float worldX = (x - y) * 0.5f;  // Cast to float
        float worldY = (x + y) * 0.25f; // Cast to float
        return new Vector3(worldX, worldY, 0);  // Return a Vector3 (float values)
    }


    // Convert World Position to Grid Coordinates
    private Vector2 WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x); // Example of conversion
        int y = Mathf.FloorToInt(worldPosition.y);
        return new Vector2(x, y);
    }

    // Convert Grid Coordinates to World Position
    private Vector3 Grid(int x, int y)
    {
        float worldX = (x - y) * 0.5f;
        float worldY = (x + y) * 0.25f; // Adjust based on isometric angle
        return new Vector3(worldX, worldY, 0);
    }

    // Get Unit Under Mouse
    private Unit GetUnitAtPosition(Vector3 worldPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            return unit;
        }
        return null;
    }

    // Tile Walkability Check
    private bool IsTileWalkable(Vector2 tile)
    {
        // Check if the tile is walkable (e.g. no obstacles, not occupied by another unit)
        return true; // For now, assume all tiles are walkable
    }

    // Trigger Attack Animation (Placeholder)
    private void TriggerAttackAnimation(Unit attacker, Unit target)
    {
        // Play attack animation here
    }

    // Kill Unit Logic
    private void KillUnit(Unit target)
    {
        Debug.Log(target.name + " has been defeated.");
        allUnits.Remove(target);
        Destroy(target.gameObject); // Remove the unit from the game
    }

    // Switch to Next Turn
    private void NextTurn()
    {
        currentUnitIndex = (currentUnitIndex + 1) % allUnits.Count;
    }
}

// Example Unit Class
public class Unit : MonoBehaviour
{
    public string unitName;
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

// This code is not done and will most likely be deleted later since 80% of it doesnt follow the scriptable objects 
//   <(￣︶￣)>