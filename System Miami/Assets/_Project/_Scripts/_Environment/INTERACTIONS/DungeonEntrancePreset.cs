using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemMiami
{
    public enum DifficultyLevel { EASY, MEDIUM, HARD }

    [CreateAssetMenu(fileName = "New Dungeon Preset", menuName = "Eviron/Dungeon Preset")]
    public class DungeonPreset : ScriptableObject
    {
        [Header("Dungeon Settings")]
        [SerializeField] private DifficultyLevel _difficulty;

        [Header("Dungeon Scenes")]
        [SerializeField] private SceneAsset[] _dungeons; // Use SceneAsset to reference scenes directly in the inspector

        [Header("Dungeon Features")]
        [SerializeField] private int _minEnemies;
        [SerializeField] private int _maxEnemies;
        [SerializeField] private GameObject[] _itemPlaceholders; // Placeholder items
        [SerializeField] private GameObject[] _abilityPlaceholders; // Placeholder abilities
        [SerializeField] private int _skillPoints; // Skill points for the dungeon

        [Header("Door Colors")]
        [SerializeField] private Color _doorOffColor = Color.black;
        [SerializeField] private Color _doorOnColor = Color.white;

        public DifficultyLevel Difficulty => _difficulty;
        public SceneAsset[] Dungeons => _dungeons;
        public int MinEnemies => _minEnemies;
        public int MaxEnemies => _maxEnemies;
        public GameObject[] ItemPlaceholders => _itemPlaceholders;
        public GameObject[] AbilityPlaceholders => _abilityPlaceholders;
        public int SkillPoints => _skillPoints;

        // Door colors for DungeonEntrance
        public Color DoorOffColor => _doorOffColor;
        public Color DoorOnColor => _doorOnColor;

        // Method to get a random dungeon scene from the list
        public SceneAsset GetRandomDungeonScene()
        {
            if (_dungeons.Length > 0)
            {
                return _dungeons[Random.Range(0, _dungeons.Length)];
            }
            return null; // No dungeon scenes available
        }
    }
}