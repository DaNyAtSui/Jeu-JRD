using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public DoorController_U1 door; // Glisser la même porte que Trigger A
    public float closeDelay = 0.6f; // Délai avant fermeture

    private bool hasClosed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasClosed) return;

        if (other.CompareTag("Player"))
        {
            hasClosed = true;
            Invoke(nameof(CloseDoor), closeDelay);
        }
    }

    private void CloseDoor()
    {
        if (door != null)
        {
            door.SendMessage("CloseDoor_FirstTime", SendMessageOptions.DontRequireReceiver);
        }
    }
}
