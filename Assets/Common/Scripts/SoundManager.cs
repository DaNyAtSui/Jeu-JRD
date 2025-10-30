using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Ambience")]
    public AudioSource rainSource;
    public AudioSource ambientSource;

    [Header("Voice & SFX")]
    public AudioSource voiceSource;
    public AudioSource sfxSource;

    public AudioClip introVoice;
    public AudioClip doorSound;

    private void Start()
    {
        // Lancer ambiance
        if (rainSource != null) rainSource.Play();
        if (ambientSource != null) ambientSource.Play();

        // Jouer la voix d'intro + ensuite la porte
        if (introVoice != null)
        {
            voiceSource.clip = introVoice;
            voiceSource.Play();
            Invoke(nameof(PlayDoorSound), introVoice.length);
        }
        else
        {
            PlayDoorSound();
        }
    }

    private void PlayDoorSound()
    {
        if (doorSound != null)
        {
            sfxSource.PlayOneShot(doorSound);
        }
    }
}
