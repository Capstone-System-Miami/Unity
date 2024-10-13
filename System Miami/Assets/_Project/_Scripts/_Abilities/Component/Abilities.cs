// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, partially tested, realll dirty

    // The actual component used for
    // storing / acessing / triggering abilities
    [RequireComponent(typeof(Targeting))]
    public class Abilities : MonoBehaviour
    {
        [SerializeField] private KeyCode[] _keys;
        [SerializeField] private Ability[] _abilities;
        [SerializeField] private bool isenabled;
        
        [SerializeField] private GameObject[] targs;

        private Targeting _targeting;


        void Awake()
        {
            _targeting = GetComponent<Targeting>();
        }

        private void Update()
        {
            if (isenabled)
            {
                // Targeting
                if (targs.Length == 0)
                {
                    if (Input.GetKeyDown(_keys[0]))
                    {
                        print($"{name} hi {_keys[0]}");
                        List<ITargetable> targets = _targeting.GetTargets(_abilities[0].Pattern);
                        GameObject[] targetObjects = new GameObject[targets.Count];

                        for (int i = 0; i < targets.Count; i++)
                        {
                            targetObjects[i] = targets[i].GameObject();
                        }

                        targs = targetObjects;
                    }

                    if (Input.GetKeyDown(_keys[1]))
                    {
                        print($"{name} hi {_keys[1]}");

                        List<ITargetable> targets = _targeting.GetTargets(_abilities[1].Pattern);
                        GameObject[] targetObjects = new GameObject[targets.Count];

                        for (int i = 0; i < targets.Count; i++)
                        {
                            targetObjects[i] = targets[i].GameObject();
                        }

                        targs = targetObjects;
                    }
                }

                // Executing
                else
                {
                    if (Input.GetKeyDown(_keys[0]))
                    {
                        print($"{name} hi {_keys[0]}");
                        _abilities[0].UseOn(targs);
                    }

                    if (Input.GetKeyDown(_keys[1]))
                    {
                        print($"{name} hi {_keys[1]}");
                        _abilities[1].UseOn(targs);
                    }
                }
            }
        }
    }
}
