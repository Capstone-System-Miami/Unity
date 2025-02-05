// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "New Damage Action",
        menuName = "Abilities/CombatActions/Damage")]
    public class Damage : CombatSubaction
    {
        [SerializeField] private float damageToDeal;

        public override ISubactionCommand GenerateCommand(ITargetable target)
        {
            Debug.LogWarning($"Generating a command for {target} about to check damage interface");

            IDamageReciever damageReciever;
            if (!target.TryGetDamageInterface(out damageReciever))
            {
                Debug.LogWarning($"Generating a command for {target} no damage interface returned");
                return null;
            }

            Debug.LogWarning($"Generating a command for {target} YES damage interface returned");
            return new DamageCommand(damageReciever, damageToDeal);
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="Damage"/> to be performed
    /// on an object.
    /// </summary>
    public interface IDamageReciever
    {
        bool IsCurrentlyDamageable();
        void PreviewDamage(float amount);
        void ReceiveDamage(float amount);
    }

    public class DamageCommand : ISubactionCommand
    {
        public readonly IDamageReciever receiver;
        public readonly float amount;

        public DamageCommand(IDamageReciever receiver, float amount)
        {
            this.receiver = receiver;
            this.amount = amount;
        }

        public void Preview()
        {
            receiver.PreviewDamage(amount);
        }

        public void Execute()
        {
            receiver.ReceiveDamage(amount);
        }
    }


}
