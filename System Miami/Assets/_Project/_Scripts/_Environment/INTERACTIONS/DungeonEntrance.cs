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

        /// <summary>
        /// This would ideally be on the generator script
        /// </summary>


        [SerializeField] private DungeonEntrancePreset[] _presets;
        [SerializeField] private Material _material;

        private DungeonEntrancePreset _currentPreset;

        private void Awake()
        {
            //Setting a new instance of the glowing material so every single entrance
            // doesn't turn on at the same time.
            _material = new Material(_material);
            TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
            tilemapRenderer.material = _material;

            DifficultyLevel _difficulty = GetRandomDifficulty();

            foreach (DungeonEntrancePreset preset in _presets)
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
