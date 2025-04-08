using System;
using UnityEngine;

namespace SystemMiami.CustomEditor
{
    public class InterfaceReferenceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; private set; }

        public InterfaceReferenceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
