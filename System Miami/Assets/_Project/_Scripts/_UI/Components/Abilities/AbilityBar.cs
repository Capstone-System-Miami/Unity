using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.UI
{
    public class AbilityBar : MonoBehaviour
    {
        [SerializeField] private AbilityType _barType;

        [SerializeField] private Abilities _playerAbilities;
        [SerializeField] private Stats _playerStats;

        [SerializeField] private AbilitySlot[] _slots;

        [SerializeField] private UnequipPrompt _unequipPrompt; 

        public AbilityType BarType { get { return _barType; } }

        private void OnEnable()
        {
            _playerAbilities.EquipAbility += onEquipAbility;
            _playerAbilities.UnequipAbility += onUnequipAbility;
        }

        private void OnDisable()
        {
            _playerAbilities.EquipAbility -= onEquipAbility;
            _playerAbilities.UnequipAbility -= onUnequipAbility;
        }

        private void onEquipAbility(AbilityType type, int index)
        {
            if (type != _barType) { return; }

            _slots[index].Select();
            _unequipPrompt.Show();
        }

        private void onUnequipAbility()
        {
            unequipAll();
            _unequipPrompt.Hide();
        }

        private void unequipAll()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Deselect();
            }
        }

        private void Start()
        {
            initializeSlots();
            fillSlots();
        }

        private void initializeSlots()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Initialize(_barType, i);
            }
        }

        private void fillSlots()
        {
            int toFill = Mathf.Min(new int[] { _slots.Length, getSlotStat(), getAbilities().Count});

            for (int i = 0 ; i < toFill; i++)
            {
                _slots[i].Fill(getAbilities()[i]);
                _slots[i].EnableSelection();
            }
        }

        private List<Ability> getAbilities()
        {
            return _barType switch
            {
                AbilityType.PHYSICAL    => _playerAbilities.Physical,
                AbilityType.MAGICAL     => _playerAbilities.Magical,
                _                       => _playerAbilities.Physical
            };
        }

        private int getSlotStat()
        {
            return _barType switch
            {
                AbilityType.PHYSICAL    => (int)_playerStats.GetStat(StatType.PHYSICAL_SLOTS),
                AbilityType.MAGICAL     => (int)_playerStats.GetStat(StatType.MAGICAL_SLOTS),
                _                       => (int)_playerStats.GetStat(StatType.PHYSICAL_SLOTS)
            };
        }
    }
}
