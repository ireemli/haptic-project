using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackDelay = 0.35f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyFollow enemyFollow;

    private float nextAttackTime;
    private bool isAttacking;

    private void Awake()
    {
        if (enemyFollow == null)
            enemyFollow = GetComponent<EnemyFollow>();
    }

    private void Update()
    {
        if (enemyFollow != null && !enemyFollow.IsActive())
            return;

        if (player == null)
            return;

        if (isAttacking)
            return;

        if (Time.time < nextAttackTime)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (enemyFollow != null)
            enemyFollow.SetCanMove(false);

        yield return new WaitForSeconds(attackDelay);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        nextAttackTime = Time.time + attackCooldown;

        if (enemyFollow != null)
            enemyFollow.SetCanMove(true);

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}