using System.Collections;
using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class ReturnLoadoutItemsToInventory : MonoBehaviour
    {
      public void Go()
      {
        PlayerManager.MGR?.inventory.ReturnLoadoutItemsToInventory();
      }
    }
}
