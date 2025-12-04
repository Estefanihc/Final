using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 5f;
    public float moveForce = 2f;
    public float jumpCooldown = 2f;
    public LayerMask groundLayer;

    [Header("Detection")]
    public float playerDetectionRange = 10f;

    private Rigidbody2D rb;
    private Transform player;
   [SerializeField] private bool isGrounded;
    private float nextJumpTime;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        CheckGrounded();

        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (Time.time >= nextJumpTime && isGrounded && distanceToPlayer <= playerDetectionRange)
        {
            JumpTowardsPlayer();
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    private void CheckGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;
    }

    private void JumpTowardsPlayer()
    {
        if (player == null)
            return;
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2(direction.x * moveForce, 1f).normalized;

        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
