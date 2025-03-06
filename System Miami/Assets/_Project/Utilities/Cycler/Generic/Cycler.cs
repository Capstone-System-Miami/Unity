// Authors: Layla
using System.Collections.Generic;
using System.Linq;

namespace SystemMiami.Utilities
{
    public class Cycler<T> where T : class, ISelectable
    {
        private readonly List<T> elements = new();
        private readonly bool wrapStart = false;
        private readonly bool wrapEnd = false;

        private int currentIndex;
        private T currentElement;

        public Cycler()
            : this(new(), false, false)
        { }

        public Cycler(List<T> elements, bool wrapStart, bool wrapEnd)
        {
            this.elements = elements;
            this.wrapStart = wrapStart;
            this.wrapEnd = wrapEnd;

            currentIndex = 0;
            currentElement = default;
        }

        private List<T> Elements
        {
            get { return elements ?? new(); }
        }


        private bool IndexIsGreater => currentIndex >= Elements.Count;
        private bool IndexIsLess => currentIndex < 0;

        public bool NeedsUpdate
        {
            get
            {
                return Elements.Any() && currentElement != Elements[currentIndex];
            }
        }

        public T CurrentElement { get { return currentElement; } }

        public void BeginCycle()
        {
            currentIndex = 0;
            UpdateCurrentElement();
        }

        public void EndCycle()
        {
            currentIndex = 0;
            currentElement = null;
            Elements.ForEach(element => element.Deselect());
        }

        public void NextElement()
        {
            currentIndex++;

            if (IndexIsGreater)
            {
                if (wrapEnd)
                {
                    WrapIndex();
                }
                else
                {
                    EndCycle();
                }
            }

            CycleToCurrentElement();
        }

        public void PrevElement()
        {
            currentIndex--;

            if (IndexIsLess)
            {
                if (wrapStart)
                {
                    WrapIndex();
                }
                else
                {
                    currentIndex = 0;
                }
            }

            CycleToCurrentElement();
        }

        private void CycleToCurrentElement()
        {
            currentElement?.Deselect();
            UpdateCurrentElement();
            currentElement?.Select();
        }

        private void UpdateCurrentElement()
        {
            if (!Elements.Any()) { return; }

            currentElement = Elements[currentIndex];
        }

        private void WrapIndex()
        {
            if (IndexIsLess)
            {
                currentIndex = Elements.Count - 1;
            }
            else if (IndexIsGreater)
            {
                currentIndex = 0;
            }
        }
    }
}
