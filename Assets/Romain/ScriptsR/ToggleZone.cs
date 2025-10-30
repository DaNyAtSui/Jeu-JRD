using UnityEngine;

public class ToggleZone2D : MonoBehaviour
{
    [Header("Objet enfant à activer/désactiver")]
    public GameObject targetObject;

    private bool isActive = false;

    void Start()
    {
        // Si rien n'est assigné, on prend automatiquement le premier enfant
        if (targetObject == null && transform.childCount > 0)
            targetObject = transform.GetChild(0).gameObject;

        if (targetObject != null)
            targetObject.SetActive(isActive);
    }

    public void ToggleObject()
    {
        if (targetObject == null) return;

        isActive = !isActive;
        targetObject.SetActive(isActive);
        Debug.Log($"[ToggleZone2D] {targetObject.name} -> {isActive}");
    }
}
