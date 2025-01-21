using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ActionEquippedState : CombatState
    {
        public ActionEquippedState(CombatStateMachine context)
            : base(context, Phase.Action) { }

        public override void OnEnter()
        {
            context.combatant.Abilities.TryEquip(context.TypeToEquip, context.IndexToEquip);
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            UpdateFocusedTile();

            if (context.Controller.UnequipTriggered())
            {
                context.SwitchState(context.actionUnequippedState);
            }

            if (context.Controller.LockTargetsTriggered())
            {
                context.SwitchState(context.actionConfirmationState);
            }
        }
    }
}
