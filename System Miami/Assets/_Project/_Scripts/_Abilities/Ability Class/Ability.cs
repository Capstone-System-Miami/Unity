// Authors: Layla Hoey
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, chaotic, and partially tested
    // Depends on the creation & refactoring of other scripts.
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField, Tooltip("Determines which type of slot this ability will occupy. Also determines the resource that will be used when the ability is executed.")]
        private AbilityType _type;

        /// <summary>
        /// Determines which Resource to consume when the ability is executed. Determined by the AbilityType.
        /// </summary>
        private ResourceType _requiredResource;
        
        [SerializeField, Tooltip("The amount of the required resource that will be consumed upon the ability being used once.")]
        private float _resourceCost;


        [Header("Actions"), Space(5)]
        [SerializeField, Tooltip("An array of combat actions. The order of the array determines the execution order, as well as the order that the actions find and gather targets.")]
        private CombatAction[] _actions;

        [Header("Animation")]
        [SerializeField, Tooltip("The animation controller to override the combatants when they perform this ability.")]
        private AnimatorOverrideController _overrideController;

        private Combatant _user;

        private void Awake()
        {
            // Assigns the _requiredResource based on _type
            _requiredResource = _type switch
            {
                AbilityType.PHYSICAL    => ResourceType.STAMINA,
                AbilityType.MAGICAL     => ResourceType.MANA,
                _                       => ResourceType.STAMINA,
            };  
        }

        public void Init(Combatant user)
        {
            _user = user;

            // I don't know how the animator override controller works
            // but that stuff would (or could?) go here.

            foreach(CombatAction action in _actions)
            {
                action.Init(user);
            }
        }

        public void UpdateTargets()
        {
            Debug.Log($"{name} trying to update targets");
            foreach (CombatAction action in _actions)
            {
                action.UpdateTargets();
            }
        }

        public void ShowTargets()
        {
            foreach(CombatAction action in _actions)
            {
                action.ShowTargets();
            }
        }

        public void HideTargets()
        {
            foreach (CombatAction action in _actions)
            {
                action.HideTargets();
            }
        }

        public void Use()
        {
            foreach(CombatAction action in _actions)
            {
                action.Perform();
                action.HideTargets();
            }
        }
    }
}
