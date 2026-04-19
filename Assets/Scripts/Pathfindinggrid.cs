using UnityEngine;

// Holds the grid of pathfinding nodes for the whole level.
// Scans for walls once at Start and builds a 2D array of Nodes.
// Place one of these in each level scene.
public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid")]
    public Vector2 gridWorldSize = new Vector2(40, 30);  // how big the level is in world units
    public float nodeSize = 1f;                          // size of each grid square
    public LayerMask wallLayer;                          // what counts as unwalkable

    [Header("Debug")]
    public bool drawGizmos = true;

    Node[,] grid;
    int gridWidth;
    int gridHeight;

    // singleton-ish accessor so enemies can find this easily
    public static PathfindingGrid Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        BuildGrid();
    }

    void BuildGrid()
    {
        gridWidth = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
        gridHeight = Mathf.RoundToInt(gridWorldSize.y / nodeSize);
        grid = new Node[gridWidth, gridHeight];

        // bottom-left corner of the grid in world space
        Vector2 origin = (Vector2)transform.position - gridWorldSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = origin + new Vector2(x * nodeSize + nodeSize * 0.5f,
                                                        y * nodeSize + nodeSize * 0.5f);
                // slightly smaller check radius so diagonal wall corners don't overly block
                bool walkable = !Physics2D.OverlapCircle(worldPos, nodeSize * 0.4f, wallLayer);
                grid[x, y] = new Node(x, y, worldPos, walkable);
            }
        }
    }

    // get the grid node at a given world position
    public Node NodeFromWorldPos(Vector2 worldPos)
    {
        Vector2 origin = (Vector2)transform.position - gridWorldSize * 0.5f;
        Vector2 local = worldPos - origin;

        int x = Mathf.Clamp(Mathf.FloorToInt(local.x / nodeSize), 0, gridWidth - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(local.y / nodeSize), 0, gridHeight - 1);
        return grid[x, y];
    }

    // return walkable neighbours of a node (up to 8)
    public System.Collections.Generic.List<Node> GetNeighbours(Node node)
    {
        var list = new System.Collections.Generic.List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;   // skip self

                int nx = node.gridX + dx;
                int ny = node.gridY + dy;

                if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight) continue;
                list.Add(grid[nx, ny]);
            }
        }

        return list;
    }

    // visualise the grid in the Scene view - green = walkable, red = blocked
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
                Node n = grid[x, y];
                Gizmos.color = n.walkable ? new Color(0f, 1f, 0f, 0.2f) : new Color(1f, 0f, 0f, 0.4f);
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeSize * 0.9f));
            }
        }
    }
}