using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialActivator : MonoBehaviour
{
    [Header("Objets à activer en séquence")]
    public List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Délai entre chaque activation (en secondes)")]
    public float delayBetweenActivations = 0.5f;

    [Header("Délai avant le début de la séquence (en secondes)")]
    public float startDelay = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(ActivateSequence());
    }

    private IEnumerator ActivateSequence()
    {
        // Petit suspense avant le show
        yield return new WaitForSeconds(startDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);

            yield return new WaitForSeconds(delayBetweenActivations);
        }
    }
}
