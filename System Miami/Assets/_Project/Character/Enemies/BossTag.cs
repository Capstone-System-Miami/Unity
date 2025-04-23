using UnityEngine;

namespace SystemMiami
{
    public class BossTag : MonoBehaviour
    {
        [field: SerializeField] public string bossName { get; private set; }
        [field: SerializeField] public string powerName { get; private set; }
    }
}
