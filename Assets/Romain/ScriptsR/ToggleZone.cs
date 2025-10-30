using UnityEngine;
using TMPro;

public class ToggleZone2D : MonoBehaviour
{
    [Header("Objet à activer/désactiver")]
    public GameObject targetObject;

    [Header("Texte d'interaction (TMP)")]
    public TMP_Text interactionText;
    [Tooltip("Texte affiché quand le joueur est dans la zone")]
    public string message = "Appuyez sur E pour interagir";

    private bool isActive = false;

    void Start()
    {
        // Si aucun objet n’est assigné, on prend le premier enfant
        if (targetObject == null && transform.childCount > 0)
            targetObject = transform.GetChild(0).gameObject;

        if (targetObject != null)
            targetObject.SetActive(isActive);

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    public void ShowText(bool show)
    {
        if (interactionText != null)
        {
            interactionText.text = show ? message : "";
            interactionText.gameObject.SetActive(show);
        }
    }

    public void ToggleObject()
    {
        if (targetObject == null) return;

        isActive = !isActive;
        targetObject.SetActive(isActive);
        Debug.Log($"[ToggleZone2D] {targetObject.name} -> {isActive}");
    }

    // Ces deux méthodes seront appelées depuis le player (par collision 2D)
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
