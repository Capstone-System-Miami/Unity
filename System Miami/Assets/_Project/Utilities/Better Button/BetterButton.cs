using SystemMiami.ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    [RequireComponent(
        typeof(SelectableSprite))]
    public abstract class BetterButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEvent AdditionalOnPointerEnter;
        [SerializeField] private UnityEvent AdditionalOnPointerExit;
        [SerializeField] private UnityEvent AdditionalOnPointerDown;
        [SerializeField] private UnityEvent AdditionalOnPointerUp;

        protected SelectableSprite selectableSprite;
        [field: SerializeField, ReadOnly] public bool isButtonEnabled { get; protected set; }
        [field: SerializeField, ReadOnly] public bool isPointerHere { get; protected set; }
        [field: SerializeField, ReadOnly] public bool isPointerDown { get; protected set; }


        protected virtual void Awake()
        {
            selectableSprite = GetComponent<SelectableSprite>();
        }

        private void OnEnable()
        {
            EnableInteraction();
        }

        private void OnDisable()
        {
            DisableInteraction();
        }

        public virtual void EnableInteraction()
        {
            isButtonEnabled = true;
            selectableSprite.EnableSelection();
        }

        public virtual void DisableInteraction()
        {
            isButtonEnabled = false;
            selectableSprite.DisableSelection();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            isPointerHere = true;
            AdditionalOnPointerEnter?.Invoke();
            selectableSprite.Highlight();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            isPointerHere = false;
            AdditionalOnPointerExit?.Invoke();
            selectableSprite.UnHighlight();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            AdditionalOnPointerDown?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            AdditionalOnPointerUp?.Invoke();
        }
    }
}
