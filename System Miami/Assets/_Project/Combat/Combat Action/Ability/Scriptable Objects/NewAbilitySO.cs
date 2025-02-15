using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Combat Action/Ability")]
    public class NewAbilitySO : CombatActionSO
    {
        [Space(20)]
        public AbilityType AbilityType;
        public float ResourceCost;
        public int CooldownTurns;
        public Data Data;
    }
}
