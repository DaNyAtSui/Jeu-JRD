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

    [Header("UI Elements (Player Two)")]
    [SerializeField] private GameObject playerTwoDialogueBox;
    [SerializeField] private TextMeshProUGUI playerTwoDialogueText;
    [SerializeField] private Button playerTwoContinueButton;

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
    public bool IsDialogueActive { get; private set; }
    public static bool IsPlayerFrozen { get; private set; }

    void Start()
    {
        // ... (votre fonction Start() reste identique) ...
        sentences = new Queue<DialogueLine>();
        if (playerContinueButton) playerContinueButton.onClick.AddListener(DisplayNextSentence);
        if (playerThoughtContinueButton) playerThoughtContinueButton.onClick.AddListener(DisplayNextSentence);
        if (playerTwoContinueButton) playerTwoContinueButton.onClick.AddListener(DisplayNextSentence);
        if (narratorContinueButton) narratorContinueButton.onClick.AddListener(DisplayNextSentence);
        
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
    }

    // --- CHANGEMENT ICI ---
    // StartDialogue accepte maintenant un paramètre "contextTransform" optionnel
    public void StartDialogue(Dialogue dialogue, Transform contextTransform = null)
    {
        IsPlayerFrozen = true;
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
        IsDialogueActive = true;
        DisplayNextSentence();
    }

    // --- Votre fonction DisplayNextSentence() reste identique ---
    public void DisplayNextSentence()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        playerContinueButton.gameObject.SetActive(false);
        entityContinueButton.gameObject.SetActive(false);
        playerThoughtContinueButton.gameObject.SetActive(false);
        playerTwoContinueButton.gameObject.SetActive(false);
        narratorContinueButton.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            playerDialogueBox.SetActive(false);
            entityDialogueBox.SetActive(false);
            playerThoughtBox.SetActive(false);
            playerTwoDialogueBox.SetActive(false);
            narratorBox.SetActive(false);
            IsDialogueActive = false;
            IsPlayerFrozen = false;
            return;
        }

        DialogueLine line = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(line));
    }

    // --- SEULE LA PARTIE "NARRATOR" CHANGE ICI ---
    private IEnumerator TypeSentence(DialogueLine line)
    {
        // 1. On prépare les variables
        TextMeshProUGUI textComponent = null;
        GameObject dialogueBox = null;
        Button continueButton = null;

        // 2. On cache toutes les boîtes par défaut
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        narratorBox.SetActive(false);

        // 3. LE SWITCH CORRIGÉ (logique combinée)
        switch (line.speaker)
        {
            case Speaker.Player:
                // Logique de pause
                IsPlayerFrozen = true;
                // Logique d'UI
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                break;

            case Speaker.PlayerThought:
                // Logique de pause
                IsPlayerFrozen = false;
                // Logique d'UI
                textComponent = playerThoughtText;
                dialogueBox = playerThoughtBox;
                continueButton = playerThoughtContinueButton;
                break;

            case Speaker.PlayerCinematic:
                // Logique de pause
                IsPlayerFrozen = true;
                // Logique d'UI
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = null; // Pas de bouton
                break;

            case Speaker.PlayerTwo:
                // Logique de pause (on suppose qu'il fige)
                IsPlayerFrozen = true;
                // Logique d'UI
                textComponent = playerTwoDialogueText;
                dialogueBox = playerTwoDialogueBox;
                continueButton = playerTwoContinueButton;
                break;

            case Speaker.Entity:
                // Logique de pause
                IsPlayerFrozen = true;
                // Logique d'UI
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = entityContinueButton;
                break;

            case Speaker.Narrator:
                // Logique de pause
                IsPlayerFrozen = false;
                // Logique d'UI
                textComponent = narratorText;
                dialogueBox = narratorBox;
                continueButton = narratorContinueButton;
                
                // Logique de position (spécifique au Narrator)
                if (currentContextTransform != null)
                {
                    dialogueBox.transform.position = currentContextTransform.position;
                }
                break;
        }

        // 4. On active la bonne boîte (si elle existe)
        if(dialogueBox != null)
            dialogueBox.SetActive(true);

        // 5. On lance l'effet de frappe
        textComponent.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 6. On gère la fin de la phrase (Bouton vs Timer)
        if (line.speaker == Speaker.Player || 
            line.speaker == Speaker.PlayerThought ||
            line.speaker == Speaker.Narrator ||
            line.speaker == Speaker.PlayerTwo)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
        else if (line.speaker == Speaker.Entity ||
                line.speaker == Speaker.PlayerCinematic)
        {
            yield return new WaitForSeconds(entityDisplayTime);
            DisplayNextSentence();
        }
    }
}