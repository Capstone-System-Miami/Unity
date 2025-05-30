// Author: Andrew, Daylan

// Modified to add the EXP + Gold quest reward (Starts on line 37) - Johnny Sosa

using System;
using SystemMiami.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SystemMiami
{
    public class PlayerLevel : MonoBehaviour
    {
        [SerializeField] private int baseXPPerLevel = 100;
        [SerializeField] private int additionalXPPerLevel = 50;

        [SerializeField, ReadOnly] private int level = 0;
        [SerializeField, ReadOnly] private int currentXP = 0;

        [SerializeField] public GameObject levelUpText;
        
        [Header("Debugging")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private KeyCode debug_GainExpKey;
        [SerializeField] private int debug_amountToGain = 50;

        private int xpToNextRemaining = 0;
        
        public int CurrentLevel => level;
        public int CurrentXP { get { return currentXP; } }
        public int XPtoNextRemaining { get { return  xpToNextRemaining; } }
        public int XpToNextTotal => GetXPtoNextLevel(level);

        public event System.Action<int> LevelUp;

        private void Start()
        {
            RecalculateXPtoNextRemaining();
           
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == GAME.MGR.NeighborhoodSceneName)
            {
                if (levelUpText == null)
                {
                    levelUpText = GameObject.Find("LevelUpNotif");
                    if (levelUpText != null)
                    {
                        levelUpText.SetActive(false);
                    }
                }
            }
        }

        private void Update()
        {
            if (debugMode && Input.GetKeyDown(debug_GainExpKey))
            {
                DEBUG_GainEXP();
            }
        }

        // Gain XP from any source (quests, combat, etc.)
        public void GainXP(int amount)
        {
            int remainderXP = 0;

            currentXP += amount;

            xpToNextRemaining -= amount;
            
            remainderXP = currentXP - XpToNextTotal;

            if (remainderXP > 0)
            {
                if (levelUpText != null && !levelUpText.activeSelf)
                {
                    levelUpText.SetActive(true);
                }
                OnLevelUp();
                GainXP(remainderXP);
                
            }
            else if (remainderXP == 0)
            {
                if (levelUpText != null && !levelUpText.activeSelf)
                {
                    levelUpText.SetActive(true);
                }
                OnLevelUp();
                
            }
        }

        public void TurnOffLevelText()
        {
            if (levelUpText == null) { return; }

            levelUpText.SetActive(false);
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
            xpToNextRemaining = XpToNextTotal;
        }
        
        public int GetTotalXPRequired(int targetLevel)
        {
            int totalXP = 0;
            for (int i = 0; i < targetLevel; i++)
            {
                totalXP += GetXPtoNextLevel(i);
            }
            return totalXP;
        }

        private void DEBUG_GainEXP()
        {
            GainXP(debug_amountToGain);
        }
    }
   
    
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}
