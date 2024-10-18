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
            AttributeSet statusEffectAttributes = new AttributeSet(statusEffectAttributesSO);

            foreach (Combatant target in targets.Combatants)
            {
                if (target != null)
                {
                    target._attributes.AddStatusEffect(statusEffectAttributes, durationTurns);
                    Debug.Log($"{target.name} received a status effect for {durationTurns} turns.");
                }
            }
        }

    }
}
