using System.Collections.Generic;
using UnityEngine;

// A* Algorithm, using pathfindings grid of the level for enemy movement.

public static class Pathfinder
{

    public static List<Vector2> FindPath(Vector2 startWorld, Vector2 endWorld) // two world cords, where now,  where to go
    {
        if (PathfindingGrid.Instance == null) return new List<Vector2>(); // ensure theres a grid if not return empty list for now. we need the grid for this to work

        Node startNode = PathfindingGrid.Instance.NodeFromWorldPos(startWorld); // returns the cell/node the enemy is in 
        Node endNode = PathfindingGrid.Instance.NodeFromWorldPos(endWorld); // returns the cell the enemy wants to go to 


        // now lets determine if start or end is a wall
        if (!startNode.walkable || !endNode.walkable) return new List<Vector2>(); // grids store nodes, in each node is walkable attribute explaining if theres a wall at current


        List<Node> openSet = new List<Node>();  //nodes to examine next
        List<Node> closedSet = new List<Node>(); // nodes that have been exammined
        openSet.Add(startNode); // lets add start node

        startNode.gCost = 0; // to reach start node from start  wil be 0, set to zero .

        startNode.hCost = Distance(startNode, endNode); // estimated cost from start to end - a heuristic, using distance() function
        startNode.parent = null; // set to null for now, startnode is the start node , theres no parent 

        while (openSet.Count > 0) //keep going whilst not empty
        {
            Node current = openSet[0]; // 

            for (int i = 1; i < openSet.Count; i++)
            {
                Node n = openSet[i];
                bool betterF = n.FCost < current.FCost;
                bool betterH = n.FCost == current.FCost && n.hCost < current.hCost;
                if (betterF || betterH) //if theres a better f or h, update current to n iteration
                {
                    current = n;
                }
            }
            openSet.Remove(current);
            closedSet.Add(current);

            if (current == endNode)
            {
                return Retrace(startNode, endNode);
            }

            // loop through neighbours
            foreach (Node neighbour in PathfindingGrid.Instance.GetNeighbours(current))
            {
                if (!neighbour.walkable) continue;
                if (closedSet.Contains(neighbour)) continue;

                int newG = current.gCost + Distance(current, neighbour);

                if (newG < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newG;
                    neighbour.hCost = Distance(neighbour, endNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return new List<Vector2>();
    }

    static List<Vector2> Retrace(Node start, Node end)  //trace path backwards
    {
        List<Vector2> path = new List<Vector2>();
        Node current = end;

        while (current != start)
        {
            path.Add(current.worldPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }


    static int Distance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return (int)(Mathf.Sqrt(dx * dx + dy * dy) * 10);
    }
}