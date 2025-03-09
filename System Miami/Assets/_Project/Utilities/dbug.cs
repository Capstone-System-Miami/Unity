using UnityEngine;

namespace SystemMiami.Utilities
{
    [System.Serializable]
    public class dbug
    {
        [SerializeField] private bool showMessages = true;

        public void print(string msg)
        {
            if (!showMessages) { return; }
            Debug.Log(msg);
        }

        public void print(string msg, Object context)
        {
            if (!showMessages) { return; }
            Debug.Log(msg, context);
        }

        public void warn(string msg)
        {
            if (!showMessages) { return; }
            Debug.LogWarning(msg);
        }

        public void warn(string msg, Object context)
        {
            if (!showMessages) { return; }
            Debug.LogWarning(msg, context);
        }

        public void error(string msg)
        {
            if (!showMessages) {return; }
            Debug.LogError(msg);
        }

        public void error(string msg, Object context)
        {
            if (!showMessages) { return; }
            Debug.LogError(msg, context);
        }

        public void on()
        {
            showMessages = true;
        }

        public void off()
        {
            showMessages = false;
        }
    }
}
