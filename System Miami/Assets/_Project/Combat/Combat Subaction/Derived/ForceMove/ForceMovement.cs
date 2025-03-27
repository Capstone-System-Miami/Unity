// Authors: Layla Hoey

using SystemMiami.CombatRefactor;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum MoveType { PUSH, PULL };
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Force Movement Subaction", menuName = "Combat Subaction/Force Movement")]
    public class ForceMovement : CombatSubactionSO
    {
        [SerializeField] private int distance;
        
        [SerializeField] private MoveType type;

        public override ISubactionCommand GenerateCommand(ITargetable target, CombatAction action)
        {
            Vector2Int userBoardPos = action.User.CurrentDirectionContext.TilePositionA;

            return new ForceMoveCommand(target, userBoardPos, type, distance);
        }
    }

    public class ForceMoveCommand : ISubactionCommand
    {
        public readonly ITargetable receiver;
        public readonly Vector2Int origin;
        public readonly MoveType type;
        public readonly int distance;

        public ForceMoveCommand(
            ITargetable receiver,
            Vector2Int origin,
            MoveType type,
            int distance)
        {
            this.receiver = receiver;
            this.origin = origin;
            this.type = type;
            this.distance = distance;
        }

        public void Preview()
        {
            receiver.GetMoveInterface()?.PreviewForceMove(origin, distance, type);
        }

        public void Execute()
        {
            receiver.GetMoveInterface()?.ReceiveForceMove(origin, distance, type);
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
        void PreviewForceMove(Vector2Int origin, int distance, MoveType type);
        void ReceiveForceMove(Vector2Int origin, int distance, MoveType type);
    }
}
