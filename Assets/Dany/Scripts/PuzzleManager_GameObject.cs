using UnityEngine;

public class PuzzleManager_GameObject : MonoBehaviour
{
    public SpotDetector[] spots; 
    public DoorController_U1 doorToUnlock;

    private bool puzzleDone = false;

    void Update()
    {
        if (puzzleDone) return;

        int count = 0;

        foreach (var spot in spots)
        {
            if (spot.isOccupied) count++;
        }

        if (count >= spots.Length)
        {
            puzzleDone = true;
            Debug.Log("✅ Puzzle Complété ! Ouverture de la 2e porte !");
            doorToUnlock.UnlockDoor();
        }
    }
}
