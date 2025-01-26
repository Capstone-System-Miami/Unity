using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerMovementTileSelection : MovementTileSelection
    {
        public PlayerMovementTileSelection(Combatant combatant)
                : base(combatant) { }

        // Decision
        protected override bool SkipPhase()
        {
            return Input.GetKeyDown(KeyCode.Tab);
        }
        protected override bool SelectTile()
        {

            return Input.GetMouseButtonDown(0);
        }

        // Decision outcomes
        protected override void GoToActionSelection()
        {
            machine.SetState(
                new PlayerActionSelection(
                    combatant
                )
            );
            return;
        }

        protected override void GoToTileConfirmation()
        {
            machine.SetState(
                new PlayerMovementConfirmation(
                    combatant,
                    path
                )
            );
            return;
        }


        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found under the mouse.
        /// </summary>
        protected override OverlayTile GetNewFocus()
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
    }
}
