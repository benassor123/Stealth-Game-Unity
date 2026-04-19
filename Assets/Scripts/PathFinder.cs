using System.Collections.Generic;
using UnityEngine;

// A* pathfinding. Call FindPath(start, end) to get a list of waypoints.
// Returns an empty list if no path exists.
public static class Pathfinder
{
    public static List<Vector2> FindPath(Vector2 startWorld, Vector2 endWorld)
    {
        if (PathfindingGrid.Instance == null) return new List<Vector2>();

        Node startNode = PathfindingGrid.Instance.NodeFromWorldPos(startWorld);
        Node endNode = PathfindingGrid.Instance.NodeFromWorldPos(endWorld);

        if (!startNode.walkable || !endNode.walkable) return new List<Vector2>();

        // open list = nodes we plan to look at, closed list = nodes we've already processed
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // reset A* costs (the Node fields are reused between searches)
        startNode.gCost = 0;
        startNode.hCost = Distance(startNode, endNode);
        startNode.parent = null;

        while (openSet.Count > 0)
        {
            // pick the node with the lowest F cost (most promising)
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < current.FCost ||
                    (openSet[i].FCost == current.FCost && openSet[i].hCost < current.hCost))
                    current = openSet[i];
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // reached the goal - reconstruct path and return it
            if (current == endNode)
                return Retrace(startNode, endNode);

            // examine each walkable neighbour
            foreach (Node neighbour in PathfindingGrid.Instance.GetNeighbours(current))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int tentativeG = current.gCost + Distance(current, neighbour);

                // only update neighbour if we found a cheaper way to reach it
                if (tentativeG < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = tentativeG;
                    neighbour.hCost = Distance(neighbour, endNode);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // no path found
        return new List<Vector2>();
    }

    // walk backwards from end to start via parent links, then reverse
    static List<Vector2> Retrace(Node start, Node end)
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

    // diagonal distance heuristic - 14 for diagonal, 10 for straight
    // (multiplied by 10 to stay in integers, which is faster than floats)
    static int Distance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        if (dx > dy)
            return 14 * dy + 10 * (dx - dy);
        return 14 * dx + 10 * (dy - dx);
    }
}