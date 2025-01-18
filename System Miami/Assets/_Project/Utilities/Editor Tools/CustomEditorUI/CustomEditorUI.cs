using UnityEngine;

namespace SystemMiami.CustomEditor
{
    public class EditorButton
    {
        public string Label { get; private set; }
        public bool IsEnabled { get; set; }
        public GUILayoutOption[] Options { get; private set; }

        public EditorButton(string label)
            : this(label, true) { }

        public EditorButton(string label, params GUILayoutOption[] options)
            : this(label, true, options) { }

        public EditorButton(string label, bool isEnabled)
            : this(label, isEnabled, null) { }

        public EditorButton(string label, bool isEnabled, params GUILayoutOption[] options)
        {
            Label = label;
            IsEnabled = isEnabled;
            Options = options;
        }

        public bool Pressed()
        {
            GUI.enabled = IsEnabled;
            bool pressed = GUILayout.Button(Label, Options);
            GUI.enabled = true;
            return pressed;
        }
    }

}
