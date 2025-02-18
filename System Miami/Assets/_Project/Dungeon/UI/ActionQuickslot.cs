// Author: Layla Hoey
using System;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class ActionQuickslot : MonoBehaviour
    {
        /* [Header("Background Panel")]
         [SerializeField] private SelectableSprite _background;

         [Header("Number")]
         [SerializeField] private SelectableText _number;
         [SerializeField] private SelectableSprite _numberBKG;
        */
        [Header("Icon")]
        [SerializeField] private SelectableSprite _icon;
        /* [SerializeField] private SelectableSprite _iconBKG; */

        /* [Header("Name")]
         [SerializeField] private SelectableText _name;
         [SerializeField] private SelectableSprite _nameBKG; */

        private CombatAction _combatAction;

        private int _index;
        private Type _type;
        private SelectionState _selectionState;

        public CombatAction CombatAction { get { return _combatAction; } }
        public int Index { get { return _index; } }
        public Type Type { get { return _type; } }

        public SelectionState State { get { return _selectionState; } }

        public void Initialize(Type type, int index)
        {
            _index = index;
            _type = type;

            // Set every string in the selectable text
            // so it won't change depending on
            // mouse over, clicking, etc.
            /* _number.SetAllMessages((_index + 1).ToString()); */

            DisableSelection();
        }

        public void Fill(CombatAction combatAction)
        {
            _combatAction = combatAction;

            // This also won't change for now, but
            // we have the option to later, depending on
            // what the artists cook up UI-wise.
            _icon.SetAllSprites(_combatAction.Icon);

            /*  _name.SetAllMessages(_ability.name); */

            _type = combatAction.GetType();
        }

        public void Click()
        {
            UI.MGR.ClickSlot(this);
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


        private void newStateAllFields(SelectionState state)
        {
            /*  _background.NewState(state);

              _number.NewState(state);
              _numberBKG.NewState(state); */

            _icon.NewState(state);
            /*  _iconBKG.NewState(state); 

              _name.NewState(state);
              _nameBKG.NewState(state); */
        }

        public void SetPoupOnEnter()
        {
            PopUpHandler.Instance?.SetPopupAblility(_combatAction);
        }

        public void SetPoupOnExit()
        {
            PopUpHandler.Instance?.SetPopupAblility();

        }
    }
}
