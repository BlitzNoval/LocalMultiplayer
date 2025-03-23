using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [Tooltip("Direction in which the conveyor moves.")]
    public Vector2 conveyorDirection = Vector2.right;

    [Tooltip("Speed of conveyor movement.")]
    public float conveyorSpeed = 2f;

    private void OnTriggerStay2D(Collider2D other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller != null)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Option A: Override the player's horizontal velocity
                // Keep their vertical velocity the same
                rb.linearVelocity = new Vector2(conveyorDirection.normalized.x * conveyorSpeed, rb.linearVelocity.y);

                // Option B: Add force (comment out Option A above)
                // rb.AddForce(conveyorDirection.normalized * conveyorSpeed, ForceMode2D.Force);
            }
        }
    }
}
