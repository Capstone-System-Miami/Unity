using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class StatSet
    {
        private Dictionary<StatType, float> _dict = new Dictionary<StatType, float>();

        public StatSet()
        {
            int len = CharacterEnums.STATS_COUNT;

            for (int i = 0; i < len; i++)
            {
                _dict[(StatType)i] = 0;
            }
        }

        public StatSet(AttributeSet attributes, StatData scriptableStatData)
        {
            setPhysicalPower(attributes.Get(AttributeType.STRENGTH), scriptableStatData);
            setMagicalPower(attributes.Get(AttributeType.WISDOM), scriptableStatData);
            setPhysicalSlots(attributes.Get(AttributeType.STRENGTH), scriptableStatData);
            setMagicalSlots(attributes.Get(AttributeType.WISDOM), scriptableStatData);
            setStamina(attributes.Get(AttributeType.DEXTERITY), scriptableStatData);
            setMana(attributes.Get(AttributeType.INTELLIGENCE), scriptableStatData);
            setMaxHealth(attributes.Get(AttributeType.CONSTITUTION), scriptableStatData);
            setDamageReduction(attributes.Get(AttributeType.CONSTITUTION), scriptableStatData);
            setSpeed(attributes.Get(AttributeType.DEXTERITY), scriptableStatData);
        }

        public StatSet(StatSetSO statSet)
        {
            _dict[StatType.PHYSICAL_PWR]    = statSet.PhysicalPower;
            _dict[StatType.MAGICAL_PWR]     = statSet.MagicalPower;
            _dict[StatType.PHYSICAL_SLOTS]  = statSet.PhysicalSlots;
            _dict[StatType.MAGICAL_SLOTS]   = statSet.MagicalSlots;
            _dict[StatType.STAMINA]         = statSet.Stamina;
            _dict[StatType.MANA]            = statSet.Mana;
            _dict[StatType.MAX_HEALTH]      = statSet.MaxHealth;
            _dict[StatType.DMG_RDX]         = statSet.DamageRDX;
            _dict[StatType.SPEED]           = statSet.Speed;
        }

        public StatSet(int[] vals)
        {
            if (vals.Length != CharacterEnums.STATS_COUNT)
            {
                zero(ref vals);
            }

            foreach (StatType attr in _dict.Keys)
            {
                _dict[attr] = vals[(int)attr];
            }
        }

        private void zero(ref int[] incoming)
        {
            incoming = new int[CharacterEnums.STATS_COUNT];

            for (int i = 0; i < incoming.Length; i++)
            {
                incoming[i] = 0;
            }
        }

        #region SETTERS/FORMULAS
        private void setPhysicalPower(int strength, StatData statData)
        {
            _dict[StatType.PHYSICAL_PWR] = strength * statData.EffectMultiplier;
        }

        private void setMagicalPower(int wisdom, StatData statData)
        {
            _dict[StatType.MAGICAL_PWR] = wisdom * statData.EffectMultiplier;
        }

        private void setPhysicalSlots(int strength, StatData statData)
        {
            int result;

            int threshold = statData.SlotAttributeThreshold;
            int minSlots = statData.MinSlots;
            int additionalSlots = (int)(strength * 2 * statData.SlotMultiplier);

            if (strength > threshold)
            {
                result = minSlots + additionalSlots;
            }
            else
            {
                result = minSlots;
            }

            _dict[StatType.PHYSICAL_SLOTS] = result;
        }

        private void setMagicalSlots(int wisdom, StatData statData)
        {
            int result;

            int threshold = statData.SlotAttributeThreshold;
            int minSlots = statData.MinSlots;
            int additionalSlots = (int)(wisdom * 2 * statData.SlotMultiplier);

            if (wisdom > threshold)
            {
                result = minSlots + additionalSlots;
            }
            else
            {
                result = minSlots;
            }

            _dict[StatType.MAGICAL_SLOTS] = result;
        }

        private void setStamina(int dexterity, StatData statData)
        {
            _dict[StatType.STAMINA] = dexterity * statData.ResourceMultiplier;
        }

        private void setMana(int intelligence, StatData statData)
        {
            _dict[StatType.MANA] = intelligence * statData.ResourceMultiplier;
        }

        private void setMaxHealth(int constitution, StatData statData)
        {
            _dict[StatType.MAX_HEALTH] = constitution * statData.HealthMultiplier;
        }

        private void setDamageReduction(int constitution, StatData statData)
        {
            _dict[StatType.DMG_RDX] = constitution * statData.DamageRdxMultiplier;
        }

        private void setSpeed(int dexterity, StatData statData)
        {
            _dict[StatType.SPEED] = dexterity;
        }
        #endregion // ^setters^

        public float Get(StatType attr)
        {
            if (_dict.ContainsKey(attr))
            {
                return _dict[attr];
            }
            else
            {
                return 0;
            }
        }

        public void Set(StatType attr, float value)
        {
            if (_dict.ContainsKey(attr))
            {
                _dict[attr] = value;
            }
        }
    }
}
