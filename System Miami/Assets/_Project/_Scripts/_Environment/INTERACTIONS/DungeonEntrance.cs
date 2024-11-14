using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {       
        [SerializeField] private Material _material;
        
        private DungeonEntrancePreset _currentPreset;

        public void Initialize(Material materialInstance, DungeonEntrancePreset preset)
        {
               //Unique material for each dungeon entrance
               _material = materialInstance;
               
               //Assign that unique material to the tilemap renderer
                TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
                tilemapRenderer.material = _material;
                
                //Load the preset
                LoadPreset(preset);
        }       
        
        public void LoadPreset(DungeonEntrancePreset preset)
        {
            _currentPreset = preset;
            ApplyPreset();
        }

        public void ApplyPreset()
        {
            //Set the color of the door to default
            _material.SetColor("_Color", _currentPreset.DoorOffColor);
        }
        
        
        public void SetDungeonColor()
        {
            _material.SetColor("_Color", _currentPreset.DoorOnColor);
            Debug.Log("Applying color!");
        }
        
        public void TurnOffDungeonColor()
        {
            _material.SetColor("_Color", _currentPreset.DoorOffColor);
        }
    }
}
