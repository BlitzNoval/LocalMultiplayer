using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceStrength = 10f; // Controls the bounce force
    public bool affectX = false; // Determines if it affects the X-axis
    public bool affectY = true; // Determines if it affects the Y-axis
    public bool overrideVelocity = false; // Whether to reset velocity before applying force

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            Vector2 bounceForce = Vector2.zero;
            
            if (affectX)
            {
                bounceForce.x = bounceStrength * Mathf.Sign(rb.linearVelocity.x);
            }
            
            if (affectY)
            {
                bounceForce.y = bounceStrength;
            }
            
            if (overrideVelocity)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            rb.AddForce(bounceForce, ForceMode2D.Impulse);
        }
    }
}