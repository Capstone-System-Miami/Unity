using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace SystemMiami
{
    public class SingleSelectButton : SelectableButton, ISingleSelectable
    {
        SingleSelector ISingleSelectable.Reference { get; set; }

        int ISingleSelectable.SelectionIndex { get; set; }

        private bool isSingleSelected => selectionInterface.Reference != null
            ? selectionInterface.Reference.CurrentSelection == selectionInterface
            : false;

        private ISingleSelectable selectionInterface => this as ISingleSelectable;


        private void Update()
        {
            if (selectionInterface.Reference == null) { return; }

            IsSelected = isSingleSelected;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            Assert.IsNotNull(selectionInterface);
            Assert.IsNotNull(selectionInterface.Reference);

            // Since we can only have one thing selected,
            // we should only deselect via another button getting selected,
            // rather than by Toggling.
            // Select Via the SingleSelector instead of directly.
            selectionInterface.Reference.Select(selectionInterface.SelectionIndex);
        }
    }
}
