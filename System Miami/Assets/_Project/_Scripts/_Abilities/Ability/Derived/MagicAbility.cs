using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    [CreateAssetMenu(fileName = "New Magical Ability", menuName = "Ability/Magical")]
    public class MagicAbility : Ability
    {
        private void Awake()
        {
            _type = AbilityType.MAGICAL;
            _requiredResource = ResourceType.MANA;
        }
    }
}
