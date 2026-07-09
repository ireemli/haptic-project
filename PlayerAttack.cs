using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.2f;

    private float lastAttackTime;

    private PlayerInventory inventory;
    private bool hasWarnedNoSword;

    private Vector2 AttackOrigin =>
        attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (inventory == null)
            return;

        if (!inventory.hasSword)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !hasWarnedNoSword)
            {
                hasWarnedNoSword = true;
                Debug.Log("PlayerInventory.hasSword = false.");
            }
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Attack();
    }

    private void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            AttackOrigin,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>() ?? hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Debug.Log("Attack!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackOrigin, attackRange);
    }
}
