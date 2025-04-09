using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;


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

        [Header("Credit reward ranges")] 
        private int easyMinCredit = 7;
        private int easyMaxCredit = 10;
       
        private int mediumMinCredit = 10;
        private int mediumMaxCredit = 14;
       
        private int hardMinCredit = 14;
        private int hardMaxCredit = 20;

        
        [Header("Available IDs (pulled from Database)")]
        [SerializeField] public List<ItemData> _abilityItemDatas = new();         // e.g. Physical or Magical
        [SerializeField] public List<ItemData> _consumableItemDatas = new();
        [SerializeField] public List<ItemData> _equipmentItemDatas = new();

        private List<ItemData> rewards;

        public dbug log;

        public void Initialize()
        {
            _abilityItemDatas.Clear();
            _consumableItemDatas.Clear();
            _equipmentItemDatas.Clear();
            rewards = new List<ItemData>();
            
            _abilityItemDatas = Database.MGR.GetAllItemsOfPlayerClass(ItemType.MagicalAbility);
            _abilityItemDatas.AddRange(Database.MGR.GetAllItemsOfPlayerClass(ItemType.PhysicalAbility));
            _consumableItemDatas = Database.MGR.GetAllItemsOfPlayerClass(ItemType.Consumable);
            _equipmentItemDatas = Database.MGR.GetAllItemsOfPlayerClass(ItemType.EquipmentMod);
        }
        
        /// <summary>
        /// Generates a list of ItemData rewards based on:
        /// - The dungeon difficulty
        /// - The player's level
        /// - The distribution method (which can be overridden by difficulty)
        /// </summary>
        public List<ItemData> GenerateItemRewards(DifficultyLevel dungeonDifficulty)
        {
           Debug.Log("Generate Rewards");
            int playerLevel = PlayerManager.MGR.CurrentLevel;

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
                        TryAddRandomItem( _abilityItemDatas, playerLevel, dungeonDifficulty);
                    if (_includeConsumables)
                        TryAddRandomItem( _consumableItemDatas, playerLevel, dungeonDifficulty);
                    if (_includeEquipmentMods)
                        TryAddRandomItem( _equipmentItemDatas, playerLevel, dungeonDifficulty);
                    break;

                case RewardDistributionMethod.OneOfOneType:
                {
                    // Collect which lists are enabled
                    
                    List<List<ItemData>> candidateLists = CollectEnabledIDLists();
                   Debug.Log($"Candidate Count {candidateLists.Count} {candidateLists[0].Count} {candidateLists[1].Count} {candidateLists[2].Count}");
                    if (candidateLists.Count > 0)
                    {
                        // Pick 1 random category from the enabled ones
                        int randomTypeIndex = Random.Range(0, candidateLists.Count);
                        TryAddRandomItem( candidateLists[randomTypeIndex], playerLevel, dungeonDifficulty);
                    }
                    break;
                }
                case RewardDistributionMethod.TwoTypes:
                {
                    List<List<ItemData>> candidateLists = CollectEnabledIDLists();
                  
                    if (candidateLists.Count > 1)
                    {
                        // Shuffle to pick 2 random distinct types
                        Shuffle(candidateLists);
                        // TryAddRandomItem from the first 2
                        TryAddRandomItem( candidateLists[0], playerLevel, dungeonDifficulty);
                        TryAddRandomItem(candidateLists[1], playerLevel, dungeonDifficulty);
                    }
                    break;
                }
            }
            List<int> rewardItemIDs = rewards.Select(r => r.ID).ToList();
            for (int i = 0; i < rewards.Count; i++)
            {
                List<int> playerItemIDs = PlayerManager.MGR.inventory.AllValidInventoryItems;
                
                rewards = rewards.Where(reward => !playerItemIDs.Contains(reward.ID)).ToList();
                rewardItemIDs = rewardItemIDs.Distinct().ToList();
            }
            rewards.Clear();
            foreach (int ID in rewardItemIDs)
            {
                rewards.Add(Database.MGR.GetDataWithJustID(ID));
            }
            return rewards;
        }


       
        public int GenerateExpReward(DifficultyLevel dungeonDifficulty)
        {
            int expReward = 0;
            switch (dungeonDifficulty)
            {
                case DifficultyLevel.EASY:
                    expReward = IntersectionManager.MGR.easyDungeonReward[Random.Range(0, IntersectionManager.MGR.easyDungeonReward.Count -1)];
                    break;
                case DifficultyLevel.MEDIUM:
                    expReward = IntersectionManager.MGR.mediumDungeonReward[Random.Range(0, IntersectionManager.MGR.mediumDungeonReward.Count -1)];
                    break;
                case DifficultyLevel.HARD:
                    expReward = IntersectionManager.MGR.hardDungeonReward[Random.Range(0, IntersectionManager.MGR.hardDungeonReward.Count -1)];
                    break;
            }
            
            return expReward;
        }

        public int GenerateCreditReward(DifficultyLevel dungeonDifficulty)
        {
            int creditReward = 0;
            int playerLevel = PlayerManager.MGR.CurrentLevel;
            switch (dungeonDifficulty)
            {
                case DifficultyLevel.EASY:
                    
                    creditReward = playerLevel > 0 ?  playerLevel * Random.Range(easyMinCredit, easyMaxCredit) : Random.Range(easyMinCredit, easyMaxCredit) ;
                    break;
                case DifficultyLevel.MEDIUM:
                    creditReward = playerLevel > 0 ?  playerLevel * Random.Range(mediumMinCredit, mediumMaxCredit) : Random.Range(mediumMinCredit, mediumMaxCredit) ;
                    break;
                case DifficultyLevel.HARD:
                    creditReward = playerLevel > 0 ?  playerLevel * Random.Range(hardMinCredit, hardMaxCredit) : Random.Range(hardMinCredit, hardMaxCredit) ;
                    break;
            }
            
            return creditReward;
        }

        /// <summary>
        /// Attempts to pick a random item from the candidateIDs, 
        /// filtered by difficulty-based level offsets. 
        /// Adds it to result (if any valid is found).
        /// </summary>
        private void TryAddRandomItem(
            List<ItemData> candidateData,
            int playerLevel,
            DifficultyLevel difficulty
        )
        {
            List<ItemData> validItems = FilterByDifficultyAndLevel(candidateData, playerLevel, difficulty);
            if (validItems.Count == 0)
            {
                log.warn($"No valid items found for difficulty {difficulty}!");
                return;
            }

            int randomIndex = Random.Range(0, validItems.Count);
            
            rewards.Add(validItems[randomIndex]);
        }

        /// <summary>
        /// Filters the IDs to only those whose (MinLevel <= playerLvl <= MaxLevel),
        /// factoring in difficulty offsets.
        /// </summary>
        private List<ItemData> FilterByDifficultyAndLevel(List<ItemData> itemData, int playerLevel, DifficultyLevel difficulty)
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

            List<ItemData> filtered = new();
           
            foreach (ItemData data in itemData)
            {
               
                if (data.ID == 0) 
                    continue; // Not found or invalid in DB

                // Adjust the item’s min/max by the offsets for this difficulty
                int adjustedMin = data.MinLevel + minOffset;
                int adjustedMax = data.MaxLevel + maxOffset;
                 
                // Ensure we don’t clamp below 1 or do something unintended
                if (adjustedMin < 0) adjustedMin = 0;
                
               log.warn($"Before {filtered.Count} {data.ID}");
               filtered.Add(data);
               log.warn($"After {filtered.Count} {data.ID}");
            }
            return Database.MGR.FilterByLevel(filtered);
        }

        

        /// <summary>
        /// Collects whichever ID lists are enabled (Abilities, Consumables, EquipmentMods).
        /// </summary>
        private List<List<ItemData>> CollectEnabledIDLists()
        {
            var results = new List<List<ItemData>>();
            if (_includeAbilities) results.Add(_abilityItemDatas);
            if (_includeConsumables) results.Add(_consumableItemDatas);
            if (_includeEquipmentMods) results.Add(_equipmentItemDatas);
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

        
        
    }
}
