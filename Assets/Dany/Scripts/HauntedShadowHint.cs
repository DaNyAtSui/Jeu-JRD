using UnityEngine;
using System.Reflection;

/// <summary>
/// HauntedShadowHint — Silhouette humanoïde floue (2D top-down) + audio spectral discret.
/// - Multi-sprites (2–3+ silhouettes).
/// - Spawn en bord de vision, fade-in/out, déplacement lent→rapide selon creepiness.
/// - Disparition INSTANT si le joueur la regarde (cône ~90°).
/// - Sorting order évolutif (derrière décor → même plan → un poil devant).
/// - Sync complète sur PlayerController.creepinessLevel (auto-link via reflection).
/// - Son spectral "background" à la disparition (progressif: quasi inaudible → clair mais pas fort).
/// </summary>
[DisallowMultipleComponent]
public class HauntedShadowHint : MonoBehaviour
{
    [Header("Sprites des silhouettes (2–3 mini)")]
    public Sprite[] shadowSprites;

    [Header("Références")]
    public Transform player;                 // auto = tag "Player"
    public SpriteRenderer shadowRenderer;    // auto = child
    [Tooltip("AudioSource pour le son spectral (auto-créé si vide)")]
    public AudioSource spectralSource;

    [Header("Apparition & Mouvement")]
    [Tooltip("Délai min/max entre apparitions")]
    public float minSpawnDelay = 8f;
    public float maxSpawnDelay = 18f;
    [Tooltip("Durée de vie visible d'une apparition")]
    public float shadowLifetime = 4f;
    [Tooltip("Vitesse du fade in/out")]
    public float fadeSpeed = 2f;

    [Tooltip("Vitesse de déplacement à creepiness faible")]
    public float moveSpeedLow = 0.3f;
    [Tooltip("Vitesse de déplacement à creepiness élevée")]
    public float moveSpeedHigh = 1.5f;

    [Header("Vision & Placement")]
    [Tooltip("Cône de vision frontal du joueur (disparition si dedans)")]
    [Range(30f, 160f)] public float viewCone = 90f;
    [Tooltip("Spawn à distance mini / maxi du joueur")]
    public float minDistance = 5f;
    public float maxDistance = 9f;

    [Header("Alpha (opacity progressive)")]
    [Tooltip("Opacité min à creepiness faible")]
    [Range(0f, 1f)] public float alphaMin = 0.05f;
    [Tooltip("Opacité max à creepiness élevée")]
    [Range(0f, 1f)] public float alphaMax = 0.55f;

    [Header("Audio Spectral (joué à la disparition)")]
    [Tooltip("Clips spectraux (2–4) pour variété)")]
    public AudioClip[] spectralClips;
    [Tooltip("Volume min à creepiness faible (mix background)")]
    [Range(0f, 1f)] public float spectralVolMin = 0.03f;
    [Tooltip("Volume max à creepiness élevée (jamais jumpscare)")]
    [Range(0f, 1f)] public float spectralVolMax = 0.18f;
    [Tooltip("Pitch de base du son spectral")]
    public float spectralPitchBase = 1.0f;
    [Tooltip("Jitter de pitch (proportionnel à creepiness)")]
    public float spectralPitchJitter = 0.08f;
    [Tooltip("Spatial Blend (0=2D, 1=3D). Mix ambiance discret ~0.15–0.25")]
    [Range(0f, 1f)] public float spectralSpatialBlend = 0.2f;

    [Header("Creepiness Sync (fallback si pas de PlayerController)")]
    [Tooltip("Temps pour atteindre creepiness=1 si aucun PlayerController trouvé")]
    public float fallbackTimeToMax = 240f;

    // --- interne ---
    float timer, lifeTimer;
    bool fadingIn, fadingOut;
    float currentAlpha;
    Color baseColor;
    Vector3 targetPos;

    // creepiness auto-link
    float internalCreepiness;
    object playerController;
    FieldInfo creepField;
    PropertyInfo creepProp;

    // éviter de rejouer le son spectral plusieurs fois par apparition
    bool spectralPlayedThisAppearance;

    void Awake()
    {
        if (shadowRenderer == null) shadowRenderer = GetComponentInChildren<SpriteRenderer>(true);
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (shadowRenderer == null)
        {
            Debug.LogWarning("[HauntedShadowHint] Pas de SpriteRenderer trouvé.", this);
            enabled = false; return;
        }

        baseColor = shadowRenderer.color;
        shadowRenderer.enabled = false;
        SetRendererAlpha(0f);

        // Auto-create spectral AudioSource si manquant
        if (spectralSource == null)
        {
            spectralSource = gameObject.AddComponent<AudioSource>();
            spectralSource.playOnAwake = false;
            spectralSource.loop = false;
            spectralSource.spatialBlend = spectralSpatialBlend;
            spectralSource.volume = spectralVolMin;
        }

        // Auto-link PlayerController (pour creepinessLevel)
        // Use Resources.FindObjectsOfTypeAll and filter to scene instances to avoid deprecated API.
        foreach (var mb in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (mb == null) continue;
            // Ignore assets/prefabs; only consider scene objects
            if (!mb.gameObject.scene.IsValid()) continue;

            if (mb.GetType().Name == "PlayerController")
            {
                playerController = mb;
                var t = mb.GetType();
                creepProp  = t.GetProperty("CreepinessLevel", BindingFlags.Public | BindingFlags.Instance);
                creepField = t.GetField("creepinessLevel", BindingFlags.NonPublic | BindingFlags.Instance);
                break;
            }
        }

        ScheduleNextSpawn();
    }

    void ScheduleNextSpawn()
    {
        timer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f && !shadowRenderer.enabled)
        {
            TrySpawnShadow();
        }

        if (shadowRenderer.enabled)
        {
            UpdateShadowBehavior();
        }
    }

    void TrySpawnShadow()
    {
        float creep = ReadCreepiness01();
        if (shadowSprites == null || shadowSprites.Length == 0 || player == null) { ScheduleNextSpawn(); return; }

        // Sprite au hasard
        shadowRenderer.sprite = shadowSprites[Random.Range(0, shadowSprites.Length)];

        // Spawn en bord de vision (cercle autour du joueur)
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + new Vector3(dir.x, dir.y, 0f) * Random.Range(minDistance, maxDistance);
        transform.position = spawnPos;

        // Légère destination locale (petit glissement)
        targetPos = spawnPos + (Vector3)(Random.insideUnitCircle.normalized * Mathf.Lerp(0.25f, 0.6f, creep));

        // Sorting Order selon creepiness
        if (creep < 0.4f)      shadowRenderer.sortingOrder = -2; // derrière décor
        else if (creep < 0.7f) shadowRenderer.sortingOrder =  0; // au plan du joueur
        else                   shadowRenderer.sortingOrder =  1; // un poil devant (rare)

        // Alpha de départ bas (progressif)
        currentAlpha = Mathf.Lerp(alphaMin * 0.5f, alphaMin, creep);
        SetRendererAlpha(currentAlpha);

        // Activer
        shadowRenderer.enabled = true;
        lifeTimer = shadowLifetime;
        fadingIn = true;
        fadingOut = false;
        spectralPlayedThisAppearance = false;

        // planifier prochain spawn (même si celui-ci est actif)
        ScheduleNextSpawn();
    }

    void UpdateShadowBehavior()
    {
        float creep = ReadCreepiness01();

        // Fade in
        if (fadingIn)
        {
            float targetAlpha = Mathf.Lerp(alphaMin, alphaMax, creep);
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(currentAlpha - targetAlpha) < 0.02f) fadingIn = false;
        }

        // Fade out
        if (fadingOut)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, Time.deltaTime * fadeSpeed * 1.5f);
            if (currentAlpha <= 0.01f)
            {
                shadowRenderer.enabled = false;
                fadingOut = false;
                return;
            }
        }

        // Déplacement (lente → rapide avec creep)
        float speed = Mathf.Lerp(moveSpeedLow, moveSpeedHigh, creep);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Temps de vie écoulé → fade out
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f && !fadingOut)
        {
            StartFadeOut(playSpectral: true);
        }

        // Disparition immédiate si le joueur "regarde" l’ombre
        if (player != null && IsPlayerLookingAtShadow(player, transform.position))
        {
            StartFadeOut(playSpectral: true);
        }

        // Applique alpha
        SetRendererAlpha(currentAlpha);
    }

    bool IsPlayerLookingAtShadow(Transform playerTf, Vector3 shadowPos)
    {
        // On récupère MoveX/MoveY (set par ton PlayerController dans l'Animator)
        var anim = playerTf.GetComponentInChildren<Animator>();
        if (anim == null) return false;

        Vector2 facing = new Vector2(anim.GetFloat("MoveX"), anim.GetFloat("MoveY")).normalized;
        if (facing.sqrMagnitude < 0.1f) return false; // si le perso n'a pas encore de facing clair, on ne trigger pas

        Vector2 dirTo = ((Vector2)(shadowPos - playerTf.position)).normalized;

        // cône frontal ~90° (seuil = cos(halfAngle))
        float threshold = Mathf.Cos(viewCone * 0.5f * Mathf.Deg2Rad);
        float dot = Vector2.Dot(facing, dirTo);
        return dot > threshold;
    }

    void StartFadeOut(bool playSpectral)
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadingIn = false;

            if (playSpectral) PlaySpectralOneShotOnce();
        }
    }

    void PlaySpectralOneShotOnce()
    {
        if (spectralPlayedThisAppearance) return;
        if (spectralSource == null || spectralClips == null || spectralClips.Length == 0) return;

        spectralPlayedThisAppearance = true;

        float creep = ReadCreepiness01();

        // Volume progressif (A→B→C discret) : quasi inaudible → subtil → clair mais pas fort
        float vol = Mathf.Lerp(spectralVolMin, spectralVolMax, creep);
        spectralSource.volume = vol;

        // Pitch léger jitter, proportionnel à creepiness
        float jitter = spectralPitchJitter * Mathf.Clamp01(creep);
        spectralSource.pitch = spectralPitchBase + Random.Range(-jitter, jitter);

        // Spatial blend discret (mix ambiance)
        spectralSource.spatialBlend = spectralSpatialBlend;

        spectralSource.clip = spectralClips[Random.Range(0, spectralClips.Length)];
        spectralSource.Play();
    }

    float ReadCreepiness01()
    {
        if (playerController != null)
        {
            if (creepProp != null)
            {
                var v = creepProp.GetValue(playerController, null);
                if (v is float f) return Mathf.Clamp01(f);
            }
            if (creepField != null)
            {
                var v = creepField.GetValue(playerController);
                if (v is float f) return Mathf.Clamp01(f);
            }
        }
        // Fallback: progression lente autonome
        internalCreepiness = Mathf.Clamp01(internalCreepiness + Time.deltaTime / Mathf.Max(1f, fallbackTimeToMax));
        return internalCreepiness;
    }

    void SetRendererAlpha(float a)
    {
        var c = shadowRenderer.color;
        c.a = a;
        shadowRenderer.color = c;
    }
}
