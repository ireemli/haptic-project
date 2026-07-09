using UnityEngine;

public class WrongBreakCounter : MonoBehaviour
{
    public static WrongBreakCounter Instance { get; private set; }

    public int WrongBreaks { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddWrongBreak()
    {
        WrongBreaks++;
        Debug.Log("Wrong breaks: " + WrongBreaks);
    }

    public void Reset()
    {
        WrongBreaks = 0;
    }
}