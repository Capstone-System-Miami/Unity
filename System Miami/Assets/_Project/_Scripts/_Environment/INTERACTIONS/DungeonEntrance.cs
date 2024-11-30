using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        /// <summary>
        /// This wouldn't be serialized in the final version --
        /// it would be set at runtime by the Generator script.
        /// </summary>
        [SerializeField] private DungeonPreset[] _presets;

        /// <summary>
        /// Material used for glowing effects on the dungeon door.
        /// This would ideally be on the generator script.
        /// </summary>
        [SerializeField] private Material _material;

        private DungeonPreset _currentPreset;

        private void Awake()
        {
            // Setting a new instance of the glowing material so every single entrance
            // doesn't turn on at the same time.
            if (_material == null)
            {
                Debug.LogWarning("Material is not assigned. Using default material.");
                _material = new Material(Shader.Find("Sprites/Default"));
            }
            _material = new Material(_material);

            // Assigning the material to the TilemapRenderer
            TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null)
            {
                tilemapRenderer.material = _material;
            }
            else
            {
                Debug.LogError("TilemapRenderer component not found on the GameObject.");
            }

            // Randomly selecting a difficulty level and loading the corresponding preset
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

        /// <summary>
        /// Loads the selected preset and applies its settings.
        /// </summary>
        public void LoadPreset(DungeonPreset preset)
        {
            _currentPreset = preset;
            ApplyPreset();
        }

        /// <summary>
        /// Applies the settings from the current preset.
        /// </summary>
        public void ApplyPreset()
        {
            // Set the color of the door to default
            _material.SetColor("_Color", _currentPreset.DoorOffColor);
        }

        /// <summary>
        /// Instantiates a random dungeon board (scene) from the current preset.
        /// </summary>
        private void InstantiateDungeonBoard()
        {
            // Get the random dungeon scene from the preset
            SceneAsset sceneAsset = _currentPreset.GetRandomDungeonScene();

            if (sceneAsset != null)
            {
                string sceneName = sceneAsset.name; // Get the scene name from SceneAsset
                Debug.Log("Selected Dungeon Scene: " + sceneName);

                // Load the dungeon scene when the player is ready to enter
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single); // Change to Additive if necessary
                Debug.Log("Dungeon Scene Loaded: " + sceneName);
            }
            else
            {
                Debug.LogWarning("No dungeon scene found for the selected preset.");
            }
        }

        /// <summary>
        /// Triggered when the player interacts with the dungeon entrance.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the player enters the dungeon entrance (adjust based on player tag)
            if (other.CompareTag("Player"))
            {
                EnterDungeon();
            }
        }

        /// <summary>
        /// Triggered when the player presses the button to enter the dungeon.
        /// </summary>
        public void OnEnterButtonPressed()
        {
            EnterDungeon();
        }

        /// <summary>
        /// Handles entering the dungeon by teleporting the player to a random dungeon.
        /// </summary>
        private void EnterDungeon()
        {
            Debug.Log("Entering the dungeon...");
            if (_currentPreset != null)
            {
                InstantiateDungeonBoard();
                SetDungeonColor();
            }
            else
            {
                Debug.LogError("No preset assigned to this door.");
            }
        }

        /// <summary>
        /// Sets the color of the door to its "active" state.
        /// </summary>
        public void SetDungeonColor()
        {
            _material.SetColor("_Color", _currentPreset.DoorOnColor);
            Debug.Log("Applying color!");
        }

        /// <summary>
        /// Sets the color of the door to its "inactive" state.
        /// </summary>
        public void TurnOffDungeonColor()
        {
            _material.SetColor("_Color", _currentPreset.DoorOffColor);
        }
    }
}
