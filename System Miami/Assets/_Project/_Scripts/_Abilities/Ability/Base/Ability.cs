// Authors: Layla Hoey
using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Mostly incomplete and chaotic, and
    // definitely not functional.
    // Depends on the creation & refactoring of other scripts.
    // Somehow need to implement a melee vs projectile system?
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
    public abstract class Ability : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _name;
        [SerializeField] private float _resourceCost;

        [Header("Actions"), Space(10)]
        [SerializeField] private CombatAction[] _actions;

        [Header("Targeting"), Space(10)]
        [SerializeField] private TargetType _targetType;
        [SerializeField] private TargetingPattern _targetingPattern;

        protected AbilityType _type;
        protected ResourceType _requiredResource;

        public void AquireTargets()
        {
            // TODO
        }

        public void Preview()
        {
            // TODO
        }

        public void UseOn(Combatant[] targets)
        {
            foreach (CombatAction action in _actions)
            {
                action.PerformOn(targets);
            }
        }
    }
}
