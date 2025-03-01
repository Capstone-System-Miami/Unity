/// Layla
using System.Collections.Generic;
using System.Linq;
using SystemMiami.AbilitySystem;
using UnityEngine;
using SystemMiami.LeeInventory;
using SystemMiami.Outdated;

namespace SystemMiami
{
    [System.Serializable]
    public class DungeonData
    {
        public GameObject Prefab;
        public List<GameObject> Enemies;
        public List<ItemData> ItemRewards;

        public int EXPToGive;
      //  public List<Outdated.Ability> AbilityRewards;
       // public List<LeeInventory.OutdatedOrDuplicates.ItemData> ItemRewards;

        private string _prefabInfo;
        private string[] _enemyInfo;
        private string[] _abilityInfo;
       private string[] _itemInfo;

        public DungeonData() : this( null, new(), new List<ItemData>(),0 ) { }

        public DungeonData(
            GameObject prefab,
            List<GameObject> enemies,
            List<ItemData> itemRewards,int expToGive
        )
        {
            Prefab = prefab;
            Enemies = enemies;
            ItemRewards = itemRewards;
            EXPToGive = expToGive;

            _prefabInfo = $"Prefab: {Prefab}";


            string hi = $"  | Enemies:\n" +
            string.Join("\n    |", Enemies.Select(e => e.ToString()));
            
        }

        public override string ToString()
        {
            string result =
                $"{GetType().Name}\n" +
                $"  | Prefab: {Prefab}\n" +
                getListInfo("Enemies", Enemies.Cast<object>().ToList()) +
                getListInfo("Item Rewards", ItemRewards.Cast<object>().ToList());

            return result;
        }

        private string getListInfo(string name, List<object> list) //unsure abt this so didnt touch
        {
            string result = "";

            string header = $"  | {name}:";
            string subheaderToken = "\n    | ";

            result += header;
            result += subheaderToken;
            result += string.Join(
                subheaderToken,
                list.Select(e => e.ToString())
                );

            return result + "\n";
        }
    }
}
