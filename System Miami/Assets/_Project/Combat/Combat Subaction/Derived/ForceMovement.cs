// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "Abilities/CombatActions/Push")]
    public class ForceMovement : CombatSubaction
    {
        [SerializeField] private int _distance;
        
        // TODO MapForwardA, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void Perform()
        {
            /// TODO
        }
    }


    /// <summary>
    /// This is the interface needed for
    /// <see cref="ForceMovement"/> to be performed
    /// on an object.
    /// </summary>
    public interface IForceMoveReciever
    {
        bool IsCurrentlyMovable();
        Vector2Int GetTilePos();
        bool TryMoveTo(Vector2Int tilePos);
        bool TryMoveInDirection(Vector2Int direction, int distance);
    }
}
