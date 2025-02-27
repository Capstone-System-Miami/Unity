using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterDropdownController : MonoBehaviour
{
    public TMP_Dropdown monsterDropdown;
    public Image monsterPreviewImage;
    public TextMeshProUGUI mobCountText;

    public List<MonsterData> monsters = new List<MonsterData>();

    void Start()
    {
        PopulateDropdown();
        monsterDropdown.onValueChanged.AddListener(UpdateMonsterUI);
        UpdateMobCount(); // Ensure the counter is updated at the start
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
        UpdateMonsterUI(0); // Set first monster as default
    }

    void UpdateMonsterUI(int index)
    {
        MonsterData selectedMonster = monsters[index];
        monsterPreviewImage.sprite = selectedMonster.monsterSprite;
        UpdateMobCount();
    }

    public void UpdateMobCount()
    {
        string selectedMonsterName = monsters[monsterDropdown.value].monsterName;

        // Find all active enemies with the "Goblin" tag (assuming all goblins use a common tag)
        GameObject[] allGoblins = GameObject.FindGameObjectsWithTag("Goblin");

        // Count only those that match the selected monster name
        int count = 0;
        foreach (GameObject goblin in allGoblins)
        {
            if (goblin.name.Contains(selectedMonsterName)) // Checks if name contains "Goblin", "Orange Goblin", etc.
            {
                count++;
            }
        }

        mobCountText.text = $"Mob Count: {count}";
    }
}
