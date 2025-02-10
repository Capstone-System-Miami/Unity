using System.Collections.Generic;
using SystemMiami.CombatSystem;
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
            return false;
        }

        protected override bool ConfirmPathRequested()
        {
            return true;
        }
    }
}
