using UnityEngine;
using UnityEngine.Tilemaps;
using SystemMiami.Dungeons;

namespace SystemMiami.Outdated
{
    public class F_P_DungeonEntrance : MonoBehaviour
    {
        
        [SerializeField] private DungeonPreset[] _presets;
        [SerializeField] private Material _material;

        private DungeonPreset _currentPreset;

        private void Awake()
        {
            //Setting a new instance of the glowing material so every single entrance
            // doesn't turn on at the same time.
            _material = new Material(_material);
            TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
            tilemapRenderer.material = _material;

            DifficultyLevel _difficulty = GetRandomDifficulty();

            foreach (DungeonPreset preset in _presets)
            {
                if (preset.Difficulty == _difficulty)
                {
                    LoadPreset(preset);
                    break;
                }
            }
            Debug.Log("Selected Difficulty for " + gameObject.name + " is " + _difficulty);
        }

        private DifficultyLevel GetRandomDifficulty()
        {
            float randomValue = Random.value; // Generates a value between 0.0 and 1.0

            if (randomValue < 0.5f)
            {
                return DifficultyLevel.EASY; // 50% chance
            }
            else if (randomValue < 0.8f)
            {
                return DifficultyLevel.MEDIUM; // 30% chance
            }
            else
            {
                return DifficultyLevel.HARD; // 20% chance
            }
        }

        public void LoadPreset(DungeonPreset preset)
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
