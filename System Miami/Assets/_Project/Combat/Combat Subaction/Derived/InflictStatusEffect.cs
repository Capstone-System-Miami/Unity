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
        [SerializeField] float damage;
        [SerializeField] int durationTurns;

        public override void Perform()
        {
            List<Combatant> finalTargets = new();
            foreach (ITargetable target in currentTargets.all)
            {
                if (target is Combatant c)
                {
                    finalTargets.Add(c);
                }
            }
            StatusEffect statusEffect = new StatusEffect(effectStats, damage, durationTurns);

            foreach (Combatant target in finalTargets)
            {
                if (target != null)
                {
                    target.InflictStatusEffect(statusEffect);
                }
            }
        }
    }
}
