/// Layla
using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.Dungeons
{
    public enum DifficultyLevel { EASY, MEDIUM, HARD, BOSS }

    [CreateAssetMenu(fileName = "New Dungeon Preset", menuName = "Eviron/Dungeon Preset")]
    public class DungeonPreset : ScriptableObject
    {
        [Header("Debug")]
        [SerializeField] private dbug log;
        [Space(10)]

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

        public DungeonData GetData(List<Style> excludedStyles)
        {
            GameObject prefab = getPrefab(excludedStyles);

            List<GameObject> enemies = _enemyPool.GetNewList();

            _rewards.Initialize();

            List<ItemData> itemRewards = _rewards.GenerateItemRewards(_difficulty);
            log.warn($"Dungeon Preset [\"{name}\"] generated rewards: {itemRewards.Count}");

            int EXPToGive = _rewards.GenerateExpReward(_difficulty);
            int creditsToGive = _rewards.GenerateCreditReward(_difficulty);

            DungeonData data = new DungeonData(prefab, enemies, Difficulty, itemRewards, EXPToGive, creditsToGive);

            return data;
        }

        /// <summary>
        /// Gets a list from the prefab pool, and pulls the first one 
        /// </summary>
        private GameObject getPrefab(List<Dungeons.Style> excludedStyles)
        {
            List<GameObject> golist = new();
            Dungeon dungeon = null;

            bool success = false;
            int iterationsRemaining = 100;
            do {
                golist = _prefabPool.GetNewList();

                if (golist == null)
                {
                    log.error(
                        $"{this} {name} {_prefabPool} " +
                        $"returned null instead of a list",
                        this);
                    continue;
                }

                if (golist.Count == 0)
                {
                    log.error(
                        $"{this} {name} {_prefabPool} " +
                        $"returned an empty list.",
                        this);
                    continue;
                }

                if (golist[0] == null)
                {
                    log.error(
                        $"{this} {name} {_prefabPool} " +
                        $"returned a list with a null " +
                        $"GameObject at [0].",
                        this);
                    continue;
                }

                if (!golist[0].TryGetComponent(out dungeon))
                {
                    log.error(
                        $"{this} {name} {_prefabPool} " +
                        $"returned {golist[0]}, " +
                        $"which is missing a Dungeon script.",
                        this);
                    continue;
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

            } while (!success && --iterationsRemaining > 0);

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

                log.warn( warning );
                return _prefabPool.DefaultPrefab;
            }
            dungeon.DifficultyLevel = _difficulty;
            return golist[0];
        }
    }
}
