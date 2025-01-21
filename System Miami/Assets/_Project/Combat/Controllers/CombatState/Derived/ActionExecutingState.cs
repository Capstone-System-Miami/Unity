using SystemMiami.CombatSystem;
using System.Collections;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ActionExecutingState : CombatState
    {
        public ActionExecutingState(CombatStateMachine context)
            : base(context, Phase.Action) { }

        public override void OnEnter()
        {
            if (context.combatant.Abilities.AbilityExecutionIsValid
                (out IEnumerator abilityProcess))
            {
                context.StartCoroutine(abilityProcess);
            }
        }

        public override void OnExit()
        {
            context.FocusedTile?.UnHighlight(); // on phase transit
        }

        /// <summary>
        /// TODO: EFFECTS
        /// </summary>
        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
