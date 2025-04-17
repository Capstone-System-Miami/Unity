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
        [SerializeField] private bool perTurn;
        [SerializeField] private int durationTurns;
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
            return new DamageCommand(target, finalDamage,perTurn,durationTurns);
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
        void PreviewDamage(float amount,bool perTurn,int durationTurns);
        void ReceiveDamage(float amount,bool perTurn,int durationTurns);
    }

    public class DamageCommand : ISubactionCommand , IPerTurn
    {
        public readonly ITargetable target;
        public readonly float amount;
        public readonly bool perTurn;
        public readonly int durationTurns;
        public int RemainingTurns { get; private set; }
        public DamageCommand(ITargetable target, float amount, bool perTurn,int durationTurns)
        {
            this.target = target;
            this.amount = amount;
            this.perTurn = perTurn;
            this.durationTurns = durationTurns;
            RemainingTurns = durationTurns;
        }

        public void Preview()
        {           
            target.GetDamageInterface()?.PreviewDamage(amount, perTurn, durationTurns);
        }

        public void Execute()
        {
            target.GetDamageInterface()?.ReceiveDamage(amount,perTurn, durationTurns);
            RemainingTurns--;
        }
    }
}
