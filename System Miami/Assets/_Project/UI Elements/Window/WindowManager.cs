using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using SystemMiami.Utilities;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(Canvas))]
    public abstract class WindowManager : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [SerializeField] private Window prefab;

        [SerializeField] private int maxWindows;

        [field: SerializeField, ReadOnly]
        protected List<Window> openWindows = new();

        [field: SerializeField, ReadOnly]
        protected List<Window> hiddenWindows = new();

        public Canvas Canvas { get; private set; }

        public event Action<Window> WindowOpened;
        public event Action<Window> WindowHidden;
        public event Action<Window> WindowShown;

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
        }

        public void RequestOpenWindow(IWindowable windowableObject)
        {
            Window newWindow = Instantiate(prefab, transform);

            Assert.IsNotNull(newWindow);

            if (windowableObject.GetType() != newWindow.GenericType)
            {
                log.error(
                    $"Type mismatch in {name}. Requested to open a window " +
                    $"of {windowableObject.GetType()}," +
                    $"but {newWindow.GetType()} only accepts " +
                    $"{newWindow.GenericType} objects",
                    this);
                return;
            }

            newWindow.Initialize(windowableObject);
            OnWindowOpened(newWindow);
        }

        public void RequestCloseWindow(Window toClose)
        {
            Assert.IsNotNull(openWindows);
            Assert.IsTrue(openWindows.Count > 0);
            Assert.IsNotNull(toClose);

            if (toClose != null && openWindows.Contains(toClose))
            {
                openWindows.Remove(toClose);
                toClose.Close();
            }
            else
            {
                log.error($"{toClose} doesn't exist in {name}", this);
            }
        }

        public void ShowWindow(Window toShow)
        {
            if (toShow != null && hiddenWindows.Contains(toShow))
            {
                toShow.Show();
                hiddenWindows.Remove(toShow);
            }
        }

        public void HideWindow(Window toHide)
        {
            if (toHide != null && openWindows.Contains(toHide))
            {
                toHide.Hide();
                hiddenWindows.Add(toHide);
            }
        }

        protected virtual void OnWindowOpened(Window window)
        {
            WindowOpened?.Invoke(window);
            Debug.Log(window.ToString(), window);
        }

        protected virtual void OnWindowShown(Window window)
        {
            WindowShown?.Invoke(window);
        }

        protected virtual void OnWindowHidden(Window window)
        {
            WindowHidden?.Invoke(window);
        }

    }
}
