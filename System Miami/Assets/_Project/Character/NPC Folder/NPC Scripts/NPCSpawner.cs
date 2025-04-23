using UnityEngine;

namespace SystemMiami
{
    public class NPCSpawner : MonoBehaviour
    {
        public GameObject spawnerNPCPrefab { get; private set; }
        public GameObject npcPrefabInstance;

        public GameObject SpawnNPC(GameObject npcPrefab)
        {
            this.spawnerNPCPrefab = npcPrefab;
            npcPrefabInstance = Instantiate(npcPrefab, transform.position, Quaternion.identity);
            npcPrefabInstance.transform.SetParent(transform);

            // Debug.Log($"Spawned NPC: {npcPrefabInstance}", npcPrefabInstance);

            return npcPrefabInstance;
        }
    }
}
