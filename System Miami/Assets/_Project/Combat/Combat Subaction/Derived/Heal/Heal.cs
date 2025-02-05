// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Action", menuName = "Abilities/CombatActions/Heal")]
    public class Heal : CombatSubaction
    {
        [SerializeField] private float healAmount;

        protected override ISubactionCommand GenerateCommand(ITargetable target)
        {
            IHealReciever healReciever;
            if (!target.TryGetHealInterface(out healReciever))
            {
                return null;
            }

            return new HealCommand(healReciever, healAmount);
        }
    }

    public class HealCommand : ISubactionCommand
    {
        public readonly IHealReciever reciever;
        public readonly float amount;
        
        public HealCommand(IHealReciever healReciever, float amount)
        {
            this.amount = amount;
        }

        public void Preview()
        {
            reciever.PreviewHeal(amount);
        }

        public void Execute()
        {
            reciever.ReceiveHeal(amount);
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
        void PreviewHeal(float amount);
        void ReceiveHeal(float amount);
    }
}
