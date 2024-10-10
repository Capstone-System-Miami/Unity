using System;

namespace SystemMiami
{
    public enum CharacterClassType
    {
        FIGHTER,
        MAGE, 
        ROGUE,
        TANK
    };

    public enum AttributeType
    {
        STRENGTH,
        DEXTERITY,
        CONSTITUTION,
        WISDOM,
        INTELLIGENCE
    };

    public enum StatType
    {
        PHYSICAL_PWR,
        PHYSICAL_SLOTS,
        STAMINA,

        MAGICAL_PWR,
        MAGICAL_SLOTS,
        MANA,

        MAX_HEALTH,
        DMG_RDX,
        SPEED
    };

    public class CharacterEnums
    {
        public static readonly string[] CHARACTER_CLASS_NAMES = Enum.GetNames(typeof(CharacterClassType));
        public static readonly string[] ATTRIBUTE_NAMES = Enum.GetNames(typeof(AttributeType));
        public static readonly string[] STATS_NAMES = Enum.GetNames(typeof(StatType));

        public static readonly int CHARACTER_CLASS_COUNT = CHARACTER_CLASS_NAMES.Length;
        public static readonly int ATTRIBUTE_COUNT = ATTRIBUTE_NAMES.Length;
        public static readonly int STATS_COUNT = STATS_NAMES.Length;
    }
}
