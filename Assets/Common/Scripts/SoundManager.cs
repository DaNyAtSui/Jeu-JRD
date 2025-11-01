using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Ambience Sources")]
    public AudioSource rainSource;
    public AudioSource ambientSource;

    [Header("Voice & SFX Sources")]
    public AudioSource voiceSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip knockClip;
    public AudioClip introVoice;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    [Header("Timing Controls")]
    [Tooltip("Pause avant le knock")]
    public float delayBeforeKnock = 1f;

    [Tooltip("Temps entre le knock et la voix")]
    public float delayAfterKnock = 1f;

    [Tooltip("Temps après le début de la voix avant que la porte s'ouvre")]
    public float delayVoiceToDoorOpen = 0.5f;

    [Tooltip("Temps entre l'ouverture de la porte et sa fermeture")]
    public float delayDoorOpenToClose = 0.6f;

    private void Start()
    {
        if (rainSource != null) rainSource.Play();
        if (ambientSource != null) ambientSource.Play();

        Invoke(nameof(PlayKnock), delayBeforeKnock);
    }

    private void PlayKnock()
    {
        if (knockClip != null)
            sfxSource.PlayOneShot(knockClip);

        // Voix après délai
        Invoke(nameof(PlayVoice), delayAfterKnock);
    }

    private void PlayVoice()
    {
        if (introVoice != null)
        {
            voiceSource.clip = introVoice;
            voiceSource.Play();
        }

        // Ouverture de porte après délai
        Invoke(nameof(PlayDoorOpen), delayVoiceToDoorOpen);
    }

    private void PlayDoorOpen()
    {
        if (doorOpenSound != null)
            sfxSource.PlayOneShot(doorOpenSound);

        // Fermeture après délai
        Invoke(nameof(PlayDoorClose), delayDoorOpenToClose);
    }

    private void PlayDoorClose()
    {
        if (doorCloseSound != null)
            sfxSource.PlayOneShot(doorCloseSound);
    }
}
