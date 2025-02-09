using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Abilities/CombatActions/Inflict Status Effect")]
    public class InflictStatusEffect : CombatSubactionSO
    {
        [SerializeField] StatSetSO effectStats;
        [SerializeField] float damagePerTurn;
        [SerializeField] float healPerTurn;
        [SerializeField] int durationTurns;

        protected override ISubactionCommand GenerateCommand(ITargetable target)
        {
            IStatusEffectReceiver statusEffectReceiver;
            if (!target.TryGetStatusEffectInterface(out statusEffectReceiver))
            {
                Debug.LogWarning(
                    $"Generating a command for {target}, " +
                    $"no status effect interface returned");
                return null;
            }

            return new StatusEffectCommand(
                statusEffectReceiver,
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
        public readonly IStatusEffectReceiver receiver;
        public readonly StatSet effect;
        public readonly float damagePerTurn;
        public readonly float healPerTurn;
        public readonly int durationTurns;

        public StatusEffectCommand(
            IStatusEffectReceiver statusEffectReceiver,
            StatSet effect,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns)
        {
            this.receiver = statusEffectReceiver;
            this.effect = effect;
            this.damagePerTurn = damagePerTurn;
            this.healPerTurn = healPerTurn;
            this.durationTurns = durationTurns;
        }

        public void Execute()
        {

        }

        public void Preview()
        {

        }
    }
}
