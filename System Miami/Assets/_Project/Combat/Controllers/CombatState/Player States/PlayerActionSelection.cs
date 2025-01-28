using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        CombatAction selectedAction;

        bool slotBeenClicked = false;

        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();
            combatant.Physical.Add(new AbilityPhysical((combatant as PlayerCombatant).test, combatant));

            UI.MGR.RefactorSlotClicked += HandleSlotClick;
        }

        private void HandleSlotClick(AbilitySlot slot)
        {
            // TODO
            // Actually determine the correct action to return

            if (combatant.Physical == null) { return; }
            if (!combatant.Physical.Any()) { return; }
            if (combatant.Physical[0] == null) { return; }

            selectedAction = combatant.Physical[0];

            slotBeenClicked = true;
        }

        protected override bool EquipRequested()
        {
            if (slotBeenClicked)
            {
                slotBeenClicked = false;
                return true;
            }

            return false;
        }

        protected override bool SkipPhaseRequested()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }
    }
}
