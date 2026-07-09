using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    public enum DoorRequirement
    {
        None,
        HasTorch,
        HasKey,
        AllEnemiesDefeated
    }

    [Header("References")]
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject messageUI;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerInventory playerInventoryFromScene;

    [Header("Frozen Door Visuals")]
    [SerializeField] private SpriteRenderer iceOverlay;
    [SerializeField] private GameObject frostEffect;

    [Header("Animator Settings")]
    [SerializeField] private string openBoolName = "isOpen";

    [Header("Door Settings")]
    [SerializeField] private DoorRequirement requirement = DoorRequirement.None;
    [SerializeField] private string nextSceneName = "CombatRoom";
    [SerializeField] private float messageDuration = 2f;
    [SerializeField] private float loadSceneDelay = 1f;
    [SerializeField] private float meltDuration = 2f;

    [Header("Database")]
    [SerializeField] private int levelId = 1;

    [Header("Hint Settings")]
    [SerializeField] private Key hintKey = Key.E;
    [SerializeField] private float hintMessageDuration = 3f;

    private bool playerInRange;
    private PlayerInventory playerInventory;
    private bool opened;
    private bool isMelting;

    private void Reset()
    {
        doorCollider = GetComponent<Collider2D>();
        animator     = GetComponent<Animator>();
    }

    private void Start()
    {
        if (messageUI != null)
            messageUI.SetActive(false);

        if (animator != null)
            animator.SetBool(openBoolName, false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (playerInventoryFromScene != null)
            playerInventory = playerInventoryFromScene;
    }

    public void SetPlayerInRange(bool inRange, PlayerInventory inventory = null)
    {
        playerInRange = inRange;

        if (inventory != null)
            playerInventory = inventory;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(inRange && !opened);
            TMPro.TextMeshProUGUI promptText = interactPrompt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (promptText != null && PromptManager.Instance != null)
                promptText.text = PromptManager.Instance.GetDoorPrompt();
        }
    }

    private void Update()
    {
        if (!playerInRange || opened || isMelting)
            return;

        bool gloveMode = GameManager.Instance != null && GameManager.Instance.IsGloveMode;

        if (!gloveMode && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            TryInteract();

        if (!gloveMode && requirement == DoorRequirement.HasKey)
        {
            if (playerInventory != null && !playerInventory.hasKey &&
                Keyboard.current != null && Keyboard.current[hintKey].wasPressedThisFrame)
                ShowHintMessage("Some purple items in this room may be worth a closer look. \nYou might feel something inside one of them");
        }

        if (!gloveMode && requirement == DoorRequirement.AllEnemiesDefeated)
        {
            if (playerInventory != null && !playerInventory.hasSword &&
                Keyboard.current != null && Keyboard.current[hintKey].wasPressedThisFrame)
                ShowHintMessage("Pick up a weapon.");
        }
    }

    public bool TryInteractFromFlexSpace()
    {
        if (!playerInRange || opened || isMelting)
            return false;
        TryInteract();
        return true;
    }

    private void TryInteract()
    {
        if (!CanOpen())
        {
            ShowLockedMessage();
            return;
        }

        if (requirement == DoorRequirement.HasTorch && iceOverlay != null && !isMelting)
        {
            StartCoroutine(MeltAndOpenRoutine());
            return;
        }

        OpenDoor();
    }

    private IEnumerator MeltAndOpenRoutine()
    {
        isMelting = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (frostEffect != null)
            frostEffect.SetActive(false);

        Color startColor = iceOverlay.color;
        float startAlpha = startColor.a;
        float timer      = 0f;

        while (timer < meltDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, timer / meltDuration);
            iceOverlay.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        iceOverlay.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        iceOverlay.gameObject.SetActive(false);

        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendHeatOff();

        OpenDoor();
        isMelting = false;
    }

    private bool CanOpen()
    {
        switch (requirement)
        {
            case DoorRequirement.None:
                return true;
            case DoorRequirement.HasTorch:
                return playerInventory != null && playerInventory.hasTorch;
            case DoorRequirement.HasKey:
                return playerInventory != null && playerInventory.hasKey;
            case DoorRequirement.AllEnemiesDefeated:
                return enemyManager != null && enemyManager.AreAllEnemiesDefeated();
            default:
                return false;
        }
    }

    private void ShowLockedMessage()
    {
        string hintPrompt = PromptManager.Instance != null
            ? PromptManager.Instance.GetHintPrompt()
            : "Press E to see hint.";

        switch (requirement)
        {
            case DoorRequirement.HasTorch:
                ShowMessage("The door is frozen. You need heat.");
                break;
            case DoorRequirement.AllEnemiesDefeated:
                if (playerInventory != null && !playerInventory.hasSword)
                    ShowMessage("Defeat all enemies first. \n " + hintPrompt);
                else
                    ShowMessage("Defeat all enemies first.");
                break;
            case DoorRequirement.HasKey:
                ShowMessage("You need a key to open the door. \n " + hintPrompt);
                break;
            default:
                ShowMessage("The door is locked.");
                break;
        }
    }

    private void ShowHintMessage(string message)
    {
        ShowMessage(message, hintMessageDuration);
    }

    public void TryShowHintFromFlex()
    {
        if (!playerInRange || opened) return;
        
        if (requirement == DoorRequirement.HasKey && playerInventory != null && !playerInventory.hasKey)
            ShowHintMessage("Some purple items in this room may be worth a closer look. \nYou might feel something inside one of them");

        if (requirement == DoorRequirement.AllEnemiesDefeated && playerInventory != null && !playerInventory.hasSword)
            ShowHintMessage("Find a weapon before your enemy finds you.");
    }

    private void ShowMessage(string message, float duration = -1f)
    {
        if (messageUI == null) return;

        messageUI.SetActive(true);

        TMPro.TextMeshProUGUI text =
            messageUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (text != null)
            text.text = message;

        CancelInvoke(nameof(HideMessage));
        float finalDuration = duration > 0f ? duration : messageDuration;
        Invoke(nameof(HideMessage), finalDuration);
    }

    private void HideMessage()
    {
        if (messageUI != null)
            messageUI.SetActive(false);
    }

    public void OpenDoor()
    {
        if (opened) return;
        if (!CanOpen())
        {
            ShowLockedMessage();
            return;
        }

        opened = true;


        if (FeedbackManager.Instance != null)
            FeedbackManager.Instance.SendVibrationPulse();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (animator != null)
            animator.SetBool(openBoolName, true);

        DisableCollision();

        // Save level result before loading next scene
        SaveAndLoad();
    }

    private void SaveAndLoad()
    {
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.CurrentUserId > 0)
        {
            float elapsed = LevelTimer.Instance != null ? LevelTimer.Instance.StopAndGet() : 0f;
            int wrongBreaks = WrongBreakCounter.Instance != null ? WrongBreakCounter.Instance.WrongBreaks : 0;

            DatabaseManager.Instance.SaveResult(levelId, elapsed, wrongBreaks, (success, score) =>
            {
                Debug.Log($"Level {levelId} saved — time: {elapsed:F1}s, wrong breaks: {wrongBreaks}, score: {score}");
                if (WrongBreakCounter.Instance != null) WrongBreakCounter.Instance.Reset();
                Invoke(nameof(LoadNextScene), loadSceneDelay);
            });
        }
        else
        {
            Invoke(nameof(LoadNextScene), loadSceneDelay);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void DisableCollision()
    {
        if (doorCollider != null)
            doorCollider.enabled = false;
    }
}