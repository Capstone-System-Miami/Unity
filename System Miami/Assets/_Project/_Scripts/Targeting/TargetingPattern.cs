// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum PatternType { SELF, AREA, MOUSE };

    [System.Serializable]
    public class TargetingPattern
    {
        public PatternType Type;

        [Tooltip("Radius of the pattern, in Tiles.")]
        [SerializeField] private int _radius;

        [Header("Directions")]
        [SerializeField] private bool _frontLeft;
        [SerializeField] private bool _front;
        [SerializeField] private bool _frontRight;
        [SerializeField] private bool _left;
        [SerializeField] private bool _right;
        [SerializeField] private bool _backLeft;
        [SerializeField] private bool _back;
        [SerializeField] private bool _backRight;

        public int Radius { get { return _radius; } }

        public List<TileDir> GetDirections()
        {
            // A list of TileDirections
            List<TileDir> result = new List<TileDir>();

            // An array of the bools set in the inspector
            bool[] checkDirections =
            {
                _front,
                _frontRight,
                _right,
                _backRight,
                _back,
                _backLeft,
                _left,
                _frontLeft,
            };

            // Add the direction of every `true` to the result List
            for (int i = 0; i < checkDirections.Length; i++)
            {
                if (checkDirections[i])
                {
                    result.Add((TileDir)i);
                }
            }

            string report = "";
            foreach (TileDir dir in result)
            {
                report += $"Check {dir}\n";
            }
            Debug.Log(report);

            return result;
        }
    }
}