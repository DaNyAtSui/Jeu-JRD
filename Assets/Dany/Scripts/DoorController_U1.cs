using UnityEngine;

public class DoorController_U1 : MonoBehaviour
{
    [Header("Door Components")]
    public Animator doorAnimator;
    public Collider2D doorCollider; // Collider qui bloque le passage

    [Header("First Opening Settings")]
    public bool autoPlayFirstOpening = true; // La première ouverture vient du trigger extérieur
    public float firstCloseDelay = 1.2f; // Temps après sortie pour fermer

    [Header("Second Opening (After Unlock)")]
    public bool unlocked = false; // Devient true uniquement quand ton énigme appelle UnlockDoor()
    public float secondOpeningDelay = 1.5f; // Dramatic pause before auto-open
    public float secondCloseDelay = 1.5f; // Temps avant fermeture définitive

    private bool firstOpened = false;
    private bool secondOpened = false;
    private bool finalLocked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || finalLocked)
            return;

        // Première fois qu'on entre → jouer l'ouverture initiale
        if (!firstOpened && autoPlayFirstOpening)
        {
            firstOpened = true;
            OpenDoor();
        }
        // Après énigme résolue → ouverture dramatique unique
        else if (unlocked && !secondOpened)
        {
            secondOpened = true;
            Invoke(nameof(OpenDoor), secondOpeningDelay);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || finalLocked)
            return;

        // Fermeture après la première ouverture
        if (firstOpened && !secondOpened)
        {
            Invoke(nameof(CloseDoor_FirstTime), firstCloseDelay);
        }

        // Fermeture finale après la seconde ouverture
        if (secondOpened)
        {
            Invoke(nameof(CloseDoor_Final), secondCloseDelay);
        }
    }

    private void OpenDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;
    }

    private void CloseDoor_FirstTime()
    {
        if (secondOpened) return; // Si déjà passé à la 2e phase, ignore

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Close");

        // On réactive le collider après un léger délai pour laisser l'anim se jouer
        Invoke(nameof(ReEnableCollider), 0.5f);
    }

    private void CloseDoor_Final()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Close");

        // Porte désormais condamnée pour toujours
        finalLocked = true;
        Invoke(nameof(ReEnableCollider), 0.5f);
    }

    private void ReEnableCollider()
    {
        if (doorCollider != null)
            doorCollider.enabled = true;
    }

    /// <summary>
    /// Appelle ceci depuis ton script d’énigme quand le joueur résout l’épreuve.
    /// </summary>
    public void UnlockDoor()
    {
        unlocked = true;
    }
}
