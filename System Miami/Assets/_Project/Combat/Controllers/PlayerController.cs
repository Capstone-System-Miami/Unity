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
                TypeToEquip = slot.Type;
                IndexToEquip = slot.Index;
                FLAG_Equip = true;
            }
            else
            {
                IndexToEquip = -1;
                FLAG_Unequip = true;
            }
        }

        #endregion // SUBSCRIPTIONS =============


        #region Triggers
        // ======================================

        public override bool EndTurnTriggered()
        {
            return Input.GetKeyDown(_endTurnKey);
        }

        public override bool NextPhaseTriggered()
        {
            if (IsMoving) { return false; }
            if (IsActing) { return false; }

            return Input.GetKeyDown(_endPhaseKey);
        }

        public override bool BeginMovementTriggered()
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

        public override bool UnequipTriggered()
        {
            return Input.GetMouseButtonDown(1) || FLAG_Unequip;
        }

        public override bool EquipTriggered()
        {
            if (!CanAct)
                { return false; }

            if (IndexToEquip == -1)
                { return false; }

            return FLAG_Equip;
        }

        public override bool LockTargetsTriggered()
        {
            if (!CanAct)
                { return false; }

            return Input.GetMouseButtonDown(0) || FLAG_LockTargets;
        }

        public override bool UseAbilityTriggered()
        {
            if (!CanAct)
                { return false; }

            return Input.GetKeyDown(KeyCode.Return) || FLAG_UseAbility;
        }

        public override void ResetFlags()
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
        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found under the mouse.
        /// </summary>
        public override OverlayTile GetFocusedTile()
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

        // ======================================
        #endregion // Focused Tile ==============
    }
}
