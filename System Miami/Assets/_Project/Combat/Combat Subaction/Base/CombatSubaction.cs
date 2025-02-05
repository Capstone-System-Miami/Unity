// Authors: Layla Hoey, Lee St. Louis
using System;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatRefactor;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ISubactionCommand
    {
        void Preview();
        void Execute();
    }

    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatSubaction.
    /// If an Object is of the class CombatSubaction,
    /// it has to be an Object of a class that inherits from CombatSubaction)
    public abstract class CombatSubaction : ScriptableObject
    {
        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public void IssueCommands(List<ITargetable> targets)
        {
            foreach (ITargetable target in targets)
            {
                ISubactionCommand command = GenerateCommand(target);
                target.TargetedBy.Add(command);
            }
        }

        protected abstract ISubactionCommand GenerateCommand(ITargetable t);
    }
}
