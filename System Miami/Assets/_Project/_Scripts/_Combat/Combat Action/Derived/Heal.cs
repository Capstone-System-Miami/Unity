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

        public override void Perform(Targets targets)
        {
            for (int i = 0; i < targets.Combatants.Length; i++)
            {
                if(!targets.Combatants[i].TryGetComponent(out IHealable target))
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
