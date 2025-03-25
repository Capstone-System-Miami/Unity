using UnityEngine;
using UnityEditor;

namespace SystemMiami.CustomEditor
{
    [CustomPropertyDrawer(typeof(InterfaceReferenceAttribute))]
    public class InterfaceReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InterfaceReferenceAttribute interfaceAttribute = (InterfaceReferenceAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            // Draw the object field
            Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);

            // Check if the assigned object implements the required interface
            if (obj != null && interfaceAttribute.InterfaceType.IsAssignableFrom(obj.GetType()))
            {
                property.objectReferenceValue = obj;
            }
            else if (obj == null)
            {
                property.objectReferenceValue = null;
            }
            else
            {
                Debug.LogWarning($"Assigned object does not implement {interfaceAttribute.InterfaceType.Name}");
            }

            EditorGUI.EndProperty();
        }
    }
}
