using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionConfirmation : CombatantState
    {
        protected CombatAction combatAction;

        public ActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.RegisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.RegisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.RegisterSubactions());

            combatAction.BeginConfirmingTargets();
        }

        public override void MakeDecision()
        {
            if (ConfirmSelection())
            {
                SwitchState(factory.ActionExecution(combatAction));
                return;
            }

            if (CancelConfirmation())
            {
                SwitchState(factory.ActionEquipped(combatAction));
                return;
            }


            // Should this be checking 'input'
            // storing a requested state transition (if any),
            // 
            // Then there's another function that checks
            // if there is request for a state transition,
            // which checks the conditions for the
            // requested transition?
        }

        public override void OnExit()
        {
            base.OnExit();

            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.DeregisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.DeregisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.DeregisterSubactions());
        }

        // Decision
        protected abstract bool ConfirmSelection();
        protected abstract bool CancelConfirmation();
    }
}
