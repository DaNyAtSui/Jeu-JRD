using UnityEngine;

public class PuzzleManager_GameObject : MonoBehaviour
{
    [Header("Liste des spots à remplir avec les blocs")]
    public SpotDetector[] spots;

    [Header("Porte à déverrouiller quand le puzzle est réussi")]
    public DoorController_PuzzleOnce doorToUnlock;

    [Header("ID d’énigme (1 ou 2)")]
    public int puzzleID = 2; // 1 ou 2 selon la salle

    private bool puzzleDone = false;

    void Update()
    {
        if (puzzleDone) return;
        if (spots == null || spots.Length == 0) return;

        int count = 0;
        foreach (var spot in spots)
        {
            if (spot != null && spot.isOccupied)
                count++;
        }

        if (count >= spots.Length)
        {
            puzzleDone = true;
            Debug.Log("✅ Puzzle des blocs complété !");

            if (doorToUnlock != null)
                doorToUnlock.UnlockDoor();

            // 🧩 Marquer l'énigme comme terminée dans le GameManager
            if (GameManager.Instance != null)
            {
                if (puzzleID == 1)
                    GameManager.Instance.puzzleRoom1Completed = true;
                else if (puzzleID == 2)
                    GameManager.Instance.puzzleRoom2Completed = true;

                Debug.Log($"📜 Enigme {puzzleID} validée !");
            }
        }
    }
}
