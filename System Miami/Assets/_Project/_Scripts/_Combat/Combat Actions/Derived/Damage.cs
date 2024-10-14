// Authors: Layla Hoey
using UnityEngine;
using SystemMiami.AbilitySystem;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Action", menuName = "CombatAction/Damage")]
    public class Damage : CombatAction
    {
        [SerializeField] private float _abilityDamage;
        public override void PerformOn(GameObject target)
        {
        }

        public override void PerformOn(GameObject me, GameObject target)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                Stats stats = me.GetComponent<Stats>();

                float addToDamage = _type switch
                {
                    AbilityType.PHYSICAL => stats.GetStat(StatType.PHYSICAL_PWR),
                    AbilityType.MAGICAL => stats.GetStat(StatType.MAGICAL_PWR),
                    _                   => stats.GetStat(StatType.PHYSICAL_PWR)
                };

                float damageToDeal = _abilityDamage + addToDamage;
                damageable.Damage(_abilityDamage);
            }
        }
    }

}
