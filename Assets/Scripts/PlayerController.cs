using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
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

    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

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

    [Header("Crouching")]
    public float crouchSpeedMultiplier = 0.5f;
    private bool isCrouching = false;

    [Header("Particles")]
    [SerializeField] private ParticleSystem dirtParticle;
    private Vector2 particleStartPos;

    private BoxCollider2D boxCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    public Vector2 crouchColliderSize = new Vector2(0.25f, 0.15f);
    public Vector2 crouchColliderOffset = new Vector2(0f, -0.05f);

    private Rigidbody2D rb;
    private bool isGrounded;
    private float originalGravityScale;

    private bool facingRight = true;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        originalGravityScale = rb.gravityScale;

        particleStartPos = dirtParticle.transform.localPosition;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        HandleCrouch();
        HandleJump();
        HandleSlide();
        HandleWallSlide();
        HandleQuickFall();
        HandleSpriteFlip();
        UpdateWallJumpTimer();
        UpdateAnimator();
        startStopParticles();
    }

    private void FixedUpdate()
    {
        if (!isSliding)
        {
            if (isWallJumping && wallJumpTimer > 0f)
            {
            }
            else
            {
                float targetSpeed = moveSpeed;
                if (isCrouching) targetSpeed *= crouchSpeedMultiplier;

                float targetVelocityX = horizontalInput * targetSpeed;
                float smoothing = isGrounded ? 1f : 0.1f;
                float newVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, smoothing);
                rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
            }
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

    private void startStopParticles()
    {
        bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

        if (isGrounded && isMovingHorizontally && !isSliding && !isCrouching)
        {
            if (!dirtParticle.isPlaying)
                dirtParticle.Play();
        }
        else
        {
            if (dirtParticle.isPlaying)
                dirtParticle.Stop();
        }
    }


    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded && !isSliding)
        {
            isCrouching = true;

            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(0.2264847f, 0.11f);
                boxCollider.offset = originalColliderOffset;
            }
        }
        else
        {
            isCrouching = false;

            if (boxCollider != null)
            {
                boxCollider.size = originalColliderSize;
                boxCollider.offset = originalColliderOffset;
            }
        }

        animator.SetBool("isCrouching", isCrouching);
    }


    private void HandleJump()
    {
        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                return;
            }
            else if (isTouchingWall && !isGrounded)
            {
                float wallSide = wallCheck.position.x > transform.position.x ? 1f : -1f;
                rb.velocity = new Vector2(-wallSide * wallJumpForceX, wallJumpForceY);

                isWallJumping = true;
                wallJumpTimer = wallJumpLockDuration;

                if ((-wallSide > 0 && !facingRight) || (-wallSide < 0 && facingRight))
                {
                    Flip();
                }

                jumpBufferCounter = 0f;
                return;
            }
        }
    }

    private void HandleSlide()
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

            transform.rotation = Quaternion.Euler(0, 0, 90f * -slideDirection);

            animator.SetBool("isFalling", true);

            Invoke(nameof(EndSlide), 0.5f);
            Invoke(nameof(ResetSlideCooldown), slideCooldown);
        }
    }

    private void EndSlide()
    {
        isSliding = false;

        transform.rotation = Quaternion.identity;

        if (isGrounded)
            animator.SetBool("isFalling", false);
    }


    private void ResetSlideCooldown()
    {
        canSlide = true;
    }

    private void HandleWallSlide()
    {
        isWallSliding = !isGrounded && isTouchingWall && rb.velocity.y < 0 && !isWallJumping;
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private void HandleQuickFall()
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

    private void UpdateWallJumpTimer()
    {
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }
    }

    private void HandleSpriteFlip()
    {
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
            dirtParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            dirtParticle.transform.localPosition = particleStartPos;
            dirtParticle.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            dirtParticle.Play();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
            Vector2 particlePos = particleStartPos;
            particlePos.x *= -1f;
            dirtParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            dirtParticle.transform.localPosition = particlePos;
            dirtParticle.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            dirtParticle.Play();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void UpdateAnimator()
    {
        bool isRunning = Mathf.Abs(horizontalInput) > 0.01f && isGrounded && !isSliding && !isCrouching;
        animator.SetBool("isRunning", isRunning);

        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}