// Authors: Layla Hoey

using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "New Damage Subaction",
        menuName = "Combat Subaction/Damage")]
    public class Damage : CombatSubactionSO
    {
        [SerializeField] private float damageToDeal;

        public override ISubactionCommand GenerateCommand(ITargetable target,CombatAction action)
        {
            float power = 0;
            if (action is AbilityPhysical)
            {
                power = action.User.Stats.GetStat(StatType.PHYSICAL_PWR);
            }
            else if (action is AbilityMagical)
            {
                power = action.User.Stats.GetStat(StatType.MAGICAL_PWR);
            }

            float finalDamage = damageToDeal + power;
            return new DamageCommand(target, finalDamage);
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="Damage"/> to be performed
    /// on an object.
    /// </summary>
    public interface IDamageReceiver
    {
        bool IsCurrentlyDamageable();
        void PreviewDamage(float amount);
        void ReceiveDamage(float amount);
    }

    public class DamageCommand : ISubactionCommand
    {
        public readonly ITargetable target;
        public readonly float amount;

        public DamageCommand(ITargetable target, float amount)
        {
            this.target = target;
            this.amount = amount ;
        }

        public void Preview()
        {           
            target.GetDamageInterface()?.PreviewDamage(amount);
        }

        public void Execute()
        {
            target.GetDamageInterface()?.ReceiveDamage(amount);
        }
    }
}
