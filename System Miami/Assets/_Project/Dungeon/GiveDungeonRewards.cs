using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using TMPro;
using UnityEngine;

namespace SystemMiami
{
    public class GiveDungeonRewards : MonoBehaviour
    {
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private PlayerLevel playerLevel;
        [SerializeField] private TextMeshProUGUI text;

        private void OnEnable()
        {
            TurnManager.MGR.DungeonCleared += GiveReward;
            playerInventory = PlayerManager.MGR.GetComponent<Inventory>();
            playerLevel = PlayerManager.MGR.GetComponent<PlayerLevel>();
        }

        private void OnDisable()
        {
            TurnManager.MGR.DungeonCleared -= GiveReward;
        }

        public void GiveReward()
        {
            if (GAME.MGR.TryGetRewards(out List<ItemData> rewards))
            {
                foreach (ItemData reward in rewards)
                {
                    playerInventory.AddToInventory(reward.ID);
                }
            }

            if (GAME.MGR.TryGetEXP(out int exp))
            {
                playerLevel.GainXP(exp);
            }

            if (GAME.MGR.TryGetCredit(out int credit))
            {
                playerInventory.AddCredits(credit);
            }
            
           
        }
    }
    
}
