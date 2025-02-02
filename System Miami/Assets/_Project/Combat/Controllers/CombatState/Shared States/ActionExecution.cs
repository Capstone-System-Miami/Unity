using System.Collections;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class ActionExecution : CombatantState
    {
        CombatAction combatAction;

        public ActionExecution(Combatant combatant, CombatAction combatAction)
            : base(combatant, Phase.Action)
        {
            this.combatAction = combatAction;
        }

        public override void OnEnter()
        {
            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.RegisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.RegisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.RegisterSubactions());

            combatant.StartCoroutine(combatAction.Execute());
        }

        public override void MakeDecision()
        {
            SwitchState(factory.TurnEnd());
            return;
        }

        public override void OnExit()
        {
            base.OnExit();
            combatant.Loadout.PhysicalAbilities.ForEach(phys => phys.DeregisterSubactions());
            combatant.Loadout.MagicalAbilities.ForEach(mag => mag.DeregisterSubactions());
            combatant.Loadout.Consumables.ForEach(cons => cons.DeregisterSubactions());
        }
    }
}
