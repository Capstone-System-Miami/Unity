using System.Collections.Generic;
using SystemMiami.AbilitySystem;
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
        [SerializeField] private List<GameObject> easyDungeonPrefabs;
        [SerializeField] private List<GameObject> mediumDungeonPrefabs;
        [SerializeField] private List<GameObject> hardDungeonPrefabs;
        
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

        public Pool<GameObject> GetEnemyPool()
        {
            return new Pool<GameObject>(_enemyPool);
        }

        public Pool<Ability> GetAbilityRewards()
        {
            return new Pool<Ability>(_abilityRewards);
        }

        public Pool<ItemData> GetItemRewards()
        {
            return new Pool<ItemData>(_itemRewards);
        }
    }
}