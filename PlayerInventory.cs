using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject torchInHand;
    [SerializeField] private GameObject swordInHand;
    [SerializeField] private GameObject vaseInHand;
    [SerializeField] private GameObject keyInHand;

    public bool hasTorch = false;
    public bool hasSword = false;
    public bool hasVase = false;
    public bool hasKey = false;
    private void Start()
    {
        if (torchInHand != null)
            torchInHand.SetActive(false);
        if (swordInHand != null)
            swordInHand.SetActive(false);
        if (vaseInHand != null)
            vaseInHand.SetActive(false);
        if (keyInHand != null)
            keyInHand.SetActive(false);
    }

    public void AddTorch()
    {
        hasTorch = true;

        if (torchInHand != null)
            torchInHand.SetActive(true);

        Debug.Log("Torch added to inventory!");
    }

    public void AddSword()
    {
        hasSword = true;

        if (swordInHand != null)
            swordInHand.SetActive(true);

        Debug.Log("Sword added to inventory!");
    }

    public void AddVase()
    {
        hasVase = true;
        if (vaseInHand != null)
            vaseInHand.SetActive(true);

        Debug.Log("Vase added to inventory!");
    }

    public void AddKey()
    {
        hasKey = true;
        if (keyInHand != null)
            keyInHand.SetActive(true);

        Debug.Log("Key added to inventory!");
    }
}