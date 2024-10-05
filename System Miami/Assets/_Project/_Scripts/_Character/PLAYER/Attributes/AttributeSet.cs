// Author: Layla Hoey
using UnityEngine;

namespace SystemMiami
{
    public enum AttributeType { STRENGTH, DEXTERITY, CONSTITUTION, WISDOM, INTELLIGENCE };

    [System.Serializable]
    public class AttributeSet
    {
        #region VARS
        //===============================

        // These vars are here mostly to identify values in the inspector
        [SerializeField] private int _strength;
        [SerializeField] private int _dexterity;
        [SerializeField] private int _constitution;
        [SerializeField] private int _wisdom;
        [SerializeField] private int _intelligence;

        // The array of values used for the actual writing & reading of the attributes.
        private int[] _values = new int[System.Enum.GetNames(typeof(AttributeType)).Length];

        //===============================
        #endregion

        #region PRIVATE METHODS
        //===============================

        /// <summary>
        /// Updates the values array to reflect the values of each attribute var. "reverse" updates the vars to reflect the array.
        /// </summary>
        /// <param name="reverse"></param>
        private void updateVals(bool reverse)
        {
            if (!reverse)
            {
                _values[(int)AttributeType.STRENGTH] = _strength;
                _values[(int)AttributeType.DEXTERITY] = _dexterity;
                _values[(int)AttributeType.CONSTITUTION] = _constitution;
                _values[(int)AttributeType.WISDOM] = _wisdom;
                _values[(int)AttributeType.INTELLIGENCE] = _intelligence;
            }
            else
            {
                _strength = _values[(int)AttributeType.STRENGTH] ;
                _dexterity = _values[(int)AttributeType.DEXTERITY];
                _constitution = _values[(int)AttributeType.CONSTITUTION];
                _wisdom = _values[(int)AttributeType.WISDOM];
                _intelligence = _values[(int)AttributeType.INTELLIGENCE];
            }
        }

        //===============================
        #endregion

        #region PUBLIC METHODS
        //===============================

        public AttributeSet(BaseAttributes attributes)
        {
            _strength = attributes._strength;
            _dexterity = attributes._dexterity;
            _constitution = attributes._constitution;
            _wisdom = attributes._wisdom;
            _intelligence = attributes._intelligence;

            updateVals(false);
        }


        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        public int GetAttribute(AttributeType type)
        {
            return type switch
            {
                AttributeType.STRENGTH => _strength,
                AttributeType.DEXTERITY => _dexterity,
                AttributeType.CONSTITUTION => _constitution,
                AttributeType.WISDOM => _wisdom,
                AttributeType.INTELLIGENCE => _intelligence,
                _ => 0
            };
        }

        /// <summary>
        /// Increases the value of the specified attribute by 1.
        /// </summary>
        public void UpgradeAttribute(AttributeType type)
        {
            _values[(int)type]++;
            updateVals(true);
        }

        /// <summary>
        /// Upgrades multiple attributes at once.
        /// </summary>
        public void UpgradeAttributes(AttributeType[] types)
        {
            // Types array could be stored for later downgrade if upgrades are temporary.

            foreach (AttributeType type in types)
            {
                UpgradeAttribute(type);
            }
        }

        /// <summary>
        /// Decreases the value of the specified attribute by 1.
        /// </summary>
        public void DowngradeAttribute(AttributeType type)
        {
            _values[((int)type)]--;
            updateVals(true);
        }

        /// <summary>
        /// Downgrades multiple attributes at once.
        /// </summary>
        public void DowngradeAttributes(AttributeType[] types)
        {
            // Types array could be stored for later upgrades if downgrades are temporary.

            foreach (AttributeType type in types)
            {
                DowngradeAttribute(type);
            }
        }

        //===============================
        #endregion
    }
}
