using UnityEngine;

namespace SystemMiami
{
    public enum CharacterClassType { FIGHTER, MAGE, ROGUE, TANK };

    public class CharacterSheet : MonoBehaviour
    {
        #region VARS
        //===============================

        [SerializeField] private CharacterClassType _characterClass;
        [SerializeField] private BaseAttributes[] _baseAttributes;

        [SerializeField] private AttributeSet _attributes;

        public AttributeSet Attributes { get { return _attributes; } }

        //===============================
        #endregion

        #region PRIVATE METHODS
        //===============================

        void Awake()
        {
            assignAttributes();
        }

        private void assignAttributes()
        {
            BaseAttributes classBaseAttributes = _baseAttributes[(int)_characterClass];

            _attributes = new AttributeSet(classBaseAttributes);
        }

        //===============================
        #endregion

        #region PUBLIC METHODS
        //===============================

        //===============================
        #endregion
    }
}
