// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "CombatAction/Heal")]
    public class Heal : CombatAction
    {
        [SerializeField] private float _amount;

        public override void PerformOn(GameObject target)
        {
            if (target.TryGetComponent(out IHealable healable))
            {
                healable.Heal(_amount);
            }
        }
    }
}
