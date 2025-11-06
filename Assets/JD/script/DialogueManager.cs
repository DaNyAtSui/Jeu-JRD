using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    // ----- Interrupteurs Globaux -----
    [HideInInspector]
    public static bool IsPlayerFrozen { get; private set; }
    [HideInInspector]
    public static bool IsDialogueActive { get; private set; }

    [SerializeField] private SpeechBubble mainSpeechBubble;
    [SerializeField] private SpeechBubble inWorldSpeechBubble;
    [SerializeField] private GameObject inWorldCanvas;
    SpeechBubble currentSpeechBubble;

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

    // ----- Variables Privées -----
    private Queue<DialogueLine> sentences;
    private Coroutine typingCoroutine;

    void Awake()
    {
        IsDialogueActive = false;
        sentences = new Queue<DialogueLine>();
        
        mainSpeechBubble?.continueButton.onClick.AddListener(DisplayNextSentence);
        inWorldSpeechBubble?.continueButton.onClick.AddListener(DisplayNextSentence);
        
        HideAllDialogueBoxes();
    }

    public void StartDialogue(Dialogue dialogue, Transform contextTransform = null)
    {
        IsDialogueActive = true; 
        IsPlayerFrozen = true; 
        
        HideAllDialogueBoxes();

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

        HideAllDialogueBoxes();

        if (sentences.Count == 0)
        {
            IsPlayerFrozen = false;
            IsDialogueActive = false;
            return;
        }

        DialogueLine line = sentences.Dequeue();

        SelectBubble(line.speaker);
        MoveBubble(line.speaker);

        typingCoroutine = StartCoroutine(TypeSentence(line));
    }
    
    private void SelectBubble(Speaker speaker)
    {
        if(speaker == Speaker.PlayerCinematic || speaker == Speaker.EntityCinematic || speaker == Speaker.Narrator)
        {
            currentSpeechBubble = mainSpeechBubble;
            inWorldCanvas.SetActive(false);
        }
        else
        {
            currentSpeechBubble = inWorldSpeechBubble;
            inWorldCanvas.SetActive(true);
        }
    }

    private void MoveBubble(Speaker speaker)
    {
        if (currentSpeechBubble == mainSpeechBubble)
        {
            // Positionnement fixe pour les bulles principales
            if (speaker == Speaker.PlayerCinematic)
            {
                currentSpeechBubble.rectTransform.position = anchorCinematicBL.position;
            }
            else if (speaker == Speaker.EntityCinematic)
            {
                currentSpeechBubble.rectTransform.position = anchorCinematicTR.position;
            }
            else if (speaker == Speaker.Narrator)
            {
                currentSpeechBubble.rectTransform.position = anchorNarratorDefault.position;
            }

        }
        else
        {
            // Positionnement dynamique pour les bulles in-world
            if (speaker == Speaker.Player || speaker == Speaker.PlayerThought)
            {
                inWorldCanvas.transform.SetParent(playerTransform);
                inWorldCanvas.transform.localPosition = Vector2.zero;
            }
            else if (speaker == Speaker.Entity)
            {
                inWorldCanvas.transform.SetParent(entityTransform);
                inWorldCanvas.transform.localPosition = Vector2.zero;
            }

            inWorldCanvas.gameObject.SetActive(true);

        }
        
        currentSpeechBubble.gameObject.SetActive(true);

    }

    private IEnumerator TypeSentence(DialogueLine line)
    {

        if (line.speaker == Speaker.Entity ||
                 line.speaker == Speaker.PlayerCinematic ||
                 line.speaker == Speaker.EntityCinematic)
        {
            if (currentSpeechBubble.continueButton != null)
                currentSpeechBubble.continueButton.gameObject.SetActive(false);  
        }

        currentSpeechBubble.SetBubbleSprite(line.speaker);
        // 6. Effet de frappe
        currentSpeechBubble.dialogueText.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            currentSpeechBubble.dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // A. Cas avec un bouton "Continuer"
        if (line.speaker == Speaker.Player ||
            line.speaker == Speaker.PlayerThought ||
            line.speaker == Speaker.Narrator ||
            line.speaker == Speaker.PlayerTwo)
        {
            if (currentSpeechBubble.continueButton != null)
                currentSpeechBubble.continueButton.gameObject.SetActive(true);    
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
    
    private void HideAllDialogueBoxes()
    {
        mainSpeechBubble?.gameObject.SetActive(false);
        inWorldSpeechBubble?.gameObject.SetActive(false);

        inWorldCanvas.SetActive(false);
    }
}