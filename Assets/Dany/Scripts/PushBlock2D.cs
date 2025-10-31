using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PushBlock2D : MonoBehaviour
{
    [Header("Push Settings")]
    [Tooltip("Force appliquée lorsque le joueur pousse l'objet.")]
    public float pushForce = 3f;

    [Tooltip("Vitesse maximale de glissement de l'objet.")]
    public float maxSpeed = 1.8f;

    [Tooltip("Distance minimale de mouvement avant de snap sur la grille.")]
    public float snapThreshold = 0.05f;

    [Header("Grid Snap")]
    public bool enableSnap = true;

    private Rigidbody2D rb;
    private Vector3 lastPos;
    private bool isSnapping = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; 
        rb.drag = 4f;          
    }

    private void FixedUpdate()
    {
        if (isSnapping) return;

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;

        // Si presque immobile → SNAP
        if (enableSnap && rb.velocity.magnitude < snapThreshold && (transform.position - lastPos).sqrMagnitude > 0.0001f)
        {
            SnapToGrid();
        }

        lastPos = transform.position;
    }

    private void SnapToGrid()
    {
        isSnapping = true;

        // On arrondit au centre de cellule
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);

        transform.position = pos;
        rb.velocity = Vector2.zero;

        // Laisse un petit temps pour éviter re-snap immédiat
        Invoke(nameof(ReleaseSnap), 0.05f);
    }

    private void ReleaseSnap()
    {
        isSnapping = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Vector2 pushDir = collision.contacts[0].normal * -1f;
            rb.AddForce(pushDir * pushForce, ForceMode2D.Force);
        }
    }
}
