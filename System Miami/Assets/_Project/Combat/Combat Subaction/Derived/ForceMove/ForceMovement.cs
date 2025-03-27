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
        public readonly ITargetable reciever;
        public readonly Vector2Int origin;
        public readonly MoveType type;
        public readonly int distance;

        public readonly OverlayTile destinationTile;

        public ForceMoveCommand(
            ITargetable receiver,
            Vector2Int origin,
            MoveType type,
            int distance)
        {
            this.reciever = receiver;
            this.origin = origin;
            this.type = type;
            this.distance = distance;

            Vector2Int dirVec = DirectionHelper.GetDirectionVec(origin, receiver.BoardPos);

            if (type == MoveType.PULL)
            {
                dirVec *= -1;
            }

            Vector2Int targetPos = reciever.BoardPos + (dirVec * distance);

            int adjustedX = System.Math.Clamp(targetPos.x, MapManager.MGR.Bounds.xMin, MapManager.MGR.Bounds.xMax);
            int adjustedY = System.Math.Clamp(targetPos.y, MapManager.MGR.Bounds.yMin, MapManager.MGR.Bounds.yMax);

            targetPos = new(adjustedX, adjustedY);

            if (MapManager.MGR.TryGetTile(targetPos, out OverlayTile targetTile))
            {
                destinationTile = targetTile;
            }
            else
            {
                Debug.LogError(
                    $"Force Movement command error. The command " +
                    $"could not find a target tile, even at the " +
                    $"adjusted position. This shouldn't be executed no matter " +
                    $"what the input is, so there is probably a " +
                    $"logical error somewhere.");
            }
        }

        public void Preview()
        {
            reciever.GetMoveInterface()?.PreviewForceMove(destinationTile);
        }

        public void Execute()
        {
            reciever.GetMoveInterface()?.RecieveForceMove(destinationTile);
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
        void PreviewForceMove(OverlayTile destinationTile);
        void RecieveForceMove(OverlayTile destinationTile);
    }
}
