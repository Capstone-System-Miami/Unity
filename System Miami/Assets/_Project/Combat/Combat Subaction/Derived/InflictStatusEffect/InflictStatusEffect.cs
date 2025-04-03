using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect Subaction", menuName = "Combat Subaction/Inflict Status Effect")]
    public class InflictStatusEffect : CombatSubactionSO
    {
        [SerializeField] StatSetSO effectStats;
     
        float damagePerTurn;
        float healPerTurn;
        [SerializeField] int durationTurns;

        public override ISubactionCommand GenerateCommand(ITargetable target, CombatAction action)
        {
            return new StatusEffectCommand(
                target,
                new StatSet(effectStats),
                damagePerTurn,
                healPerTurn,
                durationTurns);
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="InflictStatusEffect"/> to be performed
    /// on an object.
    /// </summary>
    public interface IStatusEffectReceiver
    {
        bool IsCurrentlyStatusEffectable();
        void PreviewEffect(
            StatSet statMod,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns);
        void ReceiveEffect(
            StatSet statMod,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns);
    }

    public class StatusEffectCommand : ISubactionCommand
    {
        public readonly ITargetable target;
        public readonly StatSet effect;
        public readonly float damagePerTurn;
        public readonly float healPerTurn;
        public readonly int durationTurns;

        public StatusEffectCommand(
            ITargetable target,
            StatSet effect,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns)
        {
            this.target = target;
            this.effect = effect;
            this.damagePerTurn = damagePerTurn;
            this.healPerTurn = healPerTurn;
            this.durationTurns = durationTurns;
        }

        public void Preview()
        {
            target.GetStatusEffectInterface()?.PreviewEffect(
                effect, damagePerTurn, healPerTurn, durationTurns);
        }

        public void Execute()
        {
            target.GetStatusEffectInterface()?.ReceiveEffect(
                effect, damagePerTurn, healPerTurn, durationTurns);
        }
    }
}
