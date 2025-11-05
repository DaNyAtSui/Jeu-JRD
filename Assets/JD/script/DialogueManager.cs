using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // ----- Interrupteurs Globaux -----
    [HideInInspector]
    public static bool IsPlayerFrozen { get; private set; }
    [HideInInspector]
    public static bool IsDialogueActive { get; private set; }

    // ----- Références UI -----
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

    // ----- Références Système et Positions -----
    [Header("System References")]
    [Tooltip("La position (Transform) du joueur")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("La position (Transform) de l'entité")]
    [SerializeField] private Transform entityTransform;

    [Header("Anchor Points (Pour les UI fixes)")]
    [SerializeField] private RectTransform anchorCinematicBL;
    [SerializeField] private RectTransform anchorCinematicTR;
    [SerializeField] private RectTransform anchorNarratorDefault;

    // ----- Réglages des Dialogues -----
    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float entityDisplayTime = 5.0f;
    [Tooltip("Le décalage en 3D (en mètres) au-dessus de la cible. Ex: (0, 1.5, 0)")]
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0, 1.5f, 0); 

    // ----- Variables Privées -----
    private Queue<DialogueLine> sentences;
    private Coroutine typingCoroutine;
    private Transform currentContextTransform; // Pour le Narrator

    void Awake()
    {
        IsDialogueActive = false;
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

    public void StartDialogue(Dialogue dialogue, Transform contextTransform = null)
    {
        IsDialogueActive = true; 
        IsPlayerFrozen = true; 
        
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
        playerThoughtBox.SetActive(false);
        narratorBox.SetActive(false);
        playerTwoDialogueBox.SetActive(false);
        
        this.currentContextTransform = contextTransform; 

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

        // 3. Switch (Pause + UI)
        switch (line.speaker)
        {
            // ... (VOTRE SWITCH EST BON, IL RESTE IDENTIQUE) ...
            case Speaker.Player:
                IsPlayerFrozen = true;
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                break;
            case Speaker.PlayerThought:
                IsPlayerFrozen = false; 
                textComponent = playerThoughtText;
                dialogueBox = playerThoughtBox;
                continueButton = playerThoughtContinueButton;
                break;
            case Speaker.PlayerCinematic:
                IsPlayerFrozen = true;
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = null;
                break;
            case Speaker.PlayerTwo:
                IsPlayerFrozen = true;
                textComponent = playerTwoDialogueText;
                dialogueBox = playerTwoDialogueBox;
                continueButton = playerContinueButton;
                break;
            case Speaker.Entity:
                IsPlayerFrozen = true;
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = entityContinueButton; 
                break;
            case Speaker.EntityCinematic:
                IsPlayerFrozen = true;
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = null;
                break;
            case Speaker.Narrator:
                IsPlayerFrozen = false; 
                textComponent = narratorText;
                dialogueBox = narratorBox;
                continueButton = narratorContinueButton;
                break;
        }

        // 4. Activer la bonne boîte
        if(dialogueBox != null)
            dialogueBox.SetActive(true);

        // 5. LOGIQUE DE POSITIONNEMENT (RE-CORRIGÉE, 100% PIXELS)
        RectTransform bubbleRect = dialogueBox.GetComponent<RectTransform>();
        
        // On récupère la Main Camera (Base)
        Camera baseCamera = Camera.main; // Trouve la caméra avec le Tag "MainCamera"

        if (line.speaker == Speaker.Player || line.speaker == Speaker.PlayerThought)
        {
            Vector2 screenPoint = baseCamera.WorldToScreenPoint(playerTransform.position);
            bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
        }
        else if (line.speaker == Speaker.Entity)
        {
            Vector2 screenPoint = baseCamera.WorldToScreenPoint(entityTransform.position);
            bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
        }
        else if (line.speaker == Speaker.Narrator)
        {
            if (currentContextTransform != null) 
            {
                Vector2 screenPoint = baseCamera.WorldToScreenPoint(currentContextTransform.position);
                bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
            }
            else 
            {
                // CORRECTION: Utiliser .position, pas .anchoredPosition
                bubbleRect.position = anchorNarratorDefault.position;
            }
        }
        else if (line.speaker == Speaker.PlayerCinematic)
        {
            // CORRECTION: Utiliser .position, pas .anchoredPosition
            bubbleRect.position = anchorCinematicBL.position;
        }
        else if (line.speaker == Speaker.EntityCinematic)
        {
            // CORRECTION: Utiliser .position, pas .anchoredPosition
            bubbleRect.position = anchorCinematicTR.position;
        }
        else if (line.speaker == Speaker.PlayerTwo)
        {
            // CORRECTION: Pour centrer, on utilise la position du parent (le Canvas)
            bubbleRect.position = bubbleRect.parent.position;
        }

        // 6. Effet de frappe
        textComponent.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            // --- MISE À JOUR CONTINUE (pour les bulles qui suivent) ---
            if (line.speaker == Speaker.Player || line.speaker == Speaker.PlayerThought)
            {
                Vector2 screenPoint = baseCamera.WorldToScreenPoint(playerTransform.position);
                bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
            }
            else if (line.speaker == Speaker.Entity)
            {
                Vector2 screenPoint = baseCamera.WorldToScreenPoint(entityTransform.position);
                bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
            }
            // --- FIN DE LA CORRECTION ---

            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 7. Gérer la fin de la phrase
        bool needsPostTypingFollow = (line.speaker == Speaker.Player || 
                                      line.speaker == Speaker.PlayerThought || 
                                      line.speaker == Speaker.Entity);

        // A. Cas avec un bouton "Continuer"
        if (line.speaker == Speaker.Player || 
            line.speaker == Speaker.PlayerThought ||
            line.speaker == Speaker.Narrator ||
            line.speaker == Speaker.PlayerTwo)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
            
            // --- BOUCLE DE SUIVI POST-FRAPPE ---
            while (needsPostTypingFollow)
            {
                if (line.speaker == Speaker.Player || line.speaker == Speaker.PlayerThought)
                {
                    Vector2 screenPoint = baseCamera.WorldToScreenPoint(playerTransform.position);
                    bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
                }
                else if (line.speaker == Speaker.Entity)
                {
                    Vector2 screenPoint = baseCamera.WorldToScreenPoint(entityTransform.position);
                    bubbleRect.position = new Vector3(screenPoint.x, screenPoint.y + (float)bubbleOffset.y, bubbleRect.position.z);
                }
                yield return null; 
            }
        }
        // B. Cas avec un timer
        else if (line.speaker == Speaker.Entity ||
                 line.speaker == Speaker.PlayerCinematic ||
                 line.speaker == Speaker.EntityCinematic)
        {
            yield return new WaitForSeconds(entityDisplayTime);
            DisplayNextSentence();
        }
    }
}