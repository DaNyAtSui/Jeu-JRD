// 🛑 ASSUREZ-VOUS QUE LES LIGNES "enum Speaker" et "class DialogueLine" 
// NE SONT PAS PRÉSENTES ICI 🛑

using System.Collections;
using System.Collections.Generic; // Nécessaire pour Queue
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // === REFERENCES DE L'INTERFACE (UI) ===
    [Header("UI Elements (Player)")]
    [SerializeField] private GameObject playerDialogueBox;
    [SerializeField] private TextMeshProUGUI playerDialogueText;
    [SerializeField] private Button playerContinueButton;

    [Header("UI Elements (Entity)")]
    [SerializeField] private GameObject entityDialogueBox;
    [SerializeField] private TextMeshProUGUI entityDialogueText;
    [SerializeField] private Button entityContinueButton;

    [Header("Dialogue Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float entityDisplayTime = 5.0f;

    // === NOUVELLE LOGIQUE ===
    // Notez qu'il n'y a PAS de [SerializeField] currentConversation
    private Queue<DialogueLine> sentences;

    // === VARIABLES PRIVEES (Etat) ===
    private Coroutine typingCoroutine;

    void Start()
    {
        if (playerContinueButton)
            playerContinueButton.onClick.AddListener(DisplayNextSentence);

        // On prépare la file d'attente
        sentences = new Queue<DialogueLine>();

        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);
    }

    // C'est la nouvelle fonction publique pour démarrer un dialogue
    public void StartDialogue(Dialogue dialogue)
    {
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);

        sentences.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        playerContinueButton.gameObject.SetActive(false);
        entityContinueButton.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            playerDialogueBox.SetActive(false);
            entityDialogueBox.SetActive(false);
            return;
        }

        DialogueLine line = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(line));
    }

    // Cette coroutine n'a PAS changé
    private IEnumerator TypeSentence(DialogueLine line)
    {
        TextMeshProUGUI textComponent = null;
        GameObject dialogueBox = null;
        Button continueButton = null;

        switch (line.speaker)
        {
            case Speaker.Player:
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                playerDialogueBox.SetActive(true);
                entityDialogueBox.SetActive(false);
                break;

            case Speaker.Entity:
                textComponent = entityDialogueText;
                dialogueBox = entityDialogueBox;
                continueButton = entityContinueButton;
                playerDialogueBox.SetActive(false);
                entityDialogueBox.SetActive(true);
                break;

            case Speaker.Narrator:
                textComponent = playerDialogueText;
                dialogueBox = playerDialogueBox;
                continueButton = playerContinueButton;
                playerDialogueBox.SetActive(true);
                entityDialogueBox.SetActive(false);
                break;
        }

        textComponent.text = "";
        foreach (char letter in line.sentence.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (line.speaker == Speaker.Player || line.speaker == Speaker.Narrator)
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