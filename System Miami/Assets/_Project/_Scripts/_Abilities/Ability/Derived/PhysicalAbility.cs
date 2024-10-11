using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    [CreateAssetMenu(fileName = "New Physical Ability", menuName = "Ability/Physical")]
    public class PhysicalAbility : Ability
    {
        private void Awake()
        {
            _type = AbilityType.PHYSICAL;
            _requiredResource = ResourceType.STAMINA;
        }
    }
}
