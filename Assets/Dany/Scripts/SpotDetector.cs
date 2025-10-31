using UnityEngine;

public class SpotDetector : MonoBehaviour
{
    public bool isOccupied = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PushBlock"))
            isOccupied = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PushBlock"))
            isOccupied = false;
    }
}
