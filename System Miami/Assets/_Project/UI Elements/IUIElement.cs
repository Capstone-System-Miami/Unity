using UnityEngine;

namespace SystemMiami.ui
{
    public interface IUIElement
    {
        RectTransform RT { get; }
        Canvas Canvas { get; }
        void Hide();
        void Show();
    }
}
