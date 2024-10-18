// Authors: Layla Hoey
using System;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    
    // Take the direction that something is facing,
    // and translate their local adjacent positions
    // into the static/unchanging positions on the Board/Map
    public class AdjacentPositionSet
    {
        private Dictionary<TileDir, Vector2Int> _directionsRelativeToMap;
        private Dictionary<TileDir, Vector2Int> _positionsRelativeToMap;
        private Dictionary<TileDir, Vector2Int> _directionsRelativeToSelf;
        private List<TileDir> rotatedDirs;
        private TargetingPattern targetingPattern;
        public Dictionary<TileDir, Vector2Int> Adjacent { get; private set; }
        public bool IsReady { get; private set; }

        // Constructors
        public AdjacentPositionSet(DirectionalInfo info)
        {
            // Initialize local directions to default set of directions.
            _directionsRelativeToSelf = DirectionHelper.MapDirectionsByEnum;

            // Find the map directions by rotating an amount of
            // ticks equivalent to the enumerated direction
            // of the incoming object.
            _directionsRelativeToMap = getRotatedVectors(_directionsRelativeToSelf, (int)info.DirectionName);
            
            // Get adjacent map positions by adding the map position of the
            // incoming object to the directions
            _positionsRelativeToMap = new Dictionary<TileDir, Vector2Int>();
            foreach(TileDir direction in _directionsRelativeToMap.Keys)
            {
                _positionsRelativeToMap[direction] = _directionsRelativeToMap[direction] + info.MapPosition;
            }

            Adjacent = _positionsRelativeToMap;
            IsReady = true;
        }

        public void SetRotatedDirections(List<TileDir> rotatedDirections)
        {
            rotatedDirs.Clear();
            if (rotatedDirections != null && rotatedDirections.Count > 0)
            {
                rotatedDirs.AddRange(rotatedDirections);
            }
            else
            {
                Debug.Log("rotated list is empty");
            }

        }

        public List<TileDir> GetRotatedDirs()
        {
            return rotatedDirs ?? targetingPattern.GetDirections();
        }

        /// <summary>
        /// Rotates a set of local positions by a
        /// number of quarter turns.
        /// (One clockwise quarter turn from forward
        /// means your new forward is topRight)
        /// </summary>
        /// <param name="originalVectors">
        /// The set of 8 positions to be shitfed
        /// </param>
        /// <param name="quarterTurns">
        /// The amount of times to shift by 45 degrees
        /// <returns></returns>
        private Dictionary<TileDir, Vector2Int> getRotatedVectors(Dictionary<TileDir, Vector2Int> originalVectors, int quarterTurns)
        {
            Dictionary<TileDir, Vector2Int> result = new Dictionary<TileDir, Vector2Int>();

            // Should always be 8
            int directionCount = Enum.GetValues(typeof(TileDir)).Length;

            int leftIndex = 0;
            int rightIndex = quarterTurns;
            int catchBeginning = 0;

            int total = 0;
            const int LIMIT = 10; // ( while loops are scary ¯\_( )_/¯ )
            while (leftIndex < directionCount && total++ <= LIMIT)
            {
                // At leftIndex == 0, centered is forward center.
                // Increment the indexer after we read it.
                TileDir centered = (TileDir)leftIndex++;
                TileDir shifted;

                // If right is less than the number of
                // directions in the TileDir enum,
                if (rightIndex < directionCount)
                {
                    // then the shifted index is that.
                    // Increment the indexer after we read it.
                    shifted = (TileDir)rightIndex++;
                }
                // If right is >= number of directions,
                else
                {
                    // start using this as the shifted index.
                    // Increment it after we read it.
                    shifted = (TileDir)catchBeginning++;
                }

                Debug.Log($"local pos {centered} is now original pos {shifted}");
                // Result set to the shifted position.
                result[centered] = originalVectors[shifted];
            }
            return result;
        }
    }
}
