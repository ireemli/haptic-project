using UnityEngine;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private bool IsGloveMode => GameManager.Instance != null && GameManager.Instance.IsGloveMode;

    public string GetInteractPrompt()
    {
        return IsGloveMode ? "Flex index finger to pick up" : "Press E to pick up";
    }

    public string GetAttackPrompt()
    {
        return IsGloveMode ? "Flex middle finger to attack" : "Press Q to attack";
    }

    public string GetDoorPrompt()
    {
        return IsGloveMode ? "Flex ring finger to interact" : "Press Space to interact";
    }

    public string GetHintPrompt()
    {
        return IsGloveMode ? "Flex index finger to see hint." : "Press E to see hint.";
    }

    public string GetDropPrompt()
    {
        return IsGloveMode ? "Flex index finger to drop" : "Press E to drop";
    }

    public string GetThrowPrompt()
    {
        return IsGloveMode ? "Flex middle finger to throw" : "Press Q to throw";
    }
}