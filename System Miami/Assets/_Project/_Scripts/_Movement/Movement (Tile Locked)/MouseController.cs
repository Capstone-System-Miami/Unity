using UnityEngine;
using SystemMiami.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SystemMiami
{
    public class MouseController : MonoBehaviour
    {
        public float speed;
        public GameObject characterPrefab;
        private Combatant character;

        private PathFinder pathFinder;
        private List<OverlayTile> path = new List<OverlayTile>();

        private void Start()
        {
            pathFinder = new PathFinder();

            // Remove character instantiation from Start()
            // We'll instantiate the character when the player clicks on a starting tile
        }

        // Update is called once per frame
        void LateUpdate()
        {
            // Check if it's player's turn
            if (TurnManager.Instance.isPlayerTurn)
            {
                // Handle input for ending phases
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (TurnManager.Instance.currentPhase == Phase.MovementPhase)
                    {
                        // End movement phase
                        TurnManager.Instance.EndMovementPhase();
                    }
                    else if (TurnManager.Instance.currentPhase == Phase.ActionPhase)
                    {
                        // End player turn
                        TurnManager.Instance.EndPlayerTurn();
                    }
                }

                // Handle movement phase
                if (TurnManager.Instance.currentPhase == Phase.MovementPhase)
                {
                    // Allow player to select a tile to move to
                    var focusedTileHit = GetFocusedOnTile();

                    if (focusedTileHit.HasValue)
                    {
                        OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();

                        if (Input.GetMouseButtonDown(0))
                        {
                            // If character hasn't been instantiated yet, instantiate and place on the clicked tile
                            if (character == null)
                            {
                                character = Instantiate(characterPrefab).GetComponent<Combatant>();
                                PositionCharacterOnTile(overlayTile);

                                // Add character to TurnManager
                                TurnManager.Instance.playerCharacters.Add(character);

                                Debug.Log("Character placed on the map.");
                            }
                            else
                            {
                                // Check if character has movement points
                                if (character.movementPoints > 0)
                                {
                                    // Calculate path
                                    path = pathFinder.FindPath(character.activeTile, overlayTile);

                                    // Check if path length is within movement points
                                    if (path.Count <= character.movementPoints)
                                    {
                                        // Subtract movement points
                                        character.movementPoints -= path.Count;

                                        // Start moving along path
                                        // Note: We should not call MoveAlongPath() here; instead, we should let Update() handle the movement
                                    }
                                    else
                                    {
                                        // Not enough movement points
                                        Debug.Log("Not enough movement points to move to that tile.");
                                    }
                                }
                                else
                                {
                                    // No movement points left
                                    Debug.Log("No movement points left.");
                                }
                            }
                        }
                    }

                    if (path.Count > 0)
                    {
                        MoveAlongPath();
                    }
                }
                // Handle action phase
                else if (TurnManager.Instance.currentPhase == Phase.ActionPhase)
                {
                    // Allow player to perform an action if they haven't already
                    if (!character.hasActed)
                    {
                        // Placeholder for action selection
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            // Perform basic attack
                            character.hasActed = true;
                            Debug.Log("Player performed basic attack.");
                        }
                    }
                    else
                    {
                        // Player has already acted
                        Debug.Log("Player has already performed an action.");
                    }
                }
            }
        }

        /// <summary>
        /// Moves the character along the calculated path.
        /// </summary>
        private void MoveAlongPath()
        {
            float step = speed * Time.deltaTime;

            if (path.Count > 0)
            {
                OverlayTile targetTile = path[0];
                float zIndex = targetTile.transform.position.z;
                character.transform.position = Vector2.MoveTowards(character.transform.position, targetTile.transform.position, step);
                character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

                if (Vector2.Distance(character.transform.position, targetTile.transform.position) < 0.0001f)
                {
                    PositionCharacterOnTile(targetTile);
                    path.RemoveAt(0);
                }
            }
            else
            {
                // Movement finished
                // You can check if player wants to end movement phase
            }
        }

        /// <summary>
        /// Gets the tile that the mouse is currently over.
        /// </summary>
        public RaycastHit2D? GetFocusedOnTile()
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
        /// Positions the character on the specified tile.
        /// </summary>
        private void PositionCharacterOnTile(OverlayTile tile)
        {
            // Clear previous tile's currentCharacter
            if (character.activeTile != null)
            {
                character.activeTile.currentCharacter = null;
            }

            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            // Update activeTile
            character.activeTile = tile;

            // Set tile's currentCharacter
            tile.currentCharacter = character;
        }
    }
}
