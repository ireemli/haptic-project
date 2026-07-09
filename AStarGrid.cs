using UnityEngine;
using System.Collections.Generic;

public class AStarGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float nodeSize   = 1f;
    [SerializeField] private int   gridWidth  = 30;
    [SerializeField] private int   gridHeight = 30;

    private Node[,] grid;
    private Vector2 gridOrigin;

    private void Start()
    {
        BuildGrid();
    }

    // ── Grid construction ──────────────────────────────────────────────────

    public void BuildGrid()
    {
        gridOrigin = (Vector2)transform.position
                   - new Vector2(gridWidth * nodeSize / 2f, gridHeight * nodeSize / 2f);

        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = gridOrigin + new Vector2(x * nodeSize, y * nodeSize);
                bool walkable    = Physics2D.OverlapCircle(worldPos, nodeSize * 0.4f, wallLayer) == null;
                grid[x, y]       = new Node(walkable, worldPos, x, y);
            }
        }
    }

    // ── World-to-grid conversion ───────────────────────────────────────────

    public Node GetNodeFromWorld(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / nodeSize);
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / nodeSize);
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return null;
        return grid[x, y];
    }

    // ── A* pathfinding ─────────────────────────────────────────────────────

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode  = GetNodeFromWorld(startPos);
        Node targetNode = GetNodeFromWorld(targetPos);

        // Reset g values from previous searches
        foreach (Node n in grid)
        {
            n.g      = float.MaxValue;
            n.parent = null;
        }
        startNode.g = 0;

        List<Node>     openList   = new List<Node> { startNode };
        HashSet<Node>  closedList = new HashSet<Node>();

        while (openList.Count > 0)
        {
            // Pick node with lowest f
            Node current = openList[0];
            for (int i = 1; i < openList.Count; i++)
                if (openList[i].f < current.f) current = openList[i];

            openList.Remove(current);
            closedList.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (!neighbour.walkable || closedList.Contains(neighbour))
                    continue;

                float newG = current.g + Vector2.Distance(current.worldPos, neighbour.worldPos);

                if (newG < neighbour.g)
                {
                    neighbour.g      = newG;
                    neighbour.h      = Vector2.Distance(neighbour.worldPos, targetPos);
                    neighbour.parent = current;

                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        return null; // No path found
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = node.gridX + dx;
                int ny = node.gridY + dy;

                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    if (dx != 0 && dy != 0)
                    {
                        bool sideAValid = node.gridX + dx >= 0 && node.gridX + dx < gridWidth &&
                                        node.gridY >= 0 && node.gridY < gridHeight;
                        bool sideBValid = node.gridX >= 0 && node.gridX < gridWidth &&
                                        node.gridY + dy >= 0 && node.gridY + dy < gridHeight;

                        if (!sideAValid || !sideBValid) continue;
                        if (!grid[node.gridX + dx, node.gridY].walkable ||
                            !grid[node.gridX, node.gridY + dy].walkable)
                            continue;
                    }
                    neighbours.Add(grid[nx, ny]);
                }
            }
        }

        return neighbours;
    }

    private List<Vector2> RetracePath(Node start, Node end)
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

    // ── Debug visualization ────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? new Color(1, 1, 1, 0.1f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube((Vector3)n.worldPos, Vector3.one * nodeSize * 0.9f);
        }
    }
}