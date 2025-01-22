using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionTileSelection : CombatantState
    {
        public ActionTileSelection(CombatantStateMachine context)
            : base(context, Phase.Action) { }

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
