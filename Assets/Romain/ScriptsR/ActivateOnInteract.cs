using UnityEngine;
using TMPro;

public class ActivateOnInteract : MonoBehaviour
{
    [Header("Objet à activer")]
    public GameObject targetObject;

    [Header("Texte d'interaction (TMP)")]
    public TMP_Text interactionText;
    [Tooltip("Texte affiché quand le joueur est dans la zone")]
    public string message = "E pour interagir";

    [Header("Paramètres")]
    [Tooltip("True = se détruit après activation")]
    public bool destroyAfterActivation = false;

    private bool isPlayerInZone = false;

    void Start()
    {
        // Cache le texte au départ
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (targetObject != null)
                targetObject.SetActive(true);

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);

            if (destroyAfterActivation)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

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

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}
