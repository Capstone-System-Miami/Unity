using System.Reflection;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public static class DebugHelpers
    {
        public static void Break(string s)
        {
            string msg = "======== BREAKPOINT =======\n";

            msg += s;

            msg += "\n=== UNPAUSE TO CONTINUE ===";

            Debug.LogError(msg);
            Debug.Break();
        }

        public static string GetInfo(object obj, BindingFlags binding)
        {
            FieldInfo[] fields = obj.GetType().GetFields(binding);

            string result = "";
            foreach (FieldInfo field in fields)
            {
                result += $"| {field.Name}: {field.GetValue(obj)}\n";
            }

            return result;
        }
    }
}
