using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(BoxCollider2D))]
public class ClockTrigger_Vibrate : MonoBehaviour
{
    [Header("Horloge Audio")]
    public AudioClip gongClip;                     // Ton son de gong
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.05f;
    public bool playOnce = true;                   // Joue une seule fois

    [Header("Vibration Caméra")]
    public Camera mainCamera;                      // Caméra principale (auto si vide)
    public float shakeDuration = 0.3f;             // Durée du tremblement
    public float shakeMagnitude = 0.15f;           // Intensité du tremblement

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (mainCamera == null)
            mainCamera = Camera.main;

        var col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && hasPlayed) return;

        PlayGong();
    }

    private void PlayGong()
    {
        hasPlayed = true;

        if (audioSource != null && gongClip != null)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.PlayOneShot(gongClip);
        }

        if (mainCamera != null)
            StartCoroutine(ShakeCamera());
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
}
