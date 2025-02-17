using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {
        public int level = 1;
        public int currentXP = 0;
        public int xpToNextLevel = 100;
        public int availableStatPoints = 0; // Points to allocate

        public GameObject statUI; // Reference to the stat UI panel

        // UI Text elements
        public Text levelText, xpText, pointsText;
        public Text strengthText, dexterityText, constitutionText, wisdomText, intelligenceText;

        private Attributes playerAttributes; // Reference to the Attributes script

        void Start()
        {
            playerAttributes = GetComponent<Attributes>();  // Get Attributes component
            if (playerAttributes != null)
            {
                UpdateUI();
            }
            else
            {
                Debug.LogError("PlayerAttributes reference is missing!");
            }
        }

        void Update()
        {
            // Gain XP when pressing numpad keys
            if (Input.GetKeyDown(KeyCode.Keypad0)) GainXP(1);
            if (Input.GetKeyDown(KeyCode.Keypad1)) GainXP(5);
            if (Input.GetKeyDown(KeyCode.Keypad2)) GainXP(10);
            if (Input.GetKeyDown(KeyCode.Keypad3)) GainXP(20);
            if (Input.GetKeyDown(KeyCode.Keypad4)) GainXP(30);
            if (Input.GetKeyDown(KeyCode.Keypad5)) GainXP(50);
        }

        public void GainXP(int amount)
        {
            currentXP += amount;
            Debug.Log("Gained XP: " + amount);

            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
            UpdateUI();
        }

        void LevelUp()
        {
            currentXP -= xpToNextLevel;
            level++;
            availableStatPoints += 3; // Gain 3 stat points per level
            Debug.Log("Leveled up! New level: " + level);

            statUI.SetActive(true); // Show stat allocation UI
            UpdateUI();
        }

        public void AssignStatPoint(string statName)
        {
            Debug.Log("AssignStatPoint called for: " + statName);

            if (availableStatPoints > 0)
            {
                AttributeType type;
                if (System.Enum.TryParse(statName, true, out type))
                {
                    // Check if the stat is already at the maximum value
                    int currentStatValue = playerAttributes.GetAttribute(type);
                    if (currentStatValue >= playerAttributes.GetMaxValue()) // Ensure `GetMaxValue()` is implemented
                    {
                        Debug.LogWarning($"Cannot add points to {statName}. It has reached the maximum value.");
                        return;
                    }

                    // Add the point to the player's attribute
                    playerAttributes.AddToUpgrades(type, 1);

                    availableStatPoints--; // Decrease available points

                    //Instantly upgrades stats
                    playerAttributes.ConfirmUpgrades();

                    // Update the UI to show the new stats
                    UpdateUI();

                    Debug.Log($"Added 1 point to {statName}. Available points: {availableStatPoints}");
                }
                else
                {
                    Debug.LogWarning("Invalid attribute type: " + statName);
                }
            }
        }


        public void ConfirmStatAssignment()
        {
            playerAttributes.ConfirmUpgrades(); // Apply upgrades
            statUI.SetActive(false); // Hide the stat UI
            UpdateUI(); // Update the UI to reflect the changes
        }

        void UpdateUI()
        {
            Debug.Log("Updating UI...");

            if (playerAttributes != null)
            {
                levelText.text = "Level: " + level;
                xpText.text = "XP: " + currentXP + "/" + xpToNextLevel;
                pointsText.text = "Available Points: " + availableStatPoints;

                strengthText.text = "Strength: " + playerAttributes.GetAttribute(AttributeType.STRENGTH);
                dexterityText.text = "Dexterity: " + playerAttributes.GetAttribute(AttributeType.DEXTERITY);
                constitutionText.text = "Constitution: " + playerAttributes.GetAttribute(AttributeType.CONSTITUTION);
                wisdomText.text = "Wisdom: " + playerAttributes.GetAttribute(AttributeType.WISDOM);
                intelligenceText.text = "Intelligence: " + playerAttributes.GetAttribute(AttributeType.INTELLIGENCE);

                /*
                Debug.Log("UI Updated:");
                Debug.Log($"Level: {level}, XP: {currentXP}/{xpToNextLevel}, Points: {availableStatPoints}");
                Debug.Log($"Strength: {playerAttributes.GetAttribute(AttributeType.STRENGTH)}");
                Debug.Log($"Dexterity: {playerAttributes.GetAttribute(AttributeType.DEXTERITY)}");
                Debug.Log($"Constitution: {playerAttributes.GetAttribute(AttributeType.CONSTITUTION)}");
                Debug.Log($"Wisdom: {playerAttributes.GetAttribute(AttributeType.WISDOM)}");
                Debug.Log($"Intelligence: {playerAttributes.GetAttribute(AttributeType.INTELLIGENCE)}");
                */

            }
            else
            {
                Debug.LogWarning("PlayerAttributes reference is missing!");
            }
        }    
    }   
}
