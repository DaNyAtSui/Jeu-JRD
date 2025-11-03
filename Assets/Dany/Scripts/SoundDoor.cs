using UnityEngine;

public class SoundDoor : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip openSFX;
    public AudioClip closeSFX;

    public void PlayDoorOpenSFX()
    {
        if (sfxSource != null && openSFX != null)
            sfxSource.PlayOneShot(openSFX);
    }

    public void PlayDoorCloseSFX()
    {
        if (sfxSource != null && closeSFX != null)
            sfxSource.PlayOneShot(closeSFX);
    }
}
