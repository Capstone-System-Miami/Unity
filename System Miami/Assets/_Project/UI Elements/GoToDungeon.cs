using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToDungeon : MonoBehaviour
    {
        private DungeonData storedData;

        public void Go(bool useStoredData)
        {
            if (useStoredData)
            {
                GAME.MGR.GoToDungeon(storedData);
            }
            else
            {
                GAME.MGR.GoToDungeon();
            }
        }

        public void Go(DungeonData dungeonData, bool alsoStoreData)
        {
            storedData = alsoStoreData ? dungeonData : storedData;
            GAME.MGR.GoToDungeon(dungeonData);
        }

        public void StoreData(DungeonData data)
        {
            storedData = data;
        }
    }
}
