using System.Collections.Generic;
using System;
using UnityEngine;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(Canvas))]
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private int maxWindows;

        private List<Window> openWindows = new();

        public Canvas Canvas { get; private set; }

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
        }

        public Window OpenWindow(Window prefab)
        {
            if (openWindows.Count >= maxWindows) { return null; }

            Window newWindow = Instantiate(prefab, transform);
            // positioning

            return newWindow;
        }

        public void CloseWindow(Window openWindow)
        {
            if (openWindow != null && openWindows.Contains(openWindow))
            {
                openWindows.Remove(openWindow);
                // TODO destroy window
            }
        }
    }
}
