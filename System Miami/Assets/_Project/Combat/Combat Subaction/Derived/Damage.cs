// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
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
            foreach (ITargetable target in currentTargets.all)
            {
                if (!target.TryGetDamageable(out IDamageReciever damageTarget))
                {
                    if (damageTarget.IsCurrentlyDamageable())
                    {
                        return;
                    }
                    damageTarget.RecieveDamageAmount(_abilityDamage);
                }
            }
        }
    }


    /// <summary>
    /// This is the interface needed for
    /// <see cref="Damage"/> to be performed
    /// on an object.
    /// </summary>
    public interface IDamageReciever
    {
        bool IsCurrentlyDamageable();
        void RecieveDamageAmount(float amount);
        void RecieveDamagePercent(float percent, bool ofMax);
    }
}
