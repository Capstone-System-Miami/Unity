using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.UI
{
    public class UnequipPrompt : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _text;

        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            if (_text == null)
            {
                _text = GetComponentInChildren<Text>();
            }
            
            Hide();
        }

        public void Show()
        {
            _image.enabled = true;
            _text.enabled = true;
        }

        public void Hide()
        {
            _image.enabled = false;
            _text.enabled = false;
        }
    }
}
