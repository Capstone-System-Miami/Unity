// Author: Layla Hoey
using UnityEngine;
using SystemMiami.Management;
using SystemMiami.AbilitySystem;
using System;

namespace SystemMiami.UI
{
    public class AbilitySlot : MonoBehaviour
    {
        private int _index;

        [Header("Colors")]
        [SerializeField] private Color _typeColor;

        [Header("Background Panel")]
        [SerializeField] private SelectableSprite _background;

        [Header("Number")]
        [SerializeField] private SelectableText _number;
        [SerializeField] private SelectableSprite _numberBKG;

        [Header("Icon")]
        [SerializeField] private SelectableSprite _icon;
        [SerializeField] private SelectableSprite _iconBKG;

        [Header("Name")]
        [SerializeField] private SelectableText _name;
        [SerializeField] private SelectableSprite _nameBKG;

        private SelectionState _selectionState;

        public SelectionState State { get { return _selectionState; } }

        private void newStateAllFields(SelectionState state)
        {
            _background.NewState(state);

            _number.NewState(state);
            _numberBKG.NewState(state);

            _icon.NewState(state);
            _iconBKG.NewState(state);

            _name.NewState(state);
            _nameBKG.NewState(state);
        }

        public void Initialize(int index)
        {
            _index = index;
            
            // Set every string in the selectable text
            // so it won't change depending on
            // mouse over, clicking, etc.
            _number.SetAllMessages((_index + 1).ToString());

            DisableSelection();
        }

        public void Fill(Ability ability)
        {
            // This also won't change for now, but
            // we have the option to later, depending on
            // what the artists cook up UI-wise.
            _icon.SetAllSprites(ability.Icon);

            _name.SetAllMessages(ability.name);
        }

        public void OnClick()
        {
            Management.UI.MGR.EquipAbility.Invoke(_index);
        }

        public void Select()
        {
            if (_selectionState == SelectionState.DISABLED) { return; }

            newStateAllFields(SelectionState.SELECTED);
        }

        public void Deselect()
        {
            if (_selectionState == SelectionState.DISABLED) { return; }

            newStateAllFields(SelectionState.UNSELECTED);
        }

        public void EnableSelection()
        {
            _selectionState = SelectionState.UNSELECTED;

            newStateAllFields(SelectionState.UNSELECTED);
        }

        public void DisableSelection()
        {
            _selectionState = SelectionState.DISABLED;

            newStateAllFields(SelectionState.DISABLED);
        }
    }
}