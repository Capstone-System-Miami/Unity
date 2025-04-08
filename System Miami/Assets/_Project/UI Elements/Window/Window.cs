using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.ui
{
    [RequireComponent(typeof(RectTransform) )]
    public abstract class Window : UIComponent
    {
        public abstract Type GenericType { get; }

        public abstract void Initialize(IWindowable windowableObject);
        public abstract void Open();
        public abstract void Close();

        #region UIComponent Impl.
        //==================================
        public override abstract void Show();
        public override abstract void Hide();
        #endregion // UIComponent Impl.
    }

    public abstract class Window<T> : Window where T : IWindowable
    {
        public override Type GenericType { get { return typeof(T) ; } }

        WindowData windowData;

        public bool Initialized { get; private set; }

        public virtual bool Initialize(T windowableObject)
        {
            windowData = windowableObject.WindowData;
            transform.localPosition = windowData.worldSpaceOpenPos;
            Initialized = true;
            return true;
        }

        public override void Open()
        {
            Initialized = true;
        }
        public override void Close()
        {
            Destroy(gameObject);
        }
    }
}
