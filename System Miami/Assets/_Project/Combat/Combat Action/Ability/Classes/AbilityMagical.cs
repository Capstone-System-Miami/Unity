using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class AbilityMagical : NewAbility
    {
        public AbilityMagical(NewAbilitySO preset, Combatant user)
            : base(
                preset,
                user,
                (amount) => user.Mana.Lose(amount))
        { }
    }
}
