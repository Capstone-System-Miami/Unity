using System;
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
            /*canEquip.Add(() =>
             {
                 if (selectedCombatAction is NewAbility ability)
                 {
                     return ability.IsOnCooldown == false;
                 }
                 else if(selectedCombatAction is Consumable consumable)
                 {
                     return consumable.UsesRemaining > 0;
                 }
                 return false;
                
             });*/

            /// Subscribe to FocusTile events.
            combatant.FocusTileChanged += HandleFocusTileChanged;           
        }

        public override void MakeDecision()
        {
            if (EquipRequested())
            {
                Debug.Log("equip reqd", combatant);
                if (!canEquip.AllMet()) { return; }

                SwitchState(factory.ActionEquipped(selectedCombatAction));
                return;
            }
            else if (SkipPhaseRequested())
            {
                Debug.Log("skip phase reqd", combatant);
                if (!canSkipPhase.AllMet()) {  return; }

                SwitchState(factory.TurnEnd());
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            combatant.FocusTileChanged -= HandleFocusTileChanged;
            combatant.FocusTile?.EndHover(combatant);
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
