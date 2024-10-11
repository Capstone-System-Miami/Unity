// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "CombatAction/Heal")]
    public class Heal : CombatAction
    {
        [SerializeField] private float _amount;

        public override void PerformOn(Combatant[] targets)
        {
            foreach (Combatant target in targets)
            {
                target?.Heal(_amount);
            }
        }
    }
}
