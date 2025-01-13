using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class DungeonData
    {
        public GameObject Prefab;
        public List<GameObject> Enemies;
        public List<Ability> AbilityRewardPool;
        public List<ItemData> ItemRewardPool;

        public DungeonData() : this( null, new(), new(), new() ) { }

        public DungeonData(GameObject prefab, List<GameObject> enemies, List<Ability> abilities, List<ItemData> items)
        {
            Prefab = prefab;
            Enemies = enemies;
            AbilityRewardPool = abilities;
            ItemRewardPool = items;
        }
    }
}
