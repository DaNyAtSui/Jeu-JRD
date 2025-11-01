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

        // Nouveau nom sous Unity 6 :
        rb.linearDamping = 4f;
    }

    private void FixedUpdate()
    {
        if (isSnapping) return;

        // Limiter la vitesse max (Unity 6 : linearVelocity)
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // Snap si quasi immobile mais a bougé
        if (enableSnap && rb.linearVelocity.magnitude < snapThreshold &&
            (transform.position - lastPos).sqrMagnitude > 0.0001f)
        {
            SnapToGrid();
        }

        lastPos = transform.position;
    }

    private void SnapToGrid()
    {
        isSnapping = true;

        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);

        transform.position = pos;
        rb.linearVelocity = Vector2.zero;

        // Petit délai pour éviter re-snap immédiat
        Invoke(nameof(ReleaseSnap), 0.05f);
    }

    private void ReleaseSnap()
    {
        isSnapping = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Vector2 pushDir = -collision.contacts[0].normal;

        // Ajout de force selon Unity 6
        rb.AddForce(pushDir * pushForce, ForceMode2D.Force);
    }
}
