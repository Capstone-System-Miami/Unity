using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace SystemMiami
{
    public class SingleSelector<T> where T : class, ISingleSelectable
    {
        private List<ISingleSelectable> elements;

        private int previousIndexInternal;
        private int currentIndexInternal;

        private int CurrentIndex
        {
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

        public T PreviousSelection {
            get { return elements[previousIndexInternal] as T; }
        }
        public T CurrentSelection {
            get { return elements[CurrentIndex] as T; }
        }

        public SingleSelector(List<T> elements)
            : this (elements.Cast<ISingleSelectable>().ToList())
        { }

        public SingleSelector(List<ISingleSelectable> elements)
        {
            Assert.IsNotNull(elements,
                "SingleSelector was passed NULL during construction");

            Assert.IsTrue(elements.Count > 0,
                "SingleSelector was passed an empty List during construction");

            this.elements = elements;

            for (int i = 0; i < this.elements.Count; i++)
            {
                this.elements[i].SelectionIndex = i;
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
    }
}
