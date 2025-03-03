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
            questNameText.text = quest.questName;
            questDescriptionText.text = quest.questDescriptionLine;
            progressText.text = quest.objectiveGoal.ToString();
            xpRewardText.text = quest.rewardEXP.ToString();
            creditRewardText.text = quest.rewardCurrency.ToString();
        }
    }
}
