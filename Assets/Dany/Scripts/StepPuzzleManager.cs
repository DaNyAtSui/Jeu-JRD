using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class StepPuzzleManager : MonoBehaviour
{
    [Header("Ordre correct des spots")]
    public PlayerStepSpot[] spotsInOrder;

    [Header("Porte à ouvrir (Timed)")]
    public DoorController_Timed linkedDoor;

    [Header("Ambiance sonore du temps")]
    public AudioSource clockSource;
    public AudioClip tickClip;
    public AudioClip finalGongClip;
    public float tickPitchStep = 0.05f;

    [Header("Effet d’onde lumineuse finale")]
    public Light2D centralLight;
    public float waveMaxIntensity = 5f;
    public float waveDuration = 1.5f;
    public AudioSource humSource;
    public float humFadeDuration = 2f;

    [Header("ID d’énigme (1 ou 2)")]
    public int puzzleID = 1; // 1 = première salle, 2 = deuxième

    private int currentIndex = 0;
    private bool puzzleCompleted = false;

    public void TryActivateSpot(PlayerStepSpot spot)
    {
        if (puzzleCompleted) return;

        // Vérifie si le joueur est sur le bon spot
        if (spot == spotsInOrder[currentIndex])
        {
            spot.SetActive(true);
            spot.PlayBeep(true);
            currentIndex++;

            // 🔔 petit "tic" à chaque spot
            if (clockSource != null && tickClip != null)
            {
                clockSource.pitch = 1f + tickPitchStep * currentIndex;
                clockSource.PlayOneShot(tickClip);
            }

            // Puzzle complet ?
            if (currentIndex >= spotsInOrder.Length)
            {
                puzzleCompleted = true;
                Debug.Log("✅ SCEAU DU TEMPS activé !");

                // 🕰️ Ouvre définitivement la porte après le puzzle
                if (linkedDoor != null)
                    linkedDoor.UnlockPermanently();

                // 🕰️ Grand gong final
                if (clockSource != null && finalGongClip != null)
                    clockSource.PlayOneShot(finalGongClip);

                // 🌌 Onde lumineuse + fondu sonore
                if (centralLight != null)
                    StartCoroutine(TimeSealWave());
                if (humSource != null)
                    StartCoroutine(FadeOutHum());

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
        else
        {
            // Mauvais ordre → reset
            Debug.Log("❌ Mauvais spot, reset !");
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        foreach (var s in spotsInOrder)
        {
            s.SetActive(false);
            s.PlayBeep(false);
        }
        currentIndex = 0;
    }

    private IEnumerator TimeSealWave()
    {
        float timer = 0f;
        float baseIntensity = centralLight.intensity;

        while (timer < waveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / waveDuration;
            centralLight.intensity = Mathf.Lerp(baseIntensity, waveMaxIntensity, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }
        centralLight.intensity = baseIntensity;
    }

    private IEnumerator FadeOutHum()
    {
        float startVol = humSource.volume;
        float timer = 0f;

        while (timer < humFadeDuration)
        {
            timer += Time.deltaTime;
            humSource.volume = Mathf.Lerp(startVol, 0f, timer / humFadeDuration);
            yield return null;
        }
        humSource.Stop();
        humSource.volume = startVol;
    }
}
