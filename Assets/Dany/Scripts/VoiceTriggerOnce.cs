using UnityEngine;
using System.Collections;

public class VoiceTriggerOnce : MonoBehaviour
{
    [Header("Voice Settings")]
    public AudioSource voiceSource;
    public AudioClip voiceClip;

    [Header("Door Link (une des deux suffit)")]
    public DoorController_U1 doorU1;            // première porte du jeu (script narratif)
    public DoorController_Timed doorTimed;      // deuxième salle (voix + puzzle)

    [Tooltip("Temps avant la fin du voiceClip pour ouvrir la porte (négatif = avant la fin)")]
    public float openOffset = -0.2f; // -0.2 = 0.2 sec avant la fin

    [Header("Vibration Caméra")]
    public Camera mainCamera;                  // Caméra principale (auto si vide)
    public float shakeDuration = 0.3f;         // Durée du tremblement
    public float shakeMagnitude = 0.15f;       // Intensité du tremblement

    private bool hasTriggered = false;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        // 🔊 On lance la voix
        PlayVoiceAndQueueDoorOpen();

        // 💥 Vibration caméra
        if (mainCamera != null)
            StartCoroutine(ShakeCamera());
    }

    private void PlayVoiceAndQueueDoorOpen()
    {
        float duration = 0f;

        // 🎙️ Lecture de la voix
        if (voiceSource != null && voiceClip != null)
        {
            voiceSource.clip = voiceClip;
            voiceSource.Play();
            duration = voiceClip.length;
        }

        // 🕒 Calcul du délai avant ouverture
        float delay = duration + openOffset;
        if (delay < 0f) delay = 0f;

        Invoke(nameof(OpenLinkedDoor), delay);
    }

    private void OpenLinkedDoor()
    {
        // 🚪 Pour la première porte (U1)
        if (doorU1 != null)
        {
            doorU1.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
            Debug.Log("🚪 Porte U1 ouverte par voix.");
        }

        // 🕰️ Pour la deuxième salle (DoorController_Timed)
        if (doorTimed != null)
        {
            doorTimed.OpenDoor();
            Debug.Log("🎙️ Porte Timed ouverte par voix.");
        }
    }

    // 💥 Effet de shake caméra identique à ClockTrigger_Vibrate
    private IEnumerator ShakeCamera()
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
