// Authors: Layla Hoey, Daylan Pain
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "Abilities/CombatActions/Heal")]
    public class Heal : CombatAction
    {
        [SerializeField] private float _amount; // Flat healing amount
        [SerializeField] private bool _usePercentage; // Toggle for percentage healing
        [SerializeField, Range(0f, 100f)] private float _percentageAmount; // Percentage value for healing
        [SerializeField] private bool _isHealing; // Toggle to determine if we are healing or damaging

        public override void Perform()
        {
            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target == null) { continue; }

                if (_usePercentage)
                {
                    if (_isHealing)
                    {
                        target.HealPercentage(_percentageAmount);//Heal Percentage
                    }
                    else
                    {
                        target.ReducePercentage(_percentageAmount); // Reduce health by percentage
                    }
                }
                else
                {
                    if (_isHealing)
                    {
                        target.Heal(_amount); // Heal by flat amount
                    }
                    else
                    {
                        target.Damage(_amount); // Deal flat damage
                    }
                }
            }
        }
    }
}
