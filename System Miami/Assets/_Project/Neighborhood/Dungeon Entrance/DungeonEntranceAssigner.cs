using System.Collections.Generic;
using SystemMiami.Dungeons;
using UnityEngine;

namespace SystemMiami
{
    public class DungeonEntranceAssigner : MonoBehaviour
    {
        private DungeonEntrance[] entrances;
        private int currentIndex = 0;
        public bool canAssign => entrances != null && entrances.Length < currentIndex;

        private void Awake()
        {
            entrances = GetComponentsInChildren<DungeonEntrance>();
        }

        public HashSet<DungeonEntrance> GetEntrances()
        {
            return new(entrances);
        }

        public bool TryAssignNext(DungeonPreset preset)
        {
            if (canAssign)
            {
                entrances[currentIndex].StoreNewPreset(preset);
                return true;
            }

            return false;
        }

        public DungeonPreset ReplaceRandomEntranceWithPreset(DungeonPreset replacement)
        {
            Debug.Log($"REPLACING ENTRANCE", this);
            int randEntrance = Random.Range(0, entrances.Length);
            DungeonPreset replacee = entrances[randEntrance].CurrentPreset;

            entrances[randEntrance].StoreNewPreset(replacement);
            return replacee;
        }
    }
}
