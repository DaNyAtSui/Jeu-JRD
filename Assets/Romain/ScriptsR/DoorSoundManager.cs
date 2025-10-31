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
        [HideInInspector] public bool initialized;
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

        // On enregistre les états actuels mais sans encore considérer la scène stable.
        foreach (var d in doors)
        {
            if (d.door == null) continue;
            d.lastActiveState = d.door.activeSelf;
            d.initialized = false;
        }
    }

    void LateUpdate()
    {
        // On attend une frame complète avant d’activer la détection
        foreach (var d in doors)
        {
            if (d.door == null) continue;

            bool current = d.door.activeSelf;

            // Si pas encore initialisé, on fixe la référence sans jouer de son
            if (!d.initialized)
            {
                d.lastActiveState = current;
                d.initialized = true;
                continue;
            }

            // Après initialisation, on réagit normalement
            if (current != d.lastActiveState)
            {
                if (current)
                    Play(closeSound);   // active = fermée
                else
                    Play(openSound);    // inactive = ouverte

                d.lastActiveState = current;
            }
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        source.PlayOneShot(clip, volume);
    }
}
