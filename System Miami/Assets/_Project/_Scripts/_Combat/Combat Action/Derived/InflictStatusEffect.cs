using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Abilities/CombatActions/Inflict Status Effect")]
    public class InflictStatusEffect : CombatAction
    {
        [SerializeField] AttributeSetSO statusEffectAttributesSO;
        [SerializeField] int durationTurns;

        public override void Perform()
        {
            StatusEffect statusEffect = new StatusEffect(statusEffectAttributesSO, durationTurns);

            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target != null)
                {
                    target.Attributes.AddStatusEffect(statusEffect);
                }
            }
        }

    }
}
