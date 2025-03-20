using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : MonoBehaviour, IUIElement
    {
        [SerializeField] private dbug log;

        public RectTransform RT { get; private set; }

        public Canvas Canvas { get; private set; }

        protected virtual void Awake()
        {
            RT = GetComponent<RectTransform>();

            if (!UiHelpers.TryGetCanvasInParents(transform, out Canvas c))
            {
                log.error($"{name} is a UI element, but it is not on a canvas.");
            }
            else
            {
                Canvas = c;
            }
        }

        public abstract void Show();
        public abstract void Hide();
    }
}
