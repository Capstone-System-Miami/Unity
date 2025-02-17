// Authors: Layla Hoey, Lee St Louis
using System.Linq;
using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class PlayerCombatant : Combatant
    {
        public NewAbilitySO test;
        
        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found under the mouse.
        /// </summary>
        public override OverlayTile GetNewFocus()
        {
            RaycastHit2D? mouseHit = GetMouseHitInfo();
            OverlayTile mouseTile = GetTileFromRaycast(mouseHit);

            return mouseTile;
        }

        /// <summary>
        /// Asks <see cref="UI"/> to create a loadout for this
        /// combatant. When <see cref="UI"/> is done, it will raise
        /// an event which combatants subscribe to during OnEnable().
        /// <para>
        /// This Method should be called during start.</para>
        /// <para>
        /// See <see cref="UI.CombatantLoadoutCreated"/></para>
        /// <para>
        /// See also <seealso cref="Combatant.HandleLoadoutCreated(Loadout, Combatant)"/>
        /// </para>
        /// </summary>
        protected override void InitLoadout()
        {
            UI.MGR.CreatePlayerLoadout(this);
        }

        /// <summary>
        /// Gets the raycastHit info for whatever
        /// the mouse is currently over.
        /// </summary>
        /// <returns>
        /// A <c>nullable</c>-type <see cref="RaycastHit2D"/>
        /// </returns>
        private RaycastHit2D? GetMouseHitInfo()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos2d = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

            return hits.Length > 0
                ? hits.OrderByDescending(i => i.collider.transform.position.z).First()
                : null;
        }

        /// <summary>
        /// Takes nullable type RaycastHit info and returns
        /// either the tile found within the Hit,
        /// or null if no tile was found in the Hit.
        /// </summary>
        /// <returns>
        /// The <see cref="OverlayTile"/> hit by the 
        /// <see cref="RaycastHit2D"/>, (IF ANY)</returns>
        private OverlayTile GetTileFromRaycast(RaycastHit2D? hit)
        {
            return hit.HasValue 
                ? hit.Value.collider.gameObject.GetComponent<OverlayTile>()
                : null;
        }
    }
}
