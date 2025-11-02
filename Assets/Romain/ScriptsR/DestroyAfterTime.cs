using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [Header("Durée avant destruction (secondes)")]
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
