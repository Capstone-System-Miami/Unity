using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public class SelectableText : MonoBehaviour, ISelectable
    {
        [SerializeField] Text _text;
        [SerializeField] HighlightableClassSet<string> _unselectedMessages;
        [SerializeField] HighlightableClassSet<string> _selectedMessages;
        [SerializeField] HighlightableClassSet<string> _disabledMessages;

        [SerializeField] HighlightableStructSet<Color> _unselectedColors = new HighlightableStructSet<Color>(Color.grey);
        [SerializeField] HighlightableStructSet<Color> _selectedColors = new HighlightableStructSet<Color>(Color.grey);
        [SerializeField] HighlightableStructSet<Color> _disabledColors = new HighlightableStructSet<Color>(Color.white);

        [SerializeField] protected bool useTextFromComponent;

        protected SelectionState _selectionState;
        protected bool _isHighlighted;

        protected string _currentText;
        protected Color _currentColor;

        public bool IsSelected => _selectionState == SelectionState.SELECTED;

        private void Start()
        {
            if (useTextFromComponent)
            {
                _currentText = getStringFromComponent();
                _currentColor = getColorFromComponent();
            }
        }

        protected virtual void Update()
        {
            _currentText = getCurrentText();
            _currentColor = getCurrentColor();

            if (_text.text != _currentText)
            {
                _text.text = _currentText;
            }

            if (_text.color != _currentColor)
            {
                _text.color = _currentColor;
            }
        }
        public void Highlight()
        {
            _isHighlighted = true;
        }

        public void UnHighlight()
        {
            _isHighlighted = false;
        }
        public void NewState(SelectionState state)
        {
            _selectionState = state;
        }

        public void Select()
        {
            if (_selectionState == SelectionState.DISABLED) { return; }
            NewState(SelectionState.SELECTED);
        }

        public void Deselect()
        {
            if (_selectionState == SelectionState.DISABLED) { return; }
            NewState(SelectionState.UNSELECTED);
        }

        public void EnableSelection()
        {
            Deselect();
        }

        public void DisableSelection()
        {
            NewState(SelectionState.DISABLED);
        }

        protected virtual string getStringFromComponent()
        {
            return _text.text ?? string.Empty;
        }

        protected virtual Color getColorFromComponent()
        {
            return _text.color;
        }

        protected virtual string getCurrentText()
        {
            if (_isHighlighted)
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedMessages.Highlighted,
                    SelectionState.SELECTED => _selectedMessages.Highlighted,
                    SelectionState.DISABLED => _disabledMessages.Highlighted,
                    _ => _unselectedMessages.Highlighted,
                };
            }
            else
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedMessages.Normal,
                    SelectionState.SELECTED => _selectedMessages.Normal,
                    SelectionState.DISABLED => _disabledMessages.Normal,
                    _ => _unselectedMessages.Normal,
                };
            }
        }

        protected virtual Color getCurrentColor()
        {
            if (_isHighlighted)
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedColors.Highlighted,
                    SelectionState.SELECTED => _selectedColors.Highlighted,
                    SelectionState.DISABLED => _disabledColors.Highlighted,
                    _ => _unselectedColors.Highlighted
                };
            }
            else
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedColors.Normal,
                    SelectionState.SELECTED => _selectedColors.Normal,
                    SelectionState.DISABLED => _disabledColors.Normal,
                    _ => _unselectedColors.Normal
                };
            }
        }

        public void SetAllMessages(string text)
        {
            _unselectedMessages = new HighlightableClassSet<string>(text);
            _selectedMessages = new HighlightableClassSet<string>(text);
            _disabledMessages = new HighlightableClassSet<string>(text);
        }

    }
}
