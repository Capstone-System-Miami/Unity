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

            /// Add equip conditions
            canEquip.Add(() => selectedCombatAction != null);

            /// Subscribe to FocusTile events.
            combatant.FocusTileChanged += HandleFocusTileChanged;
            
            /// Update prompts
            InputPrompts = 
                "Select an action from your Loadout Menu.\n" +
                "Or press Enter/Return to end your turn.\n";
        }

        public override void Update()
        {
            base.Update();
            combatant.UpdateFocus();
        }

        public override void MakeDecision()
        {
            if (EquipRequested())
            {
                if (!canEquip.Met()) { return; }

                SwitchState(factory.ActionEquipped(selectedCombatAction));
                return;
            }
            else if (SkipPhaseRequested())
            {
                if (!canSkipPhase.Met()) {  return; }

                SwitchState(factory.TurnEnd());
                return;
            }
            else
            {
                SwitchState(factory.TurnEnd());
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            combatant.FocusTileChanged -= HandleFocusTileChanged;
        }

        // Decision
        protected abstract bool EquipRequested();
        protected abstract bool SkipPhaseRequested();

        protected virtual void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            args.previousTile?.EndHover(combatant);
            args.newTile?.BeginHover(combatant);
        }
    }
}
