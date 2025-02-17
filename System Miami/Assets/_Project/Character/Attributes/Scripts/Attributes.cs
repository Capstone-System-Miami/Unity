using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SystemMiami
{
    public class Attributes : MonoBehaviour
    {
        #region VARS
        //===============================

        [SerializeField] public CharacterClassType _characterClass;
        [SerializeField] private AttributeSetSO[] _baseAttributes;
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;

        // These vars are here mostly to identify values in the inspector
        [Header("Current (just for visualizing; don't change)")]
        [SerializeField] private int _strength;
        [SerializeField] private int _dexterity;
        [SerializeField] private int _constitution;
        [SerializeField] private int _wisdom;
        [SerializeField] private int _intelligence;

        // Our current & confirmed attributes.
        private AttributeSet _current = new AttributeSet();

        // The upgrades being decided on.
        private AttributeSet _upgrades = new AttributeSet();

        // A preview of what our new attributes would be if we confirmed the upgrades.
        private AttributeSet _preview = new AttributeSet();

        private List<StatusEffect> _statusEffects = new List<StatusEffect>();

        [SerializeField, Space(10)] private bool _upgradeMode;

        [Header("Testing/Driver")]
        [Space(5)]
        public AttributeType upgradeType;
        public int upgradeAmt;
        [Space(5)]
        public bool trigger_EnterUpgradeMode;
        public bool trigger_LeaveUpgradeMode;
        public bool trigger_AddUpgrade;
        public bool trigger_ConfirmUpgrades;

        #endregion // ^vars^

        #region PRIVATE METHODS
        //===============================

        private void Awake()
        {
            AttributeSet classBaseAttributes = new AttributeSet(_baseAttributes[(int)_characterClass]);
            initializeWith(classBaseAttributes);
        }

        // Initializes values based on the class
        private void initializeWith(AttributeSet baseAttributes)
        {
            _strength = baseAttributes.Get(AttributeType.STRENGTH);
            _dexterity = baseAttributes.Get(AttributeType.DEXTERITY);
            _constitution = baseAttributes.Get(AttributeType.CONSTITUTION);
            _wisdom = baseAttributes.Get(AttributeType.WISDOM);
            _intelligence = baseAttributes.Get(AttributeType.INTELLIGENCE);

            updateVals(false);
        }

        private void Update()
        {
            // Updates preview based on upgrades in real-time
            updatePreview();

            // Handle triggers for entering upgrade mode
            if (trigger_EnterUpgradeMode)
            {
                _upgradeMode = true;
            }

            // Handle triggers for leaving upgrade mode
            if (trigger_LeaveUpgradeMode)
            {
                _upgradeMode = false;
                CancelUpgrades();
            }

            // If in upgrade mode, handle adding upgrades and confirming
            if (_upgradeMode)
            {
                if (trigger_ConfirmUpgrades)
                {
                    ConfirmUpgrades();
                }

                if (trigger_AddUpgrade)
                {
                    AddToUpgrades(upgradeType, upgradeAmt);
                }
            }

            // Reset triggers after they've been processed
            resetTriggers();
        }

        // Resets trigger flags after they've been handled
        private void resetTriggers()
        {
            trigger_EnterUpgradeMode = false;
            trigger_LeaveUpgradeMode = false;
            trigger_AddUpgrade = false;
            trigger_ConfirmUpgrades = false;
        }

        // Updates the current attribute values (or sets them back if needed)
        private void updateVals(bool reverse)
        {
            if (!reverse)
            {
                _current.Set(AttributeType.STRENGTH, _strength);
                _current.Set(AttributeType.DEXTERITY, _dexterity);
                _current.Set(AttributeType.CONSTITUTION, _constitution);
                _current.Set(AttributeType.WISDOM, _wisdom);
                _current.Set(AttributeType.INTELLIGENCE, _intelligence);
            }
            else
            {
                _strength = _current.Get(AttributeType.STRENGTH);
                _dexterity = _current.Get(AttributeType.DEXTERITY);
                _constitution = _current.Get(AttributeType.CONSTITUTION);
                _wisdom = _current.Get(AttributeType.WISDOM);
                _intelligence = _current.Get(AttributeType.INTELLIGENCE);
            }
        }

        // Updates the preview based on current and upgrade values
        private void updatePreview()
        {
            for (int i = 0; i < CharacterEnums.ATTRIBUTE_COUNT; i++)
            {
                AttributeType attr = (AttributeType)i;
                _preview.Set(attr, _current.Get(attr) + _upgrades.Get(attr)); // Preview = Current + Upgrades
            }
        }

        #endregion // ^private^

        #region PUBLIC METHODS
        //===============================

        // Set the class type and initialize base attributes for that class
        public void SetClass(CharacterClassType characterClass)
        {
            _characterClass = characterClass;

            AttributeSet classBaseAttributes = new AttributeSet(_baseAttributes[(int)_characterClass]);
            initializeWith(classBaseAttributes);
        }

        // Get the value of a specific attribute
        public int GetAttribute(AttributeType type)
        {
            return _upgradeMode ? _preview.Get(type) : _current.Get(type);
        }

        // Get the entire set of current or previewed attributes
        public AttributeSet GetSet()
        {
            return _upgradeMode ? _preview : _current;
        }

        public int GetMaxValue()
        {
            return _maxValue;
        }


        // Adds points to a specific attribute during upgrade mode
        public void AddToUpgrades(AttributeType type, int amount)
        {
            int currentStatValue = _current.Get(type) + _upgrades.Get(type);

            // Check if adding the amount would exceed max
            if (currentStatValue + amount > _maxValue)
            {
                Debug.LogWarning($"Cannot add points to {type}. It has reached the maximum value of {_maxValue}.");
                return;
            }

            // Add points to the upgrades
            _upgrades.Set(type, _upgrades.Get(type) + amount);
            updatePreview();

            Debug.Log($"Added {amount} to {type}. Current: {_current.Get(type)}, Upgrades: {_upgrades.Get(type)}, Preview: {_preview.Get(type)}");
        }


        public void ConfirmUpgrades()
        {
            Debug.Log("ConfirmUpgrades called.");

            for (int i = 0; i < CharacterEnums.ATTRIBUTE_COUNT; i++)
            {
                AttributeType attr = (AttributeType)i;
                _current.Set(attr, _preview.Get(attr));
            }

            _upgrades = new AttributeSet();
            updateVals(true);

            Debug.Log("Upgrades confirmed. Current values:");
            Debug.Log($"Strength: {_current.Get(AttributeType.STRENGTH)}");
            Debug.Log($"Dexterity: {_current.Get(AttributeType.DEXTERITY)}");
            Debug.Log($"Constitution: {_current.Get(AttributeType.CONSTITUTION)}");
            Debug.Log($"Wisdom: {_current.Get(AttributeType.WISDOM)}");
            Debug.Log($"Intelligence: {_current.Get(AttributeType.INTELLIGENCE)}");
        }

        // Cancels the upgrades without applying any changes
        public void CancelUpgrades()
        {
            _upgrades = new AttributeSet();
        }

        #endregion // ^public^
    }

}
