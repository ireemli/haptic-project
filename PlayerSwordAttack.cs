using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerSwordAttack : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.2f;

    private float lastAttackTime;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            AttackFromFlex();
        }
    }

    public void AttackFromFlex()
    {
        if (inventory == null || !inventory.hasSword) return;
        if (Time.time - lastAttackTime < attackCooldown) return; // cooldown 
        
        lastAttackTime = Time.time;

        Attack();
    }

    private void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position,
            attackRange,
            enemyLayer
        );
        HashSet<EnemyHealth> damaged = new HashSet<EnemyHealth>();
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>() ?? hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null && damaged.Add(enemy))
                enemy.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position,
            attackRange
        );
    }
}