using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class TurnEnd : CombatantState
    {
        public TurnEnd(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            base.OnEnter();

            InputPrompts = 
                $"Turn over.\n" +
                $"Press Enter/Return to continue.";
        }

        public override void Update()
        {
            base.Update();
        }

        public override void MakeDecision()
        {
            if (Proceed())
            {
                SwitchState(factory.Idle());
                return;
            }
        }

        public override void OnExit()
        {
            combatant.IsMyTurn = false;
            Debug.Log($"{combatant.name} health is {combatant.Health.Get()} at the end of its turn.");
        }

        // Decision
        protected abstract bool Proceed();
    }
}
