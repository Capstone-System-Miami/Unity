// Author: Layla Hoey
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.UI
{
    public class SpriteBox : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _backrgoundImage;

        [SerializeField] private ClassSwitcher<Sprite> _mainSprite;
        [SerializeField] private StructSwitcher<Color> _mainColor;

        [SerializeField] private ClassSwitcher<Sprite> _backgroundSprite;
        [SerializeField] private StructSwitcher<Color> _backgroundColor;

        private bool FLAG_change;

        private void Update()
        {
            if (!FLAG_change) { return; }

            _mainImage.sprite = _mainSprite.Get();
            _mainImage.color = _mainColor.Get();
            _backrgoundImage.sprite = _backgroundSprite.Get();
            _backrgoundImage.color = _backgroundColor.Get();

            FLAG_change = false;
        }

        public void Set(Sprite sprite)
        {
            _mainSprite.Set(sprite);
            FLAG_change = true;
        }

        public void Set(Color color)
        {
            _mainColor.Set(color);
            FLAG_change = true;
        }

        public void Set(Sprite sprite, Color color)
        {
            _mainSprite.Set(sprite);
            _mainColor.Set(color);
            FLAG_change = true;
        }

        public void Revert()
        {
            _mainSprite.Revert();
            _mainColor.Revert();
            FLAG_change = true;
        }

        public void SetDefault()
        {
            _mainSprite.Reset();
            _mainColor.Reset();
            FLAG_change = true;
        }
    }
}
