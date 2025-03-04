using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro namespace

public class EnemyCounter : MonoBehaviour
{
    public TMP_Text enemyCounterText; // Reference to the TMP Text component
    public List<GameObject> enemyPrefabs = new List<GameObject>(); // List to hold all enemy prefabs
    private int enemyCount = 0; // Counter for the enemies spawned

    // This method can be called when an enemy is spawned
    public void SpawnEnemy(int enemyIndex)
    {
        if (enemyIndex >= 0 && enemyIndex < enemyPrefabs.Count)
        {
            // Spawn the enemy from the selected prefab
            Instantiate(enemyPrefabs[enemyIndex], transform.position, Quaternion.identity);
            IncrementCounter();
        }
        else
        {
            Debug.LogError("Invalid enemy index.");
        }
    }

    // This method increments the enemy count and updates the TMP Text
    private void IncrementCounter()
    {
        enemyCount++;
        enemyCounterText.text = "Mob Count: " + enemyCount.ToString();
    }

    // Optional: You can use this method to reset the counter if needed
    public void ResetCounter()
    {
        enemyCount = 0;
        enemyCounterText.text = "Mob Count: " + enemyCount.ToString();
    }
}
