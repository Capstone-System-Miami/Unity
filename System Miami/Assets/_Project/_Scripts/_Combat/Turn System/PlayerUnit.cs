//Author: Johnny
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public Vector2 currentTile; // The unit's current tile position on the grid
    public int movementRange = 3; // The maximum distance the unit can move

    // You can add other properties like health, attack power, etc.
    public int health = 100;
    public int attackPower = 10;

    // Function to take damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // Function to handle unit's death
    private void Die()
    {
        Debug.Log("Unit has died.");
        // Add logic for removing the unit or handling its death
        Destroy(gameObject);
    }
}