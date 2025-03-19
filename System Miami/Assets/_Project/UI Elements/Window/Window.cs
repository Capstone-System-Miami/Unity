using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.ui
{
    [RequireComponent(
        typeof(RectTransform) )]
    public abstract class Window : MonoBehaviour
    {
        public WindowManager Manager { get; private set; }
        public RectTransform RT { get; private set; }

        private List<RectTransform> panels;

        private void Awake()
        {
            RT = GetComponent<RectTransform>();
        }

        protected void HandleOpen(WindowManager manager)
        {
            Manager = manager;
        }

        protected void HandleHide()
        {

        }

        protected void HandleClose()
        {

        }
    }
}
