using UnityEngine;

public class DoorController_PuzzleOnce : MonoBehaviour
{
    [Header("Door Components")]
    public Animator doorAnimator;
    public Collider2D doorCollider; // Collider qui bloque le passage

    private bool isOpen = false;
    private bool isClosedForever = false;

    /// <summary>
    /// Appelé depuis le PuzzleManager quand le puzzle est résolu.
    /// </summary>
    public void UnlockDoor()
    {
        if (isClosedForever) return;
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;
    }

    /// <summary>
    /// Appelé par ton trigger externe pour fermer définitivement.
    /// </summary>
    public void CloseDoorForever()
    {
        if (isClosedForever) return;

        isClosedForever = true;

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Close");

        if (doorCollider != null)
            doorCollider.enabled = true;
    }
}
