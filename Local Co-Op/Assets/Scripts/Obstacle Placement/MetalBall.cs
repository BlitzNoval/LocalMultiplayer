using UnityEngine;

public class MetalBall : MonoBehaviour
{
    // ----- Swing Settings -----
    [Header("Swing Settings")]
    [Tooltip("Swing amplitude in degrees.")]
    public float amplitude = 30f;

    [Tooltip("Speed of swinging.")]
    public float swingSpeed = 2f;

    [Tooltip("Offset in the swing phase (useful if multiple balls).")]
    public float phaseOffset = 0f;

    // ----- Collision Settings -----
    [Header("Collision Settings")]
    [Tooltip("Impulse force applied to the player upon collision.")]
    public float ballForce = 10f;

    private float _initialAngle;

    // Store the starting angle (in degrees) so the swing oscillates about that value.
    private void Start()
    {
        _initialAngle = transform.eulerAngles.z;
    }

    // Rotate the pivot so that the child ball swings back and forth.
    private void Update()
    {
        float angle = _initialAngle + amplitude * Mathf.Sin(Time.time * swingSpeed + phaseOffset);
        Vector3 euler = transform.eulerAngles;
        euler.z = angle;
        transform.eulerAngles = euler;
    }

    // When the ball collides with a player, apply an impulse force based on the collision normal.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verify that the collided object is a player (ensure players are tagged "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Use the first contact point's normal to determine the direction of impact.
                Vector2 collisionNormal = collision.contacts[0].normal;
                // Apply an impulse force to push the player away.
                playerRb.AddForce(collisionNormal * ballForce, ForceMode2D.Impulse);
            }
        }
    }
}
