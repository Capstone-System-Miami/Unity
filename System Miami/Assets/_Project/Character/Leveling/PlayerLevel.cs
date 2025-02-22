// Author: Andrew, Daylan

// Modified to add the EXP + Gold quest reward (Starts on line 37) - Johnny Sosa

using UnityEngine;

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {

        [SerializeField] private int baseXPPerLevel = 100;
        [SerializeField] private int additionalXPPerLevel = 50;

        private int level = 0;
        private int currentXP = 0;
        private int xpToNextTotal => GetXPtoNextLevel(level);
        private int xpToNextRemaining = 0;

        public int CurrentLevel { get { return level; } }
        public int CurrentXP { get { return currentXP; } }
        public int XPtoNextRemaining { get { return  xpToNextRemaining; } }

        public event System.Action<int> LevelUp;

        private void Start()
        {
            RecalculateXPtoNextRemaining();
        }

        // Gain XP from any source (quests, combat, etc.)
        public void GainXP(int amount)
        {
            int remainderXP = 0;

            currentXP += amount;

            xpToNextRemaining -= amount;
            
            remainderXP = currentXP - xpToNextTotal;

            if (remainderXP > 0)
            {
                OnLevelUp();
                GainXP(remainderXP);
            }
            else if (remainderXP == 0)
            {
                OnLevelUp();
            }
        }

        /// <summary>
        /// Get the XP required for the player to gain a level.
        /// </summary>
        /// <param name="newTargetLevel"></param>
        /// <returns></returns>
        public int GetXPtoNextLevel(int currentLevel)
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
                runningTotal += GetXPtoNextLevel(i);
            }

            return runningTotal;
        }

        protected virtual void OnLevelUp()
        {
            level++;
            currentXP = 0;
            RecalculateXPtoNextRemaining();
            Debug.Log($"Leveled up! New level: {level}");

            LevelUp?.Invoke(level);
        }

        private void RecalculateXPtoNextRemaining()
        {
            xpToNextRemaining = xpToNextTotal;
        }
    }
}
