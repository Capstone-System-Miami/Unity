using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami.ui
{
    public class ActionQuickslot : MonoBehaviour
    {
        [SerializeField] private SelectableSprite _icon; 
       
        
        private CombatAction _combatAction;
        private SelectionState _selectionState = SelectionState.UNSELECTED;

        /// <summary>
        /// The real combat object (AbilityPhysical, AbilityMagical, or Consumable).
        /// </summary>
        public CombatAction CombatAction => _combatAction;

        /// <summary>
        /// Assign a CombatAction object to this slot and update the UI from it.
        /// </summary>
        public void Fill(CombatAction action)
        {
            _combatAction = action;
            if (_combatAction == null)
            {
                Clear();
                return;
            }
 
           ItemData data = Database.MGR.GetDataWithJustID(action.ID);
            var iconSprite = data.Icon;
            if (iconSprite != null)
            {
                _icon.SetAllSprites(iconSprite);
            }

            
        }

        /// <summary>
        ///  remove the CombatAction and disable the slot.
        /// </summary>
        public void Clear()
        {
            _combatAction = null;
            _icon.SetAllSprites(null);
            
        }

        /// <summary>
        /// Called by a UI Button event or UnityEvent on click.
        /// </summary>
        public void Click()
        {
            // Notify the UI Manager
            UI.MGR.ClickSlot(this);
        }

        public void Select()
        {
            if (_selectionState == SelectionState.DISABLED) return;
            _selectionState = SelectionState.SELECTED;
            UpdateVisualState(SelectionState.SELECTED);
        }

        public void Deselect()
        {
            if (_selectionState == SelectionState.DISABLED) return;
            _selectionState = SelectionState.UNSELECTED;
            UpdateVisualState(SelectionState.UNSELECTED);
        }

        public void DisableSelection()
        {
            _selectionState = SelectionState.DISABLED;
            UpdateVisualState(SelectionState.DISABLED);
        }

        private void UpdateVisualState(SelectionState state)
        {
            
            _icon.NewState(state);
            
        }
    }
}
