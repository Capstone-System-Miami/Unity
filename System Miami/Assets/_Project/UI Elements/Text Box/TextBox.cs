using UnityEngine;
using UnityEngine.UI;
using TMPro;



namespace SystemMiami
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _background;

        [SerializeField] private Text _text;
        [SerializeField] private TMP_Text _TextMeshPro;

        public void ShowBackground()
        {
            if (_background != null)
            {
                _background.enabled = true;
            }
            
        }

        public void HideBackground()
        {
            if (_background != null)
            {
                _background.enabled = false;
            }
            
        }

        public void SetBackground(Sprite sprite)
        {
            if (_background != null)
            {
                _background.sprite = sprite;
            }
            
        }

        public void SetBackground(Color color)
        {
            if (_background != null)
            {
                _background.color = color;
            }
            
        }

        public void SetBackground(Sprite sprite, Color color)
        {
            if (_background != null)
            {
                _background.sprite = sprite;
                _background.color = color;
            }
            
        }

        public void ShowForeground()
        {
            if (_text != null)
            {
                _text.enabled = true;
            }
            else if (_TextMeshPro != null)
            {
                _TextMeshPro.enabled = true;
            }
            
        }

        public void HideForeground()
        {
            if (_text != null)
            {
                _text.enabled = false;
            }
            else if (_TextMeshPro != null)
            {
                _TextMeshPro.enabled = false;
            }
            
        }

        public void SetForeground(string text)
        {
            if (_text != null)
            {
                _text.text = text;
            }
            else if (_TextMeshPro != null)
            {
                _TextMeshPro.text = text;
            }
            
        }

        public void SetForeground(Color color)
        {
            if (_text != null)
            {
                
                _text.color = color;
            }
            else if(_TextMeshPro != null)
            {
                _TextMeshPro.color = color;
            }

        }

        public void SetForeground(string text, Color color)
        {
            if (_text != null)
            {
                _text.text = text;
                _text.color = color;
            }
            else if(_TextMeshPro != null)
            {
                _TextMeshPro.text = text;
                _TextMeshPro.color = color;
            }
           
        }
    }
}
