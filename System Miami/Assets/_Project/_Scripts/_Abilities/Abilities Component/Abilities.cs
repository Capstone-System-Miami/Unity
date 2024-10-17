// Authors: Layla Hoey
using SystemMiami.CombatSystem;
using System.Collections;
using SystemMiami.Utilities;
using UnityEngine;
using System.Net;
using Unity.VisualScripting;

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
        [SerializeField] private bool _isTargeting = false;
        
        [SerializeField] private GameObject[] targs;

        private Combatant _combatant;

        private Vector2Int startFrameDirection;
        private Vector2Int endFrameDirection;

        private bool _isUpdating;

        void Awake()
        {
            _combatant = GetComponent<Combatant>();
        }

        private void OnEnable()
        {
            foreach (Ability ability in _abilities)
            {
                ability.Init(_combatant);
            }
        }

        private void Update()
        {
            startFrameDirection = _combatant.DirectionInfo.DirectionVec;

            if(_isUpdating) { return; }


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

            if(_selectedAbility != null)
            {
                if (!_isTargeting)
                {
                    if (_selectedAbility.IsPreviewing)
                    {
                        StartCoroutine(_selectedAbility.HideAllTargets());
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        _isTargeting = true;
                    }
                }
                else
                {
                    if (!_selectedAbility.IsPreviewing)
                    {
                        StartCoroutine(_selectedAbility.ShowAllTargets());
                    }


                    if (Input.GetMouseButtonDown(1))
                    {
                        _isTargeting = false;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        StartCoroutine(_selectedAbility.Use());
                        _isTargeting = false;
                    }
                }
            }

        }

        private void LateUpdate()
        {
            endFrameDirection = _combatant.DirectionInfo.DirectionVec;
            if(endFrameDirection != startFrameDirection)
            {
                if (_selectedAbility != null)
                {
                    StartCoroutine(onDirectionChange());
                }
            }
        }

        private IEnumerator onDirectionChange()
        {
            _isUpdating = true;
            yield return null;

            //StartCoroutine(_selectedAbility.HideAllTargets());
            //yield return new WaitUntil(() => !_selectedAbility.IsPreviewing);
            //yield return null;

            StartCoroutine(_selectedAbility.ShowAllTargets());
            yield return new WaitUntil(() => _selectedAbility.IsPreviewing);
            yield return null;
            
            _isUpdating = false;
        }
    }
}
