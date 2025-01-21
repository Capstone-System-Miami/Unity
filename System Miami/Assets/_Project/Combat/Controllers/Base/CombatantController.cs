// Authors: Layla Hoey, Lee St Louis
using System;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.Utilities;
using UnityEngine;
using SystemMiami.CombatRefactor;

namespace SystemMiami.CombatSystem
{
    // An abstract class that PlayerController and
    // EnemyController controller can derive from.
    public abstract class CombatantController : MonoBehaviour
    {
        #region EVENTS
        // ======================================

        // ======================================
        #endregion // EVENTS

        public float movementSpeed;     
        [HideInInspector] public Combatant combatant;

        protected CombatStateMachine stateMachine;

        #region UNITY METHODS
        // ======================================

        private void Awake()
        {
            combatant = GetComponent<Combatant>();
            stateMachine = GetComponent<CombatStateMachine>();
        }

        private void Start()
        {
            if (!TryGetComponent(out combatant))
            {
                Debug.LogWarning($"Didnt find a Combatant component on {name}.");
            }
        }

        private void Update()
        {
            ResetFlags();
        }

        // ======================================
        #endregion // UNITY METHODS


        #region TRIGGERS
        // ======================================

        // Turn Control Triggers
        public abstract bool EndTurnTriggered();
        public abstract bool NextPhaseTriggered();

        // Movement Triggers
        public abstract bool BeginMovementTriggered();

        // Ability Triggers
        public abstract bool UnequipTriggered();
        public abstract bool EquipTriggered();
        public abstract bool LockTargetsTriggered();
        public abstract bool UseAbilityTriggered();

        public abstract void ResetFlags();

        // ======================================
        #endregion // TRIGGERS


        #region FOCUSED TILE
        // ======================================
        public abstract OverlayTile GetFocusedTile();

        // ======================================
        #endregion // FOCUSED TILE
    }
}