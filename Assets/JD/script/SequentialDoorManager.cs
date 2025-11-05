using UnityEngine;
using System.Collections.Generic;

public class SequentialDoorManager : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Glissez le DialogueManager de la scène ici")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Séquence de Dialogues")]
    [Tooltip("Les dialogues à jouer, dans l'ordre (Conv 1, Conv 2, etc.)")]
    [SerializeField] private Dialogue[] dialogueSequence;

    // ----- Variables privées -----
    private int currentSequenceIndex = 0;
    private List<GameObject> usedDoors = new List<GameObject>(); // Pour savoir quelles portes ont été utilisées

    // C'est la fonction que TOUTES les portes vont appeler
    public void OnDoorInteracted(GameObject doorObject)
    {
        // 1. Si un dialogue est déjà en cours, ne rien faire
        if (DialogueManager.IsDialogueActive)
            return;

        // 2. Si cette porte a déjà été utilisée, ne rien faire
        if (usedDoors.Contains(doorObject))
            return;
            
        // 3. Si on a joué tous les dialogues de la séquence, ne rien faire
        if (currentSequenceIndex >= dialogueSequence.Length)
            return;

        // --- C'est bon, on peut jouer le dialogue ---

        // On récupère le dialogue à jouer
        Dialogue dialogueToPlay = dialogueSequence[currentSequenceIndex];
        
        // On récupère la position de la porte (pour le Narrator)
        Transform context = doorObject.transform;

        // On lance le dialogue
        dialogueManager.StartDialogue(dialogueToPlay, context);

        // On "valide" cette étape
        usedDoors.Add(doorObject);  // On marque cette porte comme "utilisée"
        currentSequenceIndex++;     // On passe à l'étape suivante de la séquence
    }
}