using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemMiami.Management;
namespace SystemMiami
{

    public interface ISaveable<T> where T : class
    {
        T SaveToFile();
        void LoadFromFile(T toLoad);
    }

    public class SaveSystem : Singleton<SaveSystem>
    {
        private string saveFilePath;
        public static SaveSystem Instance;


        //for example vv
        ExamplePlayer examplePlayer;
        UnrelatedClass randomOtherVariable;
        // for example ^^

        protected override void Awake()
        {
            base.Awake();

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

            data.examplePlayer = this.examplePlayer;
            data.randomOtherClass = this.randomOtherVariable;

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
                // pretend this is from player manager vv
                examplePlayer.LoadFromFile(data.examplePlayer); 
                randomOtherVariable.LoadFromFile(data.randomOtherClass);
                // pretend this is from player manager ^^

                Debug.Log("Game Loaded!");
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

// Need to organize and find all the location this is being stored if not it will reproduce errors. 