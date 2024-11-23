using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        [SerializeField] private DungeonEntrancePreset[] _presets;
        [SerializeField] private Material _material;
        
        [HideInInspector]
        public DungeonEntrancePreset _currentPreset;
        
        
        

        private void Awake()
        {
            _material = new Material(_material);
            TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
            tilemapRenderer.material = _material;
        }
        
        public void SetDifficulty(DifficultyLevel difficulty)
        {
            foreach (DungeonEntrancePreset preset in _presets)
            {
                if (preset.Difficulty == difficulty)
                {
                    LoadPreset(preset);
                    if (_currentPreset == null)
                    {
                        Debug.LogError("Preset not loaded");
                    }
                    break;
                }
            }
        }
        
        public DungeonEntrancePreset CurrentPreset
        {
            get => _currentPreset;
            set
            {
                _currentPreset = value;
            }
        }


        public void LoadPreset(DungeonEntrancePreset preset)
        {
            if (preset == null)
            {
                Debug.LogError("Preset is null in LoadPreset!");
                return;
            }

            CurrentPreset = preset;
            ApplyPreset();
        }

        public void ApplyPreset()
        {
            if (CurrentPreset == null)
            {
                Debug.LogError($"_currentPreset is null in {gameObject.name}'s ApplyPreset()");
                return;
            }

            if (_material == null)
            {
                Debug.LogError($"_material is null in {gameObject.name}'s ApplyPreset()");
                return;
            }

            _material.SetColor("_Color", CurrentPreset.DoorOffColor);
        }


        public void TurnOnDungeonColor()
        {
            _material.SetColor("_Color", CurrentPreset.DoorOnColor);
            Debug.Log($"Turned on dungeon color for {gameObject.name}.");
        }

        public void TurnOffDungeonColor()
        {
            _material.SetColor("_Color", CurrentPreset.DoorOffColor);
            Debug.Log($"Turned off dungeon color for {gameObject.name}.");
        }
    }
}