using System;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void SubscribeTo(EventHandler<CombatActionEventArgs> combatActionEvent);
        void UnsubscribeTo(EventHandler<CombatActionEventArgs> combatActionEvent);
        void HandleCombatActionEvent(object sender, CombatActionEventArgs args);

        List<ISubactionCommand> TargetedBy { get; set; }
        string nameMessageForDB { get; set; }
        void DisplayPreview();
        void ApplyCombatAction();


        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IDamageReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        bool TryGetDamageInterface(
            out IDamageReciever damageInterface);

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IHealReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        bool TryGetHealInterface(
            out IHealReciever healInterface);

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IForceMoveReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        bool TryGetMoveInterface(
            out IForceMoveReciever forceMovementInterface);

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IStatusEffectReceiver"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        bool TryGetStatusEffectInterface(
            out IStatusEffectReceiver statusEffectInterface);
    }
}
