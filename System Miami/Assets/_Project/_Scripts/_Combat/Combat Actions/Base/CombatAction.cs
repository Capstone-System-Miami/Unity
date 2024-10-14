// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    public abstract class CombatAction : ScriptableObject
    {
        [SerializeField] private bool _affectsSelf;
        [SerializeField] private bool _affectsOpponents;

        protected AbilityType _type;

        public abstract void PerformOn(GameObject target);
        public abstract void PerformOn(GameObject me, GameObject target);
    }
}
