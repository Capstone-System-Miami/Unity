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
        ///
        
        
        [SerializeField] private DifficultyLevel _difficulty;

        //[SerializeField] private Light2D _light;
        //private Color _lightColor = Color.white;
//
        //[SerializeField] private Tilemap _signTilemap;
        //[SerializeField] private Vector3Int _signTilePosition;
        //private TileBase _tile;
        //private Color _tileColor;

        //Antony
        [ColorUsage(true, true)]
        [SerializeField]  Color easyColor;
        [ColorUsage(true, true)]
        [SerializeField] Color mediumColor;
        [ColorUsage(true, true)]
        [SerializeField] Color hardColor;
        
        
        [SerializeField] private Material _material;
        
        /// <summary>
        /// This would ideally be on the generator script
        /// </summary>
        [SerializeField] private DungeonEntrancePreset[] _presets;

        // This wouldn't be here in the final version --
        // these fns would be called from the Generator script.
        private void Start()
        {
            //LoadPreset(_presets[(int)_difficulty]);
            //SetLight();
            //SwapTileForSign();
            _difficulty = Random.Range(0, 3) == 0 ? DifficultyLevel.EASY : Random.Range(0, 3) == 1 ? DifficultyLevel.MEDIUM : DifficultyLevel.HARD;
        }

        //public void LoadPreset(DungeonEntrancePreset preset)
        //{
        //    _difficulty = preset.Difficulty;
        //    _lightColor = preset.LightColor;
        //    _tile = preset.Tile;
        //    _tileColor = preset.TileColor;
        //}
        
        public void setDungeonColor()
        {
        
            if (_difficulty == DifficultyLevel.EASY)
            {
                _material.SetColor("_Color", easyColor);
            }
            else if (_difficulty == DifficultyLevel.MEDIUM)
            {
                _material.SetColor("_Color", mediumColor);
            }
            else if (_difficulty == DifficultyLevel.HARD)
            {
                _material.SetColor("_Color", hardColor);
            }
        }
        
        public void turnOffDungeonColor()
        {
            _material.SetColor("_Color", Color.black);
        }
        
        //public void SwapTileForSign()
        //{
        //    _signTilemap.SetTile(_signTilePosition, _tile);
        //    _signTilemap.color = _tileColor;
        //}
//
        //public void SetLight()
        //{
        //    _light.color = _lightColor;
        //}
    }
}
