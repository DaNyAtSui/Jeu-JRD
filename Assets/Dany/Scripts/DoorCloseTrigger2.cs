using UnityEngine;

public class DoorCloseTrigger2 : MonoBehaviour
{
    public DoorController_PuzzleOnce door; 
    public float closeDelay = 0.6f; 

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
            door.CloseDoorForever();
        }
    }
}
