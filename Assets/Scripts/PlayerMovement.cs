using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalInput;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Sliding")]
    public float slideBoost = 8f;
    public float slideDecay = 10f;
    public float slideCooldown = 1f;
    private bool isSliding;
    private float slideTimer;
    private bool canSlide = true;

    [Header("Wall Jumping")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;

    [Header("Wall Sliding")]
    public float wallSlideSpeed = 1f;
    private bool isTouchingWall;
    private bool isWallSliding;

    [Header("Wall Jump Lockout")]
    public float wallJumpLockDuration = 0.2f;
    private bool isWallJumping = false;
    private float wallJumpTimer;

    [Header("Faster Falling")]
    public float fastFallMultiplier = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float originalGravityScale;

    private bool facingRight = true;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        jump();
        slide();
        wallSlide();
        quickFall();
        spriteFlip();
        wallJumpOvertake();

        // Animator: Set isRunning when moving on ground and not sliding
        bool isRunning = Mathf.Abs(horizontalInput) > 0.01f && isGrounded && !isSliding;
        animator.SetBool("isRunning", isRunning);
    }

    private void FixedUpdate()
    {
        if (!isSliding && !isWallJumping)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        if (isSliding)
        {
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0f, slideDecay * Time.fixedDeltaTime), rb.velocity.y);
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isTouchingWall && !isGrounded)
            {
                float wallSide = wallCheck.position.x > transform.position.x ? 1f : -1f;
                rb.velocity = new Vector2(-wallSide * wallJumpForceX, wallJumpForceY);

                isWallJumping = true;
                wallJumpTimer = wallJumpLockDuration;

                if ((-wallSide > 0 && !facingRight) || (-wallSide < 0 && facingRight))
                    Flip();
            }
        }
    }

    private void slide()
    {
        slideTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && canSlide)
        {
            isSliding = true;
            canSlide = false;
            slideTimer = slideCooldown;

            float slideDirection = Mathf.Sign(horizontalInput);
            if (slideDirection == 0) slideDirection = facingRight ? 1 : -1;

            rb.velocity = new Vector2(slideDirection * slideBoost, rb.velocity.y);
            Invoke(nameof(EndSlide), 0.5f);
            Invoke(nameof(ResetSlideCooldown), slideCooldown);
        }
    }

    private void EndSlide()
    {
        isSliding = false;
    }

    private void ResetSlideCooldown()
    {
        canSlide = true;
    }

    private void wallSlide()
    {
        isWallSliding = !isGrounded && isTouchingWall && rb.velocity.y < 0 && !isWallJumping;
    }

    private void quickFall()
    {
        if (!isGrounded && Input.GetKey(KeyCode.S))
        {
            rb.gravityScale = originalGravityScale * fastFallMultiplier;
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }
    }

    private void wallJumpOvertake()
    {
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
                isWallJumping = false;
        }
    }

    private void spriteFlip()
    {
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}
