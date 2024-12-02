using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Abilities/CombatActions/Inflict Status Effect")]
    public class InflictStatusEffect : CombatAction
    {
        [SerializeField] StatSetSO effectStats;
        [SerializeField] float damage;
        [SerializeField] int durationTurns;

        public override void Perform()
        {
            StatusEffect statusEffect = new StatusEffect(effectStats, damage, durationTurns);

            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target != null)
                {
                    target.InflictStatusEffect(statusEffect);
                }
            }
        }
    }
}
