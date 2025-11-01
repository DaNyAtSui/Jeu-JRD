using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ActivateOnInteract : MonoBehaviour
{
    [Header("Objet à activer")]
    public GameObject targetObject;

    [Header("Texte d'interaction (TMP)")]
    public TMP_Text interactionText;
    [Tooltip("Texte affiché quand le joueur est dans la zone")]
    public string message = "Interagir";

    [Header("Paramètres")]
    [Tooltip("True = se détruit après activation")]
    public bool destroyAfterActivation = false;

    private bool isPlayerInZone = false;
    private PlayerInput playerInput;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            // Récupère le PlayerInput du joueur
            playerInput = other.GetComponent<PlayerInput>();

            if (playerInput != null)
                playerInput.actions["Interact"].performed += OnInteract;

            if (interactionText != null)
            {
                interactionText.text = message;
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (playerInput != null)
                playerInput.actions["Interact"].performed -= OnInteract;

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isPlayerInZone) return;

        if (targetObject != null)
            targetObject.SetActive(true);

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        if (destroyAfterActivation)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Interact"].performed -= OnInteract;
    }
}
