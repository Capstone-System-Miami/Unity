using TMPro;
using UnityEngine;

namespace SystemMiami.ui
{
    public class SelectableTMP : SelectableText
    {
        [SerializeField] TMP_Text _tmp;

        protected override void Update()
        {
            _currentText = useTextFromComponent ? getStringFromComponent() : getCurrentText();
            _currentColor = getCurrentColor();

            if (_tmp.text != _currentText)
            {
                _tmp.text = _currentText;
            }
            if (_tmp.color != _currentColor)
            {
                _tmp.color = _currentColor;
            }
        }

        protected override string getStringFromComponent()
        {
            return _tmp?.text ?? string.Empty;
        }

        protected override Color getColorFromComponent()
        {
            return _tmp != null ? _tmp.color : Color.white;
        }
    }
}
