//Author: Johnny
// Old script do not use, keeping it in the studio just in case we use it in the future
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private Vector2 currentTile; // The unit's current tile _gridPosition on the grid
    [SerializeField] private int movementRange = 3; // The maximum distance the unit can move
    [SerializeField] private int health = 100; // Hero's health
    [SerializeField] private int attackPower = 10; // Hero's attack power (now used)

    private Rigidbody2D rb2D; // Reference to Rigidbody2D for movement

    private void Awake()
    {
        // Initialize the Rigidbody2D component (if using 2D physics)
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Rigidbody2D component missing from Hero.");
        }
    }

    // Function to take damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // Function to attack another Hero
    public void Attack(Hero targetHero)
    {
        if (targetHero != null)
        {
            // Use the attackPower to deal damage to the target Hero
            Debug.Log(gameObject.name + " is attacking " + targetHero.gameObject.name);
            targetHero.TakeDamage(attackPower); // target takes damage equal to this Hero's attackPower
        }
    }

    // Function to handle Hero's death
    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        // Add logic for removing the Hero or handling its death
        Destroy(gameObject);
    }

    // Movement logic
    public void MoveToTile(Vector2 targetTile)
    {
        if (Vector2.Distance(currentTile, targetTile) <= movementRange)
        {
            // Move the Hero to the target tile
            if (rb2D != null)
            {
                rb2D.MovePosition(targetTile); // Smooth 2D movement
            }
            else
            {
                transform.position = targetTile; // Fallback if Rigidbody2D is not present
            }
            currentTile = targetTile; // Update Hero's current tile
        }
        else
        {
            Debug.LogWarning("Target tile is out of movement range!");
        }
    }
}
