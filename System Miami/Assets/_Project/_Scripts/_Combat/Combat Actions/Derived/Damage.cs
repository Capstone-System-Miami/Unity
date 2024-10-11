// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Action", menuName = "CombatAction/Damage")]
    public class Damage : CombatAction
    {
        [SerializeField] private float _damage;

        public override void PerformOn(Combatant[] targets)
        {
            foreach (Combatant target in targets)
            {
                target?.TakeDamage(_damage);
            }
        }
    }

}
