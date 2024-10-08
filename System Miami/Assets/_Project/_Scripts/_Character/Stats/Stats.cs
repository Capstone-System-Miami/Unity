using UnityEngine;

namespace SystemMiami
{
    [RequireComponent(typeof(Attributes))]
    public class Stats : MonoBehaviour
    {
    #region VARS
    //===============================

        [SerializeField] private StatData _statData;

        // Mostly here to identify values in the inspector
        [Header("Physical")]
        [SerializeField] private float _physicalPower;
        [SerializeField] private float _physicalSlots;
        [SerializeField] private float _stamina;

        [Header("Magical")]
        [SerializeField] private float _magicalPower;
        [SerializeField] private float _magicalSlots;
        [SerializeField] private float _mana;

        [Header("Other")]
        [SerializeField] private float _health;
        [SerializeField] private float _damageReduction;

        private Attributes _attributes;

        // The array of values used for the actual writing & reading of the stats.
        private float[] _current = new float[CharacterEnums.STATS_COUNT];

        //===============================
        #endregion

        #region PRIVATE METHODS
        //===============================

        private void Awake()
        {
            _attributes = GetComponent<Attributes>();
            setAll();
        }

        private void Update()
        {
            setAll();
            //print(getStatsReport());
        }

        private void setAll()
        {
            setPhysicalPower(_attributes.GetAttribute(AttributeType.STRENGTH));
            setPhysicalSlots(_attributes.GetAttribute(AttributeType.STRENGTH));
            setStamina(_attributes.GetAttribute(AttributeType.DEXTERITY));
            
            setMagicalPower(_attributes.GetAttribute(AttributeType.WISDOM));
            setMagicalSlots(_attributes.GetAttribute(AttributeType.WISDOM));
            setMana(_attributes.GetAttribute(AttributeType.INTELLIGENCE));
            
            setMaxHealth(_attributes.GetAttribute(AttributeType.CONSTITUTION));
            setDamageReduction(_attributes.GetAttribute(AttributeType.CONSTITUTION));

            updateVals();
        }

        private void updateVals()
        {
            _current[(int)StatType.PHYSICAL_PWR]        = _physicalPower;
            _current[(int)StatType.PHYSICAL_SLOTS]      = _physicalSlots;
            _current[(int)StatType.STAMINA]             = _stamina;

            _current[(int)StatType.MAGICAL_PWR]         = _magicalPower;
            _current[(int)StatType.MAGICAL_SLOTS]       = _magicalSlots;
            _current[(int)StatType.MANA]                = _mana;

            _current[(int)StatType.MAX_HEALTH]          = _health; 
            _current[(int)StatType.DMG_RDX]             = _damageReduction;
        }

        #region SETTERS/FORMULAS
        private void setPhysicalPower(int strength)
        {
            _physicalPower = strength * _statData.EffectMultiplier;
        }

        private void setMagicalPower(int wisdom)
        {
            _magicalPower = wisdom * _statData.EffectMultiplier;
        }

        private void setPhysicalSlots(int strength)
        {
            int result;

            int threshold = _statData.SlotAttributeThreshold;
            int minSlots = _statData.MinSlots;
            int additionalSlots = (int)(strength * 2 * _statData.SlotMultiplier);

            if (strength > threshold)
            {
                result = minSlots + additionalSlots;
            }
            else
            {
                result = minSlots;
            }

            _physicalSlots = result;
        }

        private void setMagicalSlots(int wisdom)
        {
            int result;

            int threshold = _statData.SlotAttributeThreshold;
            int minSlots = _statData.MinSlots;
            int additionalSlots = (int)(wisdom * 2 * _statData.SlotMultiplier);

            if (wisdom > threshold)
            {
                result = minSlots + additionalSlots;
            }
            else
            {
                result = minSlots;
            }

            _magicalSlots = result;
        }

        private void setStamina(int dexterity)
        {
            _stamina = dexterity * _statData.ResourceMultiplier;
        }

        private void setMana(int intelligence)
        {
            _mana = intelligence * _statData.ResourceMultiplier;
        }

        private void setMaxHealth(int constitution)
        {
            _health = constitution * _statData.HealthMultiplier;
        }

        private void setDamageReduction(int constitution)
        {
            _damageReduction = constitution * _statData.DamageRdxMultiplier;
        }
        #endregion

        private string getStatsReport()
        {
            string result = "";

            for (int i = 0; i < CharacterEnums.STATS_COUNT; i++)
            {
                result += $"{CharacterEnums.STATS_NAMES[i]}:  {_current[i]}\n";
            }

            return result;
        }

    //===============================
    #endregion

    #region PUBLIC METHODS
    //===============================

        public float GetStat(StatType type)
        {
            return _current[(int)type];
        }

    //===============================
    #endregion
    }
}
