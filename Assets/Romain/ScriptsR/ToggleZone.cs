using UnityEngine;
using TMPro;

public class ToggleZone2D : MonoBehaviour
{
    [Header("Objet à activer/désactiver")]
    public GameObject targetObject;

    [Header("Texte d'interaction (TMP)")]
    public TMP_Text interactionText;
    [Tooltip("Texte affiché quand le joueur est dans la zone")]
    public string message = "E pour interagir";

    private bool isActive = false;

    void Start()
    {
        // Si aucun objet n’est assigné, on prend le premier enfant
        if (targetObject == null && transform.childCount > 0)
            targetObject = transform.GetChild(0).gameObject;

        if (targetObject != null)
            targetObject.SetActive(isActive);

        // Pré-chauffe le texte pour éviter le lag TMP
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.alpha = 0; // invisible mais actif
            interactionText.gameObject.SetActive(true); // on l’active une bonne fois pour toutes
        }
    }

    public void ShowText(bool show)
    {
        if (interactionText != null)
            interactionText.alpha = show ? 1 : 0;
    }

    public void ToggleObject()
    {
        if (targetObject == null) return;

        isActive = !isActive;
        targetObject.SetActive(isActive);
        Debug.Log($"[ToggleZone2D] {targetObject.name} -> {isActive}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ShowText(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ShowText(false);
    }
}
