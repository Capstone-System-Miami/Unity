// Author: Layla Hoey
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.UI
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Image _background;

        [SerializeField] private ClassSwitcher<string> _message;
        [SerializeField] private StructSwitcher<Color> _textColor;

        [SerializeField] private ClassSwitcher<Sprite> _backgroundSprite;
        [SerializeField] private StructSwitcher<Color> _backgroundColor;

        private bool FLAG_change;

        private void Update()
        {            
            if (!FLAG_change) { return; }

            _text.text = _message.Get();
            _text.color = _textColor.Get();
            _background.sprite = _backgroundSprite.Get();
            _background.color = _backgroundColor.Get();

            FLAG_change = false;
        }

        public void Set(string text)
        {
            _message.Set(text);
            FLAG_change = true;
        }

        public void Set(Color color)
        {
            _textColor.Set(color);
            FLAG_change = true;
        }

        public void Set(string text, Color color)
        {
            _message.Set(text);
            _textColor.Set(color);
            FLAG_change = true;
        }

        public void Revert()
        {
            _message.Revert();
            _textColor.Revert();
            FLAG_change = true;
        }

        public void SetDefault()
        {
            _message.Reset();
            _textColor.Reset();
            FLAG_change = true;
        }
    }
}
