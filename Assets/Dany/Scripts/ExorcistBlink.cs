using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class ExorcistBlink : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Image UI (Image) contenant le sprite 'exorciste' — laisser désactivée si tu veux")]
    public Image exorcistImage;

    [Header("Blink settings")]
    [Tooltip("Nombre de clignotements (on/off counts as 1 blink)")]
    public int blinkCount = 6;
    [Tooltip("Durée visible/invisible pour chaque moitié du clignot (sec)")]
    public float blinkHalfPeriod = 0.15f;

    [Header("Final fade")]
    [Tooltip("Durée du fade out final (sec)")]
    public float finalFadeDuration = 0.5f;

    [Header("Optional SFX")]
    public AudioSource sfxSource;
    public AudioClip flashSfx;

    // internal
    private CanvasGroup canvasGroup;
    private Coroutine running;

    void Awake()
    {
        if (exorcistImage == null)
        {
            Debug.LogWarning("[ExorcistBlink] Aucune Image assignée. Désactivation du composant.", this);
            enabled = false;
            return;
        }

        // Assure qu'on a un CanvasGroup pour le fade final
        canvasGroup = exorcistImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = exorcistImage.gameObject.AddComponent<CanvasGroup>();

        // Démarre caché
        canvasGroup.alpha = 0f;
        exorcistImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Lance la séquence de clignotement + disparition.
    /// Safe à appeler plusieurs fois (si déjà en cours il ignore).
    /// </summary>
    public void TriggerBlink()
    {
        if (!enabled) return;
        if (running != null) return;
        running = StartCoroutine(BlinkSequence());
    }

    private IEnumerator BlinkSequence()
    {
        exorcistImage.gameObject.SetActive(true);
        // s'assurer visible pour commencer
        canvasGroup.alpha = 1f;
        // petite attente pour être sûr que le UI a été activé (frame safety)
        yield return null;

        for (int i = 0; i < blinkCount; i++)
        {
            // toggle off
            canvasGroup.alpha = 0f;
            if (sfxSource != null && flashSfx != null) sfxSource.PlayOneShot(flashSfx);
            yield return new WaitForSeconds(blinkHalfPeriod);

            // toggle on
            canvasGroup.alpha = 1f;
            if (sfxSource != null && flashSfx != null) sfxSource.PlayOneShot(flashSfx);
            yield return new WaitForSeconds(blinkHalfPeriod);
        }

        // final quick flash then fade out
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.06f);

        // fade out smoothly
        float t = 0f;
        float start = canvasGroup.alpha;
        while (t < finalFadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, Mathf.Clamp01(t / finalFadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
        exorcistImage.gameObject.SetActive(false);

        running = null;
    }

    // Optionnel : pour debug dans l'éditeur
    #if UNITY_EDITOR
    [ContextMenu("Trigger Blink (Editor)")]
    private void EditorTrigger() => TriggerBlink();
    #endif
}
