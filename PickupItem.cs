using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    private enum PickupType
    {
        Torch,
        Sword,
        Key
    }

    [Header("Pickup")]
    [SerializeField] private PickupType pickupType = PickupType.Torch;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private bool autoAddKinematicRigidbody = true;
    [SerializeField] private string promptMessage = "Press E to pick up";

    [Header("Combat")]
    [SerializeField] private EnemyFollow enemyToActivate;

    [Header("Feedback Message")]
    [SerializeField] private GameObject messageUI;
    [SerializeField] private string feedbackMessage = "";
    [SerializeField] private float feedbackDuration = 3f;

    private bool playerInRange;
    private PlayerInventory inventory;
    private TMPro.TextMeshProUGUI promptText;

    private void Awake()
    {
        if (autoAddKinematicRigidbody)
            EnsureRigidbody2D();

        if (promptUI != null)
        {
            promptUI.SetActive(false);
            promptText = promptUI.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.Disable();
    }

    private void EnsureRigidbody2D()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) return;
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
            inventory = other.GetComponentInParent<PlayerInventory>();
        playerInRange = inventory != null;
        UpdatePromptUI();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        inventory = null;
        UpdatePromptUI();
    }

    private void Update()
    {
        if (!playerInRange || inventory == null) return;

        bool gloveMode = GameManager.Instance != null && GameManager.Instance.IsGloveMode;

        bool interactPressed =
            (!gloveMode && interactAction != null && interactAction.action != null && interactAction.action.WasPressedThisFrame()) ||
            (!gloveMode && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (interactPressed)
            Pickup();
    }

    public bool TryPickupFromFlex()
    {
        if (!playerInRange || inventory == null)
            return false;
        Pickup();
        return true;
    }

    private void Pickup()
    {
        switch (pickupType)
        {
            case PickupType.Torch:
                inventory.AddTorch();
                if (FeedbackManager.Instance != null)
                    FeedbackManager.Instance.SendHeatOn();
                break;
            case PickupType.Sword:
                inventory.AddSword();
                if (enemyToActivate != null)
                    enemyToActivate.ActivateEnemy();
                ShowFeedbackMessage();
                break;
            case PickupType.Key:
                inventory.AddKey();
                break;
        }

        if (promptUI != null)
            promptUI.SetActive(false);
        Destroy(gameObject);
    }

    private void ShowFeedbackMessage()
    {
        if (messageUI == null || string.IsNullOrEmpty(feedbackMessage)) return;
        messageUI.SetActive(true);
        TMPro.TextMeshProUGUI text = messageUI.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        if (text != null) text.text = feedbackMessage;
        Invoke(nameof(HideFeedbackMessage), feedbackDuration);
    }

    private void HideFeedbackMessage()
    {
        if (messageUI != null) messageUI.SetActive(false);
    }

    private void UpdatePromptUI()
    {
        if (promptUI == null) return;
        promptUI.SetActive(playerInRange);
        if (promptText == null)
            promptText = promptUI.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        if (promptText != null)
            promptText.text = PromptManager.Instance != null
                ? PromptManager.Instance.GetInteractPrompt()
                : promptMessage;
    }
}