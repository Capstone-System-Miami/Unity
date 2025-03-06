using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class TargetSet
    {
        public readonly List<ITargetable> all = new();
        public readonly List<ITargetable> tiles = new();
        public readonly List<ITargetable> occupants = new();

        public TargetSet()
        {
            this.tiles = new();
            this.occupants = new();
            this.all = new();
        }

        public TargetSet(List<OverlayTile> tiles)
        {
            foreach (OverlayTile tile in tiles)
            {
                if (tile is ITargetable)
                {
                    this.tiles.Add(tile);
                    this.all.Add(tile);
                }

                if (tile.Occupier is ITargetable target)
                {
                    Debug.LogWarning($"OCCUPANT: {(tile.Occupier is null ? "nothin" : tile.Occupier.ToString())}");

                    //int count = all.Count;
                    this.occupants.Add(target);
                    this.all.Add(target);
                    //int count2 = all.Count;
                    //if (count != count2) { Debug.LogWarning($"{tile.Occupier.ToString()} successfully added to targs");}
                }
            }
        }

        private TargetSet(HashSet<ITargetable> all, HashSet<ITargetable> tiles, HashSet<ITargetable> occupants)
        {
            this.all = new(all);
            this.tiles = new(tiles);
            this.occupants = new(occupants);
        }

        public void Clear()
        {
            Debug.LogWarning($"Clear called on targs");
            all.Clear();
            tiles.Clear();
            occupants.Clear();
        }

        // Operator Overloads
        public static TargetSet operator +(TargetSet a, TargetSet b)
        {
            /// Copy Lists to hashes
            HashSet<ITargetable> allTargetsHash = new(a.all);
            HashSet<ITargetable> tileHash = new(a.tiles);
            HashSet<ITargetable> occupantsHash = new(a.tiles);

            /// Hash auto-skips dupes.
            b.all.ForEach(target => allTargetsHash.Add(target));
            b.tiles.ForEach(tile => tileHash.Add(tile));
            b.occupants.ForEach(occupant => occupantsHash.Add(occupant));

            return new(allTargetsHash, tileHash, occupantsHash);
        }

        public static TargetSet operator -(TargetSet a, TargetSet b)
        {
            /// Copy Lists to hashes
            HashSet<ITargetable> allTargetsHash = new(a.all);
            HashSet<ITargetable> tileHash = new(a.tiles);
            HashSet<ITargetable> occupantsHash = new(a.tiles);

            /// Hash auto-skips removals of non-existant objects.
            b.all.ForEach(target => allTargetsHash.Remove(target));
            b.tiles.ForEach(tile => tileHash.Remove(tile));
            b.occupants.ForEach(occupant => occupantsHash.Remove(occupant));

            return new(allTargetsHash, tileHash, occupantsHash);
        }
    }
}
