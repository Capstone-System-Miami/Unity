using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class ActionSelection : CombatantState
    {
        private OverlayTile highlightOnlyFocusTile;

        public ActionSelection(Combatant combatant)
            : base(combatant, Phase.Action) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();

            // Find a new possible focus tile
            // by the means described
            // by int the derived classes.
            if (!TryGetNewFocus(out OverlayTile newFocus))
            {
                // Focus was not new.
                // Nothing to update.
                return;
            }

            // Update currentTile & tile hover
            highlightOnlyFocusTile?.EndHover(combatant);
            highlightOnlyFocusTile = newFocus;
            highlightOnlyFocusTile?.BeginHover(combatant);
        }

        public override void MakeDecision()
        {
            if (EquipRequested())
            {
                SwitchState(factory.ActionEquipped());
                return;
            }

            if (SkipPhaseRequested())
            {
                SwitchState(factory.TurnEnd());
                return;
            }
        }

        // Decision
        protected abstract bool EquipRequested();
        protected abstract bool SkipPhaseRequested();



        // Focus
        protected bool TryGetNewFocus(out OverlayTile newFocus)
        {
            newFocus = GetNewFocus() ?? combatant.GetDefaultFocus();

            return newFocus != highlightOnlyFocusTile;
        }

        /// <summary>
        /// Checks for an overlay tile under the cursor.
        /// Returns null if no tile is found under the mouse.
        /// </summary>
        protected OverlayTile GetNewFocus()
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
