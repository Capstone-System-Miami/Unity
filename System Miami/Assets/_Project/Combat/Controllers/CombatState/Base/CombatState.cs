using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatState
    {
        protected readonly CombatantController controller;

        protected CombatState(CombatantController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// To be called once,
        /// as soon as the state becomes active
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// To be called every frame while the state is active
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// To be called once,
        /// just before the state becomes inactive
        /// </summary>
        public abstract void OnExit();
    }
}
