using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class PlayerController : MonoBehaviour
    {
        private Combatant combatant;
        // Start is called before the first frame update
        void Start()
        {
          combatant = GetComponent<Combatant>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
