using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionEquipped : CombatantState
    {
        public ActionEquipped(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void Update()
        {
            /// if direction is not same as prev frame, update combatant
            /// vs
            /// if tile changes update combatant
        }

        public override void MakeDecision()
        {
            if (Unequip())
            {
                GoToActionSelection();
                return;
            }

            if (SelectTile())
            {
                GoToActionTileConfirmation();
                return;
            }
        }

        protected abstract bool SelectTile();
        protected abstract bool Unequip();


        protected abstract void GoToActionTileConfirmation();
        protected abstract void GoToActionSelection();
    }
}
