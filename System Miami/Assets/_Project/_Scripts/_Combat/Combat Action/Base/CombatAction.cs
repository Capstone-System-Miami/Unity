// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEditor.PackageManager;
using SystemMiami.Outdated;
using UnityEditor.Overlays;

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

        public Color TargetedTileColor;
        public Color TargetedCombatantColor;

        public bool AffectsSelf { get { return _affectsSelf; } }
        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        public Targets StoredTargets;


        /// <summary>
        /// Takes directional info about the user,
        /// and uses it as the basis for creating
        /// a new set of DirectionalInfo about
        /// the targetting pattern specific to
        /// this CombatAction.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private DirectionalInfo getPatternDirection(DirectionalInfo userInfo)
        {
           //calculate rotation that are needed
           int rotationSteps = GetRotationSteps(userInfo.DirectionVec);
           Debug.Log($"Rotation steps calculated: {rotationSteps}");

            //rotate the pattern
            List<TileDir> rotatedDirections = RotateDirections(TargetingPattern.GetDirections(), rotationSteps);

           //set eh rotated directions in the targetingPattern
           TargetingPattern.SetRotatedDirections(rotatedDirections);
            // The coordinates (on the Board) to start checking tiles from
            //Vector2Int patternOrigin;

            // The coordinates (on the board) that
            // the Pattern considers to be
            // its own local "forward".
            // The Pattern's definition of "forward" might be
            // the Board's definition of "right"
            // Vector2Int patternForward;

            // If action is a projectile
            //if (this is Projectile projectile)
            //{
            //    // If this is player
            //    // If the mouse boardPosition is less than the range (how will we do this?)
            //    // Pattern origin = mouse boardPosition
            //    // If this is the enemy
            //    // If the player boardPosition is less than the range (how?)
            //    // Pattern origin = player boardPosition

            //    // Return a zero'd direction info
            //    // if projectile targeting failed?
            //    patternOrigin = userInfo.MapForward;
            //    patternForward = userInfo.MapForward + userInfo.DirectionVec;
            //}
            //else
            //{
            //    patternOrigin = userInfo.MapPosition;
            //    patternForward = userInfo.DirectionVec;
            //}

            return userInfo;
        }

        private void tryGetTile(Vector2Int position, out OverlayTile tile, out Combatant character)
        {
            if (MapManager.MGR.map.ContainsKey(position))
            {
                tile = MapManager.MGR.map[position];
                character = tile.currentCharacter;
            }
            else
            {
                tile = null;
                character = null;
            }
        }

        /// <summary>
        /// Calculates and stores the targets based on the provided DirectionalInfo.
        /// </summary>
        /// <param name="userInfo">DirectionalInfo containing position and direction.</param>
        /// <returns>A Targets object containing positions, tiles, and combatants.</returns>
        public Targets GetUpdatedTargets(DirectionalInfo userInfo)
        {
            // Ensure the pattern directions are rotated based on userInfo aaaaah took me so long to figure this out
           getPatternDirection(userInfo);
            List<Vector2Int> boardPositions = new List<Vector2Int>();
            List<OverlayTile> validTilesList = new List<OverlayTile>();
            List<Combatant> targetCombatantsList = new List<Combatant>();

            Vector2Int origin = userInfo.MapPosition;
            
            //get all directions in targeting pattern
            List<TileDir> patternDirections = TargetingPattern.GetRotatedDirections();

            Debug.Log($"Pattern directions count: {patternDirections.Count}");

            // Check if patternDirections is null or empty
            if (patternDirections == null || patternDirections.Count == 0)
            {
                Debug.LogWarning("Pattern directions are null or empty.");
                // Return empty targets
                Targets updatedEmpty = new Targets(new Vector2Int[0], new OverlayTile[0], new Combatant[0]);
                StoredTargets = updatedEmpty;
                return updatedEmpty;
            }

            //loop over each direction
            foreach (TileDir dir in patternDirections)
            {
                //get the direction for this TileDir
                Vector2Int directionVec = DirectionHelper.MapDirectionsByEnum[dir];
                Debug.Log($"Processing direction {dir} with vector {directionVec}");
                //for each tile along the direction up to the radius
                for (int i = 0; i <= TargetingPattern.Radius; i++)
                {

                    Vector2Int boardPosition = origin + directionVec * i;

                    //Debug.Log($"Checking tile position {boardPosition}");
                    //check if the position is valid

                    if (MapManager.MGR.map.ContainsKey(boardPosition))
                    {
                        OverlayTile tile = MapManager.MGR.map[boardPosition];
                        //avoid adding duplicates
                        if (!boardPositions.Contains(boardPosition))
                        {
                            boardPositions.Add(boardPosition);
                            validTilesList.Add(tile);
                           // Debug.Log($"Added tile at {boardPosition} to targets.");
                        }

                        //check for combatant on tile
                        if (tile.currentCharacter != null)
                        {
                            Combatant enemy = tile.currentCharacter;
                            if (!targetCombatantsList.Contains(enemy))
                            {
                                targetCombatantsList.Add(enemy);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"Tile position {boardPosition} is out of map bounds. Stopping in this direction.");
                        //stop extending the line when we reach the edge
                        break;
                    }
                }
            }

            Targets updated = new Targets(boardPositions.ToArray(), validTilesList.ToArray(), targetCombatantsList.ToArray());
            Debug.Log($"Total tiles targeted: {updated.Tiles.Length}");
            StoredTargets = updated;
            return updated;



            //AdjacentPositionSet patternAdjacentPositions = new AdjacentPositionSet(getPatternDirection(userInfo));

            //for (int i = 0; i <= _targetingPattern.Radius; i++)
            //{
            //    List<TileDir> patternDirections = _targetingPattern.GetDirections();

            //    foreach (TileDir direction in patternDirections)
            //    {
            //        Vector2Int boardPosition = patternAdjacentPositions.Adjacent[direction] * i;

            //        // Add the tile at the action origin's adjacent tiles,
            //        // corrected for the direction the character is facing.
            //        boardPositions.Add(boardPosition);

            //        tryGetTile(boardPosition, out OverlayTile tile, out Combatant enemy);

            //        if (tile != null) { validTilesList.Add(tile); }

            //        if (enemy != null) { targetCombatantsList.Add(enemy); }
            //    }
            //}

            //Targets updated = new Targets(boardPositions.ToArray(), validTilesList.ToArray(), targetCombatantsList.ToArray());
            //StoredTargets = updated;
            //return updated;
        }

        /// <summary>
        /// Calculates the number of rotation steps to align the pattern.
        /// </summary>
        private int GetRotationSteps(Vector2Int directionVec)
        {
            // Map the direction vector to a TileDir enum
            TileDir dirEnum = DirectionHelper.GetTileDirFromVector(directionVec);

            Debug.Log($"Direction vector {directionVec} mapped to TileDir {dirEnum}");


            // Assuming FORWARD_C is the base direction (0 rotation)
            int baseDir = (int)TileDir.FORWARD_C;
            int targetDir = (int)dirEnum;

            // Calculate rotation steps
            int rotationSteps = (targetDir - baseDir + 8) % 8; // +8 to handle negative values

            Debug.Log($"Calculated rotation steps: {rotationSteps} for direction {dirEnum}");

            return rotationSteps;
        }

        /// <summary>
        /// Rotates the list of TileDirs by the specified number of steps.
        /// </summary>
        private List<TileDir> RotateDirections(List<TileDir> directions, int steps)
        {
            List<TileDir> rotated = new List<TileDir>();

            foreach (TileDir dir in directions)
            {
                int rotatedDir = ((int)dir + steps) % 8; // There are 8 directions
                rotated.Add((TileDir)rotatedDir);
            }

            return rotated;
        }


        public abstract void Perform(Targets targets);
    }
}
