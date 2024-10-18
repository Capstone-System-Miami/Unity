using System;
using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class MouseController : MonoBehaviour
    {
        public float speed;
        public GameObject characterPrefab;
        private Combatant character;

        private PathFinder pathFinder;
        private List<OverlayTile> path = new List<OverlayTile>();

        public event Action<OverlayTile> OnMouseTileChanged; // event for tile change

        #region Layla Added Vars
        private OverlayTile _mostRecentMouseTile;

        public OverlayTile MostRecentMouseTile { get { return _mostRecentMouseTile; } }
        public bool IsBusy { get; private set; }
        #endregion

        private void Start()
        {
            pathFinder = new PathFinder();

            if(!TryGetComponent(out character))
            {
                print($"Didnt find a Combatant component on {name}.");
            }

            // Remove character instantiation from Start()
            // We'll instantiate the character when the player clicks on a starting tile
            // (layla) ^^ I'm wondering why? ^^           
        }

        #region Layla Added Methods
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

        /// <summary>
        /// Checks the MouseController for an overlay tile under the cursor.
        /// If one is found, MostRecentMouseTile is set to the tile.
        /// If none is found, MostRecentMouseTile is unchanged.
        /// </summary>
        private void setMouseTile()
        {
            RaycastHit2D? mouseHit = GetFocusedOnTile();
            OverlayTile mouseTile = mouseHit?.collider.gameObject.GetComponent<OverlayTile>();

            if (mouseTile != null && mouseTile != _mostRecentMouseTile)
            {
                _mostRecentMouseTile = mouseTile;
                //raise event when mouse tile  changes //Lee change
                OnMouseTileChanged?.Invoke(mouseTile);
            }
        }

        /// <summary>
        /// Resets the mouse tile.
        /// If character's CurrentTile is found, MostRecentMouseTile is set to that.
        /// If character's CurrentTile is not found, MRMT is set to a random tile.
        /// </summary>
        private void resetMouseTile()
        {
            if(character.CurrentTile != null)
            {
                _mostRecentMouseTile = character.CurrentTile;
            }
            else
            {
                _mostRecentMouseTile = MapManager.MGR.GetRandomUnblockedTile();
            }
        }
        #endregion

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
                            IsBusy = true; // layla added

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
                                if (character.Speed.Get() > 0)
                                {
                                    // Calculate path
                                    path = pathFinder.FindPath(character.CurrentTile, overlayTile);

                                    // Check if path length is within movement points
                                    if (path.Count <= character.Speed.Get())
                                    {
                                        // Subtract movement points
                                        character.Speed.Lose(path.Count);

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

                    IsBusy = false; // layla added
                }
                // Handle action phase
                else if (TurnManager.Instance.currentPhase == Phase.ActionPhase)
                {
                    // Allow player to perform an action if they haven't already
                    if (!character.HasActed)
                    {
                        // Placeholder for action selection
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            // Perform basic attack
                            character.HasActed = true;
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
            if (character.CurrentTile != null)
            {
                character.CurrentTile.currentCharacter = null;
            }

            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            // Update CurrentTile
            character.CurrentTile = tile;

            // Set tile's currentCharacter
            tile.currentCharacter = character;
        }
    }
}
