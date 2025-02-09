using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    // input delegates class?
    public abstract class ActionEquipped : CombatantState
    {
        protected CombatAction combatAction;

        public ActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            /// Subscribe to FocusTile events
            combatant.FocusTileChanged += HandleFocusTileChanged;

            combatAction.Equip();

            InputPrompts =
                "Hover over a tile to aim.\n" +
                "Click to lock your targets.\n" +
                "(You will still be able to change your mind)\n";
        }

        public override void Update()
        {
            combatant.UpdateFocus();
            combatant.UpdateAnimDirection();
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
            combatant.FocusTileChanged -= HandleFocusTileChanged;
        }

        protected abstract bool SelectTileRequested();
        protected abstract bool UnequipRequested();

        protected virtual void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            args.previousTile?.EndHover(combatant);
            args.newTile?.BeginHover(combatant);
        }
    }
}
