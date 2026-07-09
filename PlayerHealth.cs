using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private TextMeshPro healthText;
    [Header("Hit Effect")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private int flashPulses = 2;
    [SerializeField] private float flashSegmentDuration = 0.06f;
    [SerializeField] private GameObject deathPanel;

    private int currentHealth;
    private Color originalColor;
    private Coroutine hitEffectRoutine;

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
        if (!gameObject.activeInHierarchy) return;
        currentHealth -= damage;
        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendVibrationHit();

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

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
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashSegmentDuration);
        spriteRenderer.color = originalColor;
    }

    private void UpdateHealthUI()
    {
        if (healthText == null)
            return;

        string hearts = "";

        for (int i = 0; i < currentHealth; i++)
            hearts += "<color=#FF3B3B>♥</color> ";

        for (int i = currentHealth; i < maxHealth; i++)
            hearts += "<color=#555555>♥</color> ";

        healthText.text = hearts;
    }

    private void Die()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}