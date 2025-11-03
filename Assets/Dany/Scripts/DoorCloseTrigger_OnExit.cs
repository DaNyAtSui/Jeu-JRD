using UnityEngine;

public class DoorCloseTrigger_OnExit : MonoBehaviour
{
    [Header("Référence de la porte")]
    public DoorController_Timed door;

    [Tooltip("Délai avant fermeture après sortie")]
    public float closeDelay = 0.8f;

    private bool hasClosed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasClosed) return;
        if (!other.CompareTag("Player")) return;
        if (door == null) return;

        // Ne ferme définitivement que si le puzzle est terminé
        if (!door.IsUnlocked)
        {
            Debug.Log("🧩 Le puzzle n'est pas fini, la porte reste ouverte.");
            return;
        }

        hasClosed = true;
        Invoke(nameof(CloseDoorForever), closeDelay);
    }

    private void CloseDoorForever()
    {
        if (door != null)
        {
            door.CloseForever();
            Debug.Log("🚪 Porte refermée définitivement après la sortie du joueur.");
        }
    }
}
