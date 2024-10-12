//Johnny
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        // Initialize the player's health to max at the start
        currentHealth = maxHealth;
    }

    // This function is called to deal damage to the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Assuming this script is part of the enemy attack logic
    void AttackPlayer(GameObject player)
    {
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(10);  // Deal 10 damage to the player
        }
    }

    // This function can be called to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Make sure health doesn't exceed the max limit
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Player healed: " + amount);
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Optionally, implement a respawn or game over system here
        Destroy(gameObject);  // This will destroy the player GameObject
    }
}
