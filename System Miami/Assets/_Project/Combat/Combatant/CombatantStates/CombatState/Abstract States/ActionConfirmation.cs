using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionConfirmation : CombatantState
    {
        protected CombatAction combatAction;

        public ActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            combatAction.Equip();
            combatAction.LockTargets();
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                combatAction.Reportback();
            }
        }

        public override void MakeDecision()
        {
            if (ConfirmSelection())
            {
                SwitchState(factory.ActionExecution(combatAction));
                return;
            }

            if (CancelConfirmation())
            {
                SwitchState(factory.ActionEquipped(combatAction));
                return;
            }


            // Should this be checking 'input'
            // storing a requested state transition (if any),
            // 
            // Then there's another function that checks
            // if there is request for a state transition,
            // which checks the conditions for the
            // requested transition?
        }

        public override void OnExit()
        {
            base.OnExit();
            combatAction.Unequip();
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelConfirmation();
    }
}
