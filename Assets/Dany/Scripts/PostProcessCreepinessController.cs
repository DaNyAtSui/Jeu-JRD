using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessCreepinessController : MonoBehaviour
{
    public Volume postProcessVolume;
    private ColorAdjustments colorAdjust;
    private Vignette vignette;
    private ChromaticAberration chroma;
    private Bloom bloom;

    private PlayerController player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out colorAdjust);
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out chroma);
            postProcessVolume.profile.TryGet(out bloom);
        }
    }

    void Update()
    {
        if (player == null) return;
        float creep = Mathf.Clamp01(GetCreepiness(player));

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.25f, 0.55f, creep);

        if (colorAdjust != null)
            colorAdjust.saturation.value = Mathf.Lerp(0f, -60f, creep);

        if (chroma != null)
            chroma.intensity.value = Mathf.Lerp(0f, 0.3f, creep);

        if (bloom != null)
            bloom.intensity.value = Mathf.Lerp(0.6f, 1.4f, creep);
    }

    float GetCreepiness(PlayerController pc)
    {
        var field = typeof(PlayerController).GetField("creepinessLevel", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (float)field.GetValue(pc);
        }
        return 0f;
    }
}
