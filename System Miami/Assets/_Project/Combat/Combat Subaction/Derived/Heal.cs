// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "Abilities/CombatActions/Heal")]
    public class Heal : CombatSubaction
    {
        [SerializeField] private float _amount;

        public override void Perform()
        {
            List<IHealable> targets = currentTargets.GetTargetsWith<IHealable>();
            foreach (IHealable target in targets)
            {
                if (target == null) { continue; }

                target.Heal(_amount);
            }
        }
    }
}
