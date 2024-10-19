using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "CombatAction/Status Effects")]
    public class InflictStatusEffect : CombatAction
    {
        [SerializeField] AttributeSetSO statusEffectAttributesSO;
        [SerializeField] int durationTurns;

        public override void Perform(Targets targets)
        {
            StatusEffect statusEffect = new StatusEffect(statusEffectAttributesSO, durationTurns);

            foreach (Combatant target in targets.Combatants)
            {
                if (target != null)
                {
                    target._attributes.AddStatusEffect(statusEffect);
                    
                }
            }
        }

    }
}
