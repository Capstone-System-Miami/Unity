using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.UI
{
    public class SelectableText : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] HighlightableClassSet<string> _unselectedMessages;
        [SerializeField] HighlightableClassSet<string> _selectedMessages;
        [SerializeField] HighlightableClassSet<string> _disabledMessages;

        [SerializeField] HighlightableStructSet<Color> _unselectedColors = new HighlightableStructSet<Color>(Color.grey);
        [SerializeField] HighlightableStructSet<Color> _selectedColors = new HighlightableStructSet<Color>(Color.grey);
        [SerializeField] HighlightableStructSet<Color> _disabledColors = new HighlightableStructSet<Color>(Color.white);

        private SelectionState _selectionState;
        private bool _isHighlighted;

        private string _currentText;
        private Color _currentColor;

        private void Update()
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

        private string getCurrentText()
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

        private Color getCurrentColor()
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
    }
}
