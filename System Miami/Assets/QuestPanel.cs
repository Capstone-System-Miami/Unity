using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SystemMiami
{
    public class QuestPanel : MonoBehaviour
    {
        public Quest quest;
        public TMP_Text questNameText;
        public TMP_Text questDescriptionText;
        public TMP_Text progressText;
        public TMP_Text xpRewardText;
        public TMP_Text creditRewardText;

        public void GiveQuestToPlayer()
        {
            Debug.Log("Quest given to player");
        }

        public void Initialize(Quest questArg)
        {
            quest = questArg;
            if (questNameText != null)
            {
                questNameText.text = quest.questName;
            }
            questDescriptionText.text = quest.questDescriptionLine;
            progressText.text = $"Progress: {quest.enemiesToGoal} / {quest.objectiveGoal}";
            xpRewardText.text = $"{quest.rewardEXP} EXP";
            creditRewardText.text = $"{quest.rewardCurrency} Credits";
        }

        public void UpdateQuest()
        {
            progressText.text = $"Progress: {quest.enemiesToGoal} / {quest.objectiveGoal}";
        }

        public void CompleteQuest()
        {
            questDescriptionText.text = "Quest Completed!";
            xpRewardText.text = $"Gained {quest.rewardEXP} EXP!";
            creditRewardText.text = $"Gained {quest.rewardCurrency} Credits!";
        }
    }
}
