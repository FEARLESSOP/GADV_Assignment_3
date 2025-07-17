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
    public float slideSpeed;
    public float slideFriction = 2f;
    private int slideDirection;

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
                return;
            }

            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);

            slideSpeed = Mathf.MoveTowards(slideSpeed, 0f, slideFriction * Time.deltaTime);

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0 || Mathf.Abs(slideSpeed) < 0.1f)
            {
                EndSlide();
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

        slideDirection = moveInput != 0 ? (int)Mathf.Sign(moveInput) : facingDirection;

        transform.localScale = new Vector3(crouchScale.x * slideDirection, crouchScale.y, 1f);

        slideSpeed = slideForce * slideDirection;
    }


    void EndSlide()
    {
        isSliding = false;
        transform.localScale = new Vector3(normalScale.x * facingDirection, normalScale.y, 1f);
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
            rb.velocity = new Vector2(facingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }


    void StopWallJumping()
    {
        isWallJumping = false;
    }

}

