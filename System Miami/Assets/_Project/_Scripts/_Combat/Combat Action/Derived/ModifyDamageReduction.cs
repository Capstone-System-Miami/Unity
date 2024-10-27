// Authors: Daylan Pain
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Modifier", menuName = "Abilities/CombatActions/Modify Damage")]
    public class ModifyDamageReduction : CombatAction
    {
        //[SerializeField] private AttributeSetSO damageReductionAttributesSO; OLD VERSION not in use anymore. Will Clean up
        [SerializeField] private int durationTurns;
        [SerializeField] private float damageModificationPercentage; // Positive percentage for both increase and reduction
        [SerializeField] private bool isIncrease; // Toggle to increase damage received, leave off to reduce incoming damage

        public override void Perform()
        {
            float modificationDecimal = damageModificationPercentage / 100f;

            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target != null)
                {
                    Stats targetStats = target.GetComponent<Stats>();
                    if (targetStats != null)
                    {
                        targetStats.SetDamageModificationPercentage(modificationDecimal, isIncrease);
                    }
                }
            }
        }
    }
}
