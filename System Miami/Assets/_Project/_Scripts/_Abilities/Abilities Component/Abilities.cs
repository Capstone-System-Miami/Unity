// Authors: Layla Hoey, Lee St. Louis
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    /// <summary>
    /// Manages the abilities of a combatant, handling selection, targeting, and execution.
    /// </summary>
    public class Abilities : MonoBehaviour
    {
        [SerializeField] private KeyCode[] _keys =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
        };

        [SerializeField] private List<Ability> _abilities = new List<Ability>();
        [SerializeField] private Ability _selectedAbility;
        [SerializeField] private bool _isTargeting = false;
        
        [SerializeField] private GameObject[] targs;

        private Combatant _combatant;
        private MouseController _mouseController;

        private Vector2Int startFrameDirection;
        private Vector2Int endFrameDirection;

        private bool _isUpdating;

        TargetingPattern _targetingPattern;

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
            _mouseController = _combatant.GetComponent<MouseController>();
        }

        private void OnEnable()
        {
            // Initialize abilities with the combatant reference
            foreach (Ability ability in _abilities)
            {
                ability.Init(_combatant);
            }

            // Subscribe to InputManager events
            InputManager.Instance.OnAbilityKeyPressed += HandleAbilityKeyPressed;
            InputManager.Instance.OnLeftMouseDown += HandleLeftMouseDown;
            InputManager.Instance.OnRightMouseDown += HandleRightMouseDown;
            InputManager.Instance.OnEnterPressed += HandleEnterPressed;
        }

        private void OnDisable()
        {
            // Unsubscribe from InputManager events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnAbilityKeyPressed -= HandleAbilityKeyPressed;
                InputManager.Instance.OnLeftMouseDown -= HandleLeftMouseDown;
                InputManager.Instance.OnRightMouseDown -= HandleRightMouseDown;
                InputManager.Instance.OnEnterPressed -= HandleEnterPressed;
            }

            // Unsubscribe from MouseController event
            if (_mouseController != null)
            {
                _mouseController.OnMouseTileChanged -= HandleMouseTile;
            }
        }

        /// <summary>
        /// Adds a new ability to the combatant. //thought of an easier way than resource folder, just make them game objects lol
        /// </summary>
        public void AddAbility(Ability newAbility)
        {
            if (!_abilities.Contains(newAbility))
            {
                _abilities.Add(newAbility);
                newAbility.Init(_combatant);
            }
        }

        /// <summary>
        /// Reduces cooldowns for all abilities at the end of the turn.
        /// </summary>
        public void ReduceCooldowns()
        {
            foreach (Ability ability in _abilities)
            {
                ability.ReduceCooldown();
            }
        }
        #region InputEvents


        /// <summary>
        /// Handles ability selection when an ability key is pressed.
        /// </summary>
        /// <param name="index">Index of the ability key pressed.</param>
        private void HandleAbilityKeyPressed(int index)
        {
            if (index >= 0 && index < _abilities.Count)
            {
                _selectedAbility = _abilities[index];
                _selectedAbility?.Init(_combatant);
            }
        }

        /// <summary>
        /// Handles entering targeting mode on left mouse click.
        /// </summary>
        private void HandleLeftMouseDown()
        {
            if (_selectedAbility != null)
            {
                if (!_isTargeting)
                {
                    _selectedAbility.BeginTargeting();
                    _isTargeting = true;

                    //subscribe to MouseController's tile change event
                    _mouseController.OnMouseTileChanged += HandleMouseTile;
                }
            }
        }

        /// <summary>
        /// Handles exiting targeting mode on right mouse click.
        /// </summary>
        private void HandleRightMouseDown()
        {
            if (_selectedAbility != null)
            {
                if (_isTargeting)
                {
                    _selectedAbility.CancelTargeting();
                    _isTargeting = false;

                    //unsubscribe
                    _mouseController.OnMouseTileChanged -= HandleMouseTile;
                }
            }
        }

        /// <summary>
        /// Handles ability execution when Enter key is pressed.
        /// </summary>
        private void HandleEnterPressed()
        {
            if (_selectedAbility != null && _isTargeting)
            {
                Debug.Log("Press enter");
                StartCoroutine(_selectedAbility.Use());
                _isTargeting = false;
                _mouseController.OnMouseTileChanged -= HandleMouseTile;
            }
        }

        /// <summary>
        /// Updates target preview when the mouse moves over a new tile.
        /// </summary>
        /// <param name="tile">The new tile under the mouse cursor.</param>
        private void HandleMouseTile(OverlayTile tile)
        {
            ////update target preview based on new tile
            //if (_selectedAbility != null)
            //{
            //    StartCoroutine(_selectedAbility.ShowAllTargets());
            //}
            
        }
        #endregion
        #region LaylaStuff





        //private void LateUpdate()
        //{
            
        //    endFrameDirection = _combatant.DirectionInfo.DirectionVec;
        //    if (endFrameDirection != startFrameDirection)
        //    {
        //        if (_selectedAbility != null)
        //        {
        //            StartCoroutine(onDirectionChange());
        //        }
        //    }
        //}

        //private IEnumerator onDirectionChange()
        //{
        //    _isUpdating = true;
        //    yield return null;

        //    //StartCoroutine(_selectedAbility.HideAllTargets());//yield return new WaitUntil(() => !_selectedAbility.IsTargeting);
        //    //yield return null;

        //    StartCoroutine(_selectedAbility.ShowAllTargets());
        //    yield return new WaitUntil(() => _selectedAbility.IsTargeting);
        //    yield return null;

        //  _isUpdating = false;
        //}
        #endregion
    }
}
