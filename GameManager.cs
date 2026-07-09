using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameMode { Keyboard, Glove }
    public GameMode CurrentMode { get; private set; } = GameMode.Keyboard;

    public bool IsGloveMode => CurrentMode == GameMode.Glove;

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

    public void SetKeyboardMode()
    {
        CurrentMode = GameMode.Keyboard;
        Debug.Log("Game mode: Keyboard");
    }

    public void SetGloveMode()
    {
        CurrentMode = GameMode.Glove;
        Debug.Log("Game mode: Glove");
    }
}