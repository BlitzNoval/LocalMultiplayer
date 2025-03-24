using UnityEngine;
using System.Collections;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Teleporter Setup")]
    public SimpleTeleporter linkedTeleporter; // The teleporter to exit from
    public Transform exitPoint;

    [Header("Charge Manager")]
    [Tooltip("Reference to the shared TeleporterChargeManager")]
    public TeleporterChargeManager chargeManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (chargeManager == null)
        {
            Debug.LogError("Charge Manager not assigned on " + gameObject.name);
            return;
        }

        // Ensure the teleporter is active (doors have closed) before allowing teleportation.
        if (!chargeManager.IsTeleporterActive)
        {
            Debug.Log("Teleporter is not active yet (doors still open).");
            return;
        }

        // Only allow teleport if there is at least 25% charge available.
        if (!chargeManager.CanTeleport())
        {
            Debug.Log("Not enough charge to teleport.");
            return;
        }

        if (linkedTeleporter == null || linkedTeleporter.exitPoint == null)
            return;

        // Teleport the player.
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        collision.transform.position = linkedTeleporter.exitPoint.position;
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }

        // Subtract 25% charge.
        chargeManager.UseCharge();
    }
}
