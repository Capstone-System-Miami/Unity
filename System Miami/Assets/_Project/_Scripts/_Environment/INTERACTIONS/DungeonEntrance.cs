using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        /// <summary>
        /// This wouldn't be serialized in the final version --
        /// it would be set at runtime by the Generator script.
        /// </summary>
        [SerializeField] private DifficultyLevel _difficulty;

        [SerializeField] private Light2D _light;
        private Color _lightColor = Color.white;

        [SerializeField] private Tilemap _signTilemap;
        [SerializeField] private Vector3Int _signTilePosition;
        private TileBase _tile;
        private Color _tileColor;

        /// <summary>
        /// This would ideally be on the generator script
        /// </summary>
        [SerializeField] private DungeonEntrancePreset[] _presets;

        // This wouldn't be here in the final version --
        // these fns would be called from the Generator script.
        private void Start()
        {
            LoadPreset(_presets[(int)_difficulty]);
            SetLight();
            SwapTileForSign();
        }

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

        public void SetLight()
        {
            _light.color = _lightColor;
        }
    }
}
