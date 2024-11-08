using System;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    // TODO:
    // This is just a cleaned up version of the previous MouseController.
    // It should be refactored into an abstract class that
    // A player controller & enemy controller can derive from.
    public class CombatantController : MonoBehaviour
    {
        public float movementSpeed;

        private Combatant combatant;

        private PathFinder pathFinder;
        private List<OverlayTile> path = new List<OverlayTile>();
        private int currentPathCost;

        public event Action<OverlayTile> FocusedTileChanged;
        public event Action<DirectionalInfo> PathTileChanged;

        // PLAYER
        private OverlayTile _mostRecentMouseTile;

        // PLAYER
        public OverlayTile MostRecentMouseTile { get { return _mostRecentMouseTile; } }

        private void Start()
        {
            pathFinder = new PathFinder();

            if(!TryGetComponent(out combatant))
            {
                print($"Didnt find a Combatant component on {name}.");
            }      
        }

        private void Update()
        {
            if (_mostRecentMouseTile == null)
            {
                resetMouseTile();
            }
            else if (TurnManager.Instance.isPlayerTurn)
            {
                setMouseTile();
            }
        }

        // ABSTRACT ===================================
        /// <summary>
        /// Checks the MouseController for an overlay tile under the cursor.
        /// If one is found, MostRecentMouseTile is set to the tile.
        /// If none is found, MostRecentMouseTile is unchanged.
        /// </summary>
        private void setMouseTile()
        {
            RaycastHit2D? mouseHit = getMouseHitInfo();
            OverlayTile mouseTile = getTileFromRaycast(mouseHit);

            if (mouseTile == null) { return; }
            if (mouseTile == _mostRecentMouseTile) { return; }

            _mostRecentMouseTile = mouseTile;

            // Raise event when mouse tile  changes
            FocusedTileChanged?.Invoke(mouseTile);
        }

        // ABSTRACT ===========================================
        /// <summary>
        /// Resets the mouse tile.
        /// If character's CurrentTile is found, MostRecentMouseTile is set to that.
        /// If character's CurrentTile is not found, MRMT is set to a random tile.
        /// </summary>
        private void resetMouseTile()
        {
            if(combatant.CurrentTile != null)
            {
                MapManager.MGR.map.TryGetValue((Vector2Int)combatant.CurrentTile.gridLocation, out _mostRecentMouseTile);
            }
            else
            {
                _mostRecentMouseTile = MapManager.MGR.GetRandomUnblockedTile();
            }

            FocusedTileChanged?.Invoke(_mostRecentMouseTile);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!IsMyTurn()) { return; }

            // Q To end turn
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TurnManager.Instance.EndPlayerTurn();
            }

            if (TurnManager.Instance.currentPhase != Phase.MovementPhase) { return; }
            
         
            // E to end movement
            if (Input.GetKeyDown(KeyCode.E))
            {
                TurnManager.Instance.EndMovementPhase();
            }
     
            // Allow _player to select a tile to move to
            RaycastHit2D? focusedTileHit = getMouseHitInfo();
            OverlayTile focusedTile = getTileFromRaycast(focusedTileHit);
     
            if (focusedTile != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    createPathTo(focusedTile);
                }
            }
            
            moveAlongPath();
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void moveAlongPath()
        {
            if (path.Count <= 0)
            {
                combatant.IsMoving = false;
                return;
            }

            combatant.IsMoving = true;

            float step = movementSpeed * Time.deltaTime;

            //TODO
            //i want to add it here so that you have to
            //confirm the movement too so that the arrows
            //show up before you move and can show your path
            OverlayTile targetTile = path[0];
            float zIndex = targetTile.transform.position.z;
            combatant.transform.position = Vector2.MoveTowards(combatant.transform.position, targetTile.transform.position, step);
            combatant.transform.position = new Vector3(combatant.transform.position.x, combatant.transform.position.y, zIndex);

            // If character is close enough to a new tile
            if (Vector2.Distance(combatant.transform.position, targetTile.transform.position) < 0.0001f)
            {
                // Directional info based on the current tile
                // and the one we're moving to.
                DirectionalInfo newDir = new DirectionalInfo((Vector2Int)combatant.CurrentTile.gridLocation, (Vector2Int)targetTile.gridLocation);

                // Let any subscribers know that we are moving along path
                PathTileChanged(newDir);

                positionCharacterOnTile(targetTile);
                path.RemoveAt(0);
            }

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

        private OverlayTile getTileFromRaycast(RaycastHit2D? hit)
        {
            if (!hit.HasValue) { return null; }

            return hit.Value.collider.gameObject.GetComponent<OverlayTile>();  
        }

        /// <summary>
        /// Positions the character on the specified tile.
        /// </summary>
        private void positionCharacterOnTile(OverlayTile tile)
        {
            combatant.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);

            //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            // Update CurrentTile
            combatant.CurrentTile = tile;

            // Set tile's currentCharacter
            tile.currentCharacter = combatant;
        }

        private void createPathTo(OverlayTile tile)
        {
            // Calculate path
            path = pathFinder.FindPath(combatant.CurrentTile, tile);
            currentPathCost = path.Count;

            // Check if path length is within movement points
            if (path.Count <= combatant.Speed.Get())
            {
                // Subtract movement points
                //combatant.Speed.Lose(path.Count);

                // Start moving along path
                // Note: We should not call MoveAlongPath() here; instead, we should let Update() handle the movement
            }
            else
            {
                // Not enough movement points
                Debug.Log("Not enough movement points to move to that tile.");
            }
        }

        private void updateMovement()
        {

        }

        public bool IsMyTurn()
        {
            // TODO: Ask turn manager
            return TurnManager.Instance.isPlayerTurn;
        }

        // ABSTRACT
        public void EndTurn()
        {

        }

        // ABSTRACT
        public void EndMovementPhase()
        {

        }
    }
}
