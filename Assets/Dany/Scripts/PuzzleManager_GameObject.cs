using UnityEngine;

public class PuzzleManager_GameObject : MonoBehaviour
{
    [Header("Liste des spots à remplir avec les blocs")]
    public SpotDetector[] spots;

    [Header("Porte à déverrouiller quand le puzzle est réussi")]
    public DoorController_PuzzleOnce doorToUnlock;

    private bool puzzleDone = false;

    void Update()
    {
        if (puzzleDone) return;
        if (spots == null || spots.Length == 0) return; // Sécurité : aucun spot trouvé

        int count = 0;

        foreach (var spot in spots)
        {
            if (spot != null && spot.isOccupied)
                count++;
        }

        if (count >= spots.Length)
        {
            puzzleDone = true;
            Debug.Log("✅ Puzzle Complété ! Ouverture de la porte puzzle !");

            if (doorToUnlock != null)
                doorToUnlock.UnlockDoor();
            else
                Debug.LogWarning("⚠️ Aucun DoorController_PuzzleOnce assigné dans PuzzleManager !");
        }
    }
}
