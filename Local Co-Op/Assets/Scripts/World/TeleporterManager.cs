// Title: Unity 2D Teleport System 
// Author: Rehope Games
// Date: 23 March  2025
// Link: https://youtu.be/kswg8AsHFIs?si=y5DFuVIeyKQSMN0M


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
    
  
    private bool isOnCooldown = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (isOnCooldown) return;
        
       
        if (chargeManager == null || !chargeManager.IsTeleporterActive || !chargeManager.CanTeleport())
            return;
            
        if (linkedTeleporter == null || linkedTeleporter.exitPoint == null)
            return;
        
     
        Transform rootTransform = FindRootPlayer(collision.transform);
        if (rootTransform == null) return;
        
     
        GameObject playerObj = rootTransform.gameObject;
        PlayerTagger playerTagger = playerObj.GetComponent<PlayerTagger>();
            
        if (playerTagger != null)
        {
     
            StartCoroutine(TeleportPlayerSafely(playerObj, playerTagger));
        }
        else
        {
           
            TeleportObject(playerObj);
        }
        
       
        StartCoroutine(ApplyCooldown(0.5f));
        
        
        chargeManager.UseCharge();
    }
    
  
    private Transform FindRootPlayer(Transform colliderTransform)
    {
        
        Transform current = colliderTransform;
        
        while (current != null)
        {
            if (current.GetComponent<PlayerTagger>() != null)
            {
                return current;
            }
            
            
            current = current.parent;
        }
        
        
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
        
        if (tagger.tagTrigger != null)
            tagger.tagTrigger.enabled = false;
            
    
        TeleportObject(player);
        
       
        yield return new WaitForSeconds(teleportationCooldown);
        
     
        if (tagger.tagTrigger != null)
            tagger.tagTrigger.enabled = true;
    }
    
    private void TeleportObject(GameObject obj)
    {
      
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        
      
        obj.transform.position = linkedTeleporter.exitPoint.position;
        
        
        if (rb != null)
            rb.linearVelocity = velocity;
    }
}