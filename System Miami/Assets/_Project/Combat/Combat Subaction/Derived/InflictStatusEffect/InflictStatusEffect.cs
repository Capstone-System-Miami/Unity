using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Abilities/CombatActions/Inflict Status Effect")]
    public class InflictStatusEffect : CombatSubaction
    {
        [SerializeField] StatSetSO effectStats;
        [SerializeField] float damagePerTurn;
        [SerializeField] float healPerTurn;
        [SerializeField] int durationTurns;

        public void Perform()
        {
            List<Combatant> finalTargets = new();
            foreach (ITargetable target in currentTargets.all)
            {
                if (target is Combatant c)
                {
                    finalTargets.Add(c);
                }
            }
            StatusEffect statusEffect = new StatusEffect(effectStats, damagePerTurn, durationTurns);

            foreach (Combatant target in finalTargets)
            {
                if (target != null)
                {
                    target.InflictStatusEffect(statusEffect);
                }
            }
        }

        protected override ISubactionCommand GenerateCommand(ITargetable t)
        {
            return null;
        }
    }

    public class StatusEffectData : ISubactionCommand
    {
        public readonly StatSet effect;
        public readonly float damagePerTurn;
        public readonly float healPerTurn;
        public readonly int durationTurns;

        public StatusEffectData(StatSet effect,
            float damagePerTurn,
            float healPerTurn,
            int durationTurns)
        {
            this.effect = effect;
            this.damagePerTurn = damagePerTurn;
            this.healPerTurn = healPerTurn;
            this.durationTurns = durationTurns;
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Preview()
        {
            throw new System.NotImplementedException();
        }
    }
}
