// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;


    /// <summary>
    /// Manages the QuickslotInventory of a combatant, handling selection, targeting, and execution.
    /// </summary>
    public class QuickslotInventory : MonoBehaviour
    {
        #region EVENTS
        // ======================================

        public Action ItemUnequipped;
        public Action<Item> ItemEquipped;
        public Action<Item> TargetsLocked;
        public Action<Item> ExecuteItemStarted;
        public Action<Item> ExecuteItemCompleted;

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

        private class ItemState
        {
            private QuickslotInventory _owner;
            private QuickslotInventory.State _state;
            private Item _selectedItem;

            public QuickslotInventory.State Get()
            {
                return _state;
            }

            public void Set(QuickslotInventory.State newState)
            {
                if (!validSet()) { return; }

                _state = newState;
                _selectedItem = _owner._selectedItem;

                switch (newState)
                {
                    case QuickslotInventory.State.UNEQUIPPED:
                        _owner.ItemUnequipped?.Invoke();
                        break;
                    case QuickslotInventory.State.EQUIPPED:
                        _owner.ItemEquipped?.Invoke(_selectedItem);
                        break;
                    case QuickslotInventory.State.TARGETS_LOCKED:
                        _owner.TargetsLocked?.Invoke(_selectedItem);
                        break;
                    case QuickslotInventory.State.EXECUTING:
                        _owner.ExecuteItemStarted?.Invoke(_selectedItem);
                        break;
                    case QuickslotInventory.State.COMPLETE:
                        _owner.ExecuteItemCompleted?.Invoke(_selectedItem);
                        break;
                }
            }

            public ItemState(QuickslotInventory owner, QuickslotInventory.State state)
            {
                _owner = owner;
                _state = state;
                _selectedItem = _owner._selectedItem;
            }

            private bool validSet()
            {
                if (_owner == null)
                {
                    Debug.LogWarning("Something is trying to set a new" +
                        "ItemState, but the owner is null.");

                    return false;
                }

                if (_state != QuickslotInventory.State.UNEQUIPPED && _selectedItem == null)
                {
                    Debug.LogWarning($"{_owner.name} is trying to set a new" +
                        $"ItemState, but has no selected Item");
                    return false;
                }

                return true;
            }
        }
        #endregion // STATE =====================


        #region SERIALIZED
        // ======================================

       
        [SerializeField] private AnimatorOverrideController animController;
        #endregion // SERIALIZED ================


        #region PRIVATE VARS
        // ======================================

        private Combatant _combatant; // Reference to the combatant component

        private Item _selectedItem; // Currently selected Item
        //private bool _isTargeting = false; // check if in targeting mode
        //private bool _targetsLocked = false; // check if targets are locked
        //private bool _isUsing = false; // check if the Item has been confirmed

        private ItemState _state;

        #endregion // PRIVATE VARS ==============


        #region PROPERTIES
        // ======================================
       

        public QuickslotInventory.State CurrentState { get { return _state.Get(); } }

        public Item SelectedItem { get { return _selectedItem; } }

        #endregion // PROPERTIES ================


        #region UNITY METHODS
        // ======================================

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
            _state = new ItemState(this, QuickslotInventory.State.UNEQUIPPED);
        }

        #endregion // UNITY METHODS ==============


        #region PUBLIC METHODS
        // ======================================
        /// <summary>
        /// Equip the Item at the index within the list corresponding to the type.
        /// </summary>
        public bool TryEquip(ItemType type, int index)
        {
            if (!_combatant.Controller.IsMyTurn)
            {
                Debug.LogWarning($"{name} is trying to equip an Item, " +
                    $"but it is not their turn.");
                return false;
            }

            if (index < 0)
            {
                Debug.LogWarning($"{name} is trying to equip an Item, " +
                    $"but the provided index is negative.");
                return false;
            }

            if (getItem(index) == null)
            {
                Debug.LogWarning($"{name} is trying to equip an Item, " +
                    $"but the provided index is invalid " +
                    $"for the {type} QuickslotInventory list.");
                return false;
            }

            equip(index);
            return true;
        }

        /// <summary>
        /// To be called when the combatant unequips the Item (e.g., by right-clicking).
        /// Cancels the Item, removes the preview, and allows equipping a different Item.
        /// </summary>
        public bool TryUnequip()
        {
            switch (_state.Get())
            {
                default:
                case QuickslotInventory.State.UNEQUIPPED:
                    Debug.LogWarning($"{name} failed to unequip. " +
                        $"There is nothing equipped to unequip.");
                    return false;

                case QuickslotInventory.State.EQUIPPED:
                    unequip();
                    return true;

                case QuickslotInventory.State.TARGETS_LOCKED:
                    unequip();
                    return true;

                case QuickslotInventory.State.EXECUTING:
                    Debug.LogWarning($"{name} failed to unequip. " +
                        $"{_selectedItem} is already in use!");
                    return false;

                case QuickslotInventory.State.COMPLETE:
                    Debug.LogWarning($"{name} failed to unequip. " +
                        $"{_selectedItem} is already finishing execution!");
                    return false;
            }
        }

        public bool TryLockTargets()
        {
            if (_state.Get() != State.EQUIPPED)
            {
                Debug.LogWarning($"{name} failed to lock targets. " +
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
        /// The provess will execute the Item, starting cooldowns,
        /// applying status effects, and handling animations.
        /// </summary>
        public bool ItemExecutionIsValid(out IEnumerator ItemProcess)
        {
            // Validate
            if (_selectedItem == null)
            {
                Debug.LogWarning($"{name} failed to execute an Item. " +
                    $"There was no Item selected.");
                ItemProcess = null;
                return false;
            }

            if (_combatant == null)
            {
                Debug.LogWarning($"{name} failed to execute an Item. " +
                    $"_combatant was null.");
                ItemProcess = null;
                return false;
            }

            if (_combatant.Animator == null)
            {
                Debug.LogWarning($"{name} failed to execute an Item. " +
                    $"There was no Animator on {_combatant}.");
                ItemProcess = null;
                return false;
            }

            if (_selectedItem.itemData._overrideController == null)
            {
                Debug.LogWarning($"{name} failed to execute an Item. " +
                    $"There was no override controller on the selected Item.");
                ItemProcess = null;
                return false;
            }

            if (_state.Get() != State.TARGETS_LOCKED)
            {
                Debug.LogWarning($"{name} failed to execute an Item. " +
                    $"Targets are not locked.");
                ItemProcess = null;
                return false;
            }

            ItemProcess = executeSelected();
            return true;
        }

        #endregion // PUBLIC METHODS ============


        #region PRIVATE METHODS
        // ======================================

        /// <summary>
        /// Equip the Item at the index within the list corresponding to the type.
        /// </summary>
        private void equip(int index)
        {
            // If already targeting an Item, unequip it
            if (_selectedItem != null)
            {
                unequip();
            }

            // Select the new Item
            _selectedItem = getItem(index);
            if (_selectedItem == null) { return; }

            _selectedItem.itemData.Init(_combatant);

            // Begin targeting mode
            _selectedItem.itemData.BeginTargeting();

            _state.Set(State.EQUIPPED);
        }

        /// <summary>
        /// To be called when the combatant unequips the Item (e.g., by right-clicking).
        /// Cancels the Item, removes the preview, and allows equipping a different Item.
        /// </summary>
        private void unequip()
        {
            Debug.Log("Unequip called.");

            _selectedItem.itemData.CancelTargeting();

            // Clear the selected Item
            _selectedItem = null;

            _state.Set(State.UNEQUIPPED);

        }

        /// <summary>
        /// Locks the target tiles and combatants for every
        /// combat action housed in the equipped Item.
        /// </summary>
        private void lockTargets()
        {
            Debug.Log("Locking targets.");
            _selectedItem.itemData.LockTargets();

            _state.Set(State.TARGETS_LOCKED);
        }

        /// <summary>
        /// Executes the Item, starting cooldowns, applying status effects, and handling animations.
        /// </summary>
        private IEnumerator executeSelected()
        {
            // Prepare
            _state.Set(State.EXECUTING);
            yield return null;


            // Start Animation
            if (_selectedItem.itemData._overrideController.runtimeAnimatorController != null)
            {
                _combatant.Animator.runtimeAnimatorController = _selectedItem.itemData._overrideController;
            }

            // _combatant.Animator.SetTrigger("UseItem");

            // TODO: Wait for the animation to finish
            // For now, just wait 2 secs
            for (int i = 2; i >= 0; i--)
            {
                Debug.Log($"Placeholder for animation time. Activates in {i} seconds.");
                yield return new WaitForSeconds(1f);
            }

            // Perform the Item actions
            yield return StartCoroutine(_selectedItem.itemData.Use());

            _state.Set(State.COMPLETE);
            yield return null;

            // Unequip when the Item's Use() coroutine is over.
            _combatant.Animator.runtimeAnimatorController = animController;
            unequip();
            yield return null;
        }

        /// <summary>
        /// Determines which type list to index,
        /// indexes that list to return the
        /// Item at the index.
        /// </summary>
        private Item getItem(int index)
        {
            List<Item> quickslotItems = Inventory.instance.quickslot;
            

            return quickslotItems[index];
        }

        #endregion // PRIVATE METHODS ===========
    }

