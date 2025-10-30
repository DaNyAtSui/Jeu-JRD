using UnityEngine;
using TMPro;

public class InteractLight : MonoBehaviour
{
    [Header("Références")]
    public GameObject lightObject;      // L'objet qu'on veut activer/désactiver
    public TMP_Text interactionText;    // Le texte "Appuyez sur Interagir"

    private bool playerInZone = false;
    private bool isLightOn = false;

    void Start()
    {
        // Cache le texte au début
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        // Eteint la lumière au début
        if (lightObject != null)
            lightObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            // On dit au joueur "tu peux interagir avec MOI"
            PlayerInteractor p = other.GetComponent<PlayerInteractor>();
            if (p != null)
                p.currentInteractZone = this;

            // Affiche le texte d'interaction
            if (interactionText != null)
                interactionText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            // On enlève la zone active du joueur s'il sort
            PlayerInteractor p = other.GetComponent<PlayerInteractor>();
            if (p != null && p.currentInteractZone == this)
                p.currentInteractZone = null;

            // Cache le texte
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    // Doit être publique pour que PlayerInteractor puisse l'appeler
    public void OnInteract()
    {
        if (!playerInZone) return;

        // On inverse l'état
        isLightOn = !isLightOn;

        // On applique au GameObject de lumière
        if (lightObject != null)
            lightObject.SetActive(isLightOn);
    }
}