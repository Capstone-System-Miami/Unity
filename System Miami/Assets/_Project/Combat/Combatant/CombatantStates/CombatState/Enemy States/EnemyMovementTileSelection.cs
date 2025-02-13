using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        public EnemyMovementTileSelection(EnemyCombatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            InputPrompts = 
                $"{combatant.gameObject.name} is selecting a tile...";
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
                combatant.CurrentDirectionContext.TilePositionB
                == combatant.CurrentDirectionContext.ForwardA
                );
        }

        protected override bool ConfirmPathRequested()
        {
            return true;
        }
    }
}
