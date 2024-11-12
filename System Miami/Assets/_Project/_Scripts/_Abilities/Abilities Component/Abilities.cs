// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;

namespace SystemMiami.AbilitySystem
{
    /// <summary>
    /// Manages the abilities of a combatant, handling selection, targeting, and execution.
    /// </summary>
    public class Abilities : MonoBehaviour
    {
        #region EVENTS
        // ======================================

        public Action AbilityUnequipped;
        public Action<Ability> AbilityEquipped;
        public Action<Ability> TargetsLocked;
        public Action<Ability> ExecuteAbilityStarted;
        public Action<Ability> ExecuteAbilityCompleted;

        #endregion // EVENTS ====================


        #region STATE
        // ======================================
        public enum State
        {
            UNEQUIPPED,
            EQUIPPED,
            TARGETS_LOCKED,
            EXECUTING,
            COMPLETE,
        }

        private class AbilityState
        {
            private Abilities _owner;
            private Abilities.State _state;
            private Ability _selectedAbility;

            public Abilities.State Get()
            {
                return _state;
            }

            public void Set(Abilities.State newState)
            {
                if (!validSet()) { return; }

                _state = newState;
                _selectedAbility = _owner._selectedAbility;

                switch (newState)
                {
                    case Abilities.State.UNEQUIPPED:
                        _owner.AbilityUnequipped?.Invoke();
                        break;
                    case Abilities.State.EQUIPPED:
                        _owner.AbilityEquipped?.Invoke(_selectedAbility);
                            break;
                    case Abilities.State.TARGETS_LOCKED:
                        _owner.TargetsLocked?.Invoke(_selectedAbility);
                        break;
                    case Abilities.State.EXECUTING:
                        _owner.ExecuteAbilityStarted?.Invoke(_selectedAbility);
                        break;
                    case Abilities.State.COMPLETE:
                        _owner.ExecuteAbilityCompleted?.Invoke(_selectedAbility);
                        break;
                }
            }

            public AbilityState(Abilities owner, Abilities.State state)
            {
                _owner = owner;
                _state = state;
                _selectedAbility = _owner._selectedAbility;
            }

            private bool validSet()
            {
                if (_owner == null)
                {
                    Debug.LogWarning("Something is trying to set a new" +
                        "AbilityState, but the owner is null.");

                    return false;
                }

                if (_state != Abilities.State.UNEQUIPPED && _selectedAbility == null)
                {
                    Debug.LogWarning($"{_owner.name} is trying to set a new" +
                        $"AbilityState, but has no selected ability");
                    return false;
                }

                return true;
            }
        }
        #endregion // STATE =====================


        #region SERIALIZED
        // ======================================

        [SerializeField] private List<Ability> _physical = new List<Ability>(); // List of phys abilities
        [SerializeField] private List<Ability> _magical = new List<Ability>(); // List of magic abilities

        #endregion // SERIALIZED ================


        #region PRIVATE VARS
        // ======================================

        private Combatant _combatant; // Reference to the combatant component

        private Ability _selectedAbility; // Currently selected ability
        //private bool _isTargeting = false; // check if in targeting mode
        //private bool _targetsLocked = false; // check if targets are locked
        //private bool _isUsing = false; // check if the ability has been confirmed

        private AbilityState _state;

        #endregion // PRIVATE VARS ==============


        #region PROPERTIES
        // ======================================
        public List<Ability> Physical { get { return _physical; } }
        public List<Ability> Magical { get { return _magical; } }

        public Abilities.State CurrentState { get { return _state.Get(); } }

        public Ability SelectedAbility { get { return _selectedAbility; } }

        #endregion // PROPERTIES ================


        #region UNITY METHODS
        // ======================================

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
            _state = new AbilityState(this, Abilities.State.UNEQUIPPED);
        }

        #endregion // UNITY METHODS ==============


        #region SUBSCRIPTIONS
        // ======================================
        //private void onSlotClicked(AbilitySlot slot)
        //{

        //}

        ///// <summary>
        ///// Called when the left mouse button is clicked.
        ///// </summary>
        //private void OnLeftMouseDown()
        //{
        //    Debug.Log("OnLeftMouseDown called.");
        //    TryLockTargets();
        //}

        ///// <summary>
        ///// Called when the right mouse button is clicked.
        ///// </summary>
        //private void OnRightMouseDown()
        //{
        //    if (_state.Get() == State.EQUIPPED)
        //    {
        //        unequip();
        //    }
        //}

        ///// <summary>
        ///// Called when the Enter key is pressed.
        ///// </summary>
        //private void OnEnterPressed()
        //{
        //    if (_state.Get() == State.TARGETS_LOCKED)
        //    {
        //        StartCoroutine(executeSelected());
        //    }
        //}

        #endregion // SUBSCRIPTIONS ============


        #region PUBLIC METHODS
        // ======================================
        /// <summary>
        /// Equip the ability at the index within the list corresponding to the type.
        /// </summary>
        public bool TryEquip(AbilityType type, int index)
        {
            if (!_combatant.Controller.IsMyTurn)
            {
                Debug.LogWarning($"{name} is trying to equip an ability, " +
                    $"but it is not their turn.");
                return false;
            }

            if (index < 0)
            {
                Debug.LogWarning($"{ name } is trying to equip an ability, " +
                    $"but the provided index is negative.");
                return false;
            }

            if (getAbility(type, index) == null)
            {
                Debug.LogWarning($"{ name } is trying to equip an ability, " +
                    $"but the provided index is invalid " +
                    $"for the {type} Abilities list.");
                return false;
            }

            equip(type, index);
            return true;
        }

        /// <summary>
        /// To be called when the combatant unequips the ability (e.g., by right-clicking).
        /// Cancels the ability, removes the preview, and allows equipping a different ability.
        /// </summary>
        public bool TryUnequip()
        {
            unequip();
            return true;
        }

        public bool TryLockTargets()
        {
            if (_state.Get() != State.EQUIPPED)
            {
                Debug.LogWarning($"{ name } failed to lock targets. " +
                    $"Either not targeting or targets already locked.");
                return false;
            }

            lockTargets();
            return true;
        }

        /// <summary>
        /// Returns a bool for whether the execution is valid,
        /// as well as returning (as an out param) the process
        /// to be started as a coroutine elsewhere.
        /// The provess will execute the ability, starting cooldowns,
        /// applying status effects, and handling animations.
        /// </summary>
        public bool AbilityExecutionIsValid(out IEnumerator abilityProcess)
        {
            // Validate
            if (_selectedAbility == null)
            {
                Debug.LogWarning($"{ name } failed to execute an ability. " +
                    $"There was no ability selected.");
                abilityProcess = null;
                return false;
            }

            if (_combatant == null)
            {
                Debug.LogWarning($"{name} failed to execute an ability. " +
                    $"_combatant was null.");
                abilityProcess = null;
                return false;
            }

            if (_combatant.Animator == null)
            {
                Debug.LogWarning($"{name} failed to execute an ability. " +
                    $"There was no Animator on { _combatant }.");
                abilityProcess = null;
                return false;
            }

            if (_selectedAbility._overrideController == null)
            {
                Debug.LogWarning($"{ name } failed to execute an ability. " +
                    $"There was no override controller on the selected ability.");
                abilityProcess = null;
                return false;
            }

            if (_state.Get() != State.TARGETS_LOCKED)
            {
                Debug.LogWarning($"{ name } failed to execute an ability. " +
                    $"Targets are not locked.");
                abilityProcess = null;
                return false;
            }

            abilityProcess = executeSelected();
            return true;
        }

        /// <summary>
        /// Adds a new ability to the combatant.
        /// </summary>
        public void AddAbility(Ability newAbility)
        {
            List<Ability> pool = newAbility.Type switch
            {
                AbilityType.PHYSICAL    => _physical,
                AbilityType.MAGICAL     => _magical,
                _                       => _physical
            };

            if (!pool.Contains(newAbility))
            {
                pool.Add(newAbility);
                newAbility.Init(_combatant);
            }
        }

        /// <summary>
        /// Reduces cooldowns for all abilities at the end of the turn.
        /// </summary>
        public void ReduceCooldowns()
        {
            foreach (Ability ability in _physical)
            {
                ability.ReduceCooldown();
            }
        }

        #endregion // PUBLIC METHODS ============


        #region PRIVATE METHODS
        // ======================================

        /// <summary>
        /// Equip the ability at the index within the list corresponding to the type.
        /// </summary>
        private void equip(AbilityType type, int index)
        {
            // If already targeting an ability, unequip it
            if (_selectedAbility != null)
            {
                unequip();
            }

            // Select the new ability
            _selectedAbility = getAbility(type, index);
            if (_selectedAbility == null) { return; }

            _selectedAbility.Init(_combatant);

            // Begin targeting mode
            _selectedAbility.BeginTargeting();

            // Subscribe to input events
            //if (InputManager.Instance != null)
            //{
            //    InputManager.Instance.EnterPressed += OnEnterPressed;
            //    InputManager.Instance.LeftMouseDown += OnLeftMouseDown;
            //    InputManager.Instance.RightMouseDown += OnRightMouseDown;
            //}

            _state.Set(State.EQUIPPED);
        }

        /// <summary>
        /// To be called when the combatant unequips the ability (e.g., by right-clicking).
        /// Cancels the ability, removes the preview, and allows equipping a different ability.
        /// </summary>
        private void unequip()
        {
            Debug.Log("Unequip called.");

            _selectedAbility.CancelTargeting();

            // Unsubscribe from input events
            //if (InputManager.Instance != null)
            //{
            //    InputManager.Instance.EnterPressed -= OnEnterPressed;
            //    InputManager.Instance.LeftMouseDown -= OnLeftMouseDown;
            //    InputManager.Instance.RightMouseDown -= OnRightMouseDown;
            //}

            // Clear the selected ability
            _selectedAbility = null;

            _state.Set(State.UNEQUIPPED);
        }

        /// <summary>
        /// Locks the target tiles and combatants for every
        /// combat action housed in the equipped ability.
        /// </summary>
        private void lockTargets()
        {
            Debug.Log("Locking targets.");
            _selectedAbility.LockTargets();

            _state.Set(State.TARGETS_LOCKED);
        }

        /// <summary>
        /// Executes the ability, starting cooldowns, applying status effects, and handling animations.
        /// </summary>
        private IEnumerator executeSelected()
        {
            // Prepare
            _state.Set(State.EXECUTING);
            yield return null;

            // Start Animation
            _combatant.Animator.runtimeAnimatorController = _selectedAbility._overrideController;
            _combatant.Animator.SetTrigger("UseAbility");

            // TODO: Wait for the animation to finish
            // For now, just wait 3 secs
            for (int i = 3; i >= 0; i--)
            {
                Debug.Log($"Placeholder for animation time. Activates in {i} seconds.");
                yield return new WaitForSeconds(1f);
            }

            // Perform the ability actions
            yield return StartCoroutine(_selectedAbility.Use());

            _state.Set(State.COMPLETE);
            yield return null;

            // Unequip when the Ability's Use() coroutine is over.
            unequip();
            yield return null;
        }

        private Ability getAbility(AbilityType type, int index)
        {
            List<Ability> typePool = type switch
            {
                AbilityType.PHYSICAL => _physical,
                AbilityType.MAGICAL => _magical,
                _ => _physical
            };

            if (index >= typePool.Count) { return null; }

            return typePool[index];
        }

        #endregion // PRIVATE METHODS ===========
    }
}
