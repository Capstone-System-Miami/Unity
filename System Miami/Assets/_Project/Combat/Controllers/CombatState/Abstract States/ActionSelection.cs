using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionSelection : CombatantState
    {
        private OverlayTile highlightOnlyFocusTile;

        protected CombatAction selectedCombatAction;

        protected Conditions canEquip = new();
        protected Conditions canSkipPhase = new();


        public ActionSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void OnEnter()
        {
            base.OnEnter();
            canEquip.Add(() => selectedCombatAction != null);
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
            if (canEquip.Met())
            {
                if (!EquipRequested()) { return; }

                SwitchState(factory.ActionEquipped(selectedCombatAction));
                return;
            }
            else if (canSkipPhase.Met())
            {
                if (!SkipPhaseRequested()) {  return; }

                SwitchState(factory.TurnEnd());
                return;
            }
            else
            {
                SwitchState(factory.TurnEnd());
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
