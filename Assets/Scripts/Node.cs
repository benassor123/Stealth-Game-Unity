using UnityEngine;
public class Node
{
    public int gridX;
    public int gridY;
    public Vector2 worldPos;
    public bool walkable;


    public int gCost;
    public int hCost;
    public Node parent;

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(int x, int y, Vector2 worldPos, bool walkable)
    {
        this.gridX = x;
        this.gridY = y;
        this.worldPos = worldPos;
        this.walkable = walkable;
    }
}