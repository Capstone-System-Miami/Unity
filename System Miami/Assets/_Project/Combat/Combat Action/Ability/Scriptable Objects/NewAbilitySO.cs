using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Refactored Ability")]
    public class NewAbilitySO : CombatActionSO
    {
        [Space(20)]
        public AbilityType AbilityType;
        public float ResourceCost;
        public int CooldownTurns;
    }
}
