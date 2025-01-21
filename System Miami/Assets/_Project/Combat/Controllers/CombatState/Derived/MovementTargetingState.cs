using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    // waits for input
    public class MovementTargetingState : CombatState
    {
        public MovementTargetingState(CombatStateMachine context)
            : base(context, Phase.Movement) { }

        private bool CanMove
        {
            get
            {
                return context.combatant.Speed.Get() > 0;
            }
        }

        public override void OnEnter()
        {
            ResetTileData();
        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            UpdateFocusedTile();

            if (context.Controller.EndTurnTriggered())
            {
                context.SwitchState(context.turnEndState);
            }

            if (context.Controller.NextPhaseTriggered())
            {
                context.SwitchState(context.actionUnequippedState);
                context.Controller.ResetFlags();
            }

            if (context.Controller.BeginMovementTriggered() && CanMove)
            {
                context.SwitchState(context.movementActiveState);
            }
        }
    }
}
