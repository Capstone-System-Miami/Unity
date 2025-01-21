using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ActionConfirmationState : CombatState
    {
        public ActionConfirmationState(CombatStateMachine context)
            : base(context, Phase.Action) { }

        public override void OnEnter()
        {
            context.FocusedTile?.EndHover(context.Controller);
            context.combatant.Abilities.TryLockTargets();
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            if (context.Controller.UseAbilityTriggered())
            {
                context.SwitchState(context.actionExecutingState);
            }
        }
    }
}
