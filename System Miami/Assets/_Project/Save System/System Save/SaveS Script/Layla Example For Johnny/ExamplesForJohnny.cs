/*using System.IO;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class ExamplePlayer : ISaveable<ExamplePlayer>
    {
        [SerializeField] private PlayerLevel level;
        [SerializeField] private string exampleSerializedField = "This field is serialized.";
        [SerializeField] private string testString = "This object was created from scratch and has not been saved.";

        public int pubVarExample = 5;
        public string pubVarExampleStr = "";

        private double fullPrivateField = 13.465747;

        private static string filePath => Path.Combine(Application.persistentDataPath, "savefile.json");

        void Start()
        {
            ExamplePlayer player = new ExamplePlayer();
            player.SaveToFile(); // Save example data

            ExamplePlayer loadedPlayer = ExamplePlayer.LoadFromJson(); // Load saved data
            Debug.Log($"Loaded Player Test String: {loadedPlayer.testString}");
        }



        public ExamplePlayer SaveToFile()
        {
            testString = $"This object was last saved at {System.DateTime.Now}";
            fullPrivateField = Mathf.PI;

            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved to {filePath}");

            return this;
        }

        public void LoadFromFile(ExamplePlayer toLoad)
        {
            this.level = toLoad.level;
            this.exampleSerializedField = toLoad.exampleSerializedField;
            this.testString = toLoad.testString;
            this.pubVarExample = toLoad.pubVarExample;
            this.pubVarExampleStr = toLoad.pubVarExampleStr;
        }
    }

        public static ExamplePlayer LoadFromJson()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Save file not found, creating new instance.");
                return new ExamplePlayer();
            }

            string json = File.ReadAllText(filePath);
            ExamplePlayer loaded = JsonUtility.FromJson<ExamplePlayer>(json);
            Debug.Log("Loaded from file.");
            return loaded;
        }
    }
}
*/