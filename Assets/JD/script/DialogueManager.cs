using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// --- Les définitions n'ont pas changé ---
public enum Speaker
{
    Player,
    Entity,
    Narrator
}

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    [TextArea(3, 5)]
    public string sentence;
}
// -----------------------------------------


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
    [Tooltip("Temps (en secondes) pendant lequel la bulle de l'entité reste visible AVANT de passer à la suite.")]
    [SerializeField] private float entityDisplayTime = 5.0f;


    // ====================================================================
    // <-- CHANGEMENT ICI : LA LIGNE MANQUANTE A ÉTÉ RAJOUTÉE
    // ====================================================================
    [Header("Dialogue Content")]
    [SerializeField] private DialogueLine[] currentConversation;
    // ====================================================================


    // === VARIABLES PRIVEES (Etat) ===
    private int lineIndex;
    private Coroutine typingCoroutine;

    void Start()
    {
        // On attache UNIQUEMENT le bouton du joueur
        if (playerContinueButton)
            playerContinueButton.onClick.AddListener(DisplayNextSentence);

        // On cache tout au démarrage
        playerDialogueBox.SetActive(false);
        entityDialogueBox.SetActive(false);

        StartDialogue();
    }

    public void StartDialogue()
    {
        lineIndex = 0;
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

        // Ligne 85 (ou proche) - elle fonctionnera maintenant
        if (lineIndex >= currentConversation.Length)
        {
            playerDialogueBox.SetActive(false);
            entityDialogueBox.SetActive(false);
            return;
        }

        // Cette ligne fonctionnera aussi
        DialogueLine line = currentConversation[lineIndex];
        typingCoroutine = StartCoroutine(TypeSentence(line));
        lineIndex++;
    }

    // Coroutine (n'a pas changé)
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