// Title: WOW THIS IS HARD 
// Author: ChatGPT
// Date: 22 March  2025
// This script refused to work with 2 players no matter what I idid , it refused to work when we added the tileset , it was very annoying to debug 


using UnityEngine;
using System.Collections;
using TMPro;

public class TeleporterChargeManager : MonoBehaviour
{
    [Header("Level Event Manager")]
    public LevelEventManager levelEventManager;

    [Header("Recharge Settings")]
    [Tooltip("Time in seconds between recharge events (adds 25% if not full)")]
    public float rechargeInterval = 10f;

    [Header("Charge Display")]
    public TextMeshProUGUI chargeText;

    [Header("Charge Visuals")]
    public GameObject emptyIndicator;  
    public GameObject charge25Indicator;
    public GameObject charge50Indicator;   
    public GameObject charge75Indicator;   
    public GameObject charge100Indicator;  

 
    private float chargeLevel = 0f;
   
    private bool activated = false;
 
    public bool IsTeleporterActive { get { return activated; } }
 
    private Coroutine rechargeCoroutine;

    private void Start()
    {
  
        if (levelEventManager != null)
        {
            levelEventManager.OnActivateEvent += ActivateTeleporter;
            levelEventManager.OnResetEvent += ResetTeleporter;
        }

       
        chargeLevel = (levelEventManager != null) ? levelEventManager.GetTimerProgress() : 0f;
        UpdateChargeUI();
    }

    private void Update()
    {
       
        if (!activated && levelEventManager != null)
        {
            chargeLevel = levelEventManager.GetTimerProgress();
            UpdateChargeUI();
        }
    }


    private void ActivateTeleporter()
    {
        activated = true;
        chargeLevel = 1.0f;
        UpdateChargeUI();
        // Start the periodic recharge routine if not already running.
        if (rechargeCoroutine == null)
            rechargeCoroutine = StartCoroutine(PeriodicRecharge());
    }

    private void ResetTeleporter()
    {
        activated = false;
        // Stop the recharge routine.
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
            rechargeCoroutine = null;
        }
        // Reset charge back to 0 (or level timer progress, if desired)
        chargeLevel = 0f;
        UpdateChargeUI();
    }

    
    public bool CanTeleport()
    {
        return chargeLevel >= 0.25f;
    }

    
    public void UseCharge()
    {
        if (chargeLevel >= 0.25f)
        {
            Debug.Log("Teleporter used. Charge event logged.");
            chargeLevel -= 0.25f;
            if (chargeLevel < 0f)
                chargeLevel = 0f;
            UpdateChargeUI();
        }
        else
        {
            Debug.Log("Not enough charge to teleport.");
        }
    }

   
    private void UpdateChargeUI()
    {
        if (chargeText != null)
        {
            int percentage = Mathf.RoundToInt(chargeLevel * 100f);
            chargeText.text = percentage + "%";
        }

        // Update visual indicator tiles
        emptyIndicator.SetActive(false);
        charge25Indicator.SetActive(false);
        charge50Indicator.SetActive(false);
        charge75Indicator.SetActive(false);
        charge100Indicator.SetActive(false);

        if (chargeLevel < 0.25f)
            emptyIndicator.SetActive(true);
        else if (chargeLevel < 0.5f)
            charge25Indicator.SetActive(true);
        else if (chargeLevel < 0.75f)
            charge50Indicator.SetActive(true);
        else if (chargeLevel < 1.0f)
            charge75Indicator.SetActive(true);
        else
            charge100Indicator.SetActive(true);
    }

  
    private IEnumerator PeriodicRecharge()
    {
        while (true)
        {
            yield return new WaitForSeconds(rechargeInterval);
            if (chargeLevel < 1.0f)
            {
                Debug.Log("Recharge event logged: Replenishing 25% charge.");
                yield return StartCoroutine(RechargeAnimation());
            }
        }
    }

   
    private IEnumerator RechargeAnimation()
    {
        float targetCharge = Mathf.Min(chargeLevel + 0.25f, 1.0f);
        int currentPercentage = Mathf.RoundToInt(chargeLevel * 100);
        int targetPercentage = Mathf.RoundToInt(targetCharge * 100);

        while (currentPercentage < targetPercentage)
        {
            currentPercentage += 1;
            chargeLevel = currentPercentage / 100f;
            UpdateChargeUI();
            yield return new WaitForSeconds(0.05f); 
        }
    }
}
