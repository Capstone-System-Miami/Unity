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

            combatant.FocusTileChanged += HandleFocusTileChanged;

            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.RegisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.RegisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.RegisterSubactions());
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

        public override void OnExit()
        {
            base.OnExit();
            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.DeregisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.DeregisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.DeregisterSubactions());

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
