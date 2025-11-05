using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueAdvanceInput : MonoBehaviour
{
    [Header("Référence au DialogueManager")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Nom de l'action Input")]
    [Tooltip("Nom exact de l'action dans ton Input Action Map (ex: 'Submit' ou 'Interact')")]
    [SerializeField] private string actionName = "Submit";

    private InputAction continueAction;
    private PlayerInput playerInput;

    void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();

        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            continueAction = playerInput.actions[actionName];
        }
        else
        {
            Debug.LogWarning("DialogueAdvanceInput : Aucun PlayerInput trouvé ou pas d'action correspondante.");
        }
    }

    void OnEnable()
    {
        if (continueAction != null)
            continueAction.performed += OnContinuePressed;
    }

    void OnDisable()
    {
        if (continueAction != null)
            continueAction.performed -= OnContinuePressed;
    }

    private void OnContinuePressed(InputAction.CallbackContext ctx)
    {
        if (DialogueManager.IsDialogueActive && dialogueManager != null)
        {
            dialogueManager.DisplayNextSentence();
        }
    }
}
