// Author: Andrew, Daylan

// Modified to add the EXP + Gold quest reward (Starts on line 37) - Johnny Sosa

using UnityEngine;

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {
        public int level = 1;
        public int currentXP = 0;
        public int cumulativeXP = 0;
        public int xpToNext = 0;

        [SerializeField] private int baseXPPerLevel = 100;
        [SerializeField] private int additionalXPPerLevel = 50;

        private int statSelector;

        public event System.Action<int> LevelUp;

        void Update()
        {
            // Testing XP and Gold gain manually
            if (Input.GetKeyDown(KeyCode.Keypad0)) { GainXP(1); }
            if (Input.GetKeyDown(KeyCode.Keypad1)) { GainXP(5); }
            if (Input.GetKeyDown(KeyCode.Keypad2)) { GainXP(10); }
            if (Input.GetKeyDown(KeyCode.Keypad3)) { GainXP(20); }
            if (Input.GetKeyDown(KeyCode.Keypad4)) { GainXP(30); }
            if (Input.GetKeyDown(KeyCode.Keypad5)) { GainXP(50); }
        }

        // Gain XP from any source (quests, combat, etc.)
        public void GainXP(int amount)
        {
            int remainderXP = 0;

            currentXP += amount;
            
            remainderXP = currentXP - xpToNext;

            if (remainderXP >= 0)
            {
                OnLevelUp();
            }

            GainXP(remainderXP);
        }

        /// <summary>
        /// Get the XP required for the player to gain a level.
        /// </summary>
        /// <param name="newTargetLevel"></param>
        /// <returns></returns>
        public int GetXPToNextLevel(int currentLevel)
        {
            return baseXPPerLevel + (additionalXPPerLevel * currentLevel);
        }

        /// <summary>
        /// Get the total XP the player will need in order to cross the Level
        /// threshold required to get to the specified level
        /// </summary>
        /// <param name="levelThreshold"></param>
        /// <returns></returns>
        public int GetXPtoThreshold(int levelThreshold)
        {
            int runningTotal = 0;

            for (int i = 0; i < levelThreshold; i++)
            {
                runningTotal += GetXPToNextLevel(i);
            }

            return runningTotal;
        }

        protected virtual void OnLevelUp()
        {
            level++;
            currentXP = 0;
            xpToNext = GetXPtoThreshold(level + 1);

            Debug.Log($"Leveled up! New level: {level}");

            LevelUp?.Invoke(level);
        }
    }
}
