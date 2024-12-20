// Authors: Layla
using System.Linq;
using SystemMiami.Management;
using SystemMiami.ui;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class PlayerController : CombatantController
    {
        [SerializeField] private KeyCode _endTurnKey;
        [SerializeField] private KeyCode _endPhaseKey;

        public KeyCode EndTurnKey { get { return _endTurnKey; } }
        public KeyCode EndPhaseKey { get { return _endPhaseKey; } }


        private OverlayTile _mouseTile;


        #region Flags
        private bool FLAG_Unequip;
        private bool FLAG_Equip;
        private bool FLAG_LockTargets;
        private bool FLAG_UseAbility;
        #endregion


        #region UNITY METHODS
        // ======================================

        private void OnEnable()
        {
            UI.MGR.SlotClicked += onSlotClicked;
        }


        private void OnDisable()
        {
            UI.MGR.SlotClicked -= onSlotClicked;
        }
        #endregion // UNITY METHODS =============


        #region SUBSCRIPTIONS
        // ======================================

        private void onSlotClicked(AbilitySlot slot)
        {
            if (slot.State != SelectionState.SELECTED)
            {
                typeToEquip = slot.Type;
                indexToEquip = slot.Index;
                FLAG_Equip = true;
            }
            else
            {
                indexToEquip = -1;
                FLAG_Unequip = true;
            }
        }

        #endregion // SUBSCRIPTIONS =============


        #region Triggers
        // ======================================

        protected override bool endTurnTriggered()
        {
            return Input.GetKeyDown(_endTurnKey);
        }

        protected override bool nextPhaseTriggered()
        {
            if (IsMoving) { return false; }
            if (IsActing) { return false; }

            return Input.GetKeyDown(_endPhaseKey);
        }

        protected override bool beginMovementTriggered()
        {
            if (!CanMove)
                { return false; }

            if (FocusedTile == null)
                { return false; }

            if (FLAG_Equip)
                {return false; }

            if (FLAG_Unequip)
                { return false; }

            return Input.GetMouseButtonDown(0);
        }

        protected override bool unequipTriggered()
        {
            return Input.GetMouseButtonDown(1) || FLAG_Unequip;
        }

        protected override bool equipTriggered()
        {
            if (!CanAct)
                { return false; }

            if (indexToEquip == -1)
                { return false; }

            return FLAG_Equip;
        }

        protected override bool lockTargetsTriggered()
        {
            if (!CanAct)
                { return false; }

            return Input.GetMouseButtonDown(0) || FLAG_LockTargets;
        }

        protected override bool useAbilityTriggered()
        {
            if (!CanAct)
                { return false; }

            return Input.GetKeyDown(KeyCode.Return) || FLAG_UseAbility;
        }

        protected override void resetFlags()
        {
            FLAG_Unequip = false;
            FLAG_Equip = false;
            FLAG_LockTargets = false;
            FLAG_UseAbility = false;
        }

        // ======================================
        #endregion // Triggers ==================


        #region Focused Tile
        // ======================================

        protected override void updateFocusedTile()
        {
            OverlayTile newFocus = getFocusedTile();

            if (!isValidFocus(newFocus))
            {
                if (combatant.Abilities.CurrentState != Abilities.State.TARGETS_LOCKED)
                {
                    FocusedTile?.EndHover(this);
                }

                FocusedTile = null;
                return;
            }

            if (newFocus == FocusedTile) { return; }

            FocusedTile?.EndHover(this);
            newFocus.BeginHover(this);
            FocusedTile = newFocus;

            // Raise event when mouse tile  changes            
            FocusedTileChanged?.Invoke(newFocus);
        }

        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found under the mouse.
        /// </summary>
        protected override OverlayTile getFocusedTile()
        {
            RaycastHit2D? mouseHit = getMouseHitInfo();
            OverlayTile mouseTile = getTileFromRaycast(mouseHit);

            return mouseTile;
        }

        /// <summary>
        /// Gets the raycastHit info for whatever
        /// the mouse is currently over.
        /// </summary>
        private RaycastHit2D? getMouseHitInfo()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos2d = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }

        /// <summary>
        /// Takes nullable type RaycastHit info and returns
        /// either the tile found within the Hit,
        /// or null if no tile was found in the Hit.
        /// </summary>
        private OverlayTile getTileFromRaycast(RaycastHit2D? hit)
        {
            if (!hit.HasValue) { return null; }

            return hit.Value.collider.gameObject.GetComponent<OverlayTile>();
        }

        private bool isValidFocus(OverlayTile tile)
        {
            if (tile == null) { return false; }

            if (CurrentPhase == Phase.None) { return false; }

            if (IsMoving) { return false; }

            if (IsActing) { return false; }

            if (combatant.Abilities.CurrentState != Abilities.State.UNEQUIPPED
                && combatant.Abilities.CurrentState != Abilities.State.EQUIPPED)
            {
                return false;
            }

            return true;
        }

        // ======================================
        #endregion // Focused Tile ==============
    }
}
