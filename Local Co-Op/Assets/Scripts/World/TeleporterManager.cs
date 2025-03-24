using UnityEngine;
using System.Collections;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Teleporter Setup")]
    public SimpleTeleporter linkedTeleporter;
    public Transform exitPoint;

    [Header("Charge Manager")]
    public TeleporterChargeManager chargeManager;
    
    [Header("Teleportation Settings")]
    [SerializeField] private float teleportationCooldown = 0.2f;
    
    // Cooldown to prevent multiple charges
    private bool isOnCooldown = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Skip if on cooldown
        if (isOnCooldown) return;
        
        // Basic validation
        if (chargeManager == null || !chargeManager.IsTeleporterActive || !chargeManager.CanTeleport())
            return;
            
        if (linkedTeleporter == null || linkedTeleporter.exitPoint == null)
            return;
        
        // Find the root/parent object if this is a child collider
        Transform rootTransform = FindRootPlayer(collision.transform);
        if (rootTransform == null) return;
        
        // Handle teleportation
        GameObject playerObj = rootTransform.gameObject;
        PlayerTagger playerTagger = playerObj.GetComponent<PlayerTagger>();
            
        if (playerTagger != null)
        {
            // This is a player, handle safely with cooldown
            StartCoroutine(TeleportPlayerSafely(playerObj, playerTagger));
        }
        else
        {
            // This is not a player, teleport normally
            TeleportObject(playerObj);
        }
        
        // Start cooldown to prevent multiple charges
        StartCoroutine(ApplyCooldown(0.5f));
        
        // Use charge
        chargeManager.UseCharge();
    }
    
    // Find the root player object from a collider
    private Transform FindRootPlayer(Transform colliderTransform)
    {
        // Check if this object or any parent has a PlayerTagger component
        Transform current = colliderTransform;
        
        while (current != null)
        {
            if (current.GetComponent<PlayerTagger>() != null)
            {
                return current;
            }
            
            // Move up to the parent
            current = current.parent;
        }
        
        // No player found in this hierarchy
        return colliderTransform;
    }
    
    private IEnumerator ApplyCooldown(float duration)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(duration);
        isOnCooldown = false;
    }
    
    private IEnumerator TeleportPlayerSafely(GameObject player, PlayerTagger tagger)
    {
        // Temporarily disable tag collider
        if (tagger.tagTrigger != null)
            tagger.tagTrigger.enabled = false;
            
        // Teleport the player
        TeleportObject(player);
        
        // Short delay before re-enabling tag collider
        yield return new WaitForSeconds(teleportationCooldown);
        
        // Re-enable tag collider
        if (tagger.tagTrigger != null)
            tagger.tagTrigger.enabled = true;
    }
    
    private void TeleportObject(GameObject obj)
    {
        // Save velocity
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        
        // Teleport
        obj.transform.position = linkedTeleporter.exitPoint.position;
        
        // Restore velocity
        if (rb != null)
            rb.linearVelocity = velocity;
    }
}