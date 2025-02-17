using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questTitle;
    public string questDescription;
    public int killsRequired;
    public int killsMade = 0;

    [SerializeField] private string targetMonsterTag; // Tag of the monster to kill

    public bool IsCompleted => killsMade >= killsRequired;

    public void RegisterKill(GameObject killedMonster)
    {
        if (killedMonster.CompareTag(targetMonsterTag))  // Compare against the stored tag
        {
            killsMade++;
            Debug.Log($"Target monster killed: {killsMade}/{killsRequired}");
        }
    }

    public void ResetQuest()
    {
        killsMade = 0;
    }
}
