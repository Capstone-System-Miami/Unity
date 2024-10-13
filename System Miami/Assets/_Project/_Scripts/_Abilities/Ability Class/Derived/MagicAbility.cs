// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    [CreateAssetMenu(fileName = "New Magical Ability", menuName = "Abilities/Ability/Magical")]
    public class MagicAbility : Ability
    {
        private void Awake()
        {
            _type = AbilityType.MAGICAL;
            _requiredResource = ResourceType.MANA;
        }
    }
}
