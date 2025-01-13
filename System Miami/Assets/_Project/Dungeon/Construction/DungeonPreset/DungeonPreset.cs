using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SystemMiami.AbilitySystem;
using SystemMiami.Dungeons;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami
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
        [Space(5), SerializeField] private Pool<Ability> _abilityRewards;
        [Space(5), SerializeField] private Pool<ItemData> _itemRewards;

        [Space(5)]
        [SerializeField] private int _minXP;
        [SerializeField] private int _maxXP;

        [SerializeField] private int _minSkillPoints;
        [SerializeField] private int _maxSkillPoints;

        public DungeonData GetData(List<Dungeons.Style> excludedStyles)
        {
            GameObject prefab = getPrefab(excludedStyles);
            List<GameObject> enemies = getEnemyPool().GetFinalizedList();
            List<Ability> abilities = getAbilityRewards().GetFinalizedList();
            List<ItemData> items = getItemRewards().GetFinalizedList();

            return new DungeonData(prefab, enemies, abilities, items);
        }

        /// <summary>
        /// Re-constructs the enemy pool to ensure that
        /// we have a fresh random List in there.
        /// </summary>
        private GameObject getPrefab(List<Dungeons.Style> excludedStyles)
        {            
            List<GameObject> golist = new();
            dungeondummy dungeon;

            // TODO this is probably unsafe.
            // Put a iteration limiter on it.
            bool success = false;
            do {
                golist = _prefabPool.GetFinalizedList();

                if (golist.Count < 1)
                {
                    // throw an error
                }

                if (!golist[0].TryGetComponent(out dungeon))
                {
                    // throw an error
                }

                foreach (Dungeons.Style style in excludedStyles)
                {
                    if (dungeon.Style == style)
                    {
                        success = false;
                        break;
                    }
                }
            } while (!success);

            if (golist.Count > 0)
            {
                return golist[0];
            }

            return null;
        }

        // ==========================
        private class dungeondummy
        {
            public Dungeons.Style Style;
        }
        // ============================

        /// <summary>
        /// Re-constructs the enemy pool to ensure that
        /// we have a fresh random List in there.
        /// </summary>
        private Pool<GameObject> getEnemyPool()
        {
            return new Pool<GameObject>(_enemyPool);
        }

        /// <summary>
        /// Re-constructs the ability reward pool to ensure that
        /// we have a fresh random list in there.
        /// </summary>
        private Pool<Ability> getAbilityRewards()
        {
            return new Pool<Ability>(_abilityRewards);
        }

        /// <summary>
        /// Re-constructs the item reward pool to ensure that
        /// we have fresh random list in there.
        /// </summary>
        private Pool<ItemData> getItemRewards()
        {
            return new Pool<ItemData>(_itemRewards);
        }
    }
}