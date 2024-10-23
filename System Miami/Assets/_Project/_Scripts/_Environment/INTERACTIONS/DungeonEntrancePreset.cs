using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public enum DifficultyLevel { EASY, MEDIUM, HARD }

    [CreateAssetMenu(fileName = "New Dungeon Entrance Preset", menuName = "Eviron/Dungeon Entrance Preset")]
    public class DungeonEntrancePreset : ScriptableObject
    {
        [SerializeField] private DifficultyLevel _difficulty;

        [SerializeField] private Color _lightColor = Color.white;

        [SerializeField] private Sprite _tileSprite;
        [SerializeField] private Color _tileColor;

        public DifficultyLevel Difficulty { get { return  _difficulty; } }
        public Color LightColor { get { return  _lightColor; } }
        public TileBase Tile
        {
            get
            {
                IsometricRuleTile newTile = CreateInstance<IsometricRuleTile>();
                newTile.m_DefaultSprite = _tileSprite;

                return newTile;
            }
        }
        public Color TileColor { get { return _tileColor; } }
    }
}
