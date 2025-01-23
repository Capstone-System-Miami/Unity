using SystemMiami.CombatSystem;
using System.Collections;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionExecution : CombatantState
    {
        public ActionExecution(CombatantStateMachine context)
            : base(context, Phase.Action) { }

        public override void aOnEnter()
        {
            if (machine.combatant.Abilities.AbilityExecutionIsValid
                (out IEnumerator abilityProcess))
            {
                machine.StartCoroutine(abilityProcess);
            }
        }

        public override void eOnExit()
        {
            machine.FocusedTile?.UnHighlight(); // on phase transit
        }

        /// <summary>
        /// TODO: EFFECTS
        /// </summary>
        public override void bUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
