using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        private const float selectionTimeout = 2f;
        private CountdownTimer aiTimeout;

        public EnemyMovementTileSelection(EnemyCombatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            InputPrompts = 
                $"{combatant.gameObject.name} is selecting a tile...";

            aiTimeout = new(combatant, selectionTimeout);

            aiTimeout.Start();
        }

        public override void MakeDecision()
        {
            base.MakeDecision();

            if (aiTimeout.IsFinished)
            {
                SwitchState(factory.ActionSelection());
            }
        }

        // Decision
        protected override bool TurnEndRequested()
        {
            return false;
        }

        protected override bool SkipMovementRequested()
        {
            // Are the coordinates of what the enemy is focusing
            // on the same as the coordinates right in front of them
            return (
                combatant.CurrentDirectionContext.ForwardA
                == combatant.FocusTile.BoardPos
                );
        }

        protected override bool ConfirmPathRequested()
        {
            return true;
        }
    }
}
