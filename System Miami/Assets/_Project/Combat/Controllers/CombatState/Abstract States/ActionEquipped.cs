using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    // input delegates class?
    public abstract class ActionEquipped : CombatantState
    {
        protected CombatAction combatAction;

        OverlayTile positionTile;
        OverlayTile focusTile;

        DirectionContext previousDirection;
        DirectionContext currentDirection;

        public ActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            previousDirection = combatant.CurrentDirectionContext;
            currentDirection = combatant.CurrentDirectionContext;
        }

        public override void Update()
        {
            positionTile = combatant.CurrentTile;
            if (TryGetFocus(out focusTile))
            {
                if (focusTile == null) { return; }
            }

            previousDirection = currentDirection;
            currentDirection = new(
                (Vector2Int)positionTile.GridLocation,
                (Vector2Int)focusTile.GridLocation);

            bool directionChanged
                = previousDirection.BoardDirection
                != currentDirection.BoardDirection;

            combatant.CurrentDirectionContext = currentDirection;

            combatAction.UpdateTargets(currentDirection, directionChanged);
        }

        public override void MakeDecision()
        {
            if (UnequipRequested())
            {
                SwitchState(factory.ActionSelection());
                return;
            }

            if (SelectTileRequested())
            {
                SwitchState(factory.ActionConfirmation(combatAction));
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            combatAction.Unequip();
        }

        protected abstract bool SelectTileRequested();
        protected abstract bool UnequipRequested();

        private bool TryGetFocus(out OverlayTile newFocus)
        {
            newFocus = combatant.GetNewFocus() ?? combatant.GetDefaultFocus();

            return focusTile != newFocus;
        }
    }
}
