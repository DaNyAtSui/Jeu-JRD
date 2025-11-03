using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // ... (Toutes vos références UI restent les mêmes) ...
    [Header("UI Elements (Player Speak)")]
    [SerializeField] private GameObject playerDialogueBox;
    [SerializeField] private TextMeshProUGUI playerDialogueText;
    [SerializeField] private Button playerContinueButton;

    [Header("UI Elements (Player Thought)")]
    [SerializeField] private GameObject playerThoughtBox;
    [SerializeField] private TextMeshProUGUI playerThoughtText;
    [SerializeField] private Button playerThoughtContinueButton;

    [Header("UI Elements (Entity)")]
    [SerializeField] private GameObject entityDialogueBox;
    [SerializeField] private TextMeshProUGUI entityDialogueText;
    [SerializeField] private Button entityContinueButton; 

    [Header("UI Elements (Narrator)")]
    [SerializeField] private GameObject narratorBox;
    [SerializeField] private TextMeshProUGUI narratorText;
    [SerializeField] private Button narratorContinueButton;

    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float entityDisplayTime = 5.0f;

    private Queue<DialogueLine> sentences;
    private Coroutine typingCoroutine;
    
    // --- CHANGEMENT ICI ---
    // On ajoute une variable pour stocker le "contexte" (l'objet qui parle)
    private Transform currentContextTransform;

    void Start()
    {
        // ... (votre fonction Start() reste identique) ...
        sentences = new Queue<DialogueLine>();
        if (playerContinueButton) playerContinueButton.onClick.AddListener(DisplayNextSentence);
        if (playerThoughtContinueButton) playerThoughtContinueButton.onClick.AddListener(DisplayNextSentence);
        if (narratorContinueButton) narratorContinueButton.onClick.AddListener(DisplayNextSentence);
        
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
    }

    // --- CHANGEMENT ICI ---
    // StartDialogue accepte maintenant un paramètre "contextTransform" optionnel
    public void StartDialogue(Dialogue dialogue, Transform contextTransform = null)
    {
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
        
        // On stocke le transform de l'objet qui a déclenché le dialogue
        this.currentContextTransform = contextTransform;

        sentences.Clear();
        foreach (DialogueLine line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    // --- Votre fonction DisplayNextSentence() reste identique ---
    public void DisplayNextSentence()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        playerContinueButton.gameObject.SetActive(false);
        entityContinueButton.gameObject.SetActive(false);
        playerThoughtContinueButton.gameObject.SetActive(false);
        narratorContinueButton.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            playerDialogueBox.SetActive(false);
            entityDialogueBox.SetActive(false);
            playerThoughtBox.SetActive(false);
            narratorBox.SetActive(false);
            return;
        }

        DialogueLine line = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(line));
    }

    // --- SEULE LA PARTIE "NARRATOR" CHANGE ICI ---
    private IEnumerator TypeSentence(DialogueLine line)
    {
        TextMeshProUGUI textComponent = null;
        GameObject dialogueBox = null;
        Button continueButton = null;

        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);

        switch (line.speaker)
        {
            case Speaker.Player:
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                break;

            case Speaker.PlayerThought:
                textComponent = playerThoughtText;
                dialogueBox = playerThoughtBox;
                continueButton = playerThoughtContinueButton;
                break;

            case Speaker.Entity:
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = entityContinueButton; 
                break;

            case Speaker.Narrator:
                textComponent = narratorText;
                dialogueBox = narratorBox;
                continueButton = narratorContinueButton;
                
                // --- CHANGEMENT ICI ---
                // Si on a reçu un "contexte" (l'objet interactif)
                if (currentContextTransform != null)
                {
                    // On DÉPLACE la bulle du narrateur à la position de cet objet
                    dialogueBox.transform.position = currentContextTransform.position;
                }
                // Si aucun contexte n'est donné, la bulle s'affichera
                // là où elle se trouve par défaut.
                break;
        }

        if(dialogueBox != null)
            dialogueBox.SetActive(true);

        textComponent.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // ... (La fin de la fonction (logique de bouton/timer) reste identique) ...
        if (line.speaker == Speaker.Player || 
            line.speaker == Speaker.PlayerThought ||
            line.speaker == Speaker.Narrator)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
        else if (line.speaker == Speaker.Entity)
        {
            yield return new WaitForSeconds(entityDisplayTime);
            DisplayNextSentence();
        }
    }
}