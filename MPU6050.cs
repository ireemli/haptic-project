using UnityEngine;
using System.IO.Ports;

[RequireComponent(typeof(Rigidbody2D))]
public class SerialController : MonoBehaviour
{
    private Animator animator;

    public string portName = "/dev/cu.usbserial-0001";
    public int baudRate = 115200;

    public float moveSpeed = 2.5f;
    public float smoothing = 10f;

    public PlayerSwordAttack swordAttack;

    private SerialPort serial;
    private Rigidbody2D rb;
    private Vector2 targetInput = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (GameManager.Instance == null || !GameManager.Instance.IsGloveMode)
        {
            Debug.Log("Keyboard mode — serial port not opened.");
            return;
        }

        serial = new SerialPort(portName, baudRate);
        serial.ReadTimeout = 10;
        serial.NewLine = "\n";

        try
        {
            serial.Open();
            Debug.Log("ESP32 connected: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial open failed: " + e.Message);
        }
    }

    void Update()
    {
        ReadSerial();

        currentInput = Vector2.Lerp(
            currentInput,
            targetInput,
            smoothing * Time.deltaTime
        );
    }

    void ReadSerial()
    {
        if (serial == null || !serial.IsOpen) return;

        try
        {
            string data = serial.ReadLine().Trim();
            if (string.IsNullOrEmpty(data)) return;

            Debug.Log("ESP32 -> " + data);

            if (data == "ACT:E")   { HandleFlexE();     return; }
            if (data == "ACT:Q")   { HandleFlexQ();     return; }
            if (data == "ACT:SPACE") { HandleFlexSpace(); return; }

            ParseMovement(data);
        }
        catch { }
    }

    void ParseMovement(string data)
    {
        Vector2 input = Vector2.zero;

        if (data != "IDLE")
        {
            if (data.Contains("W")) input.y += 1f;
            if (data.Contains("S")) input.y -= 1f;
            if (data.Contains("A")) input.x -= 1f;
            if (data.Contains("D")) input.x += 1f;
        }

        targetInput = input.normalized;

        if (animator != null)
            animator.SetBool("isMoving", targetInput.sqrMagnitude > 0.01f);
    }

    void HandleFlexE()
    {
        DeathPanelManager deathPanel = Object.FindFirstObjectByType<DeathPanelManager>();
        if (deathPanel != null && deathPanel.gameObject.activeInHierarchy)
        {
            deathPanel.Restart();
            return;
        }

        PickupVase[] vases = Object.FindObjectsByType<PickupVase>(FindObjectsSortMode.None);
        foreach (PickupVase vase in vases)
            if (vase.TryEFromFlex()) return;

        PickupItem[] items = Object.FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        foreach (PickupItem item in items)
            if (item.TryPickupFromFlex()) return;

        DoorInteraction[] doors = Object.FindObjectsByType<DoorInteraction>(FindObjectsSortMode.None);
        foreach (DoorInteraction door in doors)
            door.TryShowHintFromFlex();

        Debug.Log("ACT:E geldi ama alınacak/bırakılacak item yok.");
    }

    void HandleFlexQ()
    {
        PickupVase[] vases = Object.FindObjectsByType<PickupVase>(FindObjectsSortMode.None);
        foreach (PickupVase vase in vases)
            if (vase.TryQFromFlex()) return;

        if (swordAttack != null)
        {
            swordAttack.AttackFromFlex();
            return;
        }

        Debug.Log("ACT:Q geldi ama vazo/kılıç aksiyonu yok.");
    }

    void HandleFlexSpace()
    {
        DeathPanelManager deathPanel = Object.FindFirstObjectByType<DeathPanelManager>();
        if (deathPanel != null && deathPanel.gameObject.activeInHierarchy)
        {
            deathPanel.GoToMenu();
            return;
        }

        DoorInteraction[] doors = Object.FindObjectsByType<DoorInteraction>(FindObjectsSortMode.None);
        foreach (DoorInteraction door in doors)
            if (door.TryInteractFromFlexSpace()) return;

        Debug.Log("ACT:SPACE geldi ama kapı etkileşimi yok.");
    }

    void FixedUpdate()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGloveMode) return;
        rb.linearVelocity = currentInput * moveSpeed;
    }

    public void Send(string command)
    {
        if (serial != null && serial.IsOpen)
        {
            try
            {
                serial.WriteLine(command);
                Debug.Log("Unity -> ESP32: " + command);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial write failed: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}