using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class MovementConfirmationState : CombatState
    {
        public MovementConfirmationState(CombatStateMachine context)
            : base(context, Phase.Movement) { }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
            UpdateFocusedTile();
        }
    }
}
