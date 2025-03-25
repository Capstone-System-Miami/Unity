using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using SystemMiami.Outdated;
using TMPro;
using UnityEngine;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(RectTransform))]
    public class ActionQuickslot : MonoBehaviour
    {
        [SerializeField] private SelectableSprite _icon;

        private RectTransform rt;
        private CombatAction _combatAction;
        private SelectionState _selectionState = SelectionState.UNSELECTED;

        /// <summary>
        /// The real combat object (AbilityPhysical, AbilityMagical, or Consumable).
        /// </summary>
        public CombatAction CombatAction => _combatAction;

        public RectTransform RT => rt;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_combatAction is NewAbility ability)
            {
                UpdateCooldowns(ability);
            }
        }

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
            Sprite iconSprite = data.Icon;
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
            /// TODO: Need a way more elegant solution than this.
            /// There should be some indication that the slot was clicked
            /// but can't be equipped because of cooldown.
            if (_selectionState == SelectionState.DISABLED) return;

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

        public void EnableSelection()
        {
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

        private void UpdateCooldowns(NewAbility ability)
        {
            if (ability.CooldownRemaining > 0
                && _selectionState != SelectionState.DISABLED)
            {
                DisableSelection();
            }
            else if (ability.CooldownRemaining == 0
                && _selectionState == SelectionState.DISABLED)
            {
                EnableSelection();
            }
        }

        public void SetPopupOnEnter()
        {
            if (_combatAction == null) { return; }

            PopUpHandler.MGR?.OpenPopup(_combatAction, this);
        }

        public void SetPopupOnExit()
        {
            PopUpHandler.MGR?.ClosePopup();
        }
    }
}
