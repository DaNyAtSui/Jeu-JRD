using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NoteInteraction : MonoBehaviour
{
    [Header("Texte de la note")]
    [TextArea(3, 8)]
    public string noteContent = "Une lettre posée sur la table...\n\n\"Ils m’observent encore, même après la mort.\"";

    [Header("UI de la lettre (TMP)")]
    public GameObject letterUIPanel;       // Grande lettre à l'écran
    public TMP_Text letterTMPText;         // Texte TMP de la lettre

    [Header("UI d'interaction")]
    public GameObject promptUIPanel;       // Panel ou texte en bas de l'écran
    public TMP_Text promptTMPText;         // Texte type "[E] Lire la lettre"
    [Tooltip("Texte affiché quand le joueur est proche")]
    public string promptMessage = "[E] Lire la lettre";

    [Header("Audio (optionnel)")]
    public AudioSource paperSound;         // Petit son papier

    [Header("Input System")]
    public string interactActionName = "Interact"; // Action du Input System (sinon fallback clavier/manette)

    private bool playerInRange = false;
    private bool isReading = false;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        // Auto-récupère le PlayerInput s'il existe dans la scène
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
        {
            try
            {
                interactAction = playerInput.actions[interactActionName];
            }
            catch
            {
                Debug.LogWarning("⚠️ Aucun InputAction nommé '" + interactActionName + "' trouvé. Utilisation du clavier/manette par défaut.");
            }
        }

        // Cache le texte d’interaction au démarrage
        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        // Affiche le texte d’interaction uniquement si on ne lit pas déjà la lettre
        if (!isReading && promptUIPanel != null)
        {
            promptUIPanel.SetActive(true);
            if (promptTMPText != null)
                promptTMPText.text = promptMessage;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        // Cache le texte quand le joueur quitte la zone
        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);

        // Ferme la lettre s’il la lisait encore
        if (isReading)
            CloseLetter();
    }

    private void Update()
    {
        if (!playerInRange) return;

        bool pressed = false;

        // Input System action
        if (interactAction != null)
            pressed = interactAction.WasPressedThisFrame();

        // Fallback clavier / manette
        else
        {
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
            if (Gamepad.current != null)
                pressed |= Gamepad.current.buttonSouth.wasPressedThisFrame; // A / X
        }

        if (pressed)
        {
            if (!isReading) OpenLetter();
            else CloseLetter();
        }
    }

    void OpenLetter()
    {
        isReading = true;

        // Affiche la lettre
        if (letterUIPanel != null)
            letterUIPanel.SetActive(true);
        if (letterTMPText != null)
            letterTMPText.text = noteContent;

        if (paperSound != null)
            paperSound.Play();

        // Cache le texte d’interaction pendant la lecture
        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);

        // Met le jeu en pause
        Time.timeScale = 0f;
    }

    void CloseLetter()
    {
        isReading = false;

        // Ferme la lettre
        if (letterUIPanel != null)
            letterUIPanel.SetActive(false);

        if (paperSound != null)
            paperSound.Play();

        // Si le joueur est toujours proche → on réaffiche le texte d’interaction
        if (playerInRange && promptUIPanel != null)
        {
            promptUIPanel.SetActive(true);
            if (promptTMPText != null)
                promptTMPText.text = promptMessage;
        }

        // Reprend le temps
        Time.timeScale = 1f;
    }
}
