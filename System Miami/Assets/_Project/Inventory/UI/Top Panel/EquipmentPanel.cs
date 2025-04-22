using System.Linq;
using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public class EquipmentPanel : CharacterMenuSubPanel
    {
        [SerializeField] SelectableSprite skeletonLabelBKG;
        [SerializeField] SelectableTMP skeletonLabel;
        [SerializeField] int skeletonItemId;
        [SerializeField] InventoryItemSlot skeletonSlot;

        [SerializeField] SelectableSprite slimeLabelBKG;
        [SerializeField] SelectableTMP slimeLabel;
        [SerializeField] int slimeItemId;
        [SerializeField] InventoryItemSlot slimeSlot;

        [SerializeField] SelectableSprite orcLabelBKG;
        [SerializeField] SelectableTMP orcLabel;
        [SerializeField] int orcItemId;
        [SerializeField] InventoryItemSlot orcSlot;

        [SerializeField] SelectableSprite spiderLabelBKG;
        [SerializeField] SelectableTMP spiderLabel;
        [SerializeField] int spiderItemId;
        [SerializeField] InventoryItemSlot spiderSlot;

        private Dictionary<InventoryItemSlot, SelectableSprite> bkgAtSlots = new();
        private Dictionary<InventoryItemSlot, SelectableTMP> textAtSlots = new();
        private Dictionary<InventoryItemSlot, int> idsAtSlots = new();
        // private List<InventoryItemSlot> emptySlots =>
        //     slots.Where(slot => slot.ItemCount == 0).ToList();

        private void OnEnable()
        {
            bkgAtSlots = new()
            {
                { skeletonSlot, skeletonLabelBKG },
                { slimeSlot, slimeLabelBKG },
                { orcSlot, orcLabelBKG },
                { spiderSlot, spiderLabelBKG },
            };
            textAtSlots = new()
            {
                { skeletonSlot, skeletonLabel },
                { slimeSlot, slimeLabel },
                { orcSlot, orcLabel },
                { spiderSlot, spiderLabel },
            };

            idsAtSlots = new()
            {
                { skeletonSlot, skeletonItemId },
                { slimeSlot, slimeItemId },
                { orcSlot, orcItemId },
                { spiderSlot, spiderItemId },
            };
        }

        private void OnDisable()
        {
        }

        public override void Select()
        {
            base.Select();

            List<int> equipmentIds = 
                PlayerManager.MGR?.inventory.EquippedEquipmentModIDs;

            foreach (InventoryItemSlot slot in idsAtSlots.Keys)
            {
                if (equipmentIds.Contains(idsAtSlots[slot]))
                {
                    if (!slot.TryFill(idsAtSlots[slot]))
                    {
                        Debug.Log($"{name} couldn't fill {slot.name}");
                        textAtSlots[slot].Deselect();
                    }
                    else
                    {
                        bkgAtSlots[slot].Select();
                        textAtSlots[slot].Select();
                    }
                }
            }
        }

        public override void Deselect()
        {
            base.Deselect();
        }
    }
}
