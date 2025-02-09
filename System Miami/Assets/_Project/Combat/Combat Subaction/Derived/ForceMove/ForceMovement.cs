// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "Abilities/CombatActions/Push")]
    public class ForceMovement : CombatSubactionSO
    {
        [SerializeField] private int distance;
        
        // TODO MapForwardA, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int direction;

        protected override ISubactionCommand GenerateCommand(ITargetable t)
        {
            if (!t.TryGetMoveInterface(out var moveInterface))
            {
                return null;
            }

            return new ForceMoveData(moveInterface, distance, direction);
        }
    }

    public class ForceMoveData : ISubactionCommand
    {
        public readonly IForceMoveReceiver receiver;
        public readonly int distance;
        public readonly Vector2Int direction;

        public ForceMoveData(IForceMoveReceiver receiver, int distance, Vector2Int direction)
        {
            this.receiver = receiver;
            this.distance = distance;
            this.direction = direction;
        }

        public void Preview()
        {
            receiver.PreviewForceMove(distance, direction);
        }

        public void Execute()
        {
            receiver.ReceiveForceMove(distance, direction);
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
