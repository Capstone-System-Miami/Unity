/// Layla
using System.Collections.Generic;
using System.Linq;
using SystemMiami.Dungeons;
using UnityEngine;
using UnityEngine.Assertions;

namespace SystemMiami
{
    [System.Serializable]
    public class DungeonData
    {
        [SerializeField, ReadOnly] public DifficultyLevel difficulty;

        [SerializeField, ReadOnly] public readonly GameObject Prefab;
        [SerializeField, ReadOnly] public readonly List<GameObject> Enemies = new();
        [SerializeField, ReadOnly] public readonly List<ItemData> ItemRewards = new();

        [SerializeField, ReadOnly] public readonly int EXPToGive;
        [SerializeField, ReadOnly] public readonly int Credits;

        [SerializeField, ReadOnly] private readonly string _prefabInfo;
        [SerializeField, ReadOnly] private readonly string[] _enemyInfo;
        [SerializeField, ReadOnly] private readonly string[] _abilityInfo;
        [SerializeField, ReadOnly] private readonly string[] _itemInfo;

        public DungeonData() : this(null, new(), default, new List<ItemData>(), 0, 0) { }

        public DungeonData(
            GameObject prefab,
            List<GameObject> enemies,
            DifficultyLevel difficulty,
            List<ItemData> itemRewards,
            int expToGive,
            int credits)
        {
            Assert.IsNotNull(Enemies);
            Assert.IsNotNull(ItemRewards);
            Prefab = prefab;
            Enemies = enemies;
            this.difficulty = difficulty;
            ItemRewards = itemRewards;
            EXPToGive = expToGive;
            Credits = credits;

            _prefabInfo = $"Prefab: {Prefab}";

            string hi = $"  | Enemies:\n" +
            string.Join(
                "\n    |",
                Enemies?.Where(e => e != null)
                .Select(e => e.ToString()));
        }

        public override string ToString()
        {
            string result =
                $"{GetType().Name}\n" +
                $"  | Prefab: {Prefab}\n" +
                getListInfo("Enemies", Enemies?.Cast<object>().ToList()) +
                getListInfo("Item Rewards", ItemRewards?.Cast<object>().ToList());

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
                list.Select(e => e.ToString()));

            return result + "\n";
        }
    }
}
