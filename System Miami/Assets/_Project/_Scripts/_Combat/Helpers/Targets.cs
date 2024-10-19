using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public struct Targets
    {
        private Vector2Int[] _positions;
        private OverlayTile[] _tiles;
        private Combatant[] _combatants;

        public Vector2Int[] Positions { get { return _positions; } }
        public OverlayTile[] Tiles { get { return _tiles; } }
        public Combatant[] Combatants {  get { return _combatants; } }

        public Targets(Vector2Int[] positions, OverlayTile[] tiles, Combatant[] combatants)
        {
            _positions = positions;
            _tiles = tiles;
            _combatants = combatants;
        }
    }
}
