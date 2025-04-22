// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using Unity.VisualScripting;
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

        // Base Attributes of out selected class
        private AttributeSet _base = new();

        // Our current & confirmed attributes.
        private AttributeSet _current = new();

        // The upgrades being decided on.
        private AttributeSet _upgrades = new();

        // A preview of what our new attributes would be if we confirmed the upgrades.
        private AttributeSet _preview = new();

        [SerializeField] private int attributePointsPerLevel;
        // TODO: deserialize after testing
        [SerializeField, Space(10)] private bool _upgradeMode;

        public int TotalPointsAvailable { get; private set; }
        public int UpgradeCost { get; private set; }
        public int PointsRemaining { get => TotalPointsAvailable - UpgradeCost; }
        public AttributeSet CurrentCopy { get => new(_current); }
        public AttributeSet UpgradesCopy { get => new(_upgrades); }
        public AttributeSet PreviewCopy { get => new(_preview); }


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


        #region PUBLIC METHODS
        //===============================

        public void SetClass(CharacterClassType characterClass)
        {
            _characterClass = characterClass;

            _base = new(_baseAttributes[(int)_characterClass]);
            InitializeWith(_base);
        }

        public void InitializeWith(AttributeSet baseAttributes)
        {
            _strength = baseAttributes.Get(AttributeType.STRENGTH);
            _dexterity = baseAttributes.Get(AttributeType.DEXTERITY);
            _constitution = baseAttributes.Get(AttributeType.CONSTITUTION);
            _wisdom = baseAttributes.Get(AttributeType.WISDOM);
            _intelligence = baseAttributes.Get(AttributeType.INTELLIGENCE);
            updateVals(false);
        }
        
        public void AddAttributeSet(AttributeSet additionalAttributes)
        {
            List<AttributeType> allTypes = new();
            allTypes.Add(AttributeType.WISDOM);
            allTypes.Add(AttributeType.STRENGTH);
            allTypes.Add(AttributeType.INTELLIGENCE);
            allTypes.Add(AttributeType.DEXTERITY);
            allTypes.Add(AttributeType.CONSTITUTION);

            foreach (AttributeType type in allTypes)
            {
                _current.Set(type, (_current.Get(type) + additionalAttributes.Get(type)));
            }
            updateVals(true);

            Debug.Log(
                $"Added {additionalAttributes.Get(AttributeType.STRENGTH)} " +
                $"to {name}'s strength. in AddAttributeSet");
        }

        /// <summary>
        /// Returns the value of the specified attribute
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            // If upgrade mode show preview
            return _upgradeMode ? _preview.Get(type) : _current.Get(type);
        }

        public AttributeSet GetAttributeSet()
        {
            return _upgradeMode ? _preview : _current;
        }

        public void EnterUpgradeMode()
        {
            _upgradeMode = true;
            PlayerManager.MGR?.GetComponent<PlayerLevel>().levelUpText?.SetActive(false);
        }

        public void LeaveUpgradeMode()
        {
            _upgradeMode = false;
            PlayerManager.MGR?.GetComponent<PlayerLevel>().levelUpText?.SetActive(false);
            ResetUpgrades();
        }

        /// <summary>
        /// Tries to add an amount of points to an attribute in the
        /// stored _upgrades dict.
        /// </summary>
        /// <param name="type">The target attribtue</param>
        /// <param name="amount">The amount of points to add</param>
        /// <param name="failMsg">If failed, the reason why.</param>
        /// <returns>
        /// A bool representing whether the addition succeded or failed.</returns>
        public bool TryAddToUpgrades(AttributeType type, int amount, out string failMsg)
        {
            failMsg = "";

            if (!_upgradeMode)
            {
                failMsg = $"Not in upgrade mode.";
                return false;
            }
            else if (_preview.Get(type) + amount < _current.Get(type))
            {
                failMsg = $"Invalid Selection.\n" +
                    $"{type} would be under existing values after upgrade.\n" +
                    $"This selection would essentially mean a respec.";
                return false;
            }
            else if (_preview.Get(type) > _maxValue)
            {
                failMsg = $"Invalid Selection.\n" +
                    $"{type} would be over maximum value after upgrade.";
                return false;
            }
            else if (PointsRemaining < amount)
            {
                failMsg = $"Not enough available points for this upgrade.";
                return false;
            }

            _upgrades.Set(type, (_upgrades.Get(type) + amount));
            UpgradeCost += amount;
            return true;
        }

        /// <summary>
        /// Adds the stored upgrades to our current attributes.
        /// </summary>
        public void ConfirmUpgrades()
        {
            if (!_upgradeMode) { return; }

            _upgradeMode = false;

            _current = new(_preview);

            // Unused points becomes the new total
            TotalPointsAvailable = PointsRemaining;

            ResetUpgrades();

            // Update the inspector vals
            updateVals(true);
        }

        public void ResetUpgrades()
        {
            // Clear Upgrade cost
            UpgradeCost = 0;

            // Clear Upgrade set
            _upgrades = new AttributeSet();
        }
        //===============================
        #endregion // ^public^


        #region PRIVATE METHODS
        //===============================

        private void Awake()
        {
            AttributeSet classBaseAttributes = new AttributeSet(_baseAttributes[(int)_characterClass]);
            InitializeWith(classBaseAttributes);
        }

        private void OnEnable()
        {
            if (TryGetComponent(out PlayerLevel playerLevel))
            {
                playerLevel.LevelUp += HandleLevelUp;
                return;
            }           
        }


        private void OnDisable()
        {
            if (TryGetComponent(out PlayerLevel playerLevel))
            {
                playerLevel.LevelUp -= HandleLevelUp;
                return;
            }
        }

        private void Update()
        {
            updatePreview();

            if (trigger_EnterUpgradeMode)
            {
                EnterUpgradeMode();
            }

            if (trigger_LeaveUpgradeMode)
            {
                LeaveUpgradeMode();
            }

            if (_upgradeMode)
            {
                if (trigger_ConfirmUpgrades)
                {
                    ConfirmUpgrades();
                }
                
                if (trigger_AddUpgrade)
                {
                    if (!TryAddToUpgrades(upgradeType, upgradeAmt, out string failMsg))
                    {
                        Debug.LogWarning(failMsg);
                    }
                }
            }

            resetTriggers();
        }

        private void HandleLevelUp(int newLevel)
        {
            TotalPointsAvailable += attributePointsPerLevel;
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
    }
}
