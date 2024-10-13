// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Action", menuName = "CombatAction/Damage")]
    public class Damage : CombatAction
    {
        [SerializeField] private float _damage;

        public override void PerformOn(GameObject target)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(_damage);
            }
        }
    }

}
