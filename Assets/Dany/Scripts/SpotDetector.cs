using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SpotDetector : MonoBehaviour
{
    [Header("Références")]
    public Light2D spotLight;   // drag ta light ici dans l’inspecteur
    public AudioSource audioSource; // source du son
    public AudioClip beepValid;     // son quand vert
    public AudioClip beepCancel;    // (optionnel) son quand rouge

    [Header("Couleurs")]
    public Color colorEmpty = Color.red;
    public Color colorFilled = Color.green;

    [Header("État")]
    public bool isOccupied = false;

    private void Start()
    {
        if (spotLight != null)
            spotLight.color = colorEmpty;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PushBlock"))
        {
            isOccupied = true;
            UpdateLightColor();
            PlayBeep(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PushBlock"))
        {
            isOccupied = false;
            UpdateLightColor();
            PlayBeep(false);
        }
    }

    private void UpdateLightColor()
    {
        if (spotLight == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeLightColor(isOccupied ? colorFilled : colorEmpty));
    }

    private IEnumerator FadeLightColor(Color target)
    {
        Color start = spotLight.color;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f; // vitesse du fade
            spotLight.color = Color.Lerp(start, target, t);
            yield return null;
        }
    }

    private void PlayBeep(bool success)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = success ? beepValid : beepCancel;
        if (clipToPlay != null)
            audioSource.PlayOneShot(clipToPlay);
    }
}
