using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class LabeledField : MonoBehaviour
    {
        [SerializeField] private Image _panelBackground;

        public TextBox Label;
        public TextBox Value;

        public void SetBackground(Sprite sprite)
        {
            _panelBackground.sprite = sprite;
        }

        public void SetBackground(Color color)
        {
            _panelBackground.color = color;
        }

        public void SetBackground(Sprite sprite, Color color)
        {
            _panelBackground.sprite = sprite;
            _panelBackground.color = color;
        }
    }
}
