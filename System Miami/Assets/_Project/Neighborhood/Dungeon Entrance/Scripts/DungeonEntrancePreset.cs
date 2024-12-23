using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami
{
    public enum DifficultyLevel { EASY, MEDIUM, HARD }

    [CreateAssetMenu(fileName = "New Dungeon Entrance Preset", menuName = "Eviron/Dungeon Entrance Preset")]
    public class DungeonEntrancePreset : ScriptableObject
    {
        [SerializeField] private DifficultyLevel _difficulty;
        
        //Color for the door states
        [SerializeField] private Color _doorOffColor = Color.black;
        
        [ColorUsage(true, true)]
        [SerializeField]  Color _doorOnColor;

        //setters
        public DifficultyLevel Difficulty => _difficulty;
        public Color DoorOffColor => _doorOffColor;
        public Color DoorOnColor => _doorOnColor;

        [Header("Dungeon Settings")]
        public List<GameObject> easyDungeonPrefabs;
        public List<GameObject> mediumDungeonPrefabs;
        public List<GameObject> hardDungeonPrefabs;
        
        [Range(0, 100)] public int EnemyCount;
        //public int maxEnemyCount;
        public GameObject[] enemyPrefabs;

        [Range(0, 100)] public float xpAmount;
        //[Range(0, 100)] public float maxExpAmount;

        [Range(0, 100)] public int SkillPoints;
        //[Range(0, 100)] public int maxSkillPoints;

        [Range(0,100)]public float itemDropChance;
        [Range(0,100)]public float abilityDropChance;

        public Ability abilityReward;
        public Item itemReward;

      
    }
}