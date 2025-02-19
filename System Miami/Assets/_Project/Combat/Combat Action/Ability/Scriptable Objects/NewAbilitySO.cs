using SystemMiami.AbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace SystemMiami.CombatRefactor
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Combat Action/Ability")]
    public class NewAbilitySO : CombatActionSO 
    {
        [Space(20)]
        public AbilityType AbilityType;
        public float ResourceCost;
        public int CooldownTurns;
        [FormerlySerializedAs("Data")] public ItemData itemData;

        
    }
}
