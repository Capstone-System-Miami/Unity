// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Subaction", menuName = "Combat Subaction/Heal")]
    public class Heal : CombatSubactionSO
    {
        [SerializeField] private float healAmount;

        public override ISubactionCommand GenerateCommand(ITargetable target)
        {
            return new HealCommand(target, healAmount);
        }
    }

    public class HealCommand : ISubactionCommand
    {
        public readonly ITargetable reciever;
        public readonly float amount;
        
        public HealCommand(ITargetable healReciever, float amount)
        {
            this.amount = amount;
        }

        public void Preview()
        {
            reciever.GetHealInterface()?.PreviewHeal(amount);
        }

        public void Execute()
        {
            reciever.GetHealInterface()?.ReceiveHeal(amount);
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="Heal"/> to be performed
    /// on an object.
    /// </summary>
    public interface IHealReceiver
    {
        bool IsCurrentlyHealable();
        void PreviewHeal(float amount);
        void ReceiveHeal(float amount);
    }
}
