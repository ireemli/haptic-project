using UnityEngine;

public class InteractZoneTrigger : MonoBehaviour
{
    [SerializeField] private DoorInteraction door;

    private void Reset()
    {
        door = GetComponentInParent<DoorInteraction>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        door.SetPlayerInRange(true, inv);

        Debug.Log("You are in the interaction zone");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        door.SetPlayerInRange(false, null);

        Debug.Log("You are out of the interaction zone");
    }
}