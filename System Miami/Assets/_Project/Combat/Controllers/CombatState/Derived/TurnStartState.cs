using SystemMiami.CombatSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    public class TurnStartState : CombatState
    {
        public TurnStartState(CombatStateMachine context)
            : base(context, Phase.None) { }

        public override void OnEnter()
        {
            Debug.Log($"{context.name} starting turn");
            context.combatant.ResetTurn();
            ResetTileData();           

            /// Immediately transition
            /// to movement targeting.
            context.SwitchState(context.movementTargetingState);
        }

        public override void OnExit()
        {

        }

        public override void Update()
        {

        }
    }
}
