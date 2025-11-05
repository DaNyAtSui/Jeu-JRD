using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleTrigger : MonoBehaviour
{
    [Header("Références")]
    public GameObject candleTile;           // le sprite/tile de la bougie
    public Light2D candleLight;             // la lumière de la bougie au sol
    public Light2D playerLight;             // la lumière enfant du joueur
    public AudioSource extinguishSound;     // optionnel, petit son de bougie

    private bool triggered = false;
    [Header("Dialogue")]
    [SerializeField] private DialogueTrigger dialogueARecuperer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return; // évite double appel
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 1️⃣ éteindre la lumière de la bougie
        if (candleLight != null)
            candleLight.enabled = false;

        // 2️⃣ cacher la tile de la bougie
        if (candleTile != null)
            candleTile.SetActive(false);

        // 3️⃣ activer la lumière du joueur
        if (playerLight != null)
            playerLight.enabled = true;

        if (dialogueARecuperer != null)
        {
            dialogueARecuperer.TriggerDialogue();
        }

        // 4️⃣ petit son optionnel
        if (extinguishSound != null)
            extinguishSound.Play();
    }
}
