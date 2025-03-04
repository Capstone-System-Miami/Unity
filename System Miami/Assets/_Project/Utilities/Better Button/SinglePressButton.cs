using UnityEngine;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    public class SinglePressButton : BetterButton
    {
        [SerializeField] private float pressDuration = 0.1f;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }
    }
}
