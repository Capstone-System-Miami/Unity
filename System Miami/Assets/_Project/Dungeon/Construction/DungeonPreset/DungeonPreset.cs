/// Layla
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEngine;
using SystemMiami.LeeInventory;
#if UNITY_EDITOR
using UnityEditor; 
#endif
namespace SystemMiami.Dungeons
{
    public enum DifficultyLevel { EASY, MEDIUM, HARD }

    [CreateAssetMenu(fileName = "New Dungeon Preset", menuName = "Eviron/Dungeon Preset")]
    public class DungeonPreset : ScriptableObject
    {
        [SerializeField] private DifficultyLevel _difficulty;
        
        [Header("Color Settings")]
        [SerializeField] private Material _material;

        [Tooltip("OFF position. The color of the emissive texture when the player is NOT near enough to interact with the entrance.")]
        [ColorUsage(true, true)]
        [SerializeField] private Color _doorOffColor = Color.black;

        [Tooltip("ON position. The color of the emissive texture when the player IS near enough to interact with the entrance.")]
        [ColorUsage(true, true)]
        [SerializeField] private Color _doorOnColor = Color.black;


        public DifficultyLevel Difficulty { get { return _difficulty; } }
        public Material EmmissiveMaterial { get { return _material; } }
        public Color DoorOffColor { get { return _doorOffColor; } }
        public Color DoorOnColor { get { return _doorOnColor; } }

        [Header("Environment Prefabs")]
        [SerializeField] private Pool<GameObject> _prefabPool;
        
        [Header("Settings")]
        [Space(5), SerializeField] private Pool<GameObject> _enemyPool;

        [Header("Rewards")]
        [SerializeField] private DungeonRewards _rewards;

        [Space(5)]
        [SerializeField] private int _minXP;
        [SerializeField] private int _maxXP;

        [SerializeField] private int _minSkillPoints;
        [SerializeField] private int _maxSkillPoints;

        public DungeonData GetData(List<Style> excludedStyles)
        {
            GameObject prefab = getPrefab(excludedStyles);

            List<GameObject> enemies = _enemyPool.GetNewList();
            
            _rewards.Initialize();
            
            List<ItemData> itemRewards = _rewards.GenerateRewards(_difficulty);
            Debug.LogWarning($"Dungeon Preset {name} generated rewards: {itemRewards.Count}");
            
            int EXPToGive = Random.Range(_minXP, _maxXP);
            
            DungeonData data = new DungeonData(prefab, enemies, itemRewards,EXPToGive);

            return data;
        }

        /// <summary>
        /// Gets a list from the prefab pool, and pulls the first one 
        /// </summary>
        private GameObject getPrefab(List<Dungeons.Style> excludedStyles)
        {
            List<GameObject> golist = new();
            Dungeon dungeon;

            bool success = false;
            int iterationsRemaining = 100;
            do {
                iterationsRemaining--;

                golist = _prefabPool.GetNewList();

                if (golist == null)
                {
                    Debug.LogError(
                        $"{this} {name} {_prefabPool} " +
                        $"returned null instead of a list"
                        );
                    return null;
                }

                if (golist.Count == 0)
                {
                    Debug.LogError(
                        $"{this} {name} {_prefabPool} " +
                        $"returned an empty list."
                        );
                    return null;
                }

                if (golist[0] == null)
                {
                    Debug.LogError(
                        $"{this} {name} {_prefabPool} " +
                        $"returned a list with a null " +
                        $"GameObject at [0]."
                        );
                    return null;
                }

                if (!golist[0].TryGetComponent(out dungeon))
                {
                    Debug.LogError(
                        $"{this} {name} {_prefabPool} " +
                        $"returned {golist[0]}, " +
                        $"which is missing a Dungeon script."
                        );
                    return null;
                }

                bool isExcludedStyle = false;
                foreach (Style style in excludedStyles)
                {
                    if (dungeon.Style == style)
                    {
                        isExcludedStyle = true;
                        break;
                    }
                }

                success = !isExcludedStyle;

            } while (!success && iterationsRemaining > 0);

            if (iterationsRemaining == 0)
            {
                string warning =
                    $"Dungeon Preset ({this})'s search " +
                    $"for a prefab timed out!! Default prefab ";

                if (_prefabPool.DefaultPrefab == null)
                {
                    warning += "WAS ALSO NULL.";
                }
                else
                {
                    warning += "is being returned.";
                }

                Debug.LogWarning( warning );
                return _prefabPool.DefaultPrefab;
            }

            return golist[0];
        }
        
        public void AdjustEXPRewards(int requiredLevel, int totalDungeons, float easySpawnChance, float mediumSpawnChance, float hardSpawnChance)
        {
            // Get total XP required for the level
            int xpRequired = PlayerManager.MGR.GetComponent<PlayerLevel>().GetTotalXPRequired(requiredLevel);

            // Average number of each dungeon type that will spawn
            int expectedEasy = Mathf.RoundToInt(totalDungeons * easySpawnChance);
            int expectedMedium = Mathf.RoundToInt(totalDungeons * mediumSpawnChance);
            int expectedHard = Mathf.RoundToInt(totalDungeons * hardSpawnChance);

            // Ensure at least 1 of each type to prevent divide by zero
            expectedEasy = Mathf.Max(1, expectedEasy);
            expectedMedium = Mathf.Max(1, expectedMedium);
            expectedHard = Mathf.Max(1, expectedHard);

            // Calculate XP allocation per dungeon type
            float avgEasyEXP = xpRequired / (float)expectedEasy;
            float avgMediumEXP = xpRequired / (float)expectedMedium;
            float avgHardEXP = xpRequired / (float)expectedHard;

            // Set min/max EXP based on average values with some randomness
            if (_difficulty == DifficultyLevel.EASY)
            {
                _minXP = Mathf.Max(5, Mathf.FloorToInt(avgEasyEXP * 0.8f)); // 80% of average
                _maxXP = Mathf.CeilToInt(avgEasyEXP * 1.2f); // 120% of average
            }
            else if (_difficulty == DifficultyLevel.MEDIUM)
            {
                _minXP = Mathf.Max(10, Mathf.FloorToInt(avgMediumEXP * 0.8f));
                _maxXP = Mathf.CeilToInt(avgMediumEXP * 1.2f);
            }
            else if (_difficulty == DifficultyLevel.HARD)
            {
                _minXP = Mathf.Max(20, Mathf.FloorToInt(avgHardEXP * 0.8f));
                _maxXP = Mathf.CeilToInt(avgHardEXP * 1.2f);
            }

            Debug.Log($"Adjusted EXP for {_difficulty} Dungeons: MinXP = {_minXP}, MaxXP = {_maxXP}");
        }
       
    }
}