// Authors: Layla Hoey
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, chaotic, and partially tested
    // Depends on the creation & refactoring of other scripts.
    // Somehow need to implement a melee vs projectile system?
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

        public TargetingPattern Pattern { get { return _targetingPattern; } }

        protected AbilityType _type;
        protected ResourceType _requiredResource;

        public void UseOn(GameObject[] targets)
        {
            foreach (GameObject target in targets)
            {
                foreach (CombatAction action in _actions)
                {
                    action.PerformOn(target);
                }
            }
        }
    }
}
