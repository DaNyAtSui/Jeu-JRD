using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [HideInInspector] public InteractLight currentInteractZone;

    // Appelée automatiquement par le PlayerInput (Send Messages)
    void OnInteract()
    {
        if (currentInteractZone != null)
        {
            currentInteractZone.OnInteract();
        }
    }
}
