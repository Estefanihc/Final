using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerPlatformer : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 12f;
    [SerializeField] private float velPower = 0.9f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float jumpGravityScale = 1.5f;
    [SerializeField] private float fallGravityScale = 2.5f;
    [SerializeField] private float maxJumpTime = 0.35f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private Vector2 groundCheckOffset;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;

    // Jump variables
    private bool isJumping = false;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Input
    private float moveInput;
    private bool jumpInput;
    private bool jumpInputReleased;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        HandleJump();
        FlipCharacter();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandlePhysics();
    }

    private void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButton("Jump");
        jumpInputReleased = Input.GetButtonUp("Jump");
    }

    private void HandleMovement()
    {
        // Calculate target speed
        float targetSpeed = moveInput * moveSpeed;
        // Calculate difference between current and target speed
        float speedDif = targetSpeed - rb.velocity.x;
        // Calculate acceleration rate
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Apply acceleration
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    private void HandleJump()
    {
        // Coyote time
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Start jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }

        // Variable jump height (hold to jump higher)
        if (isJumping)
        {
            if (jumpInput && jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Jump cut (early release makes you fall faster)
        if (jumpInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            isJumping = false;
        }
    }

    private void HandlePhysics()
    {
        // Adjust gravity scale based on jump state
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravityScale;
        }
        else if (jumpInput && rb.velocity.y > 0)
        {
            rb.gravityScale = jumpGravityScale;
        }
        else
        {
            rb.gravityScale = fallGravityScale;
        }
    }

    private void FlipCharacter()
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.GetChild(0).localScale;
            scale.x *= -1;
            transform.GetChild(0).localScale = scale;
        }
    }

    private void UpdateAnimations()
    {
        // Running animation
        bool isRunning = Mathf.Abs(moveInput) > 0.1f && IsGrounded();
        animator.SetBool("Move", isRunning);
        bool isJumpingAnim = !IsGrounded() && rb.velocity.y > 0.1f;
        animator.SetBool("IsJumping", isJumpingAnim);

        // Falling animation
        bool isFallingAnim = !IsGrounded() && rb.velocity.y < -0.1f;
        animator.SetBool("IsFalling", isFallingAnim);

        // Grounded animation
        animator.SetBool("IsGrounded", IsGrounded());
    }

    public bool IsGrounded()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapBox(checkPosition, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
    }
}