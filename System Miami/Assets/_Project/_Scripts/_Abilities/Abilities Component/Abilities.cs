// Authors: Layla Hoey
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, partially tested

    // The actual component used for
    // storing / acessing / triggering abilities
    public class Abilities : MonoBehaviour
    {
        [SerializeField] private KeyCode[] _keys =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
        };

        [SerializeField] private Ability[] _abilities;
        [SerializeField] private Ability _selectedAbility;
        [SerializeField] private bool isenabled;
        [SerializeField] private bool _isTargeting = false;
        
        [SerializeField] private GameObject[] targs;

        private Combatant _combatant;

        private Vector2Int startFrameDirection;
        private Vector2Int endFrameDirection;


        void Awake()
        {
            _combatant = GetComponent<Combatant>();
        }

        private void OnEnable()
        {
            //foreach(Ability ability in _abilities)
            //{
            //    ability.Init(_combatant);
            //}
        }

        private void Update()
        {
            startFrameDirection = _combatant.DirectionInfo.Direction;

            // Just for testing.
            // Checks inputs and sets the selected ability
            // index to the key pressed
            for (int i = 0; i < _abilities.Length; i++)
            {
                if (_keys.Length <= i) { continue; }

                if (Input.GetKeyDown(_keys[i]))
                {
                    _selectedAbility = _abilities[i];
                    _selectedAbility?.Init(_combatant);
                }
            }

            if(_isTargeting)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    _selectedAbility?.UpdateTargets();
                }

                _selectedAbility?.ShowTargets();
            }
            else
            {
                _selectedAbility?.HideTargets();
            }
        }

        private void LateUpdate()
        {
            endFrameDirection = _combatant.DirectionInfo.Direction;

            if (startFrameDirection != endFrameDirection)
            {
                _selectedAbility?.UpdateTargets();
            }
        }
    }
}
