// Authors: Layla Hoey, Lee St. Louis
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    // TODO
    // 1. Create a function for setting targets without showing it (for enemy use)
    public enum PatternOriginType { USER, MOUSE };

    public abstract class TargetingPattern : ScriptableObject
    {
        [Tooltip("Will the pattern below be based on the player position, or the mouse position?")]
        [SerializeField] private PatternOriginType _patternOrigin;

        public Color TargetedTileColor = Color.white;
        public Color TargetedCombatantColor = Color.white;

        public PatternOriginType PatternOrigin { get { return _patternOrigin; } }

        public Targets StoredTargets;
        [HideInInspector] public bool _targetsLocked;
       
        #region Public
        public abstract void SetTargets(DirectionalInfo userInfo);

        public void ClearTargets()
        {
            StoredTargets = Targets.empty;
        }

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
            //ShowTargets();
        }

        public  void UnsubscribeToDirectionUpdates(Combatant user)
        {            
            if (_patternOrigin == PatternOriginType.USER)
            {
                user.OnDirectionChanged -= onTargetChanged;
            }
            else
            {
                user.OnSubjectChanged -= onTargetChanged;
            }
            //if (!_targetsLocked)
            //{
            //    HideTargets();
            //}
        }

        /// <summary>
        /// Locks the targets by allowing unsubscribing from moveDirection updates without hiding the targets.
        /// </summary>
        public  void LockTargets()
        {            
            _targetsLocked = true;
            //Debug.Log($"Targets locked in {name}'s TargetingPattern.");
        }

        public void UnlockTargets()
        {
            _targetsLocked = false;
            //Debug.Log($"Targets unlocked in {name}'s TargetingPattern");
        }

        public bool  ShowTargets()
        {            
            showTiles();
            showCombatants();
            return true;
        }

        public bool HideTargets()
        {
            //if (_targetsLocked)
            //{
            //    // Shouldn't this condition check be somewhere else?
            //    // This function is supposed to hide the targets no matter what.
            //    Debug.Log("hideTargets called but targets are locked. Skipping hiding.");
            //    return true;
            //}

            //Debug.Log("HideTargets called. Hiding targets.");
            hideTiles();
            hideCombatants();
            return true;
        }
        #endregion Public


        #region Protected
        protected void onTargetChanged(DirectionalInfo dir)
        {
            //Debug.Log($"OnTargetChanged called. Targets locked: {_targetsLocked}");
            //if (_targetsLocked)
            //{          
            //    ShowTargets();
            //    return;
            //}

            HideTargets();
            SetTargets(dir);
            ShowTargets();
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
                // as the 'A' point for the moveDirection of the pattern.
                // If the user is the _player, userDirectionInfo.MapPositionB will be the
                // mouse position.
                return new DirectionalInfo(userInfo.MapPositionB, userInfo.MapForwardB);
            }
        }

        protected void tryGetTile(Vector2Int position, out OverlayTile tile, out Combatant character)
        {
            if (MapManager.MGR.map.ContainsKey(position))
            {
                tile = MapManager.MGR.map[position];
                character = tile.CurrentCharacter;
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
                tile.Highlight(TargetedTileColor);
            }
        }

        private void showCombatants()
        {          
            if (StoredTargets.Combatants == null) return;
            if (StoredTargets.Combatants.Count == 0) { return; }

            for (int i = 0; i < StoredTargets.Combatants.Count; i++)
            {
                if (StoredTargets.Combatants[i] == null) { continue; }

                StoredTargets.Combatants[i].Highlight(TargetedCombatantColor);
            }
        }

        private void hideTiles()
        {            
            if (StoredTargets.Tiles == null) return;
            if (StoredTargets.Tiles.Count == 0) { return; }

            foreach (OverlayTile tile in StoredTargets.Tiles)
            {
               tile.UnHighlight();
            }
        }

        private void hideCombatants()
        {
            if (StoredTargets.Combatants == null) return;
            if (StoredTargets.Combatants.Count == 0) { return; }

            for (int i = 0; i < StoredTargets.Combatants.Count; i++)
            {                
                if (StoredTargets.Combatants[i] == null) { continue; }

                StoredTargets.Combatants[i].UnHighlight();
            }
        }
        #endregion Private
    }
}