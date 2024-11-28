// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
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
            AttributeSet classBaseAttributes = new AttributeSet(_baseAttributes[(int)_characterClass]);
            initializeWith(classBaseAttributes);
        }

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
                _current.Set(AttributeType.STRENGTH,        _strength);
                _current.Set(AttributeType.DEXTERITY,       _dexterity);
                _current.Set(AttributeType.CONSTITUTION,    _constitution);
                _current.Set(AttributeType.WISDOM,          _wisdom);
                _current.Set(AttributeType.INTELLIGENCE,    _intelligence);
            }
            else
            {
                _strength       = _current.Get(AttributeType.STRENGTH);
                _dexterity      = _current.Get(AttributeType.DEXTERITY);
                _constitution   = _current.Get(AttributeType.CONSTITUTION);
                _wisdom         = _current.Get(AttributeType.WISDOM);
                _intelligence   = _current.Get(AttributeType.INTELLIGENCE);
            }
        }

        /// <summary>
        /// Updates _preview to the current attributes + the stored upgrades.
        /// Should be called in Update()
        /// </summary>
        private void updatePreview()
        {
            // Preview should be our current stored attributes plus stored upgrades
            for ( int i = 0; i < CharacterEnums.ATTRIBUTE_COUNT; i++ )
            {
                AttributeType attr = (AttributeType)i;

                _preview.Set(attr, (_current.Get(attr) + _upgrades.Get(attr)) );
            }
        }

    //===============================
    #endregion // ^private^

    #region PUBLIC METHODS
    //===============================

        public void SetClass(CharacterClassType characterClass)
        {
            _characterClass = characterClass;

            AttributeSet classBaseAttributes = new AttributeSet(_baseAttributes[(int)_characterClass]);
            initializeWith(classBaseAttributes);
        }

        /// <summary>
        /// Returns the value of the specified attribute
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            // If upgrade mode show preview
            return _upgradeMode ? _preview.Get(type) : _current.Get(type);
        }

        public AttributeSet GetSet()
        {
            return _upgradeMode ? _preview : _current;
        }

        /// <summary>
        /// Adds an amount of points to an attribute in the
        /// stored _upgrades dict
        /// </summary>
        public void AddToUpgrades(AttributeType type, int amount)
        {
            // If the upgrade we're trying to add would bring us
            // under the min or over the max
            if (_preview.Get(type) + amount < _minValue || _preview.Get(type) > _maxValue)
            {
                // TODO: send this to UI.
                // Could also refactor to be bool TryAddToUpgrades(...)
                // and handle the validation from whatever calls this fn.
                print ($"Invalid Selection");
            }

            _upgrades.Set(type, (_upgrades.Get(type) + amount) );            
        }

        /// <summary>
        /// Adds the stored upgrades to our current attributes.
        /// </summary>
        public void ConfirmUpgrades()
        {
            _current = _preview;

            // Clear the upgrades
            _upgrades = new AttributeSet();

            // Update the inspector vals
            updateVals(true);
        }

        public void CancelUpgrades()
        {
            _upgrades = new AttributeSet();
        }
    //===============================
    #endregion // ^public^
    }
}
