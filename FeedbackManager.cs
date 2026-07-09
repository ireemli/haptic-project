using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private SerialController serialController;

    [Header("Vibration Commands")]
    [SerializeField] private string vibPulseCommand  = "VIB:PULSE";
    [SerializeField] private string vibBurstCommand  = "VIB:BURST";
    [SerializeField] private string vibHitCommand    = "VIB:HIT";

    [Header("Thermal Commands")]
    [SerializeField] private string heatOnCommand    = "HEAT:ON";
    [SerializeField] private string heatOffCommand   = "HEAT:OFF";

    private void Awake()
    {
        // Singleton — sahneler arası tek instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ── Vibration ──────────────────────────────────────────────────────────

    /// <summary>
    /// Correct vase picked up — short pulse simulating rattle inside hollow object.
    /// </summary>
    public void SendVibrationPulse()
    {
        Send(vibPulseCommand);
    }

    /// <summary>
    /// Any vase thrown and broken — burst on impact.
    /// </summary>
    public void SendVibrationBurst()
    {
        Send(vibBurstCommand);
    }

    /// <summary>
    /// Player takes damage from enemy — hit feedback.
    /// </summary>
    public void SendVibrationHit()
    {
        Send(vibHitCommand);
    }

    // ── Thermal ────────────────────────────────────────────────────────────

    /// <summary>
    /// Torch picked up / frozen door interaction — activate Peltier heat.
    /// </summary>
    public void SendHeatOn()
    {
        Send(heatOnCommand);
    }

    /// <summary>
    /// Door melted / scene transition — deactivate Peltier.
    /// </summary>
    public void SendHeatOff()
    {
        Send(heatOffCommand);
    }

    // ── Internal ───────────────────────────────────────────────────────────

    private void Send(string command)
    {
        if (serialController == null)
        {
            Debug.LogWarning("FeedbackManager: SerialController reference is missing.");
            return;
        }
        serialController.Send(command);
    }
}