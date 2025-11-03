using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection; // pour lire proprement la creepiness si privée

/// <summary>
/// Lumière "hantée" pour URP Light2D :
/// - Flicker permanent (jamais parfaitement stable)
/// - Pics chaotiques imprévisibles
/// - Micro-extinctions très brèves (0.05–0.15s)
/// - Flashs froids bleutés rares (paranormal)
/// - Synchronisée sur PlayerController.creepinessLevel (auto), sinon fallback interne
/// </summary>
[DisallowMultipleComponent]
public class HauntedFlicker2D : MonoBehaviour
{
    [Header("Target Light2D (auto si vide)")]
    public Light2D light2D;

    [Header("Intensity")]
    [Tooltip("Intensité moyenne de la lumière")]
    public float intensityBase = 1.0f;
    [Tooltip("Amplitude du jitter d'intensité (bruit permanent)")]
    public float intensityJitter = 0.25f;
    [Tooltip("Vitesse de réponse du LERP vers la valeur cible")]
    public float intensityResponse = 8f;

    [Header("Radius (Point Light Only)")]
    [Tooltip("Rayon de base. 0 = auto (prend le rayon actuel)")]
    public float radiusBase = 0f;
    [Tooltip("Variation du rayon (tremblement)")]
    public float radiusJitter = 0.12f;
    [Tooltip("Vitesse de réponse du LERP du rayon")]
    public float radiusResponse = 6f;

    [Header("Color (Hantise assumée)")]
    [Tooltip("Couleur chaude (bougie)")]
    public Color warmColor = new Color(1.0f, 0.86f, 0.6f, 1f);
    [Tooltip("Couleur froide fantomatique (flash rare)")]
    public Color coldColor = new Color(0.75f, 0.86f, 1.0f, 1f);
    [Tooltip("Durée d'un flash froid (sec)")]
    public float coldFlashDuration = 0.08f;
    [Tooltip("Chance de flash au début (0..1)")]
    [Range(0f, 1f)] public float coldFlashChanceBase = 0.0f;
    [Tooltip("Chance de flash au max creepiness (0..1)")]
    [Range(0f, 1f)] public float coldFlashChanceMax = 0.08f;

    [Header("Extinction paranormale (très brève)")]
    [Tooltip("Chance d'extinction au début (0..1 par évènement)")]
    [Range(0f, 1f)] public float extinctionChanceBase = 0.0f;
    [Tooltip("Chance d'extinction au max creepiness (0..1 par évènement)")]
    [Range(0f, 1f)] public float extinctionChanceMax = 0.12f;
    [Tooltip("Durée min d'une extinction (sec)")]
    public float extinctionMin = 0.05f;
    [Tooltip("Durée max d'une extinction (sec)")]
    public float extinctionMax = 0.15f;

    [Header("Chaos imprévisible")]
    [Tooltip("Vitesse du bruit (plus haut = plus nerveux)")]
    public float noiseSpeed = 1.6f;
    [Tooltip("Chance de pic chaotique au début (0..1 par seconde approx)")]
    [Range(0f, 1f)] public float spikeChanceBase = 0.02f;
    [Tooltip("Chance de pic chaotique au max creepiness")]
    [Range(0f, 1f)] public float spikeChanceMax = 0.15f;
    [Tooltip("Force du pic sur l'intensité")]
    public float spikeStrength = 0.6f;
    [Tooltip("Durée d'un pic (sec)")]
    public float spikeDuration = 0.12f;

    [Header("Synchronisation creepiness")]
    [Tooltip("Si aucun PlayerController trouvé, progression interne en X secondes")]
    public float fallbackTimeToMax = 240f;

    // --- runtime ---
    float _noiseSeedI, _noiseSeedR;
    float _flashTimer, _extinctionTimer, _spikeTimer;
    float _internalCreepiness; // fallback si pas trouvé
    float _radiusBaseCached;
    Color _targetColor, _currentColor;

    // lien auto vers PlayerController + lecture creepiness (public prop ou champ privé)
    object _playerController;
    FieldInfo _creepField;
    PropertyInfo _creepProp;

    void Reset()
    {
        light2D = GetComponent<Light2D>();
    }

    void Awake()
    {
        if (light2D == null) light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogWarning("[HauntedFlicker2D] Aucun Light2D trouvé sur l'objet.", this);
            enabled = false; return;
        }

        _noiseSeedI = Random.value * 1000f;
        _noiseSeedR = Random.value * 1000f;
        _currentColor = warmColor;
        _targetColor  = warmColor;

        if (radiusBase <= 0f && light2D.lightType == Light2D.LightType.Point)
            _radiusBaseCached = light2D.pointLightOuterRadius;
        else
            _radiusBaseCached = Mathf.Max(radiusBase, 0f);

        // Auto-link PlayerController
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb != null && mb.GetType().Name == "PlayerController")
            {
                _playerController = mb;
                var t = mb.GetType();
                _creepProp  = t.GetProperty("CreepinessLevel", BindingFlags.Public | BindingFlags.Instance);
                _creepField = t.GetField("creepinessLevel", BindingFlags.NonPublic | BindingFlags.Instance);
                break;
            }
        }
    }

    void Update()
    {
        float creep = ReadCreepiness01();

        // 1) Base jitter (jamais parfaitement stable)
        float tI = Time.time * noiseSpeed;
        float noiseI = (Mathf.PerlinNoise(_noiseSeedI, tI) - 0.5f) * 2f; // [-1..1]
        float noiseR = (Mathf.PerlinNoise(_noiseSeedR, tI * 0.9f) - 0.5f) * 2f;

        float targetIntensity = intensityBase + intensityJitter * noiseI;

        // 2) Pics chaotiques
        float spikeChance = Mathf.Lerp(spikeChanceBase, spikeChanceMax, creep);
        if (_spikeTimer <= 0f && Random.value < spikeChance * Time.deltaTime * 60f)
        {
            _spikeTimer = spikeDuration;
        }
        if (_spikeTimer > 0f)
        {
            targetIntensity += spikeStrength * Mathf.Sin((_spikeTimer / spikeDuration) * Mathf.PI);
            _spikeTimer -= Time.deltaTime;
        }

        // 3) Extinction paranormale très brève
        float extinctionChance = Mathf.Lerp(extinctionChanceBase, extinctionChanceMax, creep);
        if (_extinctionTimer <= 0f && Random.value < extinctionChance * Time.deltaTime * 30f)
        {
            _extinctionTimer = Random.Range(extinctionMin, extinctionMax);
        }
        if (_extinctionTimer > 0f)
        {
            targetIntensity *= 0.08f; // quasi noir
            _extinctionTimer -= Time.deltaTime;
        }

        // 4) Couleur : warm par défaut + flashs froids rares
        float flashChance = Mathf.Lerp(coldFlashChanceBase, coldFlashChanceMax, creep);
        if (_flashTimer <= 0f && Random.value < flashChance * Time.deltaTime * 30f)
        {
            _flashTimer = coldFlashDuration;
        }

        _targetColor = (_flashTimer > 0f)
            ? Color.Lerp(warmColor, coldColor, 0.85f)
            : Color.Lerp(warmColor, coldColor, Mathf.Clamp01(0.1f * creep)); // un soupçon de froid qui augmente avec le temps

        _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * 12f);
        light2D.color = _currentColor;
        if (_flashTimer > 0f) _flashTimer -= Time.deltaTime;

        // 5) Appliquer intensité (LERP smooth)
        light2D.intensity = Mathf.Lerp(light2D.intensity, Mathf.Max(0f, targetIntensity), Time.deltaTime * intensityResponse);

        // 6) Radius (si Point Light)
        if (light2D.lightType == Light2D.LightType.Point)
        {
            float targetRadius = _radiusBaseCached + radiusJitter * noiseR;
            light2D.pointLightOuterRadius = Mathf.Lerp(light2D.pointLightOuterRadius, Mathf.Max(0.01f, targetRadius), Time.deltaTime * radiusResponse);
        }
    }

    float ReadCreepiness01()
    {
        // essaie propriété publique CreepinessLevel
        if (_playerController != null)
        {
            if (_creepProp != null)
            {
                var v = _creepProp.GetValue(_playerController, null);
                if (v is float f) return Mathf.Clamp01(f);
            }
            // sinon champ privé "creepinessLevel"
            if (_creepField != null)
            {
                var v = _creepField.GetValue(_playerController);
                if (v is float f) return Mathf.Clamp01(f);
            }
        }

        // fallback : monte toute seule (progression interne)
        _internalCreepiness = Mathf.Clamp01(_internalCreepiness + Time.deltaTime / Mathf.Max(1f, fallbackTimeToMax));
        return _internalCreepiness;
    }
}
