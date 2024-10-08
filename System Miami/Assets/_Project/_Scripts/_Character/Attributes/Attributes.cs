// Author: Layla Hoey
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

        // These vars are here mostly to identify values in the inspector
        [SerializeField] private int _strength;
        [SerializeField] private int _dexterity;
        [SerializeField] private int _constitution;
        [SerializeField] private int _wisdom;
        [SerializeField] private int _intelligence;


        // The array of values used for the actual writing & reading of the attributes.
        private int[] _current = new int[CharacterEnums.ATTRIBUTE_COUNT];
        private int[] _preview = new int[CharacterEnums.ATTRIBUTE_COUNT];
        private int[] _buffer = new int[CharacterEnums.ATTRIBUTE_COUNT];

        // A dictionary of upgrades.
        // This can be manipulated while the player
        // Decides what to upgrade, then fed into AddToUpgrades()
        // to confirm their choices.
        private Dictionary<AttributeType, int> _upgrades = new Dictionary<AttributeType, int>();

        // TODO: deserialize after testing
        [SerializeField] private bool _isShowingUpgrades;

        [Header("Testing/Driver")]
        public AttributeType upgradeType;
        public int upgradeValue;
        public bool showUpgrades;
        [Space(5)]
        public bool trigger_AddUpgrade;
        public bool trigger_ConfirmUpgrades;
        public bool trigger_CancelUpgrades;

    //===============================
    #endregion

    #region PRIVATE METHODS
    //===============================

        private void Awake()
        {
            AttributeSet classBaseAttributes = _baseAttributes[(int)_characterClass];
            initializeWith(classBaseAttributes);
        }

        private void Update()
        {
            if (trigger_ConfirmUpgrades)
            {
                trigger_AddUpgrade = false;
                trigger_ConfirmUpgrades = false;
                ConfirmUpgrades();
            }
            
            if (trigger_AddUpgrade)
            {
                trigger_AddUpgrade = false;
                trigger_ConfirmUpgrades = false;
                AddToUpgrades(upgradeType, upgradeValue);
            }

            trigger_AddUpgrade = false;
            trigger_ConfirmUpgrades = false;
        }

        private void initializeWith(AttributeSet baseAttributes)
        {
            _strength = baseAttributes._strength;
            _dexterity = baseAttributes._dexterity;
            _constitution = baseAttributes._constitution;
            _wisdom = baseAttributes._wisdom;
            _intelligence = baseAttributes._intelligence;

            updateVals(false);
        }

        /// <summary>
        /// Updates the vals array to reflect the values of each attribute var. "reverse" updates the vars to reflect the array.
        /// </summary>
        private void updateVals(bool reverse)
        {
            if (!reverse)
            {
                _current[(int)AttributeType.STRENGTH] = _strength;
                _current[(int)AttributeType.DEXTERITY] = _dexterity;
                _current[(int)AttributeType.CONSTITUTION] = _constitution;
                _current[(int)AttributeType.WISDOM] = _wisdom;
                _current[(int)AttributeType.INTELLIGENCE] = _intelligence;
            }
            else
            {
                _strength = _current[(int)AttributeType.STRENGTH] ;
                _dexterity = _current[(int)AttributeType.DEXTERITY];
                _constitution = _current[(int)AttributeType.CONSTITUTION];
                _wisdom = _current[(int)AttributeType.WISDOM];
                _intelligence = _current[(int)AttributeType.INTELLIGENCE];
            }
        }

        /// <summary>
        /// Returns a modified verision of out current attributes,
        /// with the stored upgrades added.
        /// </summary>
        private int[] getUpgradesPreview()
        {
            int[] result = new int[CharacterEnums.ATTRIBUTE_COUNT];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _current[i];
            }

            // Add the stored upgrates to the appropriate attributes
            foreach (AttributeType attr in _upgrades.Keys)
            {
                result[(int)attr] += _upgrades[attr];
            }

            return result;
        }

    //===============================
    #endregion

    #region PUBLIC METHODS
    //===============================

        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            if (!_isShowingUpgrades)
            {
                //print ($"no show{type}  {_current[(int)type]}");
                return _current[(int)type];
            }
            else
            {
                return getUpgradesPreview()[(int)type];
            }
        }

        /// <summary>
        /// Adds a number of upgrades to an attribute in the
        /// stored _upgrades dict
        /// </summary>
        public void AddToUpgrades(AttributeType type, int amount)
        {
            print ($"add to upgr being called");
            if (!_upgrades.ContainsKey(type))
            {
                print ("no key");
                _upgrades.Add(type, amount);
            }
            else
            {
                print ("key");
                _upgrades[type] += amount;
            }
        }


        /// <summary>
        /// Adds the stored upgrades to our current attributes.
        /// </summary>
        public void ConfirmUpgrades()
        {
            // Just to be safe, store the current attributes
            // in a buffer before we change them.
            _buffer = _current;

            foreach (AttributeType attribute in _upgrades.Keys)
            {
                _current[(int)attribute] += _upgrades[attribute];
            }

            // Clear the upgrades dict
            _upgrades.Clear();

            // Update the inspector vals
            updateVals(true);
        }

        public void CancelUpgrades()
        {
            _isShowingUpgrades = false;
            _upgrades.Clear();
        }

        //===============================
        #endregion
    }
}
