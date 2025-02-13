using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Combat Action/Consumable")]
    public class ConsumableSO : CombatActionSO
    {
        [Space(20)]
        public int Uses;

        public Data Data;
    }
}
