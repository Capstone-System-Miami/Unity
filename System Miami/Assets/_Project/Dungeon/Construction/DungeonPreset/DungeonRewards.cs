using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.Dungeons
{
    public enum RewardDistributionMethod
    {
        OneOfEach,     // e.g., (Ability, Consumable, EquipmentMod)
        OneOfOneType,  // Just 1 item (from whichever type is toggled on)
        TwoTypes       // 2 types out of the toggled ones
    }

    [System.Serializable]
    public class DungeonRewards
    {
        [Header("Reward Distribution")]
        [Tooltip("If true, we override the chosen distribution method by difficulty.")]
        [SerializeField] private bool _overrideByDifficulty = true;

        [Tooltip("Default distribution method, used if not overridden by difficulty.")]
        [SerializeField] private RewardDistributionMethod _defaultDistributionMethod = RewardDistributionMethod.OneOfOneType;

        [Header("Distribution Method by Difficulty (optional overrides)")]
        [SerializeField] private RewardDistributionMethod _easyMethod = RewardDistributionMethod.OneOfOneType;
        [SerializeField] private RewardDistributionMethod _mediumMethod = RewardDistributionMethod.TwoTypes;
        [SerializeField] private RewardDistributionMethod _hardMethod = RewardDistributionMethod.OneOfEach;

        [Space]
        [Tooltip("tweak min/max level ranges based on difficulty.")]
        [SerializeField] private int _easyMinLevelOffset = -2;
        [SerializeField] private int _easyMaxLevelOffset = -1;

        [SerializeField] private int _mediumMinLevelOffset = 0;
        [SerializeField] private int _mediumMaxLevelOffset = 0;

        [SerializeField] private int _hardMinLevelOffset = 1;
        [SerializeField] private int _hardMaxLevelOffset = 2;

        [Header("item categories to include")]
        [SerializeField] private bool _includeAbilities = true;
        [SerializeField] private bool _includeConsumables = true;
        [SerializeField] private bool _includeEquipmentMods = true;

        [Header("Available IDs (pulled from Database)")]
        [SerializeField] private List<int> _abilityIDs = new();         // e.g. Physical or Magical
        [SerializeField] private List<int> _consumableIDs = new();
        [SerializeField] private List<int> _equipmentModIDs = new();

        /// <summary>
        /// Generates a list of ItemData rewards based on:
        /// - The dungeon difficulty
        /// - The player's level
        /// - The distribution method (which can be overridden by difficulty)
        /// </summary>
        public List<ItemData> GenerateRewards(DifficultyLevel dungeonDifficulty)
        {
           
            int playerLevel = GetPlayerLevel();

            //  Determine the final distribution method
            RewardDistributionMethod finalMethod = _defaultDistributionMethod;
            if (_overrideByDifficulty)
            {
                switch (dungeonDifficulty)
                {
                    case DifficultyLevel.EASY:
                        finalMethod = _easyMethod;
                        break;
                    case DifficultyLevel.MEDIUM:
                        finalMethod = _mediumMethod;
                        break;
                    case DifficultyLevel.HARD:
                        finalMethod = _hardMethod;
                        break;
                }
            }

            // Generate items using finalMethod + difficulty-based filters
             List<ItemData> result = new List<ItemData>();

            switch (finalMethod)
            {
                case RewardDistributionMethod.OneOfEach:
                    if (_includeAbilities)
                        TryAddRandomItem(result, _abilityIDs, playerLevel, dungeonDifficulty);
                    if (_includeConsumables)
                        TryAddRandomItem(result, _consumableIDs, playerLevel, dungeonDifficulty);
                    if (_includeEquipmentMods)
                        TryAddRandomItem(result, _equipmentModIDs, playerLevel, dungeonDifficulty);
                    break;

                case RewardDistributionMethod.OneOfOneType:
                {
                    // Collect which lists are enabled
                    var candidateLists = CollectEnabledIDLists();
                    if (candidateLists.Count > 0)
                    {
                        // Pick 1 random category from the enabled ones
                        int randomTypeIndex = Random.Range(0, candidateLists.Count);
                        TryAddRandomItem(result, candidateLists[randomTypeIndex], playerLevel, dungeonDifficulty);
                    }
                    break;
                }
                case RewardDistributionMethod.TwoTypes:
                {
                    var candidateLists = CollectEnabledIDLists();
                    if (candidateLists.Count > 1)
                    {
                        // Shuffle to pick 2 random distinct types
                        Shuffle(candidateLists);
                        // TryAddRandomItem from the first 2
                        TryAddRandomItem(result, candidateLists[0], playerLevel, dungeonDifficulty);
                        TryAddRandomItem(result, candidateLists[1], playerLevel, dungeonDifficulty);
                    }
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Attempts to pick a random item from the candidateIDs, 
        /// filtered by difficulty-based level offsets. 
        /// Adds it to result (if any valid is found).
        /// </summary>
        private void TryAddRandomItem(
            List<ItemData> result,
            List<int> candidateIDs,
            int playerLevel,
            DifficultyLevel difficulty
        )
        {
            var validItems = FilterByDifficultyAndLevel(candidateIDs, playerLevel, difficulty);
            if (validItems.Count == 0) return; 
            
            int randomIndex = Random.Range(0, validItems.Count);
            result.Add(validItems[randomIndex]);
        }

        /// <summary>
        /// Filters the IDs to only those whose (MinLevel <= playerLvl <= MaxLevel),
        /// factoring in difficulty offsets.
        /// </summary>
        private List<ItemData> FilterByDifficultyAndLevel(List<int> itemIDs, int playerLevel, DifficultyLevel difficulty)
        {
            // Decide level offsets based on difficulty
            int minOffset = 0;
            int maxOffset = 0;
            switch (difficulty)
            {
                case DifficultyLevel.EASY:
                    minOffset = _easyMinLevelOffset;
                    maxOffset = _easyMaxLevelOffset;
                    break;
                case DifficultyLevel.MEDIUM:
                    minOffset = _mediumMinLevelOffset;
                    maxOffset = _mediumMaxLevelOffset;
                    break;
                case DifficultyLevel.HARD:
                    minOffset = _hardMinLevelOffset;
                    maxOffset = _hardMaxLevelOffset;
                    break;
            }

            var filtered = new List<ItemData>();
            foreach (int id in itemIDs)
            {
                ItemData data = Database.MGR.GetDataWithJustID(id);
                if (data.ID == 0) 
                    continue; // Not found or invalid in DB

                // Adjust the item’s min/max by the offsets for this difficulty
                int adjustedMin = data.MinLevel + minOffset;
                int adjustedMax = data.MaxLevel + maxOffset;

                // Ensure we don’t clamp below 1 or do something unintended
                if (adjustedMin < 1) adjustedMin = 1;
                
                // Does the player's level fit in the adjusted range?
                if (playerLevel >= adjustedMin && playerLevel <= adjustedMax)
                {
                    filtered.Add(data);
                }
            }
            return filtered;
        }

        /// <summary>
        /// Collects whichever ID lists are enabled (Abilities, Consumables, EquipmentMods).
        /// </summary>
        private List<List<int>> CollectEnabledIDLists()
        {
            var results = new List<List<int>>();
            if (_includeAbilities) results.Add(_abilityIDs);
            if (_includeConsumables) results.Add(_consumableIDs);
            if (_includeEquipmentMods) results.Add(_equipmentModIDs);
            return results;
        }

        /// <summary>
        /// Simple list shuffle. 
        /// </summary>
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        /// <summary>
        /// Utility method to get player's level. //feel like this should be moved to a mgr
        /// 
        /// </summary>
        private int GetPlayerLevel()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return 1;
            PlayerLevel playerLevelscript = player.GetComponent<PlayerLevel>();
            if (playerLevelscript == null) return 1;
            return playerLevelscript.CurrentLevel; 
        }
    }
}
