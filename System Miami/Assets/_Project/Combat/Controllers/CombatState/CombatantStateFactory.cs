using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class CombatantStateFactory
    {
        private Combatant combatant;
        private bool isPlayer;

        public CombatantStateFactory(Combatant combatant)
        {
            this.combatant = combatant;
            this.isPlayer = combatant is PlayerCombatant;
        }

        public CombatantState Idle()
        {
            return isPlayer ?
                new PlayerIdle(combatant)
                : new EnemyIdle(combatant);
        }
        public CombatantState TurnStart()
        {
            return isPlayer ?
                new PlayerTurnStart(combatant)
                : new EnemyTurnStart(combatant);
        }
        public CombatantState MovementTileSelection()
        {
            return isPlayer ?
                new PlayerMovementTileSelection(combatant)
                : new EnemyMovementTileSelection(combatant);
        }
        public CombatantState MovementConfirmation(MovementPath path)
        {
            return isPlayer ?
                new PlayerMovementConfirmation(combatant, path)
                : new EnemyMovementConfirmation(combatant, path);
        }
        public CombatantState MovementExecution(MovementPath path)
        {
            return isPlayer ?
                new PlayerMovementExecution(combatant, path)
                : new EnemyMovementExecution(combatant, path);
        }
        public CombatantState ActionSelection()
        {
            return isPlayer ?
                new PlayerActionSelection(combatant)
                : new EnemyActionSelection(combatant);
        }
        public CombatantState ActionEquipped()
        {
            return isPlayer ?
                new PlayerActionEquipped(combatant)
                : new EnemyActionEquipped(combatant);
        }
        public CombatantState ActionConfirmation()
        {
            return isPlayer ?
                new PlayerActionConfirmation(combatant)
                : new EnemyActionConfirmation(combatant);
        }
        public CombatantState ActionExecution()
        {
            return isPlayer ?
                new PlayerActionExecution(combatant)
                : new EnemyActionExecution(combatant);
        }
        public CombatantState TurnEnd()
        {
            return isPlayer ?
                new PlayerTurnEnd(combatant)
                : new EnemyTurnEnd(combatant);
        }
        public CombatantState Dying()
        {
            return isPlayer ?
                new PlayerDying(combatant)
                : new EnemyDying(combatant);
        }
        public CombatantState Dead()
        {
            return isPlayer ?
                new PlayerDead(combatant)
                : new EnemyDead(combatant);
        }
    }
}
