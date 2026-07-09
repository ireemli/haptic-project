using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private TextMeshPro healthText;
    [Header("Hit Effect")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private int flashPulses = 2;
    [SerializeField] private float flashSegmentDuration = 0.06f;

    private Color originalColor;
    private Coroutine hitEffectRoutine;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendVibrationBurst();

        if (spriteRenderer != null)
        {
            if (hitEffectRoutine != null)
                StopCoroutine(hitEffectRoutine);
            hitEffectRoutine = StartCoroutine(HitEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffect()
    {
        for (int i = 0; i < flashPulses; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashSegmentDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashSegmentDuration);
        }

        hitEffectRoutine = null;
    }

    private void UpdateHealthUI()
    {
        if (healthText == null)
        {
            return;
        }

        string hearts = "";

        for (int i = 0; i < currentHealth; i++)
            hearts += "<color=#FF3B3B>♥</color> ";

        for (int i = currentHealth; i < maxHealth; i++)
            hearts += "<color=#555555>♥</color> ";

        healthText.text = hearts;
    }

    private void Die()
    {
        GameObject msg = GameObject.FindWithTag("FeedbackMessage");
        if (msg != null) msg.SetActive(false);

        Destroy(gameObject);
    }
}
