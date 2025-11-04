using UnityEngine;
using System.Collections;

public class IntroSequenceManager : MonoBehaviour
{
    [Header("Références de Scène")]
    [SerializeField] private SoundManager soundManager; 

    [Header("Vos Triggers de Dialogue")]
    [SerializeField] private DialogueTrigger dialoguePhase1;
    [SerializeField] private DialogueTrigger dialoguePhase2;

    void Start()
    {
        if (soundManager == null || dialoguePhase1 == null || dialoguePhase2 == null)
        {
            Debug.LogError("Branchements manquants sur IntroSequenceManager !", this);
            return;
        }

        // On lance la coroutine principale
        StartCoroutine(PlayFullIntroSequence());
    }

    private IEnumerator PlayFullIntroSequence()
    {
        // 1. Lancer la Phase 1 (PlayerCinematic + Entity)
        dialoguePhase1.TriggerDialogue();

        // 2. Calculer le temps de l'audio et l'attendre
        float totalAudioWaitTime = soundManager.delayBeforeKnock +
                                   soundManager.delayAfterKnock +
                                   soundManager.delayVoiceToDoorOpen +
                                   soundManager.delayDoorOpenToClose;
        
        yield return new WaitForSeconds(totalAudioWaitTime);

        // 3. L'audio est fini. MAIS on attend aussi que le dialogue Phase 1 soit fini.
        // On vérifie la variable statique du DialogueManager
        while (DialogueManager.IsDialogueActive)
        {
            yield return null; // Attendre la prochaine frame
        }

        // 4. Audio ET Dialogue Phase 1 sont finis. On peut lancer la Phase 2.
        Debug.Log("Audio et Phase 1 terminés. Lancement de la Phase 2.");
        dialoguePhase2.TriggerDialogue();
    }
}