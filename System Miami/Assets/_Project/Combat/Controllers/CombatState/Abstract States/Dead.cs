using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class Dead : CombatantState
    {
        protected Dead(Combatant combatant)
            : base(combatant, Phase.None) { }
    }
}
