using UnityEngine;

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

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        PlayVoiceAndQueueDoorOpen();
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
}
