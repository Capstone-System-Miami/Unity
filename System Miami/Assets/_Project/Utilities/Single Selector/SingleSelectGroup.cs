using System.Collections;
using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace SystemMiami.ui
{
    public abstract class SingleSelectGroup<T> : MonoBehaviour where T : class, ISingleSelectable
    {
        [SerializeField] dbug log = new();

        [SerializeField] private List<SingleSelectButton> buttons = new();

        private List<T> selectables;

        public SingleSelector<SingleSelectButton> ButtonSelector { get; private set; }
        public SingleSelector<T> ElementSelector { get; private set; }

        public T CurrentElement => ElementSelector?.CurrentSelection;

        protected virtual void Awake()
        {
            selectables = GetSelectables();

            Assert.IsTrue(buttons.Count == selectables.Count,
                $"Amount of buttons was not equal to" +
                $"amount of elements in {name}");

            ButtonSelector = new(buttons);
            buttons.ForEach(button => button.Init( (ind) => SelectElement(ind) ));

            ElementSelector = new(selectables);
        }

        // FIX: Figure out why the fuck this is the only thing that works
        //
        private void OnEnable()
        {
            IEnumerator godHelpUsAll()
            {
                float remaining = 1f;
                while (remaining > 0)
                {
                    remaining -= Time.deltaTime;
                    SelectElement(0, true);
                    yield return null;
                }
            }

            StartCoroutine(godHelpUsAll());
        }

        private void Start()
        {
            SelectElement(0, true);
        }

        public virtual void SelectElement(int index)
        {
            ButtonSelector.Select(index);
            ElementSelector.Select(index);
        }


        public virtual void SelectElement(int index, bool selectIfSame)
        {
            ButtonSelector.Select(index, selectIfSame);
            ElementSelector.Select(index, selectIfSame);
        }

        public virtual void ReSelectCurrent()
        {
            ButtonSelector.Select(CurrentElement.SelectionIndex, true);
            ElementSelector.Select(CurrentElement.SelectionIndex, true);
        }

        protected abstract List<T> GetSelectables();
    }
}
