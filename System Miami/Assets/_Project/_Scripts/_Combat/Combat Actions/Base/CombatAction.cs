// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEditor.PackageManager;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatAction.
    /// If an Object is of the class CombatAction,
    /// it has to be an Object of a class that inherits from CombatAction)
    public abstract class CombatAction : ScriptableObject
    {
        /// <summary>
        /// Whether or not the action will afftect the Combatant performing the action.
        /// </summary>
        [SerializeField] private bool _affectsSelf;

        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        [SerializeField] protected Color _targetedTileColor;
        [SerializeField] protected Color _targetedCombatantColor;

        protected Combatant _user;

        /// <summary>
        /// Object that checks for targets given a specific pattern,
        /// adjusting for the direction the user is facing.
        /// </summary>
        protected Targeting _targeting;

        protected OverlayTile[] _targetTiles;
        protected Combatant[] _targetCombatants;

        public bool AffectsSelf { get { return _affectsSelf; } }
        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public abstract void SetTargeting();
        public abstract void Perform();

        public void Init(Combatant user)
        {
            _user = user;
            SetTargeting();
        }

        public void ShowTargets()
        {
            TargetTiles();
            TargetCombatants();
        }

        public void UpdateTargets()
        {
            Debug.Log($"{name} trying to update with {_user.DirectionInfo}");
            _targeting.GetUpdatedTargets(out _targetTiles, out _targetCombatants);
        }

        public void HideTargets()
        {
            UntargetTiles();
            UntargetCombatants();
        }

        public void TargetTiles()
        {
            OverlayTile[] targetTiles = _targetTiles;

            Debug.Log($"{name} Trying to target tiles");
            for (int i = 0; i < targetTiles.Length; i++)
            {
                targetTiles[i].Target(_targetedTileColor);
            }
        }

        public void TargetCombatants()
        {
            Combatant[] targetCombatants = _targetCombatants;

            for (int i = 0; i < _targetCombatants.Length; i++)
            {
                _targetCombatants[i].Target(_targetedCombatantColor);
            }
        }

        public void UntargetTiles()
        {
            OverlayTile[] targetTiles = _targetTiles;

            for (int i = 0; i < targetTiles.Length; i++)
            {
                targetTiles[i].UnTarget();
            }
        }

        public void UntargetCombatants()
        {
            Combatant[] targetCombatants = _targetCombatants;

            for (int i = 0; i < _targetCombatants.Length; i++)
            {
                _targetCombatants[i].UnTarget();
            }
        }

    }
}
