using UnityEngine;

// Represents a single square in the pathfinding grid.
// A* builds paths as chains of Nodes.
public class Node
{
    public int gridX;       // column in the grid
    public int gridY;       // row in the grid
    public Vector2 worldPos; // center of the node in world coordinates
    public bool walkable;

    // A* scores - recalculated for each path search
    public int gCost;    // real distance travelled from start to this node
    public int hCost;    // estimated distance from this node to target
    public Node parent;  // which node we came from, used to reconstruct the final path

    public int FCost { get { return gCost + hCost; } }

    public Node(int x, int y, Vector2 worldPos, bool walkable)
    {
        this.gridX = x;
        this.gridY = y;
        this.worldPos = worldPos;
        this.walkable = walkable;
    }
}