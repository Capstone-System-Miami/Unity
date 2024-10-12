// Johnny
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;      // Enemy's maximum health
    public int currentHealth;        // Enemy's current health
    public int damage = 10;          // Damage the enemy deals to the player

    void Start()
    {
        // Initialize the enemy's health to max at the start
        currentHealth = maxHealth;
    }

    // This function is called when the player clicks on the enemy
    void OnMouseDown()
    {
        Debug.Log("Enemy clicked!");
        TakeDamage(10);  // Deal 10 damage to the enemy when clicked
    }

    // Method for the enemy to take damage
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Enemy took damage: " + damageAmount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AttackPlayer(GameObject player)
    {
        Debug.Log("Enemy is trying to attack the player!");  // Add this line
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            Debug.Log("Enemy attacks the player!");  // Add this line
            playerScript.TakeDamage(damage);  // Deal damage to the player
        }
        else
        {
            Debug.Log("Player script not found!");  // Add this line to check if playerScript is null
        }
    }


    // Method to handle the enemy's death
    void Die()
    {
        Debug.Log("Enemy has died!");
        Destroy(gameObject);  // Destroy the enemy GameObject
    }
}
