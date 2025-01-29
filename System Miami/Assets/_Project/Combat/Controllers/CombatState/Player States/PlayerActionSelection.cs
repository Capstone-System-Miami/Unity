using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        bool slotBeenClicked = false;

        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();
            UI.MGR.RefactorSlotClicked += HandleSlotClick;
        }

        private void HandleSlotClick(ActionQuickslot slot)
        {
            // TODO
            // Actually determine the correct action to return

            if (combatant.loadout.PhysicalAbilities == null) { Debug.LogWarning("physnull");return; }
            if (!combatant.loadout.PhysicalAbilities.Any()) { Debug.LogWarning("physnone"); return; }
            if (combatant.loadout.PhysicalAbilities[0] == null) { Debug.LogWarning("phys[0]null"); return; }

            selectedCombatAction = combatant.loadout.PhysicalAbilities[0];

            slotBeenClicked = true;
        }

        protected override bool EquipRequested()
        {
            if (slotBeenClicked)
            {
                Debug.LogWarning("Made it to equip request");
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
