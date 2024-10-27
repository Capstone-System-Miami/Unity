// Authors: Daylan Pain
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Mana Action", menuName = "Abilities/CombatActions/ModifyMana")]
    public class ManaModification : CombatAction
    {
        [SerializeField, Range(0f, 100f)] private float _percentageAmount; // Percentage value for mana modification
        [SerializeField] private bool _isIncreasing; // Toggle to determine if we are increasing or decreasing mana

        public override void Perform()
        {
            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target == null) { continue; }

                // Call the method to modify mana based on percentage and toggle
                target.ModifyManaPercentage(_percentageAmount, _isIncreasing);
            }
        }
    }
}
