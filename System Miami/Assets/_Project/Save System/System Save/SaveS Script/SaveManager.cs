using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SystemMiami;

namespace SystemMiami
{
    public class SaveManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public void Save()
        {
            FileStream file = new FileStream(Application.persistentDataPath + "/Player.dat", FileMode.OpenOrCreate);
        
        }

        // Update is called once per frame
        public void Load()
        {
        
        }
    }
}
