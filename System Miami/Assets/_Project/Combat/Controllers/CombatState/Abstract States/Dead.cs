using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class Dead : CombatantState
    {
        protected Dead(Combatant combatant)
            : base(combatant, Phase.None) { }

        public override void OnEnter()
        {
            base.OnEnter();

            // Player died
            Debug.Log($"{combatant.name} has died.");
            MonoBehaviour.Destroy(combatant.gameObject);
        }
    }
}
