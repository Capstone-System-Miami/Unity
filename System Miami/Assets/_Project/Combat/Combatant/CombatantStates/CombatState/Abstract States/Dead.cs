using SystemMiami.CombatSystem;
using SystemMiami.Management;
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
            GAME.MGR.CombatantDeath?.Invoke(combatant);
            
            // Player died
            Debug.Log($"{combatant.name} has died.");
            MonoBehaviour.Destroy(combatant.gameObject,0.06f);
        }
    }
}
