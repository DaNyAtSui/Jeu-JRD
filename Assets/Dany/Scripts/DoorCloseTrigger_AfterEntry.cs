using UnityEngine;

public class DoorCloseTrigger_AfterEntry : MonoBehaviour
{
    public DoorController_Timed door;
    public float closeDelay = 0.6f;

    private bool hasClosed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasClosed) return;
        if (!other.CompareTag("Player")) return;

        hasClosed = true;
        Invoke(nameof(CloseDoor), closeDelay);
    }

    private void CloseDoor()
    {
        if (door != null)
        {
            door.CloseDoor();
            Debug.Log("🚪 Porte refermée derrière le joueur.");
        }
    }
}
