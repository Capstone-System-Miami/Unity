// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Action", menuName = "Abilities/CombatActions/Damage")]
    public class Damage : CombatSubaction
    {
        [SerializeField] private float _abilityDamage;

        public override void Perform()
        {
            List<IDamageable> finalTargets = currentTargets.GetTargetsWith<IDamageable>();

            // Loop through each combatant in the targets and apply damage.
            foreach (IDamageable target in finalTargets)
            {
                if (target == null) { continue; }
                
                if (!target.IsCurrentlyDamageable())
                {
                    return;
                }

                target.Damage(_abilityDamage);
            }
        }
    }
}
