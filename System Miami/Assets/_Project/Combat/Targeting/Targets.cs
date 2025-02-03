using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class Targets
    {
        public readonly List<OverlayTile> tiles = new();
        public readonly List<ITargetable> all = new();

        public static readonly Targets empty = new Targets(new());

        public Targets(List<OverlayTile> tiles)
        {
            this.tiles = tiles;

            foreach (OverlayTile tile in tiles)
            {
                if (tile is ITargetable)
                {
                    all.Add(tile);
                }

                if (!tile.Occupied) { continue; }

                if (tile.Occupier is ITargetable target)
                {
                    all.Add(target);
                }
            }
        }

        public static Targets operator +(Targets a, Targets b)
        {
            List<OverlayTile> newTiles = new(a.tiles);

            newTiles.AddRange(b.tiles);

            return new(newTiles);
        }
    }
}
