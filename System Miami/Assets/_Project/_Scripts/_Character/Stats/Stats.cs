// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [RequireComponent(typeof(Attributes))]
    public class Stats : MonoBehaviour
    {
        #region VARS
        //===============================
        [SerializeField] private bool _printReports;

        [SerializeField] private StatData _statData;

        private Attributes _attributes;

        // The Dictionary of values used for the actual writing & reading of the stats.
        private Dictionary<StatType, float> _currentStats = new Dictionary<StatType, float>();

        //===============================
        #endregion // ^vars^

        #region PRIVATE METHODS
        //===============================

        private void Awake()
        {
            _attributes = GetComponent<Attributes>();
        }

        private void Start()
        {
            setAll();
        }

        private void Update()
        {
            setAll();
            if (_printReports) { print(getStatsReport()); }
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
            setSpeed(_attributes.GetAttribute(AttributeType.DEXTERITY));
        }

    #region SETTERS/FORMULAS
        private void setPhysicalPower(int strength)
        {
            _currentStats[StatType.PHYSICAL_PWR] = strength * _statData.EffectMultiplier;
        }

        private void setMagicalPower(int wisdom)
        {
            _currentStats[StatType.MAGICAL_PWR] = wisdom * _statData.EffectMultiplier;
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

            _currentStats[StatType.PHYSICAL_SLOTS] = result;
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

            _currentStats[StatType.MAGICAL_SLOTS] = result;
        }

        private void setStamina(int dexterity)
        {
            _currentStats[StatType.STAMINA] = dexterity * _statData.ResourceMultiplier;
        }

        private void setMana(int intelligence)
        {
            _currentStats[StatType.MANA] = intelligence * _statData.ResourceMultiplier;
        }

        private void setMaxHealth(int constitution)
        {
            _currentStats[StatType.MAX_HEALTH] = constitution * _statData.HealthMultiplier;
        }

        private void setDamageReduction(int constitution)
        {
            _currentStats[StatType.DMG_RDX] = constitution * _statData.DamageRdxMultiplier;
        }

        private void setSpeed(int dexterity)
        {
            _currentStats[StatType.SPEED] = dexterity;
        }
        #endregion // ^setters^

        private string getStatsReport()
        {
            string result = "";

            foreach(StatType stat in _currentStats.Keys)
            {
                result += $"{stat}: {_currentStats[stat]}\n";
            }

            return result;
        }

        //===============================
        #endregion // ^private^

        #region PUBLIC METHODS
        //===============================

        public float GetStat(StatType type)
        {
            if (_currentStats == null || _currentStats.Count == 0) { return 0f; }

            return _currentStats[type];
        }

        //===============================
        #endregion // ^public^
    }
}
