using UnityEngine;

public class PlayerInteractor2D : MonoBehaviour
{
    private ToggleZone2D currentZone;

    void OnInteract()
    {
        if (currentZone != null)
            currentZone.ToggleObject();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ToggleZone2D zone = other.GetComponent<ToggleZone2D>();
        if (zone != null)
            currentZone = zone;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ToggleZone2D zone = other.GetComponent<ToggleZone2D>();
        if (zone != null && currentZone == zone)
            currentZone = null;
    }
}
