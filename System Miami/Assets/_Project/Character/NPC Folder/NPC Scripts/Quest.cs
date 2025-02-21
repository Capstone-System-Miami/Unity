// Author: Johnny Sosa


using UnityEngine;
using SystemMiami;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questTitle;
    public string questDescription;

    [Header("Kill Requirements")]
    public string targetMonsterTag;
    public int killsRequired;
    private int killsMade = 0;

    [Header("Rewards")]
    public int rewardGold;
    public int rewardXP;

    public bool IsCompleted => killsMade >= killsRequired;

    public void RegisterKill(GameObject killedMonster)
    {
        if (killedMonster.CompareTag(targetMonsterTag))
        {
            killsMade++;
            Debug.Log($"Killed {killsMade}/{killsRequired} {targetMonsterTag}s.");
        }
    }

    public void ResetQuest()
    {
        killsMade = 0;
    }

    public void GrantRewards()
    {
        PlayerLevel player = FindObjectOfType<PlayerLevel>();
        if (player != null)
        {
            player.GainXP(rewardXP); // Give XP
            player.AddGold(rewardGold); // Give Gold
        }

        Debug.Log($"Quest Completed: {questTitle} | Reward: {rewardGold} Gold, {rewardXP} XP");
    }
}
