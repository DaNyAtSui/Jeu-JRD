using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class PlayerStepSpot : MonoBehaviour
{
    [Header("Références")]
    public Light2D spotLight;
    public AudioSource audioSource;
    public AudioClip beepValid;
    public AudioClip beepReset;

    [Header("Couleurs")]
    public Color colorInactive = Color.red;
    public Color colorActive = Color.green;

    [Header("Puzzle Manager")]
    public StepPuzzleManager puzzleManager;

    private bool isActive = false;

    private void Start()
    {
        if (spotLight != null)
            spotLight.color = colorInactive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isActive)
        {
            puzzleManager.TryActivateSpot(this);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (spotLight != null)
            spotLight.color = active ? colorActive : colorInactive;
    }

    public void PlayBeep(bool success)
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(success ? beepValid : beepReset);
    }
}
