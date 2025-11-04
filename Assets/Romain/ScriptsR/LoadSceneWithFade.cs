using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadSceneWithFade : MonoBehaviour
{
    [Header("Nom de la scène à charger")]
    public string sceneName;

    [Header("Temps d'attente une fois l'écran noir (secondes)")]
    public float delayAfterBlack = 1f;

    [Header("Durée du fondu vers noir (secondes)")]
    public float fadeDuration = 1f;

    [Header("Lancer automatiquement au Start ?")]
    public bool autoStart = false;

    [Header("Image UI noire plein écran")]
    public Image fadeImage;

    private bool isLoading = false;

    void Start()
    {
        if (autoStart)
            StartCoroutine(FadeThenLoad());
    }

    public void StartLoading()
    {
        if (!isLoading)
            StartCoroutine(FadeThenLoad());
    }

    private IEnumerator FadeThenLoad()
    {
        isLoading = true;

        // 1. On fait le fade vers noir
        if (fadeImage != null)
            yield return StartCoroutine(FadeToBlack());
        else
            Debug.LogWarning("LoadSceneWithFade: aucune fadeImage assignée.");

        // 2. On attend en noir
        if (delayAfterBlack > 0)
            yield return new WaitForSeconds(delayAfterBlack);

        // 3. On charge la scène
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // On force le full black
        c.a = 1f;
        fadeImage.color = c;
    }
}
