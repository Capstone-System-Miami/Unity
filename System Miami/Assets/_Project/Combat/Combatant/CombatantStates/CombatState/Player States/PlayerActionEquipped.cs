using System;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.ui;
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

            InputPrompts =
                $"{Database.MGR.GetDataWithJustID(selectedCombatAction.ID).Name} equipped.\n" +
                $"Hover over a tile to aim.\n\n" +
                $"Click to lock your targets\n" +
                $"(You will still be able to change your mind),\n\n" +
                $"Or Right Click to select a different Action.";

            UI.MGR.UpdateInputPrompt(InputPrompts);

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
