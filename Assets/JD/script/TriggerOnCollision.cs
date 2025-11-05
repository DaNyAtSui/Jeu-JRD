using UnityEngine;

// Ce script a besoin qu'un DialogueTrigger soit sur le même objet
[RequireComponent(typeof(DialogueTrigger))]
public class TriggerOnCollision : MonoBehaviour
{
    // Référence au "bouton play"
    private DialogueTrigger dialogueTrigger; 
    
    // Optionnel : pour ne le jouer qu'une fois
    [SerializeField] private bool triggerOnce = true; 
    private bool hasBeenTriggered = false;

    void Awake()
    {
        // On récupère le "bouton play" qui est sur le même objet
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // On vérifie si c'est bien le joueur
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasBeenTriggered)
                return; // On l'a déjà joué

            // On "appuie" sur le bouton !
            dialogueTrigger.TriggerDialogue();
            hasBeenTriggered = true;
        }
    }
}