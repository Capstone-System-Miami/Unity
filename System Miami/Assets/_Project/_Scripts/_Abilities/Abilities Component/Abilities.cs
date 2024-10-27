// Authors: Layla Hoey, Lee St Louis
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    /// <summary>
    /// Manages the abilities of a combatant, handling selection, targeting, and execution.
    /// </summary>
    public class Abilities : MonoBehaviour
    {
        [SerializeField] private List<Ability> _abilities = new List<Ability>(); // List of abilities 
        [SerializeField] Ability _selectedAbility; // Currently selected ability
        [SerializeField] private bool _isTargeting = false; // check if in targeting mode
        [SerializeField] private bool _isAbilityConfirmed = false; // check if the ability has been confirmed
        [SerializeField] private bool _areTargetsLocked = false; // heck if targets are locked

        private Combatant _combatant; // Reference to the combatant component

        private InputManager _inputManager; // Reference to the InputManager

        public List<Ability> List { get { return _abilities; } }

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
            _inputManager = InputManager.Instance;
           
            if (_inputManager != null)
            {
                // Always listen for ability key presses
                _inputManager.OnAbilityKeyPressed += OnAbilityKeyPressed;
            }
        }

        void OnDestroy()
        {
            if (_inputManager != null)
            {
                _inputManager.OnAbilityKeyPressed -= OnAbilityKeyPressed;
            }
        }

       
        /// <summary>
        /// Called when an ability key is pressed.
        /// </summary>
        private void OnAbilityKeyPressed(int index)
        {
            OnEquip(index);
        }

        /// <summary>
        /// Called when the left mouse button is clicked.
        /// </summary>
        private void OnLeftMouseDown()
        {
            Debug.Log("OnLeftMouseDown called.");
            if (_isTargeting && !_areTargetsLocked)
            {
                Debug.Log("Locking targets.");
                _selectedAbility.ConfirmTargets();
                _areTargetsLocked = true;
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
                OnUnequip();
            }
        }

        /// <summary>
        /// Called when the Enter key is pressed.
        /// </summary>
        private void OnEnterPressed()
        {
            if (_isTargeting && _areTargetsLocked && !_isAbilityConfirmed)
            {
                OnConfirm();
            }
        }

        // CALL THIS FROM ABILITY BUTTON
        /// <summary>
        /// Called when an ability is equipped (selected) by pressing the corresponding key.
        /// </summary>
        /// <param name="index">Index of the ability in the abilities list.</param>
        public void OnEquip(int index)
        {
            if (index >= 0 && index < _abilities.Count)
            {
                // If already targeting an ability, unequip it
                if (_isTargeting)
                {
                    OnUnequip();
                }

                // Select the new ability
                _selectedAbility = _abilities[index];
                _selectedAbility?.Init(_combatant);

                // Begin targeting mode
                _selectedAbility.BeginTargeting();
                _isTargeting = true;
                _isAbilityConfirmed = false;
                _areTargetsLocked = false;

                // Subscribe to input events
                if (_inputManager != null)
                {
                    _inputManager.OnEnterPressed += OnEnterPressed;
                    _inputManager.OnLeftMouseDown += OnLeftMouseDown;
                    _inputManager.OnRightMouseDown += OnRightMouseDown;
                }
            }
        }

        /// <summary>
        /// Called when the player confirms the ability usage (e.g., by pressing Enter).
        /// Executes the ability.
        /// </summary>
        private void OnConfirm()
        {
            if (_selectedAbility != null && _isTargeting && _areTargetsLocked)
            {
                // Ability is confirmed
                _isAbilityConfirmed = true;

                // Execute the ability
                StartCoroutine(OnUse());
            }
        }

        /// <summary>
        /// Executes the ability, starting cooldowns, applying status effects, and handling animations.
        /// </summary>
        private System.Collections.IEnumerator OnUse()
        {
            
            if (_selectedAbility._overrideController != null && _combatant.Animator != null)
            {
                _combatant.Animator.runtimeAnimatorController = _selectedAbility._overrideController;
                _combatant.Animator.SetTrigger("UseAbility");
            }

            // Wait for the animation to finish TODO
            // For now, just wait 5 secs
            for (int i = 5; i >= 0; i--)
            {
                Debug.Log($"Placeholder for animation time. Activates in {i} seconds.");
                yield return new WaitForSeconds(1f);
            }

            // Perform the ability actions
            yield return StartCoroutine(_selectedAbility.Use());

            
            // For now,targets are locked until unequipped can change later
        }

        /// <summary>
        /// Called when the player unequips the ability (e.g., by right-clicking).
        /// Cancels the ability, removes the preview, and allows equipping a different ability.
        /// </summary>
        public void OnUnequip()
        {
            Debug.Log("OnUnequip called.");
            if (_selectedAbility != null && _isTargeting)
            {
                // Cancel the ability, which hides the targets
                _selectedAbility.CancelTargeting();
                _isTargeting = false;
                _isAbilityConfirmed = false;
                _areTargetsLocked = false;

                // Unsubscribe from input events
                if (_inputManager != null)
                {
                    _inputManager.OnEnterPressed -= OnEnterPressed;
                    _inputManager.OnLeftMouseDown -= OnLeftMouseDown;
                    _inputManager.OnRightMouseDown -= OnRightMouseDown;
                }

                // Clear the selected ability
                _selectedAbility = null;
            }

            Management.UI.MGR.UnequipAbility.Invoke();
        }

        /// <summary>
        /// Adds a new ability to the combatant.
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
    }
}
