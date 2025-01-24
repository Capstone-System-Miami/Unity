using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionSelection : CombatantState
    {
        public ActionSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void aOnEnter()
        {
            machine.combatant.Abilities.TryUnequip();
        }


        public override void bUpdate()
        {
            // TODO
            //UpdateFocusedTile();
        }

        public override abstract void cMakeDecision();

        public override void eOnExit()
        {
            throw new System.NotImplementedException();
        }

        protected abstract bool ActionSelected();

    }
}
