using System;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class PlayerActionEquipped : ActionEquipped
    {
        private bool slotBeenClicked;
        private bool actionBeenChanged;

        public PlayerActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }

        public override void OnEnter()
        {
            base.OnEnter();

            UI.MGR.SlotClicked += HandleSlotClicked;
        }

        public override void OnExit()
        {
            base.OnExit();
            UI.MGR.SlotClicked -= HandleSlotClicked;
        }

        private void HandleSlotClicked(ActionQuickslot slot)
        {
            slotBeenClicked = true;

            if (selectedCombatAction != slot.CombatAction)
            {
                requestedReEquip = slot.CombatAction;
                actionBeenChanged = true;
            }
        }

        protected override bool ReEquipRequested()
        {
            bool temp = slotBeenClicked && actionBeenChanged;
            slotBeenClicked = false;
            actionBeenChanged = false;
            return temp;
        }

        protected override bool UnequipRequested()
        {
            // Player right clicks?
            return Input.GetMouseButtonDown(1);
        }

        protected override bool SelectTileRequested()
        {
            // Player clicks? Event from UI buttons?
            return Input.GetMouseButtonDown(0);
        }
    }
}
