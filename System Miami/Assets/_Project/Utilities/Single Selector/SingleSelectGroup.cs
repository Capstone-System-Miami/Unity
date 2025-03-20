using System;
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

        private void OnEnable()
        {
            if (CurrentElement != null)
            {
                ReSelectCurrent();
            }
        }

        private void Start()
        {
            ButtonSelector.Reset();
            ElementSelector.Reset();
        }

        public virtual void SelectElement(int index)
        {
            ButtonSelector.Select(index);
            ElementSelector.Select(index);
        }

        public virtual void ReSelectCurrent()
        {
            ButtonSelector.Select(CurrentElement.SelectionIndex, true);
            ElementSelector.Select(CurrentElement.SelectionIndex, true);
        }

        protected abstract List<T> GetSelectables();

        //protected abstract List<MonoBehaviour> GetRawMonos();

        //private List<T> GetAsT()
        //{
        //    List<T> result = new();
        //    List<MonoBehaviour> monos = GetRawMonos();

        //    foreach (MonoBehaviour mono in monos)
        //    {
        //        if (mono is not T t)
        //        {
        //            log.error($"{mono.name} does NOT implement {typeof(ISingleSelectable)}");
        //        }
        //        else
        //        {
        //            result.Add(t);
        //        }
        //    }

        //    return result;
        //}
    }
}
