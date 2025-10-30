using UnityEngine;

public class RoomFade : MonoBehaviour
{
    public SpriteRenderer darkCover;
    public float fadeDuration = 1f;

    private Coroutine currentFade;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeToAlpha(0f)); // fade-out
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeToAlpha(1f)); // fade-in
        }
    }

    System.Collections.IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color color = darkCover.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            darkCover.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        darkCover.color = color;
    }
}
