// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.XR;

namespace SystemMiami.AbilitySystem
{
    /// <summary>
    /// Manages the abilities of a combatant, handling selection, targeting, and execution.
    /// </summary>
    public class Abilities : MonoBehaviour
    {
        // ======================================
        #region Serialized

        [SerializeField] private List<Ability> _physical = new List<Ability>(); // List of phys abilities
        [SerializeField] private List<Ability> _magical = new List<Ability>(); // List of magic abilities

        #endregion // Serialized ================

        // ======================================
        #region Private Vars
        #endregion // Private Vars
        [SerializeField] Ability _selectedAbility; // Currently selected ability
        [SerializeField] private bool _isTargeting = false; // check if in targeting mode
        [SerializeField] private bool _isUsing = false; // check if the ability has been confirmed
        [SerializeField] private bool _isConfirming = false; // heck if targets are locked

        private Combatant _combatant; // Reference to the combatant component

        public List<Ability> Physical { get { return _physical; } }
        public List<Ability> Magical { get { return _magical; } }

        public Action<AbilityType, int> EquipAbility;
        public Action UnequipAbility;
        public Action<Ability> LockTargets;
        public Action<Ability> UseAbility;

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
        }

        private void OnEnable()
        {
            EquipAbility += OnEquip;
            UnequipAbility += OnUnequip;
            UI.MGR.SlotClicked += onSlotClicked;
        }

        private void OnDisable()
        {
            EquipAbility -= OnEquip;
            UnequipAbility -= OnUnequip;
            UI.MGR.SlotClicked -= onSlotClicked;
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

        private void onSlotClicked(AbilitySlot slot)
        {
            if (getAbility(slot.Type, slot.Index) == _selectedAbility)
            {
                UnequipAbility.Invoke();
            }
            else
            {
                EquipAbility.Invoke(slot.Type, slot.Index);
            }
        }

        /// <summary>
        /// Called when the left mouse button is clicked.
        /// </summary>
        private void OnLeftMouseDown()
        {
            Debug.Log("OnLeftMouseDown called.");
            if (_isTargeting && !_isConfirming)
            {
                Debug.Log("Locking targets.");
                _selectedAbility.ConfirmTargets();
                _isConfirming = true;

                LockTargets.Invoke(_selectedAbility);
            }
            else
            {
                Debug.Log("Left-click ignored. Either not targeting or targets already locked.");
            }
        }

        /// <summary>
        /// Called when the right mouse button is clicked.
        /// </summary>
        private void OnRightMouseDown()
        {
            if (_isTargeting)
            {
                UnequipAbility.Invoke();
            }
        }

        /// <summary>
        /// Called when the Enter key is pressed.
        /// </summary>
        private void OnEnterPressed()
        {
            if (_isTargeting && _isConfirming && !_isUsing)
            {
                OnConfirm();
            }
        }

        // CALL THIS FROM ABILITY BUTTON
        /// <summary>
        /// Called when an ability is equipped (selected) by pressing the corresponding key.
        /// </summary>
        /// <param name="index">Index of the ability in the abilities list.</param>
        private void OnEquip(AbilityType type, int index)
        {
            if (!TurnManager.MGR.IsPlayerTurn) { return; }
            if (index < 0) { return; }

            
            // If already targeting an ability, unequip it
            if (_isTargeting)
            {
                UnequipAbility.Invoke();
            }

            // Select the new ability
            _selectedAbility = getAbility(type, index);
            if (_selectedAbility == null) { return; }

            _selectedAbility.Init(_combatant);

            // Begin targeting mode
            _selectedAbility.BeginTargeting();
            _isTargeting = true;
            _isUsing = false;
            _isConfirming = false;

            // Subscribe to input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.EnterPressed += OnEnterPressed;
                InputManager.Instance.LeftMouseDown += OnLeftMouseDown;
                InputManager.Instance.RightMouseDown += OnRightMouseDown;
            }
        }

        /// <summary>
        /// Called when the _player confirms the ability usage (e.g., by pressing Enter).
        /// Executes the ability.
        /// </summary>
        private void OnConfirm()
        {
            if (_selectedAbility == null) { return; }
            if (!_isTargeting) { return; }
            if (!_isConfirming) { return; }

            // Ability is confirmed
            _isUsing = true;

            // Execute the ability
            StartCoroutine(OnUse());

            UseAbility.Invoke(_selectedAbility);
        }

        /// <summary>
        /// Executes the ability, starting cooldowns, applying status effects, and handling animations.
        /// </summary>
        private System.Collections.IEnumerator OnUse()
        {
            if (_selectedAbility._overrideController == null) { yield break; }
            if (_combatant.Animator == null) { yield break; }

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

            // Unequip when the Ability's Use() coroutine is over.
            UnequipAbility.Invoke();
        }

        /// <summary>
        /// Called when the _player unequips the ability (e.g., by right-clicking).
        /// Cancels the ability, removes the preview, and allows equipping a different ability.
        /// </summary>
        private void OnUnequip()
        {
            Debug.Log("OnUnequip called.");
            if (_selectedAbility == null) { return; }
            if (!_isTargeting) { return; }

            // Cancel the ability, which hides the targets
            _selectedAbility.CancelTargeting();
            _isTargeting = false;
            _isUsing = false;
            _isConfirming = false;

            // Unsubscribe from input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.EnterPressed -= OnEnterPressed;
                InputManager.Instance.LeftMouseDown -= OnLeftMouseDown;
                InputManager.Instance.RightMouseDown -= OnRightMouseDown;
            }

            // Clear the selected ability
            _selectedAbility = null;            
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
    }
}
