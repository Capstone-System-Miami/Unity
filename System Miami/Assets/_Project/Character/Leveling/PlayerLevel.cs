using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {
        public int level = 1;
        public int currentXP = 0;
        public int xpToNextLevel = 100;

        public int str = 3; // strength
        public int con = 3; // constitution
        public int dex = 3; // dexterity
        public int inte = 3; // intelligence
        public int wis = 3; // wisdom

        private int statSelector;

        void Update()
        {
            // Gain XP when the player presses the one of the kaypads
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                GainXP(1); // Gain 1 XP
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                GainXP(5); // Gain 5 XP
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                GainXP(10); // Gain 10 XP
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                GainXP(20); // Gain 20 XP
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                GainXP(30); // Gain 30 XP
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                GainXP(50); // Gain 50 XP
            }
        }

        public void GainXP(int amount)
        {
            currentXP += amount;
            Debug.Log("Gained XP: " + amount);

            // Check if the player has enough XP to level up
            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }

        void LevelUp()
        {
            currentXP -= xpToNextLevel;
            level++;
            Debug.Log("Leveled up! New level: " + level);

            for (int i = 0; i < 2; i++)
            {
                statSelector = Random.Range(1, 6);
                switch (statSelector)
                {
                    case 1:
                        str++;
                        print("strength level: " + str);
                        break;
                    case 2:
                        con++;
                        print("Constitution level: " + con);
                        break;
                    case 3:
                        dex++;
                        print("Dexterity level: " + dex);
                        break;
                    case 4:
                        inte++;
                        print("Intelligence level: " + inte);
                        break;
                    case 5:
                        wis++;
                        print("Wisdom level: " + wis);
                        break;
                }
            }
        }
    }
}
