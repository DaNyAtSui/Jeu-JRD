using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progression des énigmes")]
    public bool puzzleRoom1Completed = false;
    public bool puzzleRoom2Completed = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AllPuzzlesCompleted()
    {
        return puzzleRoom1Completed && puzzleRoom2Completed;
    }
}
