using System.Collections.Generic;
using System;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class CombatStateMachine : MonoBehaviour
    {
        #region PUBLIC VARS
        public CombatantController Controller;

        // Tiles
        public Action<OverlayTile> FocusedTileChanged;
        public Action<DirectionalInfo> PathTileChanged;

        [HideInInspector] public TurnStartState turnStartState;
        [HideInInspector] public TurnEndState turnEndState;
        [HideInInspector] public MovementTargetingState movementTargetingState;
        [HideInInspector] public MovementConfirmationState movementConfirmationState;
        [HideInInspector] public MovementActiveState movementActiveState;
        [HideInInspector] public ActionUnequippedState actionUnequippedState;
        [HideInInspector] public ActionEquippedState actionEquippedState;
        [HideInInspector] public ActionConfirmationState actionConfirmationState;
        [HideInInspector] public ActionExecutingState actionExecutingState;

        [HideInInspector] public Combatant combatant;

        // Pathing
        public PathFinder PathFinder = new PathFinder();
        public List<OverlayTile> CurrentPath = new List<OverlayTile>();
        public int CurrentPathCost;

        // Abilities
        public AbilityType TypeToEquip;
        public int IndexToEquip;

        // Tiles
        public OverlayTile FocusedTile { get; set; }
        public OverlayTile DestinationTile { get; set; } // movement state?


        #endregion // PUBLIC VARS

        #region PROTECTED VARS

        protected CombatState currentState;

        #endregion PROTECTED VARS

        #region PROPERTIES
        // ======================================

        public Phase CurrentPhase
        {
            get
            {
                return currentState.Phase;
            }
        }

        // Abilities
        //public bool CanAct
        //{
        //    get
        //    {
        //        if (combatant == null)
        //        { return false; }

        //        if (CurrentPhase != Phase.Action)
        //        { return false; }

        //        if (combatant.Abilities.CurrentState == Abilities.State.COMPLETE)
        //        {
        //            HasActed = true;
        //            return false;
        //        }

        //        if (combatant.Abilities.CurrentState == Abilities.State.EXECUTING)
        //        { return false; }

        //        if (IsActing)
        //        {
        //            //Debug.Log($"{name} is already acting");
        //            return false;
        //        }

        //        if (HasActed)
        //        {
        //            //Debug.Log($"{name} has already acted");
        //            return false;
        //        }


        //        return true;
        //    }
        //}
        // ======================================
        #endregion // PROPERTIES


        #region UNITY METHODS
        // ======================================

        private void Awake()
        {
            TurnStartState turnStartState = new(this);
            TurnEndState turnEndState = new(this);
            MovementTargetingState movementTargetingState = new(this);
            MovementConfirmationState movementConfirmationState = new(this);
            MovementActiveState movementActiveState = new(this);
            ActionUnequippedState actionUnequippedState = new(this);
            ActionEquippedState actionEquippedState = new(this);
            ActionConfirmationState actionConfirmationState = new(this);
            ActionExecutingState actionExecutingState = new(this);
        }

        private void Start()
        {
            SwitchState(turnStartState);
        }

        private void Update()
        {
            // state manage
            currentState.Update();
        }

        public void LateUpdate()
        {
            currentState.LateUpdate();
        }

        // ======================================
        #endregion // UNITY METHODS


        #region STATE MANAGEMENT
        // ======================================
        public void SwitchState(CombatState newState)
        {
            /// Call OnExit on
            /// the current state object
            /// before setting a new one
            currentState.OnExit();

            /// Set the current state to the new state.
            /// passed into this function as an arg.
            currentState = newState;

            /// Call OnEnter on the new state object
            currentState.OnEnter();
        }
        // ======================================
        #endregion // STATE MANAGEMENT
    }
}
