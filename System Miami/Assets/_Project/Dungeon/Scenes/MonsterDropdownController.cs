using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterDropdownController : MonoBehaviour
{
    public TMP_Dropdown monsterDropdown; // Dropdown menu
    public Image monsterPreviewImage;   // Preview image
    public TextMeshProUGUI mobCountText; // Mob count text

    public List<MonsterData> monsters = new List<MonsterData>(); // Monster list

    void Start()
    {
        PopulateDropdown();
        monsterDropdown.onValueChanged.AddListener(UpdateMonsterUI);
        UpdateMonsterUI(0); // Set first monster preview
    }

    void PopulateDropdown()
    {
        monsterDropdown.ClearOptions();
        List<string> monsterNames = new List<string>();

        foreach (MonsterData monster in monsters)
        {
            monsterNames.Add(monster.monsterName);
        }

        monsterDropdown.AddOptions(monsterNames);
    }

    void UpdateMonsterUI(int index)
    {
        MonsterData selectedMonster = monsters[index];

        // Update preview image
        monsterPreviewImage.sprite = selectedMonster.monsterSprite;

        // Update mob count dynamically
        UpdateMobCount();
    }

    public void UpdateMobCount()
    {
        string selectedMonsterName = monsters[monsterDropdown.value].monsterName;

        GameObject[] allGoblins = GameObject.FindGameObjectsWithTag("Goblin");

        int count = 0;
        foreach (GameObject goblin in allGoblins)
        {
            if (goblin.name.StartsWith(selectedMonsterName))
            {
                count++;
            }
        }

        mobCountText.text = $"Mob Count: {count}";
    }
}
