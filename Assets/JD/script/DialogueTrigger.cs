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
    [SerializeField] private bool sendContextTransform = true; 

    
    // C'est la fonction que vos événements appellent
    public void TriggerDialogue()
    {
        // Si le dialogueManager n'est pas assigné, on essaie de le trouver
        if (dialogueManager == null)
        {
            // --- C'EST LA CORRECTION ---
            // On utilise la nouvelle fonction, plus rapide
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            // ---------------------------
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

        // On appelle StartDialogue en fonction de la case à cocher
        if (sendContextTransform)
        {
            dialogueManager.StartDialogue(dialogue, this.transform);
        }
        else
        {
            dialogueManager.StartDialogue(dialogue, null);
        }
    }
}