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

        public override void SetTargeting()
        {
            _targeting = new Targeting(_user, this);
        }

        public override void Perform()
        {
            for (int i = 0; i < _targetCombatants.Length; i++)
            {
                if(!_targetCombatants[i].TryGetComponent(out IHealable target))
                {
                    Debug.Log($"Invalid heal target.");
                }
                else
                {
                    target.Heal(_amount);
                }
            }
        }
    }
}
