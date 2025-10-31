using System.Collections.Generic;
using UnityEngine;

public class ConditionalActivator : MonoBehaviour
{
    [Header("Objets à surveiller")]
    public List<GameObject> watchedObjects = new List<GameObject>();

    [Header("Condition de déclenchement")]
    [Tooltip("True = Tous doivent être actifs, False = Tous doivent être inactifs")]
    public bool requireAllActive = true;

    [Header("Action à effectuer")]
    public GameObject targetObject;
    [Tooltip("True = activer l’objet cible, False = le désactiver quand la condition est remplie")]
    public bool setTargetActive = false;
    [Tooltip("Empêche la répétition si la condition a déjà été remplie")]
    public bool triggerOnce = true;

    [Header("État initial au démarrage")]
    [Tooltip("True = actif au lancement, False = inactif au lancement")]
    public bool initialActiveState = true;

    private bool hasTriggered = false;

    void Start()
    {
        // Fixe l'état de départ du GameObject cible
        if (targetObject != null)
            targetObject.SetActive(initialActiveState);
    }

    void Update()
    {
        if (hasTriggered && triggerOnce)
            return;

        if (AllMatchCondition())
        {
            if (targetObject != null)
            {
                targetObject.SetActive(setTargetActive);
                hasTriggered = true;
            }
        }
    }

    private bool AllMatchCondition()
    {
        foreach (var obj in watchedObjects)
        {
            if (obj == null)
                continue;

            bool active = obj.activeSelf;

            if (requireAllActive && !active)
                return false;

            if (!requireAllActive && active)
                return false;
        }

        return true;
    }
}
