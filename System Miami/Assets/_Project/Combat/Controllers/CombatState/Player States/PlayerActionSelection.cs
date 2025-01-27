using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();
            combatant.Physical.Add(new AbilityPhysical((combatant as PlayerCombatant).test, combatant));
        }

        protected override bool EquipRequested()
        {
            // Event from UI?
            return false;
        }

        protected override bool SkipPhaseRequested()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }
    }
}
