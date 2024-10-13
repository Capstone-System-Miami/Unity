// Authors: Layla Hoey
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class Attributes : MonoBehaviour
    {
    #region VARS
    //===============================

        [SerializeField] private CharacterClassType _characterClass;
        [SerializeField] private AttributeSet[] _baseAttributes;
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
        private Dictionary<AttributeType, int> _current = new Dictionary<AttributeType, int>();

        // The upgrades being decided on.
        private Dictionary<AttributeType, int> _upgrades = new Dictionary<AttributeType, int>();

        // A preview of what our new attributes would be if we confirmed the upgrades.
        private Dictionary<AttributeType, int> _preview = new Dictionary<AttributeType, int>();

        // Buffer for storing the attributes prior to a confirmed upgrade
        private Dictionary<AttributeType, int> _buffer = new Dictionary<AttributeType, int>();

        // TODO: deserialize after testing
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

    //===============================
    #endregion // ^vars^

    #region PRIVATE METHODS
    //===============================

        private void Awake()
        {
            AttributeSet classBaseAttributes = _baseAttributes[(int)_characterClass];
            initializeWith(classBaseAttributes);
        }

        private void initializeWith(AttributeSet baseAttributes)
        {
            _current = getNewDict();
            _upgrades = getNewDict();
            _preview = getNewDict();
            _buffer = getNewDict();

            _strength = baseAttributes._strength;
            _dexterity = baseAttributes._dexterity;
            _constitution = baseAttributes._constitution;
            _wisdom = baseAttributes._wisdom;
            _intelligence = baseAttributes._intelligence;

            updateVals(false);
        }

        /// <summary>
        /// Returns a dictionary with all attributes initialized to zero
        /// </summary>
        private Dictionary<AttributeType, int> getNewDict()
        {
            Dictionary<AttributeType, int> result = new Dictionary<AttributeType, int>();

            foreach (int enumVal in Enum.GetValues(typeof(StatType)))
            {
                result[(AttributeType)enumVal] = 0;
            }

            return result;
        }

        private void Update()
        {
            updatePreview();

            if (trigger_EnterUpgradeMode)
            {
                _upgradeMode = true;    
            }

            if (trigger_LeaveUpgradeMode)
            {
                _upgradeMode = false;
                CancelUpgrades();
            }

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

            resetTriggers();
        }

        /// <summary>
        /// Resets the testing triggers
        /// </summary>
        private void resetTriggers()
        {
            trigger_EnterUpgradeMode = false;
            trigger_LeaveUpgradeMode = false;
            trigger_AddUpgrade = false;
            trigger_ConfirmUpgrades = false;
        }


        /// <summary>
        /// THIS SHOULD BE CHANGED AFTER WE HAVE A FUNCTIONAL UI PANEL
        /// Updates current attributes to reflect the values of
        /// each attribute var in the inspector.
        /// "reverse" updates the vars to reflect the array.
        /// </summary>
        private void updateVals(bool reverse)
        {
            if (!reverse)
            {
                _current[AttributeType.STRENGTH]        = _strength;
                _current[AttributeType.DEXTERITY]       = _dexterity;
                _current[AttributeType.CONSTITUTION]    = _constitution;
                _current[AttributeType.WISDOM]          = _wisdom;
                _current[AttributeType.INTELLIGENCE]    = _intelligence;
            }
            else
            {
                _strength       = _current[AttributeType.STRENGTH] ;
                _dexterity      = _current[AttributeType.DEXTERITY];
                _constitution   = _current[AttributeType.CONSTITUTION];
                _wisdom         = _current[AttributeType.WISDOM];
                _intelligence   = _current[AttributeType.INTELLIGENCE];
            }
        }

        /// <summary>
        /// Updates _preview to the current attributes + the stored upgrades.
        /// Should be called in Update()
        /// </summary>
        private void updatePreview()
        {
            // Preview should be our current stored attributes plus stored upgrades
            foreach (AttributeType attr in _current.Keys)
            {
                _preview[attr] = _current[attr] + _upgrades[attr];
            }
        }

    //===============================
    #endregion // ^private^

    #region PUBLIC METHODS
    //===============================

        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            // In case something asks for this while it's still being initialized
            if (_current.Keys.Count == 0) { return 0; }

            return _upgradeMode ? _preview[type] : _current[type];
        }

        /// <summary>
        /// Adds an amount of points to an attribute in the
        /// stored _upgrades dict
        /// </summary>
        public void AddToUpgrades(AttributeType type, int amount)
        {
            // If the upgrade we're trying to add would bring us
            // under the min or over the max
            if (_preview[type] + amount < _minValue || _preview[type] > _maxValue)
            {
                // TODO: send this to UI.
                // Could also refactor to be bool TryAddToUpgrades(...)
                // and handle the validation from whatever calls this fn.
                print ($"Invalid Selection");
            }

            _upgrades[type] += amount;            
        }


        /// <summary>
        /// Adds the stored upgrades to our current attributes.
        /// </summary>
        public void ConfirmUpgrades()
        {
            // Just to be safe, store the current attributes
            // in a buffer before we change them.
            _buffer = _current;

            _current = _preview;

            // Clear the upgrades
            _upgrades = getNewDict();

            // Update the inspector vals
            updateVals(true);
        }

        public void CancelUpgrades()
        {
            _upgrades = getNewDict();
        }

    //===============================
    #endregion // ^public^
    }
}
