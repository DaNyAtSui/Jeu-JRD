using System.Collections;
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
        [HideInInspector] public bool firstChangeIgnored; // empêche le son du démarrage
    }

    [Header("Portes à surveiller")]
    public List<DoorData> doors = new List<DoorData>();

    [Header("Sons à jouer")]
    [Tooltip("Joué quand la porte devient inactive (SetActive(false)) → porte ouverte")]
    public AudioClip openSound;

    [Tooltip("Joué quand la porte devient active (SetActive(true)) → porte fermée")]
    public AudioClip closeSound;

    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        // On enregistre l'état initial sans rien jouer
        foreach (var d in doors)
        {
            if (d.door == null) continue;
            d.lastActiveState = d.door.activeSelf;
            d.firstChangeIgnored = false;
        }
    }

    void Update()
    {
        foreach (var d in doors)
        {
            if (d.door == null) continue;

            bool currentState = d.door.activeSelf;

            if (currentState != d.lastActiveState)
            {
                // Si c’est la première transition depuis le début, on l’ignore.
                if (!d.firstChangeIgnored)
                {
                    d.firstChangeIgnored = true;
                    d.lastActiveState = currentState;
                    continue;
                }

                // Sinon on joue le bon son.
                if (currentState)
                    Play(closeSound);   // porte fermée
                else
                    Play(openSound);    // porte ouverte

                d.lastActiveState = currentState;
            }
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        source.PlayOneShot(clip, volume);
    }
}
