using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float     moveSpeed        = 1f;
    [SerializeField] private float     stopRange        = 0.8f;
    [SerializeField] private float     pathUpdateInterval = 0.5f;
    [SerializeField] private float     waypointThreshold  = 0.15f;
    [SerializeField] private AStarGrid aStarGrid;
    [SerializeField] private Animator  animator;

    private Rigidbody2D    rb;
    private bool           isActive  = false;
    private bool           canMove   = true;

    private List<Vector2>  path;
    private int            pathIndex = 0;
    private float          pathTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!canMove || !isActive || player == null)
        {
            StopEnemy();
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= stopRange)
        {
            StopEnemy();
            return;
        }

        // Recalculate path at intervals
        pathTimer += Time.fixedDeltaTime;
        if (pathTimer >= pathUpdateInterval || path == null)
        {
            pathTimer = 0f;
            RecalculatePath();
        }

        FollowPath();
    }

    private void RecalculatePath()
    {
        if (aStarGrid == null) return;
        path      = aStarGrid.FindPath(transform.position, player.position);
        pathIndex = 0;
    }

    private void FollowPath()
    {
        if (path == null || path.Count == 0)
        {
            StopEnemy();
            return;
        }

        // Skip waypoints we've already passed
        while (pathIndex < path.Count &&
               Vector2.Distance(transform.position, path[pathIndex]) < waypointThreshold)
        {
            pathIndex++;
        }

        if (pathIndex >= path.Count)
        {
            StopEnemy();
            return;
        }

        Vector2 direction = ((Vector2)path[pathIndex] - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (animator != null)
            animator.SetBool("isMoving", true);
    }

    private void StopEnemy()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null)
            animator.SetBool("isMoving", false);
    }

    // ── Public API ─────────────────────────────────────────────────────────

    public void ActivateEnemy()
    {
        isActive = true;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove) StopEnemy();
    }

    public bool IsActive() => isActive;
}