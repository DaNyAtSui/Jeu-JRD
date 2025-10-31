using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorSoundManager : MonoBehaviour
{
    [System.Serializable]
    public class DoorData
    {
        public GameObject door;
        [HideInInspector] public bool lastActiveState;
    }

    [Header("Portes à surveiller")]
    public List<DoorData> doors = new List<DoorData>();

    [Header("Sons à jouer")]
    [Tooltip("Son joué quand la porte PASSE en inactive (SetActive(false)) donc quand elle s'ouvre visuellement")]
    public AudioClip openSound;

    [Tooltip("Son joué quand la porte PASSE en active (SetActive(true)) donc quand elle se ferme visuellement")]
    public AudioClip closeSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // On mémorise l'état de départ de chaque porte
        foreach (var d in doors)
        {
            if (d.door != null)
                d.lastActiveState = d.door.activeSelf;
        }
    }

    void Update()
    {
        foreach (var d in doors)
        {
            if (d.door == null)
                continue;

            bool currentState = d.door.activeSelf;

            // L'état a changé depuis la dernière frame
            if (currentState != d.lastActiveState)
            {
                // Ici on inverse :
                // active == true  => porte "fermée" => son de fermeture
                // active == false => porte "ouverte" => son d'ouverture
                if (currentState)
                {
                    // vient de passer à active = true
                    PlaySound(closeSound);
                }
                else
                {
                    // vient de passer à active = false
                    PlaySound(openSound);
                }

                d.lastActiveState = currentState;
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }
}
