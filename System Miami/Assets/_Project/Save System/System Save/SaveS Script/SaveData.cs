
// ***IMPORTANT***
// This script needs to be connected with the current System Miami game, I will be doing this on Monday
// Most of what we should be using has been imported but will need to double check everything
// ***IMPORTANT***


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SystemMiami.CombatSystem;
using SystemMiami.InventorySystem;
using SystemMiami.Management;

namespace SystemMiami
{



    public class SaveData : MonoBehaviour
    {
        public ExamplePlayer1 examplePlayer1 = new ExamplePlayer1();

        public TextMeshProUGUI inventoryDisplay;
        public Button saveButton, loadButton, saveAttributesButton;
        public TMP_InputField levelInput, constitutionInput, vigorInput, manaInput, strengthInput;

        private void Start()
        {
            if (saveButton != null)
                saveButton.onClick.AddListener(SaveToJson); // Save to a .json File

            if (loadButton != null)
                loadButton.onClick.AddListener(() => { LoadFromJson(); UpdateUI(); }); // Loads a .json File

            if (saveAttributesButton != null)
                saveAttributesButton.onClick.AddListener(UpdateAttributesFromInput); // Example of saving attributes

            UpdateUI();
        }

        public void SaveToJson()  // Save to a .json File
        {
            string playerData = JsonUtility.ToJson(examplePlayer1);
            string filePath = Application.persistentDataPath + "/PlayerData.json";
            System.IO.File.WriteAllText(filePath, playerData);
            Debug.Log("Data Saved!");
        }

        public void LoadFromJson() // Loads a .json File
        {
            string filePath = Application.persistentDataPath + "/PlayerData.json";
            if (System.IO.File.Exists(filePath))
            {
                string playerData = System.IO.File.ReadAllText(filePath);
                examplePlayer1 = JsonUtility.FromJson<ExamplePlayer1>(playerData);
                Debug.Log("Data Loaded!");
                UpdateUI(); // Refresh UI after loading
            }
            else
            {
                Debug.LogWarning("Save file not found!");
            }
        }

        public void UpdateUI()
        {
            if (inventoryDisplay != null)
            {
                inventoryDisplay.text = $"Currency: {examplePlayer1.currency}\n" +
                                        $"Level: {examplePlayer1.level}\n" +
                                        $"Constitution: {examplePlayer1.Constitution}\n" +
                                        $"Vigor: {examplePlayer1.Vigor}\n" +
                                        $"Mana: {examplePlayer1.Mana}\n" +
                                        $"Strength: {examplePlayer1.Strength}\n";

                inventoryDisplay.text += "\nItems:\n";
                foreach (var item in examplePlayer1.items)
                {
                    inventoryDisplay.text += $"{item.name} - {item.desc}\n";
                }
            }

            // Update input fields with current values
            if (levelInput != null) levelInput.text = examplePlayer1.level.ToString();
            if (constitutionInput != null) constitutionInput.text = examplePlayer1.Constitution.ToString();
            if (vigorInput != null) vigorInput.text = examplePlayer1.Vigor.ToString();
            if (manaInput != null) manaInput.text = examplePlayer1.Mana.ToString();
            if (strengthInput != null) strengthInput.text = examplePlayer1.Strength.ToString();
        }

        public void UpdateAttributesFromInput()
        {
            // Parse user input and update attributes
            if (levelInput != null) int.TryParse(levelInput.text, out examplePlayer1.level);
            if (constitutionInput != null) int.TryParse(constitutionInput.text, out examplePlayer1.Constitution);
            if (vigorInput != null) int.TryParse(vigorInput.text, out examplePlayer1.Vigor);
            if (manaInput != null) int.TryParse(manaInput.text, out examplePlayer1.Mana);
            if (strengthInput != null) int.TryParse(strengthInput.text, out examplePlayer1.Strength);

            UpdateUI();
        }
    }

    [System.Serializable]
    public class ExamplePlayer1
    {
        public int currency;
        public int level;
        public int Constitution;
        public int Vigor;
        public int Mana;
        public int Strength;
        public List<Items1> items = new List<Items1>();
    }

    [System.Serializable]
    public class Items1
    {
        public string name;
        public string desc;
    }

}
