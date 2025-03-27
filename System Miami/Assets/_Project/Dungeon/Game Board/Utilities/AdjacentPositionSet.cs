// Authors: Layla Hoey
using System;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    /// <summary>
    /// Take the direction that something is facing,
    /// and translate its local adjacent positions
    /// into static/unchanging positions on the Board/Map
    /// </summary>
    public class AdjacentPositionSet
    {
        public readonly Dictionary<TileDir, Vector2Int>
            BoardDirectionVectors = new();

        public readonly Dictionary<TileDir, Vector2Int>
            AdjacentBoardPositions = new();

        // Constructors
        public AdjacentPositionSet(DirectionContext info)
        {
            // Find the map directions by rotating an amount of
            // ticks equivalent to the enumerated direction
            // of the incoming object.
            BoardDirectionVectors = GetRotatedVectors((int)info.BoardDirection);
            //DirectionHelper.Print(_directionsRelativeToMap, "Map directions");
            
            // Get adjacent map positions by adding the map position of the
            // incoming object to the directions
            foreach(TileDir direction in BoardDirectionVectors.Keys)
            {
                AdjacentBoardPositions[direction]
                    = info.TilePositionA + BoardDirectionVectors[direction];
            }

            //DirectionHelper.Print(AdjacentPositions, "Adjacent");
        }

        /// <summary>
        /// Rotates a set of standard local positions by
        /// a number of quarter turns.
        /// 
        /// <para>
        /// - Ex. 1 | 1 clockwise turn from board-forward
        /// means the value at returnedDict[FORWARD_C]
        /// will be the same value found at
        /// BoardDirectionVecByEnum[FORWARD_R]</para>
        /// 
        /// <para>
        /// - Ex. 2 | 6 clockwise turns from board-forward
        /// means the value at returnedDict[FORWARD_C]
        /// will be the same value found at
        /// BoardDirectionVecByEnum[BACKWARD_L]</para>
        /// </summary>
        /// 
        /// <param name="clockwiseEighthTurns">
        /// The amount of times to shift by 45 degrees
        /// </param>
        private Dictionary<TileDir, Vector2Int> GetRotatedVectors(int clockwiseEighthTurns)
        {
            // Copy of the standard dictionary
            // of directions is stored to
            // represent our original
            // 'local' directions
            Dictionary<TileDir, Vector2Int>
                localDirs = new(DirectionHelper.BoardDirectionVecByEnum);

            Dictionary<TileDir, Vector2Int>
                result = new();

            // Should always be 8
            int directionCount = Enum.GetValues(typeof(TileDir)).Length;

            int leftIndex = 0;
            int rightIndex = clockwiseEighthTurns;
            int catchBeginning = 0;

            int iterations = 0;
            const int LIMIT = 10; // ( while loops are scary ¯\_( )_/¯ )
            while (leftIndex < directionCount && iterations++ <= LIMIT)
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

                //Debug.Log($"local pos {centered} is now original pos {shifted}");
                // Result set to the shifted position.
                result[centered] = localDirs[shifted];
            }
            return result;
        }
    }
}
