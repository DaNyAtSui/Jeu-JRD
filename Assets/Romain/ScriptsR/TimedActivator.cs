using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActivator : MonoBehaviour
{
    [Header("Objets à activer après le timer")]
    public List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Objets à désactiver après le timer")]
    public List<GameObject> objectsToDeactivate = new List<GameObject>();

    [Header("Temps avant déclenchement (en secondes)")]
    public float delay = 2f;

    [Header("Lancement automatique au Start")]
    public bool autoStart = true;

    private bool hasTriggered = false;

    void Start()
    {
        if (autoStart)
            StartCoroutine(ActivationRoutine());
    }

    public void TriggerTimer()
    {
        if (!hasTriggered)
            StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        hasTriggered = true;
        yield return new WaitForSeconds(delay);

        // Active les objets
        foreach (GameObject go in objectsToActivate)
        {
            if (go != null)
                go.SetActive(true);
        }

        // Désactive les objets
        foreach (GameObject go in objectsToDeactivate)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}
