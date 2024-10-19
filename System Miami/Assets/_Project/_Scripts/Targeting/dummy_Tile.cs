using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class dummy_Tile : MonoBehaviour, ISelectable
    {
        [SerializeField] private Vector2Int _gridPosition;

        public Vector2Int Position { get { return _gridPosition; } }

        public void Select()
        {
            print ($"Tile at {_gridPosition} became selected");
        }

        public void Deselect()
        {
            print($"Tile at {_gridPosition} became deselected");
        }
    }
}
