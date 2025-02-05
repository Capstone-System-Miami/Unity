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

        public Targets()
        {
            this.tiles = new();
            this.all = new();
        }

        public Targets(List<OverlayTile> tiles)
        {
            this.tiles = tiles;

            foreach (OverlayTile tile in tiles)
            {
                if (tile is ITargetable)
                {
                    all.Add(tile);
                }

                if (tile.Occupier is ITargetable target)
                {
                    Debug.LogWarning($"OCCUPANT: {(tile.Occupier is null ? "nothin" : tile.Occupier.ToString())}");

                    int count = all.Count;
                    all.Add(target);
                    int count2 = all.Count;
                    if (count != count2) { Debug.LogWarning($"{tile.Occupier.ToString()} successfully added to targs");}
                }
            }
        }

        private Targets(HashSet<OverlayTile> tileHash, HashSet<ITargetable> allTargetsHash)
        {
            tiles = new(tileHash);
            all = new(allTargetsHash);
        }

        public void Clear()
        {
            Debug.LogWarning($"Clear called on targs");
            tiles.Clear();
            all.Clear();
        }


        // Operator Overloads
        public static Targets operator +(Targets a, Targets b)
        {
            HashSet<OverlayTile> tileHash = new(a.tiles);
            HashSet<ITargetable> allTargetsHash = new(a.all);

            /// Hash auto-skips dupes.
            b.tiles.ForEach(tile => tileHash.Add(tile));
            b.all.ForEach(target => allTargetsHash.Add(target));        

            return new(tileHash, allTargetsHash);
        }

        public static Targets operator -(Targets a, Targets b)
        {
            HashSet<OverlayTile> tileHash = new(a.tiles);
            HashSet<ITargetable> allTargetsHash = new(a.all);

            /// Hash auto-skips removals of non-existant objects.
            b.tiles.ForEach(tile => tileHash.Remove(tile));
            b.all.ForEach(target => allTargetsHash.Remove(target));        

            return new(tileHash, allTargetsHash);
        }
    }
}
