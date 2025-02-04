using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Abilities/CombatActions/Inflict Percent Status Effect")]
    public class InflictPercentStatusEffect : CombatSubaction
    {
        [SerializeField] StatSetSO effectStats;
        [SerializeField] float damage;
        [SerializeField] int durationTurns;
        public override void Perform()
        {
            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                StatusEffect statusEffect = new StatusEffect(effectStats, damage, durationTurns);
                target.Stats.AddStatusEffect(statusEffect);
                if (target != null)
                {
                    target.InflictStatusEffect(statusEffect);
                }
            }
        }
    }
}
