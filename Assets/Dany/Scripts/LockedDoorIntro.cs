using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class LockedDoorIntro : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string nextSceneName;           // Laisser vide sauf pour la dernière porte
    public bool isFinalDoor = false;       // coche ça pour la dernière porte
    public float sceneDelay = 1.5f;        // délai avant chargement de la scène suivante

    [Header("Audio Settings")]
    public AudioSource audioSource;        // AudioSource de la porte
    public AudioClip lockedClip;           // son de porte verrouillée
    public AudioClip finalDoorClip;        // son spécial (claquement final)
    public float soundCooldown = 1.2f;     // délai entre deux sons
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.05f; // variation aléatoire du pitch

    [Header("Effets visuels (optionnel)")]
    public CanvasGroup fadeCanvas;         // ✅ CanvasGroup avec image noire
    public float fadeDuration = 1.2f;      // durée du fondu

    [Header("UI Feedback (optionnel)")]
    public GameObject interactHint;        // texte "Appuyez sur E"
    public GameObject lockedHint;          // texte "C’est fermé"

    private bool playerInZone = false;
    private PlayerInput playerInput;
    private bool canInteract = true;

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (interactHint != null) interactHint.SetActive(false);
        if (lockedHint != null) lockedHint.SetActive(false);
    }

    void Update()
    {
        if (playerInZone && playerInput != null && canInteract)
        {
            if (playerInput.actions["Interact"].WasPressedThisFrame())
            {
                StartCoroutine(HandleInteraction());
            }
        }
    }

    private IEnumerator HandleInteraction()
    {
        canInteract = false;
        InteractWithDoor();
        yield return new WaitForSeconds(soundCooldown);
        canInteract = true;
    }

    void InteractWithDoor()
    {
        if (audioSource != null)
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

        if (isFinalDoor)
        {
            // Dernière porte → son + fade + chargement de la scène
            StartCoroutine(FinalDoorSequence());
        }
        else
        {
            // Porte classique → son + texte "fermée"
            if (audioSource && lockedClip)
                audioSource.PlayOneShot(lockedClip);

            if (lockedHint != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShowLockedHint());
            }

            Debug.Log("🔒 Porte verrouillée : impossible de l’ouvrir.");
        }
    }

    private IEnumerator FinalDoorSequence()
    {
        Debug.Log("🚪 Dernière porte : transition vers la scène suivante...");

        // Son de porte spéciale
        if (audioSource && finalDoorClip)
            audioSource.PlayOneShot(finalDoorClip);
        else if (audioSource && lockedClip)
            audioSource.PlayOneShot(lockedClip);

        // 🔥 Petit délai avant le fade
        yield return new WaitForSeconds(0.3f);

        // 🔳 Fade vers le noir
        if (fadeCanvas != null)
            yield return StartCoroutine(FadeScreen(1));

        // 🕰️ Pause pendant le noir
        yield return new WaitForSeconds(sceneDelay);

        // 🔁 Chargement de la scène suivante
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log("🌑 Passage à la scène suivante !");
        }
        else
        {
            Debug.LogWarning("⚠️ nextSceneName non défini !");
        }
    }

    IEnumerator FadeScreen(float targetAlpha)
    {
        if (fadeCanvas == null) yield break;

        float startAlpha = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }

    IEnumerator ShowLockedHint()
    {
        lockedHint.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        lockedHint.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (interactHint != null) interactHint.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (interactHint != null) interactHint.SetActive(false);
        }
    }
}
