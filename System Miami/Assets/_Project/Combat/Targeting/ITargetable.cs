using System;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void SubscribeTo(ref EventHandler<TargetingEventArgs> combatActionEvent);
        void UnsubscribeTo(ref EventHandler<TargetingEventArgs> combatActionEvent);
        void HandleTargetingEvent(object sender, TargetingEventArgs args);

        Vector2Int BoardPos { get; }
        List<ISubactionCommand> TargetedBy { get; set; }
        string nameMessageForDB { get; set; }
        void PreviewOn();
        void PreviewOff();
        void ApplyCombatAction();


        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IDamageReceiver"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IDamageReceiver GetDamageInterface();

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IHealReceiver"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IHealReceiver GetHealInterface();

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IForceMoveReceiver"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IForceMoveReceiver GetMoveInterface();

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IStatusEffectReceiver"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IStatusEffectReceiver GetStatusEffectInterface();
    }
}
