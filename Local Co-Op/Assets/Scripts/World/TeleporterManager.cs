using UnityEngine;
using System.Collections;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Teleporter Setup")]
    public SimpleTeleporter linkedTeleporter;
    public Transform exitPoint;

    [Header("Charge Settings")]
    public GameObject emptyIndicator;   // 0% charge
    public GameObject charge25Indicator; // 25% charge
    public GameObject charge50Indicator; // 50% charge
    public GameObject charge75Indicator; // 75% charge
    public GameObject charge100Indicator; // 100% charge

    private float chargeLevel = 1.0f; // 1.0 = 100%, 0 = empty
    private bool isRecharging = false;

    private void Start()
    {
        // Initialize teleporter visuals
        UpdateChargeIndicators();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we have charge and a linked teleporter
        if (chargeLevel > 0 && linkedTeleporter != null && linkedTeleporter.exitPoint != null)
        {
            // Preserve momentum for any object with a Rigidbody2D
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            Vector2 velocity = Vector2.zero;
            
            if (rb != null)
            {
                velocity = rb.linearVelocity;
            }
            
            // Teleport to the linked teleporter's exit point
            collision.transform.position = linkedTeleporter.exitPoint.position;
            
            // Restore momentum
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
            
            // Reduce charge
            UseCharge();
        }
    }
    
    private void UseCharge()
    {
        // Reduce charge by 25%
        chargeLevel -= 0.25f;
        if (chargeLevel < 0)
            chargeLevel = 0;
            
        // Update visuals
        UpdateChargeIndicators();
        
        // Start recharging if not already
        if (!isRecharging && chargeLevel < 1.0f)
        {
            StartCoroutine(RechargeOverTime());
        }
    }
    
    private IEnumerator RechargeOverTime()
    {
        isRecharging = true;
        
        // Wait 2.5 seconds before starting to recharge
        yield return new WaitForSeconds(2.5f);
        
        // Recharge in 25% increments every 2.5 seconds
        while (chargeLevel < 1.0f)
        {
            chargeLevel += 0.25f;
            if (chargeLevel > 1.0f)
                chargeLevel = 1.0f;
                
            UpdateChargeIndicators();
            yield return new WaitForSeconds(2.5f);
        }
        
        isRecharging = false;
    }
    
    private void UpdateChargeIndicators()
    {
        // Disable all indicators first
        emptyIndicator.SetActive(false);
        charge25Indicator.SetActive(false);
        charge50Indicator.SetActive(false);
        charge75Indicator.SetActive(false);
        charge100Indicator.SetActive(false);
        
        // Enable the appropriate indicator based on charge level
        if (chargeLevel == 0)
            emptyIndicator.SetActive(true);
        else if (chargeLevel <= 0.25f)
            charge25Indicator.SetActive(true);
        else if (chargeLevel <= 0.5f)
            charge50Indicator.SetActive(true);
        else if (chargeLevel <= 0.75f)
            charge75Indicator.SetActive(true);
        else
            charge100Indicator.SetActive(true);
    }
}