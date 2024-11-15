using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public struct Targets
    {
        private List<Vector2Int> _positions;
        private List<OverlayTile> _tiles;
        private List<Combatant> _combatants;

        public List<Vector2Int> Positions { get { return _positions; } }
        public List<OverlayTile> Tiles { get { return _tiles; } }
        public List<Combatant> Combatants {  get { return _combatants; } }

        public static readonly Targets empty = new Targets(new List<Vector2Int>(), new List<OverlayTile>(), new List<Combatant>());

        public Targets(List<Vector2Int> positions, List<OverlayTile> tiles, List<Combatant> combatants)
        {
            _positions = positions;
            _tiles = tiles;
            _combatants = combatants;
        }

        public static Targets operator +(Targets a, Targets b)
        {
            a._positions.AddRange(b._positions);
            a._tiles.AddRange(b._tiles);
            a._combatants.AddRange(b._combatants);

            return a;
        }
    }
}
