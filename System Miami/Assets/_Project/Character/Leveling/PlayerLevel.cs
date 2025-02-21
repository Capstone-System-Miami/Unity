// Author: Andrew, Daylan

// Modified to add the EXP + Gold quest reward (Starts on line 37) - Johnny Sosa

using UnityEngine;

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {
        public int level = 1;
        public int currentXP = 0;
        public int xpToNextLevel = 100;
        public int credits = 0;

        public int str = 3; // Strength
        public int con = 3; // Constitution
        public int dex = 3; // Dexterity
        public int inte = 3; // Intelligence
        public int wis = 3; // Wisdom

        private int statSelector;

        void Update()
        {
            // Testing XP and Gold gain manually
            if (Input.GetKeyDown(KeyCode.Keypad0)) { GainXP(1); }
            if (Input.GetKeyDown(KeyCode.Keypad1)) { GainXP(5); }
            if (Input.GetKeyDown(KeyCode.Keypad2)) { GainXP(10); }
            if (Input.GetKeyDown(KeyCode.Keypad3)) { GainXP(20); }
            if (Input.GetKeyDown(KeyCode.Keypad4)) { GainXP(30); }
            if (Input.GetKeyDown(KeyCode.Keypad5)) { GainXP(50); }
            if (Input.GetKeyDown(KeyCode.G)) { AddCredits(100); } //  Test adding gold
        }

        // Gain XP from any source (quests, combat, etc.)
        public void GainXP(int amount)
        {
            currentXP += amount;
            Debug.Log($"Gained {amount} XP! Current XP: {currentXP}/{xpToNextLevel}");

            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }

        // Gain Gold from quests and other sources
        public void AddCredits(int amount)
        {
            credits += amount;
            Debug.Log($"Gained {amount} Gold! Current Gold: {credits}");
        }

        void LevelUp()
        {
            currentXP -= xpToNextLevel;
            level++;
            xpToNextLevel += 50; // Increase XP requirement for next level
            Debug.Log($"Leveled up! New level: {level}");

            for (int i = 0; i < 2; i++) // Random stat boost on level-up
            {
                statSelector = Random.Range(1, 6);
                switch (statSelector)
                {
                    case 1: str++; Debug.Log($"Strength increased: {str}"); break;
                    case 2: con++; Debug.Log($"Constitution increased: {con}"); break;
                    case 3: dex++; Debug.Log($"Dexterity increased: {dex}"); break;
                    case 4: inte++; Debug.Log($"Intelligence increased: {inte}"); break;
                    case 5: wis++; Debug.Log($"Wisdom increased: {wis}"); break;
                }
            }
        }
    }
}
