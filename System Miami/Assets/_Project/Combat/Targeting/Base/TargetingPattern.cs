// Authors: Layla Hoey, Lee St. Louis
using SystemMiami.Utilities;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum PatternOriginType { USER, FOCUS };
    public abstract class TargetingPattern : ScriptableObject
    {
        [Tooltip(
            "Will the pattern below be based on the combatant's " +
            "position, or the position of the tile being focused on?")]
        [SerializeField] private PatternOriginType _patternOrigin;

        public Color TargetedTileColor = Color.white;
        public Color TargetedCombatantColor = Color.white;

        public PatternOriginType PatternOrigin
        {
            get { return _patternOrigin; }
            set => _patternOrigin = value;
        }

        #region Public
        /// <summary>
        /// Method by which to return Targets.
        /// Defined in the derived classes.
        /// </summary>
        /// 
        /// <param name="userDirection">
        /// The direction on which the pattern
        /// will determine its own direction.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="TargetSet"/> object,
        /// containing a List{} of all
        /// <see cref="ITargetable"/> objects found
        /// on any tile, including tiles themselves.
        /// </returns>
        public abstract TargetSet GetTargets(DirectionContext userDirection);

        /// <summary>
        /// Uses the <see cref="DirectionContext"/>
        /// of the combatant, as well as the
        /// <see cref="PatternOriginType"/>
        /// of the pattern to create a new 
        /// <see cref="DirectionContext"/> of
        /// the targetting pattern.
        /// </summary>
        /// 
        /// <param name="userDirection">
        /// The current direction of the combatant user.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="DirectionContext"/> object
        /// representing the direction of this pattern,
        /// including its Origin position on the game board
        /// (as <see cref="DirectionContext.TilePositionA"/>)
        /// and Focus position
        /// (as <see cref="DirectionContext.TilePositionB"/>).
        /// </returns>
        protected DirectionContext GetPatternDirection(DirectionContext userDirection)
        {
            if (PatternOrigin == PatternOriginType.USER)
            {
                // If the pattern originates from the user,
                // we already have it's directional info.
                return userDirection;
            }
            else
            {
                // If the pattern originates from a non-user target,
                // We should use the Focus (arg.TilePositionB) as the
                // new origin point (new.TilePositionA) for the direction of the pattern,
                // And one point forward from the incoming Focus (arg.ForwardB)
                // as the new focus point (new.TilePositionB) in determining the direction
                return new DirectionContext(userDirection.TilePositionB, userDirection.ForwardB);
            }
        }
        #endregion Protected
    }
}