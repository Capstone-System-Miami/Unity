/*using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemMiami
{
    public class SaveSystem : MonoBehaviour
    {
        private string saveFilePath;
        public static SaveSystem Instance;

        void Awake()
        {
            // Ensure only one SaveSystem exists
            if (Instance == null) 
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } 
            else 
            {
                Destroy(gameObject);
            }

            saveFilePath = Application.persistentDataPath + "/savegame.json";
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Save when a scene loads
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SaveGame();
        }

        public void SaveGame()
        {
            SaveData data = new SaveData();

            data.playerLevel = PlayerManager.Instance.playerLevel;
            data.playerCredits = PlayerManager.Instance.playerCredits;
            data.playerExperience = PlayerManager.Instance.playerExperience;
            data.playerClassType = PlayerManager.Instance.playerClassType;

            data.magicalAbilities = new List<string>(PlayerManager.Instance.MagicalAbilities); // Magic
            data.physicalAbilities = new List<string>(PlayerManager.Instance.PhysicalAbilities); //  Physical
            data.playerTools = new List<string>(PlayerManager.Instance.tools);
            data.activeQuests = QuestManager.Instance.GetActiveQuests();

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game Saved!");
        }

        public void LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // Restore Player Data
                PlayerManager.Instance.playerLevel = data.playerLevel;
                PlayerManager.Instance.playerCredits = data.playerCredits;
                PlayerManager.Instance.playerExperience = data.playerExperience;
                PlayerManager.Instance.playerClassType = data.playerClassType;

                PlayerManager.Instance.MagicalAbilities = new List<string>(data.magicalAbilities); // ✅ Loading separately
                PlayerManager.Instance.PhysicalAbilities = new List<string>(data.physicalAbilities); // ✅ Loading separately
                PlayerManager.Instance.tools = new List<string>(data.playerTools);
                QuestManager.Instance.LoadQuests(data.activeQuests);

                Debug.Log("Game Loaded!");
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
*/


// Need to organize and find all the location this is being stored if not it will reproduce errors. 