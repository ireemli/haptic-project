using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PickupVase : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private GameObject promptUI;

    private TMPro.TextMeshProUGUI promptText;
    private Transform holdPoint;
    private bool isHeld = false;
    private bool playerInRange = false;
    private Transform player;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    [Header("Break")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject breakEffectPrefab;
    [SerializeField] private bool isCorrectVase = false;

    private bool isBroken = false;

    private void Awake()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
            promptText = promptUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGloveMode) return;

        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isHeld)
                PickUp();
            else
                Drop();
        }

        if (isHeld)
        {
            transform.position = holdPoint.position;

            if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
                BreakInFront();
        }
    }

    // Glove modunda pozisyon güncellemesi için ayrı FixedUpdate
    private void FixedUpdate()
    {
        if (isHeld && holdPoint != null)
            transform.position = holdPoint.position;
    }

    public bool TryEFromFlex()
    {
        if (!playerInRange && !isHeld)
            return false;

        if (!isHeld)
            PickUp();
        else
            Drop();

        return true;
    }

    public bool TryQFromFlex()
    {
        if (!isHeld)
            return false;

        BreakInFront();
        return true;
    }

    private void PickUp()
    {
        isHeld = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (isCorrectVase && FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendVibrationPulse();

        UpdatePrompt();
    }

    private void Drop()
    {
        isHeld = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        playerInRange = false;

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void BreakInFront()
    {
        if (player == null) return;

        Vector2 direction = Vector2.down;

        if (playerMovement != null)
            direction = GetCardinalDirection(playerMovement.FacingDirection);

        Vector3 breakPosition = player.position + (Vector3)(direction * 0.8f);
        transform.position = breakPosition;

        BreakVase();
    }

    private void BreakVase()
    {
        if (isBroken) return;
        isBroken = true;
        isHeld = false;
        playerInRange = false;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendVibrationBurst();

        if (breakEffectPrefab != null)
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);

        if (isCorrectVase && keyPrefab != null)
            Instantiate(keyPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
        else if (!isCorrectVase)
        {
            if (WrongBreakCounter.Instance != null)
                WrongBreakCounter.Instance.AddWrongBreak();
        }

        Destroy(gameObject);
    }

    private Vector2 GetCardinalDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Vector2.right : Vector2.left;
        else
            return dir.y > 0 ? Vector2.up : Vector2.down;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        player = other.transform;
        holdPoint = player.Find("HoldPoint");
        playerMovement = other.GetComponent<PlayerMovement>();

        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (!isHeld)
        {
            player = null;
            holdPoint = null;
            playerMovement = null;
        }

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (promptUI == null) return;

        if (!playerInRange && !isHeld)
        {
            promptUI.SetActive(false);
            return;
        }

        promptUI.SetActive(true);

        if (promptText == null)
            promptText = promptUI.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);

        if (!isHeld)
            promptText.text = PromptManager.Instance != null
                ? PromptManager.Instance.GetInteractPrompt()
                : "Press E to pick up";
        else
            promptText.text = (PromptManager.Instance != null
                ? PromptManager.Instance.GetDropPrompt() + "\n" + PromptManager.Instance.GetThrowPrompt()
                : "Press E to drop\nPress Q to break");
    }
}