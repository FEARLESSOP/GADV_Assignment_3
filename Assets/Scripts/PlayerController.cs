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
    private CapsuleCollider2D capsuleCollider;
    private Vector2 originalCapsuleSize = new Vector2(0.1452234f, 0.2561355f);
    public Vector2 crouchCapsuleSize = new Vector2(0.1452234f, 0.1018364f);

    [Header("Particles")]
    [SerializeField] private ParticleSystem dirtParticle;
    private Vector2 particleStartPos;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float originalGravityScale;

    private bool facingRight = true;
    private Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;


    private void Start()
    {
        //get components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        //save original gravity and particle position
        originalGravityScale = rb.gravityScale;
        particleStartPos = dirtParticle.transform.localPosition;
    }

    private void Update()
    {
        //get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //check ground and wall status
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        //update coyote time counter
        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        //update jump buffer counter
        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        //handle all actions
        HandleCrouch();
        HandleJump();
        HandleSlide();
        HandleWallSlide();
        HandleQuickFall();
        HandleSpriteFlip();
        UpdateWallJumpTimer();
        UpdateAnimator();
        StartStopParticles();
    }

    private void FixedUpdate()
    {
        //move player if not sliding
        if (!isSliding)
        {
            if (!isWallJumping || wallJumpTimer <= 0f)
            {
                float targetSpeed = moveSpeed;
                if (isCrouching) targetSpeed *= crouchSpeedMultiplier;

                float targetVelocityX = horizontalInput * targetSpeed;
                float smoothing = isGrounded ? 1f : 0.1f;
                float newVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, smoothing);
                rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
            }
        }

        //apply slide decay
        if (isSliding)
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0f, slideDecay * Time.fixedDeltaTime), rb.velocity.y);

        //limit wall slide speed
        if (isWallSliding)
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    private void StartStopParticles()
    {
        //play dirt particles when moving on ground
        bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

        if (isGrounded && isMovingHorizontally && !isSliding && !isCrouching)
        {
            if (!dirtParticle.isPlaying) dirtParticle.Play();
        }
        else
        {
            if (dirtParticle.isPlaying) dirtParticle.Stop();
        }
    }

    private void HandleCrouch()
    {
        //crouch while holding ctrl on ground
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded && !isSliding)
        {
            isCrouching = true;
            if (capsuleCollider != null) capsuleCollider.size = crouchCapsuleSize;
        }
        else
        {
            isCrouching = false;
            if (capsuleCollider != null) capsuleCollider.size = originalCapsuleSize;
        }

        animator.SetBool("isCrouching", isCrouching);
    }

    private void HandleJump()
    {
        //check jump buffer and coyote time
        if (jumpBufferCounter > 0f)
        {
            //normal jump
            if (coyoteTimeCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                PlayJumpSound();
                return;
            }
            //wall jump
            else if (isTouchingWall && !isGrounded)
            {
                float wallSide = wallCheck.position.x > transform.position.x ? 1f : -1f;
                rb.velocity = new Vector2(-wallSide * wallJumpForceX, wallJumpForceY);

                isWallJumping = true;
                wallJumpTimer = wallJumpLockDuration;

                if ((-wallSide > 0 && !facingRight) || (-wallSide < 0 && facingRight))
                    Flip();

                jumpBufferCounter = 0f;
                PlayJumpSound();
                return;
            }
        }
    }

    private void PlayJumpSound()
    {
        //play jump sound with SFX volume
        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound, GameManager.sfxVolume);
    }

    private void HandleSlide()
    {
        slideTimer -= Time.deltaTime;

        //start slide on shift if grounded
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

            Invoke(nameof(EndSlide), 1f);
            Invoke(nameof(ResetSlideCooldown), slideCooldown);
        }
    }

    private void EndSlide()
    {
        //stop slide and reset rotation
        isSliding = false;
        transform.rotation = Quaternion.identity;

        if (isGrounded)
            animator.SetBool("isFalling", false);
    }

    private void ResetSlideCooldown()
    {
        //allow sliding again
        canSlide = true;
    }

    private void HandleWallSlide()
    {
        //slide down wall when touching it in air
        isWallSliding = !isGrounded && isTouchingWall && rb.velocity.y < 0 && !isWallJumping;
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private void HandleQuickFall()
    {
        //increase fall speed if holding S
        if (!isGrounded && Input.GetKey(KeyCode.S))
            rb.gravityScale = originalGravityScale * fastFallMultiplier;
        else
            rb.gravityScale = originalGravityScale;
    }

    private void UpdateWallJumpTimer()
    {
        //count down wall jump lock
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0) isWallJumping = false;
        }
    }

    private void HandleSpriteFlip()
    {
        //flip sprite when changing direction
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
            ResetParticles(true);
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
            ResetParticles(false);
        }
    }

    private void ResetParticles(bool facingRight)
    {
        //reset dirt particle position/rotation
        Vector2 particlePos = particleStartPos;
        if (!facingRight) particlePos.x *= -1f;

        dirtParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dirtParticle.transform.localPosition = particlePos;
        dirtParticle.transform.rotation = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
        dirtParticle.Play();
    }

    private void Flip()
    {
        //reverse player facing direction
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void UpdateAnimator()
    {
        //update running state
        bool isRunning = Mathf.Abs(horizontalInput) > 0.01f && isGrounded && !isSliding && !isCrouching;
        animator.SetBool("isRunning", isRunning);

        //update jumping and falling states
        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
    }

    private void OnDrawGizmosSelected()
    {
        //draw ground and wall check spheres in editor
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        if (wallCheck != null) Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}
