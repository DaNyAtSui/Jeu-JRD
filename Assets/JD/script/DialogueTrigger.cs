using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le DialogueManager présent dans la scène")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Dialogue à jouer")]
    [Tooltip("L'asset de dialogue ('DVD') à charger")]
    [SerializeField] private Dialogue dialogue;

    
    // C'est la fonction que vos événements appellent
    public void TriggerDialogue()
    {
        // Si le dialogueManager n'est pas assigné, on essaie de le trouver
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        // Sécurités
        if (dialogueManager == null)
        {
            Debug.LogError("Aucun DialogueManager n'est assigné ou trouvé dans la scène !", this);
            return;
        }
        if (dialogue == null)
        {
            Debug.LogError("Aucun asset de Dialogue n'est assigné à ce trigger !", this);
            return;
        }

        // --- CHANGEMENT ICI ---
        // On appelle StartDialogue en lui passant DEUX choses :
        // 1. Le dialogue à jouer (dialogue)
        // 2. Le "contexte", c'est-à-dire la position de cet objet (this.transform)
        dialogueManager.StartDialogue(dialogue, this.transform);
    }
}