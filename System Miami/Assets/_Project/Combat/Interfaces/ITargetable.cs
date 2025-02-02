using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public interface ITargetable
    {
        void HandleBeginTargeting(Color preferredColor);
        void HandleEndTargeting(Color preferredColor);


        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IDamageReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IDamageReciever GetDamageInterface();

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IHealReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IHealReciever GetHealInterface();

        /// <summary>
        /// The method by which a targeting
        /// object can request the appropriate
        /// interface from the targeted object.
        /// </summary>
        /// <returns>
        /// The <see cref="IForceMoveReciever"/> (IF ANY)
        /// provided by the targeted object.
        /// </returns>
        IForceMoveReciever GetForceMoveInterface();
    }
}
