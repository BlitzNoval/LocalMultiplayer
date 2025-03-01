using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;   // Speed of movement
    public float jumpForce = 15f;   // Jump strength
    public float acceleration = 8f; // How fast the character reaches top speed
    public float deceleration = 6f; // How fast the character slows down
    public LayerMask groundLayer;   // Layer that defines the ground

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool jumpPressed;
    
    [Header("Ground Check")]
    public Transform groundCheck;   // Empty GameObject at feet position
    public float groundCheckRadius = 0.2f;  // Small radius for detecting ground

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump logic
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        jumpPressed = false; // Reset jump press after applying force
    }

    private void FixedUpdate()
    {
        // Smooth movement with acceleration & deceleration
        float targetSpeed = moveInput.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.1f) ? acceleration : deceleration;
        float movement = speedDiff * accelRate;

        rb.AddForce(new Vector2(movement, 0f), ForceMode2D.Force);
    }

    // Called by the Input System for movement
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Called by the Input System for jumping
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }
}
