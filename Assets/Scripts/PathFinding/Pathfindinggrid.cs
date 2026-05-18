using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid")]
    public Vector2 gridWorldSize = new Vector2(40, 30);
    public float nodeSize = 1f;
    public LayerMask wallLayer; //layer mask lets us filter physics checks by layer - this is to check only for walls as blocked path.

    [Header("Debug")]
    public bool drawGizmos = true; // green for floor, red for walls

    Node[,] grid; //2d arr
    int gridWidth; //width dimensions
    int gridHeight; //height dimensions

    public static PathfindingGrid Instance;

    void Awake() // awake is more suited than start to ensure grid  created before game begins
    {
        Instance = this;
        BuildGrid();
    }
    void BuildGrid() //build grid for what is walkable, what is not.
    {
        gridWidth = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
        gridHeight = Mathf.RoundToInt(gridWorldSize.y / nodeSize);
        grid = new Node[gridWidth, gridHeight];

        Vector2 origin = (Vector2)transform.position - gridWorldSize * 0.5f;
        float halfNode = nodeSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // cell centre in world space
                float cellX = origin.x + x * nodeSize + halfNode;
                float cellY = origin.y + y * nodeSize + halfNode;
                Vector2 worldPos = new Vector2(cellX, cellY);

                bool walkable = !Physics2D.OverlapCircle(worldPos, nodeSize * 0.4f, wallLayer);
                grid[x, y] = new Node(x, y, worldPos, walkable);
            }
        }
    }
    public Node NodeFromWorldPos(Vector2 worldPos)  //convert pos in Unity to find grid cell
    {
        Vector2 origin = (Vector2)transform.position - gridWorldSize * 0.5f; // find where grids origin [0,0] is in unity space
        Vector2 local = worldPos - origin; // how far worldPos(passed in) is from grids origin


        //convert to grid space
        int x = Mathf.FloorToInt(local.x / nodeSize);
        int y = Mathf.FloorToInt(local.y / nodeSize);

        // clamp so positions outside the grid return the nearest edge cell
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        return grid[x, y]; // now return 
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;

                int checkY = node.gridY + y;

                if (checkX < 0 || checkX >= gridWidth)
                    continue;

                if (checkY < 0 || checkY >= gridHeight)
                    continue;

                neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

        if (grid == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Node node = grid[x, y];

                if (node.walkable)
                    Gizmos.color = new Color(0f, 1f, 0f, 0.2f); // green
                else
                    Gizmos.color = new Color(1f, 0f, 0f, 0.4f); // red

                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeSize * 0.9f));
            }
        }
    }
}