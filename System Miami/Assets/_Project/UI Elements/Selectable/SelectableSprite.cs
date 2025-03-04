using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public class SelectableSprite : MonoBehaviour, ISelectable
    {
        [SerializeField] Image _image;

        [SerializeField] HighlightableClassSet<Sprite> _unselectedSprites;
        [SerializeField] HighlightableClassSet<Sprite> _selectedSprites;
        [SerializeField] HighlightableClassSet<Sprite> _disabledSprites;

        [SerializeField] HighlightableStructSet<Color> _unselectedColors = new HighlightableStructSet<Color>(Color.white);
        [SerializeField] HighlightableStructSet<Color> _selectedColors = new HighlightableStructSet<Color>(Color.yellow);
        [SerializeField] HighlightableStructSet<Color> _disabledColors = new HighlightableStructSet<Color>(Color.grey);

        private SelectionState _selectionState;
        private bool _isHighlighted;

        private Sprite _currentSprite;
        private Color _currentColor;

        bool ISelectable.IsSelected => _selectionState == SelectionState.SELECTED;

        private void Start()
        {
            checkSprites();
        }

        private void checkSprites()
        {
            // If nothing was set in the inspector, just use the current
            // sprite on the Image component for everything.
            if (_unselectedSprites.Normal == null)
            {
                SetAllSprites(_image.sprite);
            }
        }

        private void Update()
        {
            _currentSprite = getCurrentSprite();
            _currentColor = getCurrentColor();

            if (_image.sprite != _currentSprite)
            {
                _image.sprite = _currentSprite;
            }

            if (_image.color != _currentColor)
            {
                _image.color = _currentColor;
            }
        }

        private Sprite getCurrentSprite()
        {
            if (_isHighlighted)
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedSprites.Highlighted,
                    SelectionState.SELECTED => _selectedSprites.Highlighted,
                    SelectionState.DISABLED => _disabledSprites.Highlighted,
                    _ => _unselectedSprites.Highlighted,
                };
            }
            else
            {
                return _selectionState switch
                {
                    SelectionState.UNSELECTED => _unselectedSprites.Normal,
                    SelectionState.SELECTED => _selectedSprites.Normal,
                    SelectionState.DISABLED => _disabledSprites.Normal,
                    _ => _unselectedSprites.Normal,
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

        public void SetAllSprites(Sprite sprite)
        {
            _unselectedSprites = new HighlightableClassSet<Sprite>(sprite);
            _selectedSprites = new HighlightableClassSet<Sprite>(sprite);
            _disabledSprites = new HighlightableClassSet<Sprite>(sprite);
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
    }
}
