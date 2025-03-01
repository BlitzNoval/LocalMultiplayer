using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Player speed
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private string playerID;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by the Input System (New Input System sends messages to functions with matching names)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

}
