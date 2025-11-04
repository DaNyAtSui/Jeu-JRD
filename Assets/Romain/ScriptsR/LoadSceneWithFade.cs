using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadSceneWithFade : MonoBehaviour
{
    [Header("Nom de la scène à charger")]
    public string sceneName;

    [Header("Temps avant le chargement (secondes)")]
    public float delay = 3f;

    [Header("Durée du fondu (secondes)")]
    public float fadeDuration = 1f;

    [Header("Lancer automatiquement au Start ?")]
    public bool autoStart = false;

    [Header("Référence vers une Image noire (UI)")]
    public Image fadeImage;

    private bool isLoading = false;

    void Start()
    {
        if (autoStart)
            StartCoroutine(LoadSceneAfterDelay());
    }

    public void StartLoading()
    {
        if (!isLoading)
            StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        isLoading = true;

        // Attendre avant le fade (si delay > 0)
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        // Lancer le fondu noir
        if (fadeImage != null)
            yield return StartCoroutine(FadeToBlack());

        // Charger la scène
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
    }
}
