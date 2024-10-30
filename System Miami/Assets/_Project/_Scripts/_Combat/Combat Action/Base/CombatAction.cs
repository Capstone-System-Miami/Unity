// Authors: Layla Hoey, Lee St. Louis
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatAction.
    /// If an Object is of the class CombatAction,
    /// it has to be an Object of a class that inherits from CombatAction)
    public abstract class CombatAction : ScriptableObject
    {
        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public abstract void Perform();
    }
}
