using System.Collections.Generic;
using UnityEngine;

public class ConditionalActivator : MonoBehaviour
{
    [Header("Objets à surveiller")]
    public List<GameObject> watchedObjects = new List<GameObject>();

    [Header("Condition de déclenchement")]
    [Tooltip("True = Tous doivent être actifs, False = Tous doivent être inactifs")]
    public bool requireAllActive = true;

    [Header("Objets cibles à activer/désactiver")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Tooltip("True = activer les objets cibles quand la condition est remplie, False = les désactiver")]
    public bool setTargetActive = false;

    [Header("État initial au démarrage")]
    [Tooltip("True = actif au lancement, False = inactif au lancement")]
    public bool initialActiveState = true;

    private bool lastConditionState = false; // permet de détecter les changements d’état

    void Start()
    {
        // Fixe l'état initial pour tous les objets cibles
        foreach (var target in targetObjects)
        {
            if (target != null)
                target.SetActive(initialActiveState);
        }
    }

    void Update()
    {
        bool condition = AllMatchCondition();

        // Si la condition vient de changer d’état
        if (condition != lastConditionState)
        {
            lastConditionState = condition;

            foreach (var target in targetObjects)
            {
                if (target != null)
                {
                    // Si la condition est vraie → applique l’action prévue
                    // Si elle redevient fausse → restaure l’état initial
                    target.SetActive(condition ? setTargetActive : initialActiveState);
                }
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
