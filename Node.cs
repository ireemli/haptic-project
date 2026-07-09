public class Node
{
    public bool walkable;
    public UnityEngine.Vector2 worldPos;
    public int gridX, gridY;
    public float g, h;
    public Node parent;

    public float f => g + h;

    public Node(bool walkable, UnityEngine.Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable  = walkable;
        this.worldPos  = worldPos;
        this.gridX     = gridX;
        this.gridY     = gridY;
    }
}