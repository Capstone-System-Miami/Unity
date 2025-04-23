using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    // TODO:
    // LOOK HERE THE NEXT TIME WE HIT A WEIRD THING WHERE
    // COMBATANT SEEMS LIKE IT THINKS THAT SELF IS NULL

    #nullable enable
    public class CombatantStateFactory
    {
        private Combatant combatant;
        private PlayerCombatant? playerCombatant;
        private EnemyCombatant? enemyCombatant;
        private bool isPlayer;

        public CombatantStateFactory(Combatant combatant)
        {
            this.combatant = combatant;
            this.isPlayer = combatant is PlayerCombatant;

            playerCombatant = isPlayer ? combatant as PlayerCombatant : null;
            enemyCombatant = !isPlayer ? combatant as EnemyCombatant : null;
        }

        public CombatantState Idle()
        {
            return isPlayer ?
                new PlayerIdle(playerCombatant)
                : new EnemyIdle(enemyCombatant);
        }
        public CombatantState TurnStart()
        {
            return isPlayer ?
                new PlayerTurnStart(playerCombatant)
                : new EnemyTurnStart(enemyCombatant);
        }
        public CombatantState MovementTileSelection()
        {
            return isPlayer ?
                new PlayerMovementTileSelection(playerCombatant)
                : new EnemyMovementTileSelection(enemyCombatant);
        }
        public CombatantState MovementConfirmation(MovementPath path)
        {
            return isPlayer ?
                new PlayerMovementConfirmation(playerCombatant, path)
                : new EnemyMovementConfirmation(enemyCombatant, path);
        }
        public CombatantState MovementExecution(MovementPath path)
        {
            return new MovementExecution(combatant, path);
        }

        public CombatantState ForcedMovementExecution(MovementPath path)
        {
            return new ForcedMovementExecution(combatant, path);
        }
        public CombatantState ActionSelection()
        {
            return isPlayer ?
                new PlayerActionSelection(playerCombatant)
                : new EnemyActionSelection(enemyCombatant);
        }
        public CombatantState ActionEquipped(CombatAction combatAction)
        {
            return isPlayer ?
                new PlayerActionEquipped(playerCombatant, combatAction)
                : new EnemyActionEquipped(enemyCombatant, combatAction);
        }
        public CombatantState ActionConfirmation(CombatAction combatAction)
        {
            return isPlayer ?
                new PlayerActionConfirmation(playerCombatant, combatAction)
                : new EnemyActionConfirmation(enemyCombatant, combatAction);
        }
        public CombatantState ActionExecution(CombatAction combatAction)
        {
            return new ActionExecution(combatant, combatAction);
        }
        public CombatantState TurnEnd()
        {
            return isPlayer ?
                new PlayerTurnEnd(playerCombatant)
                : new EnemyTurnEnd(enemyCombatant);
        }
        public CombatantState Dying()
        {
            return isPlayer ?
                new PlayerDying(playerCombatant)
                : new EnemyDying(enemyCombatant);
        }
        public CombatantState Dead()
        {
            return isPlayer ?
                new PlayerDead(playerCombatant)
                : new EnemyDead(enemyCombatant);
        }
    }
    #nullable disable
}
