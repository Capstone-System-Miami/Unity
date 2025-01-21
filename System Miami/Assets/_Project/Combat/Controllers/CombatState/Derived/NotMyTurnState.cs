using SystemMiami.CombatSystem;
using UnityEngine;
using SystemMiami.CombatRefactor;

namespace SystemMiami
{
    public class NotMyTurnState : CombatState
    {
        public NotMyTurnState(CombatStateMachine context)
            : base(context, Phase.Movement) { }

        public override void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            context.Controller.ResetFlags();
        }

        public override void LateUpdate()
        {
            context.Controller.ResetFlags();
        }
    }
}
