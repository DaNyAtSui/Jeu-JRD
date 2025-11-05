using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le DialogueManager présent dans la scène")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Dialogue à jouer")]
    [Tooltip("L'asset de dialogue ('DVD') à charger")]
    [SerializeField] private Dialogue dialogue;

    [Header("Options")]
    [Tooltip("Faut-il envoyer la position de cet objet au Narrateur ? Décochez pour l'intro ou les dialogues 'full screen'.")]
    [SerializeField] private bool sendContextTransform = true; // Par défaut, on envoie le contexte

    
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

        // --- C'EST LA CORRECTION ---
        if (sendContextTransform)
        {
            // Comportement normal : on envoie la position de cet objet
            dialogueManager.StartDialogue(dialogue, this.transform);
        }
        else
        {
            // Comportement "Intro" : on envoie 'null' comme position
            // Le DialogueManager utilisera donc sa position par défaut
            dialogueManager.StartDialogue(dialogue, null);
        }
    }
}