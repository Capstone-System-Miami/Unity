using System.Collections.Generic;
using System.Linq;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using SystemMiami.ui;
using SystemMiami.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{

    public class DungeonRewardsPanel : Singleton<DungeonRewardsPanel>
    {
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private InventoryUI inventoryUI;

        [SerializeField] private GameObject panel;

        // [SerializeField] private ItemGrid inventoryGridPhysical;
        // [SerializeField] private ItemGrid inventoryGridMagical;
        // [SerializeField] private ItemGrid inventoryGridConsumable;
        [SerializeField] private ItemGrid loadoutGridPhysical;
        [SerializeField] private ItemGrid loadoutGridMagical;
        [SerializeField] private ItemGrid loadoutGridConsumable;

        
        [SerializeField] private ItemGrid dungeonRewardsGrid;

        [SerializeField] private TextMeshProUGUI textExpReward;
        [SerializeField] private TextMeshProUGUI textCreditReward;
        [SerializeField] private Button continueButton;

        [SerializeField] private dbug log;

        
        private DungeonData currentDungeonData;

        public DungeonData CurrentDungeonData
        {
            get => currentDungeonData;
        }

        private void Start()
        {
            
            if (panel != null)
                panel.SetActive(false);
            
        }

        private void OnEnable()
        {
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshPanel;
                
            }
            SubscribeToGrid(inventoryUI.Tabs.TabConsumable.ItemGrid);
            SubscribeToGrid(inventoryUI.Tabs.TabPhysical.ItemGrid);
            SubscribeToGrid(inventoryUI.Tabs.TabMagical.ItemGrid);
            SubscribeToGrid(inventoryUI.Tabs.TabEquipment.ItemGrid);
            SubscribeToGrid(loadoutGridPhysical);
            SubscribeToGrid(loadoutGridMagical);
            SubscribeToGrid(loadoutGridConsumable);
            
            
        }

        private void OnDisable()
        {
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= RefreshPanel;
            }
            UnsubscribeToGrid(inventoryUI.Tabs.TabConsumable.ItemGrid);
            UnsubscribeToGrid(inventoryUI.Tabs.TabPhysical.ItemGrid);
            UnsubscribeToGrid(inventoryUI.Tabs.TabMagical.ItemGrid);
            UnsubscribeToGrid(inventoryUI.Tabs.TabEquipment.ItemGrid);
            UnsubscribeToGrid(loadoutGridPhysical);
            UnsubscribeToGrid(loadoutGridMagical);
            UnsubscribeToGrid(loadoutGridConsumable);
           
        }

       
        public void ShowPanel(DungeonData dungeonData)
        {
            currentDungeonData = dungeonData;
            continueButton.onClick.AddListener ( () => HandleContinueButtonClickedClicked());
           
            FillDungeonRewardsUI(dungeonData);

            
            //FillInventoryUI();
            FillloadoutUI();

           
            if (panel != null)
                panel.SetActive(true);
        }

       
        public void HidePanel()
        {
            if (panel != null)
                panel.SetActive(false);
            continueButton.onClick.RemoveAllListeners();
        }

        
        public void RefreshPanel()
        {
            //FillInventoryUI();
            Debug.Log("Refresh Panel");
            FillloadoutUI();
            FillDungeonRewardsUI(currentDungeonData);
        }

       
        /*private void FillInventoryUI()
        {
            if (playerInventory == null)
            {
                log?.error("No player inventory assigned. Cannot fill inventory UI.");
                return;
            }

           
            // inventoryGridPhysical?.ClearSlots();
            // inventoryGridMagical?.ClearSlots();
            // inventoryGridConsumable?.ClearSlots();
            //
            //
            // inventoryGridPhysical?.FillSlots(playerInventory.PhysicalAbilityIDs);
            // inventoryGridMagical?.FillSlots(playerInventory.MagicalAbilityIDs);
            // inventoryGridConsumable?.FillSlots(playerInventory.ConsumableIDs);

            
        }*/

        public void CheckGrid(ItemGrid grid, bool InventoryGrid)
        {
           
            foreach (InventoryItemSlot slot in grid.Slots )
            {
               
              ItemData dataToMove = slot.ClearSlot();
              if (InventoryGrid)
              {
                  MoveItemToloadout(dataToMove.ID);
              }
              else
              {
                  MoveItemToInventory(dataToMove.ID);
              }
            }
            
        }

        public void HandleSlotDoubleClick(InventoryItemSlot slot)
        {
            
            ItemData dataToMove = slot.ClearSlot();
            if (playerInventory.AllValidInventoryItems.Contains(dataToMove.ID))
            {
                MoveItemToloadout(dataToMove.ID);
            }
            else
            {
                MoveItemToInventory(dataToMove.ID);
            }
        }

        public void SubscribeToGrid(ItemGrid grid)
        {
            foreach (InventoryItemSlot slot in grid.Slots)
            {
               
                slot.slotDoubleClicked += HandleSlotDoubleClick;
            }
        } 
        
        
        public void UnsubscribeToGrid(ItemGrid grid)
        {
            foreach (InventoryItemSlot slot in grid.Slots)
            {
               
                slot.slotDoubleClicked -= HandleSlotDoubleClick;
            }
        } 
        

        public void MoveItemToloadout(int itemID)
        {
            if (playerInventory == null) return;
            playerInventory.MoveToQuickslot(itemID);
            Debug.Log($"MoveItemToQuickslot: {itemID}");
            RefreshPanel();
        }

       
        public void MoveItemToInventory(int itemID)
        {
            if (playerInventory == null) return;
            playerInventory.MoveToInventory(itemID);
            Debug.Log($"MoveItemToInventory: {itemID}");
            RefreshPanel();
        }

        private void FillloadoutUI()
        {
            if (!PlayerManager.MGR.TryGetComponent(out playerInventory))
            {
                string[] error = new string[]
                {
                    "DungeonRewardsPanel couldn't find the Inventory component on Player"
                };
                UI.MGR.StartDialogue(this, true, true, false, "ERROR", error);
                log.error("No player inventory assigned. Cannot fill loadout UI.");
            }


            loadoutGridPhysical?.ClearSlots();
            loadoutGridMagical?.ClearSlots();
            loadoutGridConsumable?.ClearSlots();


            loadoutGridPhysical?.FillSlots(playerInventory.QuickslotPhysicalAbilityIDs);
            loadoutGridMagical?.FillSlots(playerInventory.QuickslotMagicalAbilityIDs);
            loadoutGridConsumable?.FillSlots(playerInventory.QuickslotConsumableIDs);
        }


        private void FillDungeonRewardsUI(DungeonData data)
        {
            if (data == null)
            {

                dungeonRewardsGrid?.ClearSlots();
                return;
            }

            dungeonRewardsGrid?.ClearSlots();


            List<int> rewardIDs = new List<int>();
            if (data.ItemRewards != null)
            {
                foreach (ItemData itemData in data.ItemRewards)
                {
                    rewardIDs.Add(itemData.ID);
                }
            }


            dungeonRewardsGrid?.FillSlots(rewardIDs);


            if (textExpReward != null)
                textExpReward.text = $"{data.EXPToGive}";

            if (textCreditReward != null)
                textCreditReward.text = $"{data.Credits}";
        }


        private void HandleContinueButtonClickedClicked()
        {
            if (!playerInventory.QuickslotPhysicalAbilityIDs.Any()
                && !playerInventory.QuickslotMagicalAbilityIDs.Any()
                && !playerInventory.QuickslotConsumableIDs.Any())
            {
                Debug.LogError(
                    "NOTHING IN PLAYER LOADOUT." +
                    "Handle this. For now, do nothing",
                    this);

                return;
            }

            HidePanel();
            GAME.MGR.GoToDungeon(CurrentDungeonData);
        }
    }
}
