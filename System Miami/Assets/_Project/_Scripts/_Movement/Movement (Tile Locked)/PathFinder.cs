// Author: Alec
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

            while(openList.Count > 0)
            {
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
                    //skip any tiles already explored or tiles that have different z-axis, (can be edited to include tiles with blocked tag)
                    if(neighbour.isBlocked || closedList.Contains(neighbour)|| Mathf.Abs(currentOverlayTile.gridLocation.z - neighbour.gridLocation.z) > 1)
                    {
                        continue;
                    }

                    //calculate g and h
                    neighbour.G = GetManhattenDistance(start, neighbour);
                    neighbour.H = GetManhattenDistance(end, neighbour);

                    //set the current tile as the parent of the neighbor (for path reconstrcution)
                    neighbour.previous = currentOverlayTile;

                    //add neighbor to open list
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            //if there is no path return empty list
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
                currentTile = currentTile.previous;
            }
            finishedList.Reverse();

            DrawArrows.Instance.DrawPath(finishedList);

            return finishedList;
         }

        private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
        {
            return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
        }

        public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
        {
            var map = MapManager.MGR.map;

            List<OverlayTile> neighbours = new List<OverlayTile>();

            //top
            Vector2Int locationToCheck = new Vector2Int(
                currentOverlayTile.gridLocation.x,
                currentOverlayTile.gridLocation.y + 1);
            if(map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Bottom
             locationToCheck = new Vector2Int(
                currentOverlayTile.gridLocation.x,
                currentOverlayTile.gridLocation.y - 1);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Right
             locationToCheck = new Vector2Int(
                currentOverlayTile.gridLocation.x + 1,
                currentOverlayTile.gridLocation.y);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //Left
             locationToCheck = new Vector2Int(
                currentOverlayTile.gridLocation.x - 1,
                currentOverlayTile.gridLocation.y);
            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            return neighbours;
        }

    }
}
