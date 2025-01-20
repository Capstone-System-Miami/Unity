using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Items/Consumable")]
    public class ConsumableSO : CombatActionSO
    {
        [Space(20)]
        public int Uses;
    }
}
