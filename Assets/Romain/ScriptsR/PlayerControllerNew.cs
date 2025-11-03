using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerNew : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private PlayerINputAct input;
    private Vector2 moveInput;
    private Vector2 moveDir;

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Footsteps Settings")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.35f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;
    private float footstepTimer;

    [Header("Psychological Audio Progression")]
    [Range(0f, 1f)] public float echoStartChance = 0.02f;
    [Range(0f, 1f)] public float echoMaxChance = 0.12f;
    public float timeToMaxCreepiness = 240f;
    private float creepinessLevel = 0f;
    public AudioSource echoSource;

    private void Awake()
    {
        input = new PlayerINputAct();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        bool isWalking = moveInput.sqrMagnitude > 0.01f;

        // Soft 4-Way Direction Lock
        if (isWalking)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveDir = new Vector2(Mathf.Sign(moveInput.x), 0f);
            else
                moveDir = new Vector2(0f, Mathf.Sign(moveInput.y));
        }

        UpdateCreepiness();
        HandleFootsteps(isWalking);
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimations()
{
    float speed = moveInput.magnitude;
    animator.SetFloat("Speed", speed);

    // met à jour MoveX/Y seulement quand il y a du mouvement
    if (speed > 0.01f)
    {
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
    }
}

    private void UpdateCreepiness()
    {
        creepinessLevel = Mathf.Clamp01(creepinessLevel + Time.deltaTime / timeToMaxCreepiness);
    }

    private void HandleFootsteps(bool isWalking)
    {
        if (!isWalking || footstepClips.Length == 0 || footstepSource == null)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            PlayFootstep();
            footstepTimer = footstepInterval;
        }
    }

    private void PlayFootstep()
    {
        footstepSource.pitch = Random.Range(pitchMin, pitchMax);
        footstepSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.panStereo = Random.Range(-0.15f, 0.15f);
        footstepSource.Play();
        TryPlayEcho();
    }

    private void TryPlayEcho()
    {
        if (echoSource == null) return;

        float chance = Mathf.Lerp(echoStartChance, echoMaxChance, creepinessLevel);

        if (Random.value <= chance)
        {
            echoSource.pitch = Random.Range(0.85f, 1.05f);
            echoSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            echoSource.panStereo = Random.Range(-0.4f, 0.4f);
            echoSource.volume = 0.4f;
            echoSource.PlayDelayed(Random.Range(0.05f, 0.25f));
        }
    }
}
