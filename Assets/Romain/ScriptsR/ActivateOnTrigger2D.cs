using UnityEngine;

public class ActivateOnTrigger2D : MonoBehaviour
{
    [Header("Objet à activer")]
    public GameObject objectToActivate;

    [Header("Options")]
    public bool destroyAfterActivation = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (destroyAfterActivation)
        {
            Destroy(gameObject);
        }
    }
}
