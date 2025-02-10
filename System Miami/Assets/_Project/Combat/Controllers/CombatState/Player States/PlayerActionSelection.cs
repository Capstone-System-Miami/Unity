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

        public override void Update()
        {
            // For the player, we update the focus every frame.
            combatant.UpdateFocus();
        }

        /// <summary>
        /// TODO: Actually determine the correct CombatAction to
        /// return based on the input. Right now it just returns
        /// the first thing in the player's loadout, if there is
        /// anything in there.
        /// </summary>
        /// <param name="slot"></param>
        private void HandleSlotClick(ActionQuickslot slot)
        {
            if (combatant.Loadout.PhysicalAbilities == null) { Debug.LogWarning("physnull");return; }
            if (!combatant.Loadout.PhysicalAbilities.Any()) { Debug.LogWarning("physnone"); return; }
            if (combatant.Loadout.PhysicalAbilities[0] == null) { Debug.LogWarning("phys[0]null"); return; }

            selectedCombatAction = combatant.Loadout.PhysicalAbilities[0];

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
