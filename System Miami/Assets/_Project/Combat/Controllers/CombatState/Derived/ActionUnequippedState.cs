using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ActionUnequippedState : CombatState
    {
        public ActionUnequippedState(CombatStateMachine context)
            : base(context, Phase.Action) { }

        public override void OnEnter()
        {
            context.combatant.Abilities.TryUnequip();
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            UpdateFocusedTile();

            if (context.Controller.EquipTriggered())
            {
                context.SwitchState(context.actionEquippedState);
            }

            if (context.Controller.LockTargetsTriggered())
            {
                context.SwitchState(context.actionConfirmationState);
            }
        }
    }
}
