using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static bool IsDialogueActive { get;  private set; }
    public static bool IsPlayerFrozen { get; private set; }

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

    [Header("System References")]
    [Tooltip("La caméra principale de la scène")]
    [SerializeField] private Camera mainCamera; 
    [Tooltip("La position (Transform) du joueur")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("La position (Transform) de l'entité")]
    [SerializeField] private Transform entityTransform;
    [Header("Anchor Points")]
    [SerializeField] private RectTransform anchorCinematicBL;
    [SerializeField] private RectTransform anchorCinematicTR;
    [SerializeField] private RectTransform anchorNarratorDefault;

    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float entityDisplayTime = 5.0f;
    [SerializeField] private Vector2 bubbleOffset = new Vector2(0, 100); // Décalage de la bulle (en pixels)

    private Queue<DialogueLine> sentences;
    private Coroutine typingCoroutine;
    private Transform currentContextTransform; // Pour le Narrator

    // Awake() s'exécute AVANT Start(), ce qui empêche les erreurs de "course"
    void Awake()
    {
        IsDialogueActive = false;
        sentences = new Queue<DialogueLine>();
        
        if (playerContinueButton) playerContinueButton.onClick.AddListener(DisplayNextSentence);
        if (playerThoughtContinueButton) playerThoughtContinueButton.onClick.AddListener(DisplayNextSentence);
        if (playerTwoContinueButton) playerTwoContinueButton.onClick.AddListener(DisplayNextSentence);
        if (narratorContinueButton) narratorContinueButton.onClick.AddListener(DisplayNextSentence);
        
        // S'assurer que tout est caché au démarrage
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue, Transform contextTransform = null)
    {
        IsDialogueActive = true;
        IsPlayerFrozen = true; // On fige le joueur par défaut au début
        
        // On cache toutes les boîtes
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        
        this.currentContextTransform = contextTransform; // On mémorise l'objet (pour le Narrator)

        sentences.Clear();
        foreach (DialogueLine line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // On cache les boutons
        playerContinueButton.gameObject.SetActive(false);
        entityContinueButton.gameObject.SetActive(false);
        playerThoughtContinueButton.gameObject.SetActive(false);
        playerTwoContinueButton.gameObject.SetActive(false);
        narratorContinueButton.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            // On cache toutes les boîtes à la fin
            playerDialogueBox.SetActive(false);
            entityDialogueBox.SetActive(false);
            playerThoughtBox.SetActive(false);
            playerTwoDialogueBox.SetActive(false);
            narratorBox.SetActive(false);

            IsPlayerFrozen = false;
            IsDialogueActive = false;
            return;
        }

        DialogueLine line = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(line));
    }

    private IEnumerator TypeSentence(DialogueLine line)
    {
        // 1. Préparer les variables
        TextMeshProUGUI textComponent = null;
        GameObject dialogueBox = null;
        Button continueButton = null;

        // 2. Cacher toutes les boîtes
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        narratorBox.SetActive(false);

        // 3. Switch unique pour la logique de pause ET la logique d'UI
        switch (line.speaker)
        {
            case Speaker.Player:
                IsPlayerFrozen = true;
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                break;

            case Speaker.PlayerThought:
                IsPlayerFrozen = false; // Le joueur peut bouger
                textComponent = playerThoughtText;
                dialogueBox = playerThoughtBox;
                continueButton = playerThoughtContinueButton;
                break;

            case Speaker.PlayerCinematic:
                IsPlayerFrozen = true;
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = null; // Pas de bouton
                break;

            case Speaker.PlayerTwo:
                IsPlayerFrozen = true;
                textComponent = playerTwoDialogueText;
                dialogueBox = playerTwoDialogueBox;
                continueButton = playerTwoContinueButton;
                break;

            case Speaker.Entity:
                IsPlayerFrozen = true;
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = entityContinueButton;
                break;
                
            case Speaker.EntityCinematic:
                IsPlayerFrozen = true;                // Fige le joueur
                textComponent = entityDialogueText;   // Réutilise la bulle de l'Entity
                dialogueBox = entityDialogueBox;      // Réutilise la bulle de l'Entity
                continueButton = null;                // Pas de bouton (mode timer)
                break;

            case Speaker.Narrator:
                IsPlayerFrozen = false; // Le joueur peut bouger
                textComponent = narratorText;
                dialogueBox = narratorBox;
                continueButton = narratorContinueButton;
                break;
        }

        // 4. Activer la bonne boîte
        if(dialogueBox != null)
            dialogueBox.SetActive(true);

        // 5. LOGIQUE DE POSITIONNEMENT (AMÉLIORÉE)
        if (line.speaker == Speaker.Player || line.speaker == Speaker.PlayerThought)
        {
            // Suivre le joueur (vous pouvez ajuster "bubbleOffset" dans l'Inspecteur)
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(playerTransform.position);
            dialogueBox.transform.position = screenPoint + bubbleOffset;
        }
        else if (line.speaker == Speaker.Entity)
        {
            // Suivre l'entité (vous pouvez ajuster "bubbleOffset" dans l'Inspecteur)
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(entityTransform.position);
            dialogueBox.transform.position = screenPoint + bubbleOffset;
        }
        else if (line.speaker == Speaker.Narrator)
        {
            // Si on a un objet (contexte), on le suit.
            if (currentContextTransform != null)
            {
                Vector2 screenPoint = mainCamera.WorldToScreenPoint(currentContextTransform.position);
                dialogueBox.transform.position = screenPoint;
            }
            else // Sinon, on utilise la position par défaut (en bas au centre)
            {
                dialogueBox.transform.position = anchorNarratorDefault.position;
            }
        }
        else if (line.speaker == Speaker.PlayerCinematic)
        {
            // Se coller à l'ancre en bas à gauche
            dialogueBox.transform.position = anchorCinematicBL.position;
        }
        else if (line.speaker == Speaker.EntityCinematic)
        {
            // Se coller à l'ancre en haut à droite
            dialogueBox.transform.position = anchorCinematicTR.position;
        }
        else if (line.speaker == Speaker.PlayerTwo)
        {
            // PlayerTwo reste au centre par défaut
            dialogueBox.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // 6. Effet de frappe
        textComponent.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 7. Gérer la fin de la phrase (Bouton vs Timer)
        if (line.speaker == Speaker.Player || 
            line.speaker == Speaker.PlayerThought ||
            line.speaker == Speaker.Narrator ||
            line.speaker == Speaker.PlayerTwo)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
        else if (line.speaker == Speaker.Entity ||
                 line.speaker == Speaker.PlayerCinematic ||
                 line.speaker == Speaker.EntityCinematic)
        {
            yield return new WaitForSeconds(entityDisplayTime);
            DisplayNextSentence();
        }
    }
}