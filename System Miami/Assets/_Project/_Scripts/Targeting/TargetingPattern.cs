// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum TargetOrigin { SELF, MOUSE }

    [CreateAssetMenu(fileName = "New Targeting Pattern", menuName = "Abilities/TargetingPattern")]
    public class TargetingPattern : ScriptableObject
    {
        [Tooltip("The position to start checking the radius from. Either the combatant doing the targeting, or the position of the mouse.")]
        [SerializeField] private TargetOrigin _origin;

        [Tooltip("Radius of the pattern, in Tile lengths.")]
        [SerializeField] private int _radius;

        [Header("Directions")]
        [SerializeField] private bool _frontLeft;
        [SerializeField] private bool _front;
        [SerializeField] private bool _frontRight;
        [SerializeField] private bool _left;
        [SerializeField] private bool _center;
        [SerializeField] private bool _right;
        [SerializeField] private bool _backLeft;
        [SerializeField] private bool _back;
        [SerializeField] private bool _backRight;

        public TargetOrigin Origin { get { return _origin; } }
        public int Radius { get { return _radius; } }

        public Dictionary<TileDir, bool> Directions;

        private void OnEnable()
        {
            Directions = new Dictionary<TileDir, bool>();
            Directions[TileDir.FORWARD_L] = _frontLeft;
            Directions[TileDir.FORWARD_C] = _front;
            Directions[TileDir.FORWARD_R] = _frontRight;
            Directions[TileDir.MIDDLE_L] = _left;
            Directions[TileDir.MIDDLE_C] = _center;
            Directions[TileDir.MIDDLE_R] = _right;
            Directions[TileDir.BACKWARD_L] = _backLeft;
            Directions[TileDir.BACKWARD_C] = _back;
            Directions[TileDir.BACKWARD_R] = _backRight;
        }
    }
}
