// Author: Johnny Sosa


using UnityEngine;
using SystemMiami;
using SystemMiami.InventorySystem;

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
    public int rewardCredits;
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

    /// <summary>
    /// Grants rewards to the player
    /// TODO: there has to be a better way than FindObjectOfType to get these
    /// components. Can we have the player pass this info to the NPC when they
    /// start or finish a Quest?
    /// </summary>
    public void GrantRewards()
    {
        PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
        Inventory playerInventory = FindObjectOfType<Inventory>();

        if (playerLevel != null)
        {
            playerLevel.GainXP(rewardXP); // Give XP
        }

        if (playerInventory != null)
        {
            playerInventory.AddCredits(rewardCredits); // Give Credits
        }
        Debug.Log($"Quest Completed: {questTitle} | Reward: {rewardCredits} Credits, {rewardXP} XP");
    }
}
