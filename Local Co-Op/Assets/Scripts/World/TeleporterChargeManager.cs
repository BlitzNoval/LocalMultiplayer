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
    public GameObject emptyIndicator;    // 0% charge
    public GameObject charge25Indicator;   // 25% charge
    public GameObject charge50Indicator;   // 50% charge
    public GameObject charge75Indicator;   // 75% charge
    public GameObject charge100Indicator;  // 100% charge

    // Charge is stored as a value between 0 and 1 (1 = 100%)
    private float chargeLevel = 0f;
    // This flag indicates if the teleporter system has been activated (doors closed)
    private bool activated = false;
    // Expose whether the teleporter is active
    public bool IsTeleporterActive { get { return activated; } }
    // Keep a handle to the periodic recharge coroutine so we can stop it on reset.
    private Coroutine rechargeCoroutine;

    private void Start()
    {
        // Subscribe to LevelEventManager events if assigned
        if (levelEventManager != null)
        {
            levelEventManager.OnActivateEvent += ActivateTeleporter;
            levelEventManager.OnResetEvent += ResetTeleporter;
        }

        // Before activation, use the level timer progress as the charge (typically less than 100%)
        chargeLevel = (levelEventManager != null) ? levelEventManager.GetTimerProgress() : 0f;
        UpdateChargeUI();
    }

    private void Update()
    {
        // While not activated, continuously update the charge level based on the level timer progress.
        if (!activated && levelEventManager != null)
        {
            chargeLevel = levelEventManager.GetTimerProgress();
            UpdateChargeUI();
        }
    }

    /// <summary>
    /// Called when LevelEventManager triggers activation (doors closed).
    /// Sets charge to full (100%) and starts periodic recharge.
    /// </summary>
    private void ActivateTeleporter()
    {
        activated = true;
        chargeLevel = 1.0f;
        UpdateChargeUI();
        // Start the periodic recharge routine if not already running.
        if (rechargeCoroutine == null)
            rechargeCoroutine = StartCoroutine(PeriodicRecharge());
    }

    /// <summary>
    /// Called when LevelEventManager resets the level.
    /// Resets activation state and charge.
    /// </summary>
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

    /// <summary>
    /// Returns true if there is at least 25% charge available.
    /// </summary>
    public bool CanTeleport()
    {
        return chargeLevel >= 0.25f;
    }

    /// <summary>
    /// Deducts 25% charge (if available) and updates the UI.
    /// </summary>
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

    /// <summary>
    /// Updates the shared TextMeshProUGUI display and the visual indicator tiles.
    /// </summary>
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

    /// <summary>
    /// Every rechargeInterval seconds, if charge is below 100% then recharge 25% 
    /// with a gradual visual animation (in 1% increments).
    /// </summary>
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

    /// <summary>
    /// Gradually increases the charge by 1% increments until 25% is replenished or full charge is reached.
    /// </summary>
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
            yield return new WaitForSeconds(0.05f); // adjust delay for smoother/faster animation
        }
    }
}
