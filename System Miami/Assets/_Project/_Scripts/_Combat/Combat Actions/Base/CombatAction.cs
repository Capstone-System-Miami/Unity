// Authors: Layla Hoey
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    public abstract class CombatAction : ScriptableObject
    {
        [SerializeField] private bool _affectsSelf;
        [SerializeField] private bool _affectsOpponents;

        public abstract void PerformOn(Combatant[] targets);
    }
}
