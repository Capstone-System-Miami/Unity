using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionEquipped : CombatantState
    {
        protected CombatAction combatAction;

        public ActionEquipped(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

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
                SwitchState(factory.ActionSelection());
                return;
            }

            if (SelectTile())
            {
                SwitchState(factory.ActionConfirmation(combatAction));
                return;
            }
        }

        protected abstract bool SelectTile();
        protected abstract bool Unequip();
    }
}
