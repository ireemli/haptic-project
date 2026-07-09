using UnityEngine;
using TMPro;
using System.Collections;

public class IntroMessage : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Settings")]
    [SerializeField] private string message = "Find a way to escape through the door.";
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (messagePanel != null)
            StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        if (messageText != null)
            messageText.text = message;

        messagePanel.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        CanvasGroup cg = messagePanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = messagePanel.AddComponent<CanvasGroup>();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        messagePanel.SetActive(false);
        cg.alpha = 1f;
    }
}