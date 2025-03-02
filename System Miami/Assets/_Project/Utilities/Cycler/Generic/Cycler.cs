using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

namespace SystemMiami
{
    [System.Serializable]
    public class Cycler<T> where T : class, ICycleable
    {
        private readonly List<T> elements = new();
        private readonly bool wrapStart = false;
        private readonly bool wrapEnd = false;

        private int currentIndex;
        private T currentElement;

        public Cycler()
            : this( new(), false, false )
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
                if (!Elements.Any()) { return false; }

                return currentElement != Elements[currentIndex];
            }
        }

        public T CurrentElement { get { return currentElement; } }

        //private void OnEnable()
        //{
        //    GAME.MGR.Pause += HandlePause;
        //    GAME.MGR.Resume += HandleResume;
        //}

        //private void OnDisable()
        //{
        //    GAME.MGR.Pause -= HandlePause;
        //    GAME.MGR.Resume -= HandleResume;
        //}

        private void Update()
        {
            UpdateCurrentElement();
            ActivateCurrentElement();
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
        }

        public void BeginCycle()
        {
            currentIndex = 0;
        }

        public void EndCycle()
        {
            currentIndex = 0;
            currentElement = null;
            Elements.ForEach(element => element.CycleAway());
        }

        private void UpdateCurrentElement()
        {
            if (!Elements.Any()) { return; }

            currentElement = Elements[currentIndex];
        }

        private void ActivateCurrentElement()
        {
            foreach (T element in Elements)
            {
                if (element == currentElement)
                {
                    element.CycleTo();
                }
                else
                {
                    element.CycleAway();
                }                
            }
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

        //private void HandlePause()
        //{
        //    Button[] buttons = GetComponentsInChildren<Button>();
        //    buttons.ToList().ForEach(button => button.interactable = false);
        //}

        //private void HandleResume()
        //{
        //    Button[] buttons = GetComponentsInChildren<Button>();
        //    buttons.ToList().ForEach(button => button.interactable = true);
        //}
    }
}
