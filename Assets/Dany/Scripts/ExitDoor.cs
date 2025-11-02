using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ExitDoor : MonoBehaviour
{
    [Header("Paramètres de la porte finale")]
    public Transform loopRespawnPoint;     // point de réapparition si boucle temporelle
    public string nextSceneName;           // nom de la scène suivante si tout est validé

    [Header("Références liées")]
    public Animator doorAnimator;          // Animator de la porte
    public SoundDoor soundDoor;            // Script SoundDoor pour jouer les sons
    public AudioSource extraAudio;         // optionnel : autre AudioSource pour sons d'ambiance

    [Header("Audio secondaire")]
    public AudioClip deniedClip;           // son si énigmes incomplètes (porte bloquée)
    public AudioClip unlockClip;           // son de déverrouillage (facultatif)

    [Header("Effets visuels (optionnels)")]
    public CanvasGroup fadeCanvas;         // fade blanc ou noir
    public float fadeDuration = 1.2f;

    private bool playerInZone = false;
    private Transform player;
    private bool isOpening = false;

    // 🧠 nouvelle référence au PlayerInput pour capter l’action "Interact"
    private PlayerInput playerInput;

    private void Start()
    {
        // récupère le PlayerInput du joueur dans la scène
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
            Debug.LogWarning("⚠️ Aucun PlayerInput trouvé dans la scène — la touche Interact ne fonctionnera pas.");
    }

    private void Update()
    {
        if (!playerInZone || playerInput == null) return;

        var interact = playerInput.actions["Interact"];

        if (interact != null && interact.WasPressedThisFrame())
        {
            Debug.Log("🎯 Action Interact détectée par ExitDoor !");
            TryExit();
        }
    }

    private void TryExit()
    {
        if (isOpening) return; // éviter de spammer la touche

        if (!GameManager.Instance.AllPuzzlesCompleted())
        {
            Debug.Log("🚫 Les énigmes ne sont pas encore terminées !");
            StartCoroutine(PlayLockedDoorFeedback());
        }
        else
        {
            Debug.Log("✅ Toutes les énigmes sont terminées ! Ouverture de la porte finale...");
            StartCoroutine(OpenFinalDoorSequence());
        }
    }

    private IEnumerator PlayLockedDoorFeedback()
    {
        // animation d’essai d’ouverture refusée
        if (doorAnimator != null)
            doorAnimator.SetTrigger("TryOpen");

        // son de porte verrouillée
        if (extraAudio && deniedClip)
            extraAudio.PlayOneShot(deniedClip);

        yield return new WaitForSeconds(1f);

        // 🌀 Boucle temporelle → replacer le joueur
        StartCoroutine(TimeLoopReset());
    }

    private IEnumerator OpenFinalDoorSequence()
    {
        isOpening = true;

        // animation + son de déverrouillage
        if (unlockClip && extraAudio)
            extraAudio.PlayOneShot(unlockClip);

        yield return new WaitForSeconds(0.3f);

        // animation d’ouverture
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        // son de porte via SoundDoor
        if (soundDoor != null)
            soundDoor.PlayDoorOpenSFX();

        yield return new WaitForSeconds(1.5f);

        // transition vers la scène suivante
        if (fadeCanvas != null)
            yield return StartCoroutine(FadeScreen(1));

        yield return new WaitForSeconds(0.5f);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log("🌅 Passage à la scène suivante !");
        }

        isOpening = false;
    }

    private IEnumerator TimeLoopReset()
    {
        // 🕳️ 1. Fade IN (noir complet)
        if (fadeCanvas != null)
            yield return StartCoroutine(FadeScreen(1));

        // 🕰️ 2. petite pause pour bien couvrir la transition visuelle
        yield return new WaitForSeconds(0.4f);

        // 🌀 3. Téléportation du joueur
        if (player != null && loopRespawnPoint != null)
        {
            player.position = loopRespawnPoint.position;
            Debug.Log("🔁 Boucle temporelle : retour au point de départ !");
        }

        // 🧩 4. Pause supplémentaire pour laisser la caméra se recentrer
        yield return new WaitForSeconds(0.6f);

        // 🌅 5. Fade OUT (retour à la lumière)
        if (fadeCanvas != null)
            yield return StartCoroutine(FadeScreen(0));
    }

    private IEnumerator FadeScreen(float targetAlpha)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            player = other.transform;
            Debug.Log("👉 Appuyez sur E pour interagir avec la porte.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            player = null;
        }
    }
}
