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
                if (!target.TryGetDamageable(out IDamageable damageTarget))
                {
                    if (damageTarget.IsCurrentlyDamageable())
                    {
                        return;
                    }
                    damageTarget.Damage(_abilityDamage);
                }
            }
        }
    }
}

namespace SystemMiami
{
    public interface IDamageable
    {
        bool IsCurrentlyDamageable();
        void Damage(float amount);
    }
}
