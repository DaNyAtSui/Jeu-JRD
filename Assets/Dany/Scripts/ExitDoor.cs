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
    public Collider2D doorCollider;        // ✅ Collider physique à désactiver lors de l'ouverture

    [Header("Audio secondaire")]
    public AudioClip deniedClip;           // son si énigmes incomplètes (porte bloquée)
    public AudioClip unlockClip;           // son de déverrouillage (facultatif)

    [Header("Effets visuels (optionnels)")]
    public CanvasGroup fadeCanvas;         // fade blanc ou noir
    public float fadeDuration = 1.2f;

    private bool playerInZone = false;
    private Transform player;
    private bool isOpening = false;

    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
            Debug.LogWarning("⚠️ Aucun PlayerInput trouvé dans la scène — la touche Interact ne fonctionnera pas.");

        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider2D>();
            if (doorCollider == null)
                Debug.LogWarning("⚠️ Aucun Collider2D trouvé sur la porte !");
        }
    }

    private void Update()
    {
        if (!playerInZone || playerInput == null) return;

        var interact = playerInput.actions["Interact"];
        if (interact != null && interact.WasPressedThisFrame())
        {
            TryExit();
        }
    }

    private void TryExit()
    {
        if (isOpening) return;

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
        if (doorAnimator != null)
            doorAnimator.SetTrigger("TryOpen");

        if (extraAudio && deniedClip)
            extraAudio.PlayOneShot(deniedClip);

        yield return new WaitForSeconds(1f);
        StartCoroutine(TimeLoopReset());
    }

    private IEnumerator OpenFinalDoorSequence()
    {
        isOpening = true;

        // 🔓 Animation + son de déverrouillage
        if (unlockClip && extraAudio)
            extraAudio.PlayOneShot(unlockClip);

        yield return new WaitForSeconds(0.3f);

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (soundDoor != null)
            soundDoor.PlayDoorOpenSFX();

        // ⏳ Laisse l'anim se jouer avant de désactiver le collider
        yield return new WaitForSeconds(1f);

        // ✅ Désactivation du collider (le joueur peut sortir)
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
            Debug.Log("🟩 Collider de la porte désactivé — passage autorisé.");
        }

        // Transition (facultative selon ton design)
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
        if (fadeCanvas != null)
            yield return StartCoroutine(FadeScreen(1));

        yield return new WaitForSeconds(0.4f);

        if (player != null && loopRespawnPoint != null)
        {
            player.position = loopRespawnPoint.position;
            Debug.Log("🔁 Boucle temporelle : retour au point de départ !");
        }

        yield return new WaitForSeconds(0.6f);

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
