using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami.Utilities
{
    [System.Serializable]
    public class dbug
    {
        [SerializeField] private bool showMessages = true;

        public dbug() : this (false) { }

        public dbug(bool show)
        {
            showMessages = show;
        }

        public void print(string msg)
        {
            if (!showMessages) { return; }
            Debug.Log(msg);
        }

        public void print<T>(string msg, IList<T> collection)
        {
            if (!showMessages) { return; }
            string formatted = format(collection);
            msg += $"\n{formatted}";
            Debug.Log(msg);
        }

        public void print<T>(string msg, IList<T> collection, Object context)
        {
            if (!showMessages) { return; }
            Debug.Log(msg, context);
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

        public void warn<T>(string msg, IList<T> collection)
        {
            string formatted = format(collection);
            if (!showMessages) { return; }
            Debug.LogWarning(msg);
        }

        public void warn<T>(string msg, IList<T> collection, Object context)
        {
            string formatted = format(collection);
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

        public void error<T>(string msg, IList<T> collection)
        {
            string formatted = format(collection);
            if (!showMessages) { return; }
            Debug.LogError(msg);
        }

        public void error<T>(string msg, IList<T> collection, Object context)
        {
            string formatted = format(collection);
            if (!showMessages) { return; }
            Debug.LogError(msg, context);
        }

        public static string format<T>(IList<T> collection)
        {
            string result = "";

            for (int i = 0; i < collection.Count; i++)
            {
                result += $"item{i}:  ";
                result += ( $"{collection[i]}" ?? "null" );
                result += "\n";
            }

            return result;
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
