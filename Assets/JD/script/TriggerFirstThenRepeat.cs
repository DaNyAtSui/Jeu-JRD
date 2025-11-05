using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerFirstThenRepeat : MonoBehaviour
{
    [Header("Dialogues à jouer")]
    [SerializeField] private Dialogue dialogueFirstTime;
    [SerializeField] private Dialogue dialogueRepeatable;

    // --- C'EST L'AJOUT ---
    [Header("Options")]
    [Tooltip("Faut-il envoyer la position de cet objet au Narrateur ?")]
    [SerializeField] private bool sendContextTransform = true; 
    // -----------------------

    private DialogueManager dialogueManager;
    private bool hasBeenTriggeredOnce = false; 

    void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
            Debug.LogError("Aucun DialogueManager n'a été trouvé !", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogueManager.IsDialogueActive)
                return;
            
            // On détermine quel dialogue jouer
            Dialogue dialogueToPlay = hasBeenTriggeredOnce ? dialogueRepeatable : dialogueFirstTime;

            if (dialogueToPlay == null)
                return;

            // --- C'EST LA CORRECTION ---
            // On détermine quel contexte envoyer (la position 3D ou 'rien')
            Transform context = sendContextTransform ? this.transform : null;
            
            // On lance le dialogue
            dialogueManager.StartDialogue(dialogueToPlay, context);
            // ---------------------------
            
            hasBeenTriggeredOnce = true;
        }
    }
}