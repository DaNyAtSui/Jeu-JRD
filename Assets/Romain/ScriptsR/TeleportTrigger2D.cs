using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportTrigger2D : MonoBehaviour
{
    [Header("Références")]
    public Transform destinationPoint;    // Point B
    public Image fadeImage;               // Image UI noire plein écran

    [Header("Durée du fondu")]
    public float fadeDuration = 1f;

    private bool isTeleporting = false;

    private void Start()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // transparent au début
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            StartCoroutine(FadeTeleport(other.transform));
        }
    }

    private IEnumerator FadeTeleport(Transform player)
    {
        isTeleporting = true;

        // Fade In (noir complet)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Téléporte le joueur
        player.position = destinationPoint.position;

        // Petit délai pour plus de naturel
        yield return new WaitForSeconds(0.1f);

        // Fade Out (retour à la vision)
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        isTeleporting = false;
    }
}
