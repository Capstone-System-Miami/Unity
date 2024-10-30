using UnityEngine;
using UnityEngine.Tilemaps;

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
    }
}
