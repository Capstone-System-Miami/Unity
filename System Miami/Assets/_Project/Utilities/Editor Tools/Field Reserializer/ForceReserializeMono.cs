using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public class ForceReserializeMono : MonoBehaviour
    {
        public MonoBehaviour targetMono;

        private const int MAX_RECURSION_DEPTH = 10;
        private static int recursiveDepth = 0;

        /// <summary>
        /// Proper implementation:
        /// <para>
        /// 1. Attach a <see cref="ForceReserializeMono"/>
        /// component to the GameObject </para>
        /// 3. Attach IRemoveUnderscores to the code of the mono.
        /// you will be able to remove this later, along with all of the
        /// underscored fields.</para>
        /// <para>
        /// 2. Drag in the component you want to swap fields with</para>
        /// <para>
        /// 4. Ensure that the new field has the EXACT SAME NAME (CASE SENSITIVE)
        /// as the old field, just without the underscore.</para>
        /// <para>
        /// 5. Find every SerializedField that begins with an underscore
        /// <para>
        /// 6. Create a new SerializedField with the exact same datatype.</para>
        /// <para>
        /// 7. Right click the ForceReserialize component</para>
        /// <para>
        /// 8. > DANGER ZONE > (READ DOCUMENTATION) Swap Underscore Fields</para>
        /// 
        /// Good job for actually reading this before you tried it :)
        /// </summary>
        [ContextMenu("Danger Zone/(READ DOCUMENTATION) Swap Underscore Fields")]
        private void SwapUnderscoredFields()
        {
            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] allPrivateFields = targetMono.GetType().GetFields(bf);

            FieldInfo[] underscored = allPrivateFields.Where(f => f.Name.StartsWith("_")).ToArray();
            FieldInfo[] nonUnderscored = new FieldInfo[underscored.Length];

            string[] underscoredNames = underscored.Select(f => f.Name).ToArray();
            string[] nonUnderscoredNames = underscoredNames.Select(name => name[1..]).ToArray();

            if (underscoredNames.Length == 0 || nonUnderscoredNames.Length == 0
                || underscoredNames.Length != nonUnderscoredNames.Length)
            {
                Debug.LogError(
                    "The number of fields with underscores does not " +
                    "match the number of fields without underscores.");
                return;
            }

            for (int i = 0; i < nonUnderscoredNames.Length; i++)
            {
                FieldInfo[] innerList = allPrivateFields.Where(f => f.Name == nonUnderscoredNames[i]).ToArray();

                if (innerList.Length != 1)
                {
                    Debug.LogError(
                        $"Fatal. The search for a matching non-underscore" +
                        $"name at index {i} didn't turn up asingle result." +
                        $"was {(innerList.Length > 1 ? "More than" : "Less than")}");
                    return;
                }

                nonUnderscored[i] = innerList[0];
            }



            for (int i = 0; i < underscored.Length; i++)
            {
                int widerIndexUnderscored = allPrivateFields.ToList().IndexOf(underscored[i]);
                int widerIndexNonUnderscored = allPrivateFields.ToList().IndexOf(nonUnderscored[i]);

                if (widerIndexUnderscored == -1 || widerIndexNonUnderscored == -1)
                {
                    Debug.LogError(
                        "Fatal. FieldInfo not found in wider array.");
                    return;
                }

                if (underscored[i].Name != underscoredNames[i])
                {
                    Debug.LogError(
                        "Fatal. underscoredField.Name doesn't match the string at" +
                        "its own list of names.");
                    return;
                }
                if (allPrivateFields[widerIndexNonUnderscored].Name != nonUnderscoredNames[i])
                {
                    Debug.LogError(
                        "Fatal. underscoredField.Name doesn't match the string at" +
                        "its own list of names.");
                    return;
                }
                if ($"{allPrivateFields[widerIndexUnderscored].Name[1..]}"
                    != allPrivateFields[widerIndexNonUnderscored].Name)
                {
                    Debug.LogError(
                        "Fatal. Array mismatch within the wider fields arr.");
                    return;
                }

                object currentField = allPrivateFields[widerIndexUnderscored].GetValue(targetMono);
                if (currentField is IRemoveUnderScores)
                {
                    SwapUnderscoredFields(currentField as IRemoveUnderScores);
                }

                allPrivateFields[widerIndexNonUnderscored]
                    .SetValue(targetMono, allPrivateFields[widerIndexUnderscored]
                        .GetValue(targetMono));
            }

            Debug.Log(
                $"All private SerializedFields with underscores " +
                $"in {targetMono.name}, and any applicable IReplaceUnderscore\n" +
                $"fields to a depth of {recursiveDepth} now have " +
                "matching fields without underscores that hold identical refs.");
        }

        private void SwapUnderscoredFields(IRemoveUnderScores target)
        {
            if (++recursiveDepth > MAX_RECURSION_DEPTH)
            {
                Debug.LogError($"Max recursion depth reached @{recursiveDepth}.");
                return;
            }

            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] allPrivateFields = target.Type.GetFields(bf);

            FieldInfo[] underscored = allPrivateFields.Where(f => f.Name.StartsWith("_")).ToArray();
            FieldInfo[] nonUnderscored = new FieldInfo[underscored.Length];

            string[] underscoredNames = underscored.Select(f => f.Name).ToArray();
            string[] nonUnderscoredNames = underscoredNames.Select(name => name[1..]).ToArray();

            if (underscoredNames.Length == 0 || nonUnderscoredNames.Length == 0
                || underscoredNames.Length != nonUnderscoredNames.Length)
            {
                Debug.LogError(
                    "The number of fields with underscores does not " +
                    "match the number of fields without underscores.");
                return;
            }

            for (int i = 0; i < nonUnderscoredNames.Length; i++)
            {
                FieldInfo[] innerList = allPrivateFields.Where(
                    f => f.Name == nonUnderscoredNames[i]).ToArray();

                if (innerList.Length != 1)
                {
                    Debug.LogError(
                        $"Fatal. The search for a matching non-underscore" +
                        $"name at index {i} didn't turn up asingle result." +
                        $"was {(innerList.Length > 1 ? "More than" : "Less than")}");
                    return;
                }

                nonUnderscored[i] = innerList[0];
            }

            for (int i = 0; i < underscored.Length; i++)
            {
                int widerIndexUnderscored = allPrivateFields.ToList().IndexOf(underscored[i]);
                int widerIndexNonUnderscored = allPrivateFields.ToList().IndexOf(nonUnderscored[i]);

                if (widerIndexUnderscored == -1 || widerIndexNonUnderscored == -1)
                {
                    Debug.LogError(
                        "Fatal. FieldInfo not found in wider array.");
                    return;
                }

                if (underscored[i].Name != underscoredNames[i])
                {
                    Debug.LogError(
                        "Fatal. underscoredField.Name doesn't match the string at" +
                        "its own list of names.");
                    return;
                }
                if (allPrivateFields[widerIndexNonUnderscored].Name != nonUnderscoredNames[i])
                {
                    Debug.LogError(
                        "Fatal. underscoredField.Name doesn't match the string at" +
                        "its own list of names.");
                    return;
                }
                if ($"{allPrivateFields[widerIndexUnderscored].Name[1..]}"
                    != allPrivateFields[widerIndexNonUnderscored].Name)
                {
                    Debug.LogError(
                        "Fatal. Array mismatch within the wider fields arr.");
                    return;
                }

                object currentField = allPrivateFields[widerIndexUnderscored].GetValue(target);
                if (currentField is IRemoveUnderScores)
                {
                    SwapUnderscoredFields(currentField as IRemoveUnderScores);
                }

                allPrivateFields[widerIndexNonUnderscored]
                    .SetValue(target, allPrivateFields[widerIndexUnderscored]
                        .GetValue(target));
            }
        }
    }
    public interface IRemoveUnderScores
    {
        Type Type { get; }
    }
}
