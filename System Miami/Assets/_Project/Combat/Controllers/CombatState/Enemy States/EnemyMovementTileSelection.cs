using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyMovementTileSelection : MovementTileSelection
    {
        public EnemyMovementTileSelection(Combatant combatant)
            : base(combatant) { }

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
