using UnityEngine;

public class VoiceTriggerOnce : MonoBehaviour
{
    [Header("Voice")]
    public AudioSource voiceSource;
    public AudioClip voiceClip;

    [Header("Door Link")]
    public DoorController_U1 door; // Drag & Drop la porte avec le script DoorController_U1

    [Tooltip("Temps avant la fin du voiceClip pour ouvrir la porte (négatif = après)")]
    public float openOffset = -0.2f; // -0.2 = 0.2 sec AVANT la fin

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            PlayVoiceAndQueueDoorOpen();
        }
    }

    private void PlayVoiceAndQueueDoorOpen()
    {
        float duration = 0f;

        if (voiceSource != null && voiceClip != null)
        {
            voiceSource.clip = voiceClip;
            voiceSource.Play();
            duration = voiceClip.length;
        }

        if (door != null)
        {
            float delay = duration + openOffset;
            if (delay < 0f) delay = 0f; // sécurité

            Invoke(nameof(OpenDoor), delay);
        }
    }

    private void OpenDoor()
    {
        door.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
    }
}
