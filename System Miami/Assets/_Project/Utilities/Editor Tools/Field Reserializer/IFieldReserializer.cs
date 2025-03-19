using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CustomEditor
{
    public interface IFieldReserializer
    {
        Dictionary<string, string> OldFieldName_NewFieldName();
    }
}
