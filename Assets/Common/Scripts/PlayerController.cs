using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private PlayerINputAct input; 
    private Vector2 moveInput;
    private Vector2 moveDir;

    private Rigidbody2D rb;
    private Animator animator;

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

        // Soft 4-Way Direction Lock
        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveDir = new Vector2(Mathf.Sign(moveInput.x), 0f);
            else
                moveDir = new Vector2(0f, Mathf.Sign(moveInput.y));
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimations()
    {
        bool isWalking = moveInput.sqrMagnitude > 0.01f;

        animator.SetBool("IsWalking", isWalking);
        animator.SetFloat("MoveX", moveDir.x);
        animator.SetFloat("MoveY", moveDir.y);
    }
}
