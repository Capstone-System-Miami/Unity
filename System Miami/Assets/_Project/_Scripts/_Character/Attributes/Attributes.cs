// Authors: Layla Hoey
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

        [SerializeField] private CharacterClassType _characterClass;
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

        // Buffer for storing the attributes prior to a confirmed upgrade
        private AttributeSet _buffer = new AttributeSet();

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
            _strength = baseAttributes.Dict[AttributeType.STRENGTH];
            _dexterity = baseAttributes.Dict[AttributeType.DEXTERITY];
            _constitution = baseAttributes.Dict[AttributeType.CONSTITUTION];
            _wisdom = baseAttributes.Dict[AttributeType.WISDOM];
            _intelligence = baseAttributes.Dict[AttributeType.INTELLIGENCE];

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
            if (_current.Dict == null) { return; }
            if (_current.Dict.Count == 0) { return; }

            if (!reverse)
            {
                _current.Dict[AttributeType.STRENGTH]        = _strength;
                _current.Dict[AttributeType.DEXTERITY]       = _dexterity;
                _current.Dict[AttributeType.CONSTITUTION]    = _constitution;
                _current.Dict[AttributeType.WISDOM]          = _wisdom;
                _current.Dict[AttributeType.INTELLIGENCE]    = _intelligence;
            }
            else
            {
                _strength       = _current.Dict[AttributeType.STRENGTH] ;
                _dexterity      = _current.Dict[AttributeType.DEXTERITY];
                _constitution   = _current.Dict[AttributeType.CONSTITUTION];
                _wisdom         = _current.Dict[AttributeType.WISDOM];
                _intelligence   = _current.Dict[AttributeType.INTELLIGENCE];
            }
        }

        /// <summary>
        /// Updates _preview to the current attributes + the stored upgrades.
        /// Should be called in Update()
        /// </summary>
        private void updatePreview()
        {
            // Preview should be our current stored attributes plus stored upgrades
            foreach (AttributeType attr in _current.Dict.Keys)
            {
                _preview.Dict[attr] = _current.Dict[attr] + _upgrades.Dict[attr];
            }
        }

    //===============================
    #endregion // ^private^

    #region PUBLIC METHODS
    //===============================

        /// <summary>
        /// Returns the value of the specified attribute, including status effects
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            // In case something asks for this while it's still being initialized
            if (_current.Dict == null) { return 0; }

            int baseValue = _upgradeMode ? _preview.Get(type) : _current.Get(type);
            int statusEffectValue = GetStatusEffectValue(type);

            return baseValue + statusEffectValue;

            
        }

        /// <summary>
        /// Adds an amount of points to an attribute in the
        /// stored _upgrades dict
        /// </summary>
        public void AddToUpgrades(AttributeType type, int amount)
        {
            // If the upgrade we're trying to add would bring us
            // under the min or over the max
            if (_preview.Dict[type] + amount < _minValue || _preview.Dict[type] > _maxValue)
            {
                // TODO: send this to UI.
                // Could also refactor to be bool TryAddToUpgrades(...)
                // and handle the validation from whatever calls this fn.
                print ($"Invalid Selection");
            }

            _upgrades.Dict[type] += amount;            
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
            _upgrades = new AttributeSet();

            // Update the inspector vals
            updateVals(true);
        }

        public void CancelUpgrades()
        {
            _upgrades = new AttributeSet();
        }

        public void AddStatusEffect(StatusEffect effect)
        {
            _statusEffects.Add(effect);
            Debug.Log($"{name} received a status effect for {effect.Duration} turns.");
            updateVals(false);
        }

        public int GetStatusEffectValue(AttributeType type)
        {
            int result = 0;

            foreach (StatusEffect statusEffect in _statusEffects)
            {
                result += statusEffect.Effect.Get(type);
            }

            return result;
        }

        public void UpdateStatusEffects()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                _statusEffects[i].DecrementDuration();

                if (_statusEffects[i].IsExpired())
                {
                    _statusEffects.RemoveAt(i);
                }
            }
        }

    //===============================
    #endregion // ^public^
    }
}
