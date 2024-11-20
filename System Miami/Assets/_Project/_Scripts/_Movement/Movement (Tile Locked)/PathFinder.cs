// Author: Alec, layla minor edits
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class PathFinder
    {
        //finds shortest path between two overlay tiles
        public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
        {
            //tiles to explore
            List<OverlayTile> openList = new List<OverlayTile>();
            //tiles that have been explored
            List<OverlayTile> closedList = new List<OverlayTile>();

            //adds starting tile to list
            openList.Add(start);

            //int i = 0;
            while(openList.Count > 0)
            {
                //Debug.Log($"in loop. iteration {i++}");

                //tile with lowest F score
                OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

                //move current tile to closed list
                openList.Remove(currentOverlayTile);
                closedList.Add(currentOverlayTile);

                //if we are at our destination reconstruct path and return
                if(currentOverlayTile == end)
                {
                    //finalize out path.
                    return GetFinishedList(start, end);
                }

                //get all neighbor tiles to current tile
                var neighbourTiles = GetNeighbourTiles(currentOverlayTile);

                //loop through eac
                foreach (var neighbour in neighbourTiles)
                {                    
                    // skip any tiles already explored
                    if (closedList.Contains(neighbour)) { continue; }

                    // skip tiles that have different z-axis
                    if (Mathf.Abs(currentOverlayTile.GridLocation.z - neighbour.GridLocation.z) > 1) { continue; }

                    // skip blocked, unless it's the end tile
                    if(!neighbour.Valid && neighbour != end) { continue; }


                    //calculate g and h
                    neighbour.G = GetManhattenDistance(start, neighbour);
                    neighbour.H = GetManhattenDistance(end, neighbour);

                    //set the current tile as the parent of the neighbor (for path reconstrcution)
                    neighbour.PreviousTile = currentOverlayTile;

                    //add neighbor to open list
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            //if there is no path return empty list
            //Debug.Log ("returning an empty list");
            return new List<OverlayTile>();
        }

        //reconstructs path by backtracking from end to start
        private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            List<OverlayTile> finishedList = new List<OverlayTile>();

            OverlayTile currentTile = end;

            while(currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.PreviousTile;
            }

            finishedList.Reverse();

            return finishedList;
         }

        private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
        {
            return Mathf.Abs(start.GridLocation.x - neighbour.GridLocation.x) + Mathf.Abs(start.GridLocation.y - neighbour.GridLocation.y);
        }

        public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
        {
            var map = MapManager.MGR.map;

            List<OverlayTile> neighbours = new List<OverlayTile>();

            //top
            Vector2Int locationToCheck = new Vector2Int(
                currentOverlayTile.GridLocation.x,
                currentOverlayTile.GridLocation.y + 1);
            if(map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Bottom
             locationToCheck = new Vector2Int(
                currentOverlayTile.GridLocation.x,
                currentOverlayTile.GridLocation.y - 1);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Right
             locationToCheck = new Vector2Int(
                currentOverlayTile.GridLocation.x + 1,
                currentOverlayTile.GridLocation.y);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Left
             locationToCheck = new Vector2Int(
                currentOverlayTile.GridLocation.x - 1,
                currentOverlayTile.GridLocation.y);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            return neighbours;
        }

    }
}
