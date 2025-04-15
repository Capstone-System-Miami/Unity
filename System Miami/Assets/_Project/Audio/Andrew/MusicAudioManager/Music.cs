using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class Music : Sound
    {
        [field: SerializeField] public int BuildIndex { get; private set; }
    }
}
