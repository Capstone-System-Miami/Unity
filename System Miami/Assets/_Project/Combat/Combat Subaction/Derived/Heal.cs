// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "Abilities/CombatActions/Heal")]
    public class Heal : CombatSubaction
    {
        [SerializeField] private float _amount;

        public override void Perform()
        {
            List<IHealReciever> targets = currentTargets.GetTargetsWith<IHealReciever>();
            foreach (IHealReciever target in targets)
            {
                if (target == null) { continue; }

                target.ReceiveHealAmount(_amount);
            }
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="Heal"/> to be performed
    /// on an object.
    /// </summary>
    public interface IHealReciever
    {
        bool IsCurrentlyHealable();
        void RecieveFullHeal();
        void ReceiveHealAmount(float amount);
        void RecieveHealPercent(float percent, bool ofMax);
    }
}
