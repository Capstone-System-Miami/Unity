using System.Linq;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatState
    {
        public readonly Phase Phase;

        protected readonly CombatStateMachine context;

        protected CombatState(CombatStateMachine context, Phase phase)
        {
            this.context = context;
            Phase = phase;
        }

        /// <summary>
        /// To be called once,
        /// as soon as the state becomes active
        /// </summary>
        public abstract void OnEnter();


        /// <summary>
        /// Called once, inside TransitionTo(newState),
        /// just before the state becomes inactive
        /// </summary>
        public abstract void OnExit();

        /// <summary>
        /// To be called every frame while the state is active
        /// </summary>
        public abstract void Update();

        public virtual void LateUpdate() { }

        protected void ResetTileData()
        {
            context.FocusedTile = null;
            context.DestinationTile = null;
        }

        protected void UpdateFocusedTile()
        {
            if (context.FocusedTile != null) { return; } // check this before calling

            if (context.FocusedTile == context.Controller.GetFocusedTile()) { return; }

            context.FocusedTile?.EndHover(context.Controller);

            context.FocusedTile = context.Controller.GetFocusedTile();

            context.FocusedTile?.BeginHover(context.Controller);

            context.FocusedTileChanged?.Invoke(context.FocusedTile);

        }
    }
}