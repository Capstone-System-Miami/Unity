using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    // input delegates class?
    public abstract class ActionEquipped : CombatantState
    {
        protected CombatAction selectedCombatAction;
        protected CombatAction requestedReEquip;

        protected Conditions canSelectTile = new();
        protected Conditions canReEquip = new();

        public ActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.selectedCombatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            /// Directly calling GetNewFocus, rather than
            /// reading the event. This way we can know if 
            /// the mouse is actually over the board or not.
            canSelectTile.Add( () => combatant.GetNewFocus() != null);

            canReEquip.Add( () => selectedCombatAction != null);
            canReEquip.Add( () => requestedReEquip != null);

            /// Subscribe to FocusTile events
            combatant.FocusTileChanged += HandleFocusTileChanged;

            selectedCombatAction.Equip();
        }

        public override void Update()
        {
            base.Update();
            combatant.UpdateFocus();
            combatant.UpdateAnimDirection();

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                selectedCombatAction.Reportback();
            }
        }

        public override void MakeDecision()
        {
            if (SelectTileRequested())
            {
                if (canSelectTile.AllMet())
                {
                    SwitchState(factory.ActionConfirmation(selectedCombatAction));
                    return;
                }
            }

            if (UnequipRequested())
            {
                SwitchState(factory.ActionSelection());
                return;
            }

            if (ReEquipRequested())
            {
                if (canReEquip.AllMet())
                {
                    SwitchState(factory.ActionEquipped(requestedReEquip));
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            selectedCombatAction.Unequip();
            combatant.FocusTileChanged -= HandleFocusTileChanged;
        }

        protected abstract bool ReEquipRequested();
        protected abstract bool SelectTileRequested();
        protected abstract bool UnequipRequested();

        protected virtual void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            //args.previousTile?.EndHover(combatant);
            //args.newTile?.BeginHover(combatant);
        }
    }
}
