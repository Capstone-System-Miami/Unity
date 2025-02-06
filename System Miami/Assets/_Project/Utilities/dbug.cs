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

        public void warn(string msg)
        {
            if (!showMessages) { return; }
            Debug.LogWarning(msg);
        }

        public void error(string msg)
        {
            if (!showMessages) {return; }
            Debug.LogError(msg);
        }
    }
}
