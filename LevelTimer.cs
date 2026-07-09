using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    private float startTime;
    private bool  running = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        startTime = Time.time;
        running   = true;
    }

    public float GetElapsed()
    {
        return running ? Time.time - startTime : 0f;
    }

    public float StopAndGet()
    {
        running = false;
        return Time.time - startTime;
    }
}