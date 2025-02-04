using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class AbilityPhysical : NewAbility
    {
        public AbilityPhysical(NewAbilitySO preset, Combatant user)
            : base(
                  preset,
                  user,
                  user.Stamina)
        { }
    }
}
