using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private Image _background;

        [SerializeField] private Text _text;

        public void ShowBackground()
        {
            _background.enabled = true;
        }

        public void HideBackground()
        {
            _background.enabled = false;
        }

        public void SetBackground(Sprite sprite)
        {
            _background.sprite = sprite;
        }

        public void SetBackground(Color color)
        {
            _background.color = color;
        }

        public void SetBackground(Sprite sprite, Color color)
        {
            _background.sprite = sprite;
            _background.color = color;
        }

        public void ShowForeground()
        {
            _text.enabled = true;
        }

        public void HideForeground()
        {
            _text.enabled = false;
        }

        public void SetForeground(string text)
        {
            _text.text = text;
        }

        public void SetForeground(Color color)
        {
            _text.color = color;
        }

        public void SetForeground(string text, Color color)
        {
            _text.text = text;
            _text.color = color;
        }
    }
}
