using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        private DifficultyLevel _difficulty;

        private Color _lightColor = Color.white;

        [SerializeField] private Tilemap _signTilemap;
        [SerializeField] private Vector3Int _signTilePosition;
        private TileBase _tile;
        private Color _tileColor;

        public void LoadPreset(DungeonEntrancePreset preset)
        {
            _difficulty = preset.Difficulty;
            _lightColor = preset.LightColor;
            _tile = preset.Tile;
            _tileColor = preset.TileColor;
        }

        public void SwapTileForSign()
        {
            _signTilemap.SetTile(_signTilePosition, _tile);
            _signTilemap.color = _tileColor;
        }

        public void LightOn()
        {
            //etc...
        }
    }
}
