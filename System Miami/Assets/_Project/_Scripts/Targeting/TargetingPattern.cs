// Authors: Layla Hoey
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum PatternOriginType { USER, MOUSE };

    public abstract class TargetingPattern : ScriptableObject
    {
        [Tooltip("Will the pattern below be based on the player position, or the mouse position?")]
        [SerializeField] private PatternOriginType _patternOrigin;

        public Color TargetedTileColor = Color.white;
        public Color TargetedCombatantColor = Color.white;

        public PatternOriginType PatternOrigin { get { return _patternOrigin; } }

        public Targets StoredTargets;

        #region Public
        public abstract void SetTargets(DirectionalInfo userInfo);

        public void SubscribeToDirectionUpdates(Combatant user)
        {
            if (_patternOrigin == PatternOriginType.USER)
            {
                user.OnDirectionChanged += onTargetChanged;
            }
            else
            {
                user.OnSubjectChanged += onTargetChanged;
            }

            showTargets();
        }

        public void UnsubscribeToDirectionUpdates(Combatant user)
        {
            hideTargets();

            if (_patternOrigin == PatternOriginType.USER)
            {
                user.OnDirectionChanged -= onTargetChanged;
            }
            else
            {
                user.OnSubjectChanged -= onTargetChanged;
            }
        }
        #endregion Public


        #region Protected
        protected void onTargetChanged(DirectionalInfo dir)
        {
            hideTargets();
            SetTargets(dir);
            showTargets();
        }

        protected bool showTargets()
        {
            showTiles();
            showCombatants();
            return true;
        }

        protected bool hideTargets()
        {
            hideTiles();
            hideCombatants();
            return true;
        }

        /// <summary>
        /// Takes directional info about the user,
        /// and uses it as the basis for creating
        /// a new set of DirectionalInfo about
        /// the targetting pattern specific to
        /// the CombatAction.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        protected DirectionalInfo getPatternDirection(DirectionalInfo userInfo)
        {
            if (PatternOrigin == PatternOriginType.USER)
            {
                // If the pattern originates from the user,
                // we already have it's directional info.
                return userInfo;
            }
            else
            {
                // If the pattern originates from a non-user target,
                // We should use whatever the user is looking at
                // as the 'A' point for the direction of the pattern.
                // If the user is the player, userDirectionInfo.MapPositionB will be the
                // mouse position.
                return new DirectionalInfo(userInfo.MapPositionB, userInfo.MapForwardB);
            }
        }

        protected void tryGetTile(Vector2Int position, out OverlayTile tile, out Combatant character)
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
        #endregion Protected


        #region Private
        private void showTiles()
        {
            if (StoredTargets.Tiles == null) return;
            if (StoredTargets.Tiles.Count == 0) { return; }

            foreach (OverlayTile tile in StoredTargets.Tiles)
            {
                tile.Target(TargetedTileColor);
            }
        }

        private void showCombatants()
        {
            if (StoredTargets.Combatants == null) return;
            if (StoredTargets.Combatants.Count == 0) { return; }

            foreach (Combatant combatant in StoredTargets.Combatants)
            {
                combatant.Target(TargetedCombatantColor);
            }
        }

        private void hideTiles()
        {
            if (StoredTargets.Tiles == null) return;
            if (StoredTargets.Tiles.Count == 0) { return; }

            foreach (OverlayTile tile in StoredTargets.Tiles)
            {
                tile.UnTarget();
            }
        }

        private void hideCombatants()
        {
            if (StoredTargets.Combatants == null) return;
            if (StoredTargets.Combatants.Count == 0) { return; }

            foreach (Combatant combatant in StoredTargets.Combatants)
            {
                combatant.UnTarget();
            }
        }
        #endregion Private
    }
}