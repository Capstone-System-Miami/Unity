using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionTileSelection : CombatantState
    {
        public ActionTileSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void aOnEnter()
        {
            machine.combatant.Abilities.TryEquip(machine.TypeToEquip, machine.IndexToEquip);
        }


        public override void bUpdate()
        {
            UpdateFocusedTile();

            if (machine.Decisions.UnequipTriggered())
            {
                machine.SwitchState(machine.actionSelection);
            }

            if (machine.Decisions.LockTargetsTriggered())
            {
                machine.SwitchState(machine.actionTileConfirmation);
            }


            /// if direction is not same as prev frame, update combatant
            /// vs
            /// if tile changes update combatant
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
            throw new System.NotImplementedException();
        }

        protected abstract bool SelectTile();
        protected abstract bool SkipPhase();
    }
}
