using System.Linq;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class PlayerController : CombatantController
    {
        [SerializeField] private KeyCode _endTurnKey;
        [SerializeField] private KeyCode _endPhaseKey;

        #region Triggers
        protected override bool endTurnTriggered()
        {
            return Input.GetKeyDown(_endTurnKey);
        }

        protected override bool nextPhaseTriggered()
        {
            return Input.GetKeyDown(_endPhaseKey);
        }

        protected override bool beginMovementTriggered()
        {
            if (IsMoving)
                { return false; }

            if (FocusedTile == null)
                { return false; }

            return Input.GetMouseButtonDown(0);
        }
        #endregion Triggers

        #region Focused Tile
        /// <summary>
        /// Resets the mouse tile to board (1, 1)
        /// </summary>
        protected override void resetFocusedTile()
        {
            MapManager.MGR.map.TryGetValue(Vector2Int.one, out OverlayTile newFocus);

            FocusedTile = newFocus;

            FocusedTileChanged?.Invoke(FocusedTile);
        }

        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found.
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
        /// <param name="hit"></param>
        /// <returns></returns>
        private OverlayTile getTileFromRaycast(RaycastHit2D? hit)
        {
            if (!hit.HasValue) { return null; }

            return hit.Value.collider.gameObject.GetComponent<OverlayTile>();
        }
        #endregion Focused Tile

        #region Movement

        #endregion Movement

        #region Phase Handling
        protected override void handleMovementPhase()
        {
            base.handleMovementPhase();
        }

        protected override void handleActionPhase()
        {
            base.handleActionPhase();
        }
        #endregion Phase Handling


    }
}
