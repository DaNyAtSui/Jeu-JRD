using UnityEngine;

public class DoorController_Timed : MonoBehaviour
{
    [Header("Door Components")]
    public Animator doorAnimator;
    public Collider2D doorCollider;

    private bool isOpen = false;
    private bool isUnlocked = false; // devient vrai après le puzzle
    private bool isClosedForever = false; // 🚫 plus jamais de réouverture

    public bool IsUnlocked => isUnlocked;
    public bool IsClosedForever => isClosedForever;

    public void OpenDoor()
    {
        if (isClosedForever) // 🚫 plus jamais d’ouverture après fermeture finale
        {
            Debug.Log("🚷 Tentative d'ouverture ignorée : porte définitivement fermée.");
            return;
        }

        if (isOpen) return;
        isOpen = true;

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("🚪 Porte ouverte !");
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Close");

        if (doorCollider != null)
            doorCollider.enabled = true;

        Debug.Log("🔒 Porte fermée !");
    }

    /// <summary>
    /// Appelé par ton puzzle quand il est fini → déverrouille et ouvre.
    /// </summary>
    public void UnlockPermanently()
    {
        isUnlocked = true;
        OpenDoor();
        Debug.Log("✅ Porte déverrouillée et ouverte (puzzle fini).");
    }

    /// <summary>
    /// Appelé par le trigger de sortie → fermeture définitive.
    /// </summary>
    public void CloseForever()
    {
        if (isClosedForever) return;

        isClosedForever = true;
        CloseDoor();

        Debug.Log("🚫 Porte définitivement fermée (fin de la salle).");
    }
}
