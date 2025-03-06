using System.Collections.Generic;
using UnityEngine.Assertions;

namespace SystemMiami
{
    public class SingleSelector
    {
        private List<ISingleSelectable> elements = new();

        private int previousIndexInternal;
        private int currentIndexInternal;

        private int CurrentIndex {
            get
            {
                return currentIndexInternal;
            }
            set
            {
                Assert.IsTrue(elements != null);
                Assert.IsTrue(value < elements.Count);
                Assert.IsTrue(value >= 0);

                previousIndexInternal = currentIndexInternal;
                currentIndexInternal = value;
            }
        }

        public ISingleSelectable PreviousSelection {
            get { return elements[previousIndexInternal]; }
        }
        public ISingleSelectable CurrentSelection {
            get { return elements[CurrentIndex]; }
        }

        public SingleSelector(List<ISingleSelectable> elements)
        {
            Assert.IsNotNull(elements,
                $"SingleSelector was passed NULL during construction");

            Assert.IsTrue(elements.Count > 0,
                $"SingleSelector was passed an empty List during construction");

            Assert.IsNotNull(elements[0],
                $"SingleSelector was passed a list of NULL items during construction");

            this.elements = elements;

            Assert.IsNotNull(this.elements);
            Assert.IsTrue(this.elements.Count > 0);
            Assert.IsNotNull(elements[0]);

            for (int i = 0; i < this.elements.Count; i++)
            {
                this.elements[i].SelectionIndex = i;
                this.elements[i].Reference = this;

                Assert.IsNotNull(this);
                Assert.IsNotNull(elements[i]);
                Assert.IsNotNull(this.elements[i].Reference);
            }
        }

        public ISingleSelectable Select(int index)
        {
            return Select(index, false);
        }

        public ISingleSelectable Select(int index, bool reselectIfSame)
        {
            if (CurrentIndex == index && !reselectIfSame)
            {
                return CurrentSelection;
            }

            CurrentIndex = index;

            PreviousSelection.Deselect();
            CurrentSelection.Select();

            return CurrentSelection;
        }

        public ISingleSelectable Reset()
        {
            foreach (ISingleSelectable element in elements)
            {
                if (element == CurrentSelection) { continue; }
                element.Deselect();
            }
            return Select(0, true);
        }

        public bool IsCurrent(ISingleSelectable element)
        {
            return element == CurrentSelection;
        }
    }
}
