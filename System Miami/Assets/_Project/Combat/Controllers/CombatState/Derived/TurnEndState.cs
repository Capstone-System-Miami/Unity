using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class TurnEndState : CombatState
    {
        public TurnEndState(CombatStateMachine context)
            : base(context, Phase.None) { }

        public override void OnEnter()
        {
            Debug.Log($"{context.name}Calling end of turn");

            context.FocusedTile?.UnHighlight();

            context.Controller.ResetFlags();
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
