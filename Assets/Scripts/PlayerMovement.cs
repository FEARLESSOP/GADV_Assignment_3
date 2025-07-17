using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float DownForce = 20f;
    private float moveInput;



    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Slide Settings")]
    public float slideForce = 10f;
    public float slideDuration = 0.5f;
    public Vector2 crouchScale = new Vector2(1f, 0.5f);
    public Vector2 normalScale = new Vector2(1f, 1f);

    [Header("Wall Jump Settings")]
    public Transform wallCheck;
    public float wallSlidingSpeed = 2f;
    public bool isWallSliding;

    public bool isWallJumping;
    public float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private bool isSliding;
    private float slideTimer;
    private int facingDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!isSliding)
        {
            Move();
            Jump();

            if (Input.GetKeyDown(KeyCode.S) && isGrounded)
            {
                StartSlide();
            }


            else if (!isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    ForceDown();
                }
            }
        }

        else
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                EndSlide();
            }
            else
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0)
                {
                    EndSlide();
                }
            }
        }

        WallSlide();
        WallJump();

    }

    bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);

    }

    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            facingDirection = moveInput > 0 ? 1 : -1;

            // Flip the character
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDirection;
            transform.localScale = scale;
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        transform.localScale = crouchScale;

        rb.velocity = new Vector2(facingDirection * slideForce, rb.velocity.y);
    }

    void EndSlide()
    {
        isSliding = false;

        transform.localScale = normalScale;
    }
    void ForceDown()
    {
        rb.AddForce(-transform.up * DownForce, ForceMode2D.Impulse);
    }

    void WallSlide()
    {
        if (IsWalled() && !isGrounded && moveInput != 0f)

        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }


    void StopWallJumping()
    {
        isWallJumping = false;
    }

}

