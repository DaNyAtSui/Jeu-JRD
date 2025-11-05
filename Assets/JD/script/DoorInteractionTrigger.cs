using UnityEngine;

// A besoin d'un Collider pour détecter le joueur
[RequireComponent(typeof(Collider2D))]
public class DoorInteractionTrigger : MonoBehaviour
{
    [Header("Référence")]
    [Tooltip("Glissez le 'SequentialDoorManager' de la scène ici")]
    [SerializeField] private SequentialDoorManager doorManager;

    private bool playerIsNearby = false;

    private void Update()
    {
        // Si le joueur est proche ET appuie sur "E"
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (doorManager != null)
            {
                // On prévient le "cerveau" et on lui dit "c'est moi qui appelle"
                doorManager.OnDoorInteracted(this.gameObject);
            }
        }
    }

    // Identique à votre TriggerOnInteract
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
        }
    }
}