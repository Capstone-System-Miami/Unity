using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class SpriteBox : MonoBehaviour
    {
        [SerializeField] private Image _background;

        [SerializeField] private Image _foreground;

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
            _foreground.enabled = true;
        }

        public void HideForeground()
        {
            _foreground.enabled = false;
        }

        public void SetForeground(Sprite sprite)
        {
            _foreground.sprite = sprite;
        }

        public void SetForeground(Color color)
        {
            _foreground.color = color;
        }

        public void SetForeground(Sprite sprite, Color color)
        {
            _foreground.sprite = sprite;
            _foreground.color = color;
        }
    }
}
