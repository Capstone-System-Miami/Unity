using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.UI
{
    public class AbilityBar : MonoBehaviour
    {
        [SerializeField] private Abilities _playerAbilities;

        [SerializeField] private AbilitySlot[] _slots;

        [SerializeField] private UnequipPrompt _unequipPrompt;

        private int _selectedIndex;

        private void Start()
        {
            initializeSlots();
            fillSlots();
        }

        private void initializeSlots()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Initialize(i);
            }
        }

        private void fillSlots()
        {
            int toFill = Mathf.Min(_slots.Length, _playerAbilities.List.Count);

            for (int i = 0 ; i < toFill; i++)
            {
                _slots[i].Fill(_playerAbilities.List[i]);
                _slots[i].EnableSelection();
            }
        }

        public void EquipAbility(int index)
        {
            if (_slots[index].State == SelectionState.DISABLED) { return; }

            _selectedIndex = index;

            _slots[_selectedIndex].Select();
            _playerAbilities.OnEquip(_selectedIndex);
            _unequipPrompt.Show();
        }

        public void UnEquipSelected()
        {
            _slots[_selectedIndex].Deselect();
            _unequipPrompt.Hide();
        }
    }
}
