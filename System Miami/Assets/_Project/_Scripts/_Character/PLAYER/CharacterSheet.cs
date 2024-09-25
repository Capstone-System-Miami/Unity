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
        [SerializeField] private StatData _statData;

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

        private void Update()
        {
            print (getStatsReport());
        }

        private string getStatsReport()
        {
            string result = "";

            result += $"Physical Power: { GetPhysicalPower().ToString("F2") }\n";
            result += $"Magical Power: { GetMagicalPower().ToString("F2") }\n";
            result += $"Physical Slots: { GetPhysicalSlots() }\n";
            result += $"Magical Slots : { GetMagicalSlots() }\n";
            result += $"Stam: { GetStamina() }\n";
            result += $"Mana: { GetMana() }\n";
            result += $"Max Health: { GetMaxHealth() }\n";
            result += $"Damage RDX: { GetDamageReduction() }\n";

            return result;
        }

        //===============================
        #endregion

        #region PUBLIC METHODS
        //===============================

        public float GetPhysicalPower()
        {
            int strength = _attributes.GetAttribute(AttributeType.STRENGTH);

            return strength * _statData.EffectMultiplier;
        }

        public float GetMagicalPower()
        {
            int wisdom = _attributes.GetAttribute(AttributeType.WISDOM);

            return wisdom * _statData.EffectMultiplier;
        }

        public int GetPhysicalSlots()
        {
            int strength = _attributes.GetAttribute(AttributeType.STRENGTH);

            int threshold = _statData.SlotAttributeThreshold;
            int minSlots = _statData.MinSlots;
            int additionalSlots = (int)(strength * 2 * _statData.SlotMultiplier);

            if (strength > threshold)
            {
                return minSlots + additionalSlots;
            }
            else
            {
                return minSlots;
            }
        }

        public int GetMagicalSlots()
        {
            int wisdom = _attributes.GetAttribute(AttributeType.WISDOM);

            int threshold = _statData.SlotAttributeThreshold;
            int minSlots = _statData.MinSlots;
            int additionalSlots = (int)(wisdom * 2 * _statData.SlotMultiplier);

            if (wisdom > threshold)
            {
                return minSlots + additionalSlots;
            }
            else
            {
                return minSlots;
            }
        }

        public float GetStamina()
        {
            int dexterity = _attributes.GetAttribute(AttributeType.DEXTERITY);

            return dexterity * _statData.ResourceMultiplier;
        }

        public float GetMana()
        {
            int intelligence = _attributes.GetAttribute(AttributeType.INTELLIGENCE);

            return intelligence * _statData.ResourceMultiplier;
        }

        public float GetMaxHealth()
        {
            int constitution = _attributes.GetAttribute(AttributeType.CONSTITUTION);

            return constitution * _statData.HealthMultiplier;
        }

        public float GetDamageReduction()
        {

            int constitution = Attributes.GetAttribute(AttributeType.CONSTITUTION);

            return constitution * _statData.DamageRdxMultiplier;
        }

        //===============================
        #endregion
    }
}
