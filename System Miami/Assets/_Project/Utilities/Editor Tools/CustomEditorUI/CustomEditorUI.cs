using UnityEngine;

namespace SystemMiami.CustomEditor
{
    public class EditorButton
    {
        public string Label { get; private set; }
        public bool IsEnabled { get; set; }

        public EditorButton(string label) : this(label, true) { }

        public EditorButton(string label, bool isEnabled)
        {
            Label = label;
            IsEnabled = isEnabled;
        }

        public bool Pressed()
        {
            GUI.enabled = IsEnabled;
            bool pressed = GUILayout.Button(Label);
            GUI.enabled = true;
            return pressed;
        }
    }

}
