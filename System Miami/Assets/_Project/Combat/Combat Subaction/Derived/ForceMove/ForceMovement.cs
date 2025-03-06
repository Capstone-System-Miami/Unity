// Authors: Layla Hoey

using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Force Movement Subaction", menuName = "Combat Subaction/Force Movement")]
    public class ForceMovement : CombatSubactionSO
    {
        [SerializeField] private int distance;
        
        // TODO MapForwardA, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int direction;

        public override ISubactionCommand GenerateCommand(ITargetable target,CombatAction action)
        {
            return new ForceMoveData(target, distance, direction);
        }
    }

    public class ForceMoveData : ISubactionCommand
    {
        public readonly ITargetable receiver;
        public readonly int distance;
        public readonly Vector2Int direction;

        public ForceMoveData(
            ITargetable receiver,
            int distance,
            Vector2Int direction)
        {
            this.receiver = receiver;
            this.distance = distance;
            this.direction = direction;
        }

        public void Preview()
        {
            receiver.GetMoveInterface()
                ?.PreviewForceMove(distance, direction);
        }

        public void Execute()
        {
            receiver.GetMoveInterface()
                ?.ReceiveForceMove(distance, direction);
        }
    }


    /// <summary>
    /// This is the interface needed for
    /// <see cref="ForceMovement"/> to be performed
    /// on an object.
    /// </summary>
    public interface IForceMoveReceiver
    {
        bool IsCurrentlyMovable();
        void PreviewForceMove(int distance, Vector2Int direction);
        void ReceiveForceMove(int distance, Vector2Int direction);
    }
}
