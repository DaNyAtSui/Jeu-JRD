using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NoteInteraction : MonoBehaviour
{
    [Header("Texte de la note")]
    [TextArea(3, 8)]
    public string noteContent = "Une lettre posée sur la table...\n\n\"Ils m’observent encore, même après la mort.\"";

    [Header("UI de la lettre (TMP)")]
    public GameObject letterUIPanel;
    public TMP_Text letterTMPText;

    [Header("UI d'interaction")]
    public GameObject promptUIPanel;
    public TMP_Text promptTMPText;
    public string promptMessage = "[E] Lire la lettre";

    [Header("Audio (optionnel)")]
    public AudioSource paperSound;

    [Header("Input System")]
    public string interactActionName = "Interact";

    private bool playerInRange = false;
    private bool isReading = false;
    private bool hasPlayedSound = false; // ✅ Nouveau : empêche de rejouer le son

    private PlayerInput playerInput;
    private InputAction interactAction;
    private PlayerController playerController;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        playerController = FindFirstObjectByType<PlayerController>();

        if (playerInput != null)
        {
            try { interactAction = playerInput.actions[interactActionName]; }
            catch { Debug.LogWarning($"⚠️ Aucun InputAction nommé '{interactActionName}' trouvé."); }
        }

        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

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

        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);

        if (isReading)
            CloseLetter(force: true);
    }

    private void Update()
    {
        bool pressed = false;

        if (interactAction != null)
            pressed = interactAction.WasPressedThisFrame();
        else
        {
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
            if (Gamepad.current != null)
                pressed |= Gamepad.current.buttonSouth.wasPressedThisFrame;
        }

        if (!pressed) return;

        if (playerInRange && !isReading)
            OpenLetter();
        else if (isReading)
            CloseLetter();
    }

    void OpenLetter()
    {
        isReading = true;

        // ✅ Désactiver le prompt manuellement
        if (promptUIPanel != null)
            promptUIPanel.SetActive(false);

        if (letterUIPanel != null)
            letterUIPanel.SetActive(true);
        if (letterTMPText != null)
            letterTMPText.text = noteContent;

        // ✅ Jouer le son une seule fois
        if (!hasPlayedSound && paperSound != null)
        {
            paperSound.Play();
            hasPlayedSound = true;
        }

        // ✅ Bloquer le joueur sans geler le temps
        if (playerController != null)
            playerController.enabled = false;
    }

    void CloseLetter(bool force = false)
    {
        isReading = false;

        if (letterUIPanel != null)
            letterUIPanel.SetActive(false);

        // ❌ Supprimé : ne rejoue plus le son ici
        // if (paperSound != null) paperSound.Play();

        if (playerController != null)
            playerController.enabled = true;

        // ✅ Ne réaffiche le texte que si le joueur est encore dans le trigger
        if (!force && playerInRange && promptUIPanel != null)
        {
            promptUIPanel.SetActive(true);
            if (promptTMPText != null)
                promptTMPText.text = promptMessage;
        }
        else if (promptUIPanel != null)
        {
            promptUIPanel.SetActive(false);
        }
    }
}
