using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionSelection : CombatantState
    {
        private OverlayTile highlightOnlyFocusTile;

        protected CombatAction selectedCombatAction;

        public ActionSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();

            // Find a new possible focus tile
            // by the means described
            // by int the derived classes.
            if (!TryGetNewFocus(out OverlayTile newFocus))
            {
                // Focus was not new.
                // Nothing to update.
                return;
            }

            // Update currentTile & tile hover
            highlightOnlyFocusTile?.EndHover(combatant);
            highlightOnlyFocusTile = newFocus;
            highlightOnlyFocusTile?.BeginHover(combatant);
        }

        public override void MakeDecision()
        {
            if (EquipRequested())
            {
                SwitchState(factory.ActionEquipped(selectedCombatAction));
                return;
            }

            if (SkipPhaseRequested())
            {
                SwitchState(factory.TurnEnd());
                return;
            }
        }

        // Decision
        protected abstract bool EquipRequested();
        protected abstract bool SkipPhaseRequested();

        // Focus Tile
        protected bool TryGetNewFocus(out OverlayTile newFocus)
        {
            newFocus = combatant.GetNewFocus() ?? combatant.GetDefaultFocus();

            return newFocus != highlightOnlyFocusTile;
        }
    }
}
