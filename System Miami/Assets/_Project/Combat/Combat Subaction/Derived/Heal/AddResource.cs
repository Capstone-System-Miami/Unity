// Authors: Layla Hoey

using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Heal Subaction", menuName = "Combat Subaction/AddOrSubtractResource")]
    public class AddResource : CombatSubactionSO
    {
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private float amount;
        [SerializeField] private bool perTurn;
        [SerializeField] private int durationTurns;

        public override ISubactionCommand GenerateCommand(ITargetable target, CombatAction action)
        {
            return new RestoreResourceCommand(target,resourceType, amount,perTurn,durationTurns);
        }
    }

    public class RestoreResourceCommand : ISubactionCommand
    {
        public readonly ITargetable reciever;
        public readonly float amount;
        public readonly ResourceType type;
        public readonly bool perTurn;
        public readonly int durationTurns;

        public RestoreResourceCommand(ITargetable healReciever,ResourceType type, float amount, bool perTurn,int durationTurns)
        {
            this.reciever = healReciever;
            this.amount = amount;
            this.type = type;
            this.perTurn = perTurn;
            this.durationTurns = durationTurns;
        }

        public void Preview()
        {
            reciever.GetHealInterface()?.PreviewResourceReceived(amount, type,perTurn, durationTurns);
        }

        public void Execute()
        {
            reciever.GetHealInterface()?.ReceiveResource(amount,type,perTurn, durationTurns);
        }
    }

    /// <summary>
    /// This is the interface needed for
    /// <see cref="AddResource"/> to be performed
    /// on an object.
    /// </summary>
    public interface IResourceReceiver
    {
        bool IsCurrentlyHealable();
        void PreviewResourceReceived(float amount,ResourceType type, bool perTurn,int durationTurns);
        void ReceiveResource(float amount,ResourceType type, bool perTurn,int duraationTurns);
    }
}

public enum ResourceType
{
    Health,
    Stamina,
    Mana,
}