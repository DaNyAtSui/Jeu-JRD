using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HorrorCredits : MonoBehaviour
{
    [System.Serializable]
    public class CreditElement
    {
        [Tooltip("Texte à afficher (laisser vide si image)")]
        [TextArea(2, 4)] public string textLine;

        [Tooltip("Image à afficher (laisser vide si texte)")]
        public Sprite imageSprite;

        [Tooltip("Durée d’affichage de cet élément")]
        public float displayDuration = 2.5f;
    }

    [Header("🎬 Références UI")]
    public CanvasGroup textCanvasGroup;
    public TextMeshProUGUI creditText;

    public CanvasGroup imageCanvasGroup;
    public Image creditImage;

    [Header("🩸 Liste des crédits (texte ou image)")]
    public CreditElement[] credits;

    [Header("⏱️ Timings globaux")]
    public float fadeInDuration = 1.2f;
    public float fadeOutDuration = 1.2f;
    public float startDelay = 1f;

    [Header("🎞️ Option de fin")]
    public string nextSceneName = "MainMenu";
    public float endDelay = 1f;

    private void Start()
    {
        StartCoroutine(PlayCredits());
    }

    IEnumerator PlayCredits()
    {
        yield return new WaitForSeconds(startDelay);

        textCanvasGroup.alpha = 0f;
        imageCanvasGroup.alpha = 0f;
        creditText.text = "";
        creditImage.gameObject.SetActive(false);

        foreach (var item in credits)
        {
            // Si c’est du texte
            if (!string.IsNullOrEmpty(item.textLine))
            {
                creditText.text = item.textLine;
                creditImage.gameObject.SetActive(false);

                yield return StartCoroutine(FadeCanvas(textCanvasGroup, 1f, fadeInDuration));
                yield return new WaitForSeconds(item.displayDuration);
                yield return StartCoroutine(FadeCanvas(textCanvasGroup, 0f, fadeOutDuration));
            }
            // Si c’est une image
            else if (item.imageSprite != null)
            {
                creditImage.sprite = item.imageSprite;
                creditImage.preserveAspect = true;
                creditImage.gameObject.SetActive(true);

                yield return StartCoroutine(FadeCanvas(imageCanvasGroup, 1f, fadeInDuration));
                yield return new WaitForSeconds(item.displayDuration);
                yield return StartCoroutine(FadeCanvas(imageCanvasGroup, 0f, fadeOutDuration));
                creditImage.gameObject.SetActive(false);
            }
        }

        yield return new WaitForSeconds(endDelay);
        if (!string.IsNullOrEmpty(nextSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        cg.alpha = target;
    }
}
