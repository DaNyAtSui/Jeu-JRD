using UnityEngine;

// A besoin d'un DialogueTrigger et d'un Collider
[RequireComponent(typeof(DialogueTrigger))]
[RequireComponent(typeof(Collider2D))]
public class TriggerOnInteract : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    private bool playerIsNearby = false;
    
    // Optionnel : pour afficher "Appuyez sur E"
    // [SerializeField] private GameObject interactPrompt; 

    void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        // if (interactPrompt) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        // Si le joueur est proche ET appuie sur "E"
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E))
        {
            // On vérifie que le joueur n'est pas déjà figé par un dialogue
            if (!DialogueManager.IsPlayerFrozen)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
            // if (interactPrompt) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            // if (interactPrompt) interactPrompt.SetActive(false);
        }
    }
}