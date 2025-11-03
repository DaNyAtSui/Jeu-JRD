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

    [Header("Activation unique")]
    [Tooltip("Si activé, le changement ne peut se produire qu'une seule fois (permanent après la première activation).")]
    public bool oneTimeTrigger = false;

    private bool lastConditionState = false;
    private bool hasBeenTriggered = false; // empêche de rejouer l’action

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
        if (hasBeenTriggered && oneTimeTrigger)
            return; // plus rien à faire si l'action est définitive

        bool condition = AllMatchCondition();

        // Si la condition vient de changer d’état
        if (condition != lastConditionState)
        {
            lastConditionState = condition;

            // Si la condition est remplie
            if (condition)
            {
                foreach (var target in targetObjects)
                {
                    if (target != null)
                        target.SetActive(setTargetActive);
                }

                if (oneTimeTrigger)
                    hasBeenTriggered = true; // bloque toute nouvelle mise à jour
            }
            else
            {
                // Si on ne veut pas un déclenchement unique, on restaure l’état initial
                if (!oneTimeTrigger)
                {
                    foreach (var target in targetObjects)
                    {
                        if (target != null)
                            target.SetActive(initialActiveState);
                    }
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
