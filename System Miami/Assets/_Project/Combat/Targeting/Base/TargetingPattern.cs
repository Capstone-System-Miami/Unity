// Authors: Layla Hoey, Lee St. Louis
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    // TODO
    // 1. Create a function for setting targets without showing it (for enemy use)
    public enum PatternOriginType { USER, FOCUS };
    public abstract class TargetingPattern : ScriptableObject
    {
        [Tooltip(
            "Will the pattern below be based on the combatant's " +
            "position, or the position of the tile being focused on?")]
        [SerializeField] private PatternOriginType _patternOrigin;

        public Color TargetedTileColor = Color.white;
        public Color TargetedCombatantColor = Color.white;

        public PatternOriginType PatternOrigin { get { return _patternOrigin; } }

        public Targets StoredTargets;
        [HideInInspector] public bool _targetsLocked;
       
        #region Public
        public abstract Targets GetTargets(DirectionContext userDirection);

        /// <summary>
        /// Takes directional info about the user,
        /// and uses it as the basis for creating
        /// a new set of DirectionalInfo about
        /// the targetting pattern specific to
        /// the CombatAction.
        /// </summary>
        /// <param name="userDirection"></param>
        /// <returns></returns>
        protected DirectionContext getPatternDirection(DirectionContext userDirection)
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