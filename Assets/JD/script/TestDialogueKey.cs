using UnityEngine;

// Ce script a juste besoin de connaître le trigger
public class TestDialogueKey : MonoBehaviour
{
    // Glissez votre objet 'Trigger_Intro_Scene' ici dans l'inspecteur
    public DialogueTrigger dialoguePourTester;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            dialoguePourTester.TriggerDialogue();
        }
    }
}