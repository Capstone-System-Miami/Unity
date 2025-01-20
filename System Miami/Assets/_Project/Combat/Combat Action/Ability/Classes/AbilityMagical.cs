using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class AbilityMagical : NewAbility
    {
        public AbilityMagical(NewAbilitySO preset, Combatant user)
            : base(
                  preset,
                  user,
                  user.Mana)
        { }
    }
}
