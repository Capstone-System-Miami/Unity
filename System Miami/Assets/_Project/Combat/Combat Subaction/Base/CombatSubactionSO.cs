// Authors: Layla Hoey, Lee St. Louis
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatSubaction.
    /// If an Object is of the class CombatSubaction,
    /// it has to be an Object of a class that inherits from CombatSubaction)
    public abstract class CombatSubactionSO : ScriptableObject
    {
        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public abstract ISubactionCommand GenerateCommand(ITargetable target);
    }
}
