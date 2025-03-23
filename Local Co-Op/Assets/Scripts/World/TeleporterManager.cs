using UnityEngine;
using System.Collections;
using TMPro;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Teleporter Setup")]
    public SimpleTeleporter linkedTeleporter;
    public Transform exitPoint;

    [Header("Charge Settings")]
    [Tooltip("Time in seconds to recharge 25% after activation")]
    public float rechargeTimePerLevel = 10f;
    public GameObject emptyIndicator;   // 0% charge
    public GameObject charge25Indicator; // 25% charge
    public GameObject charge50Indicator; // 50% charge
    public GameObject charge75Indicator; // 75% charge
    public GameObject charge100Indicator; // 100% charge

    [Header("Charge Display")]
    public TextMeshProUGUI chargeText;

    [Header("Level Event Manager")]
    public LevelEventManager levelEventManager;

    private float chargeLevel = 0.0f;
    private bool isActivated = false;
    private bool isRecharging = false;

    private void Start()
    {
        chargeLevel = 0.0f;
        UpdateChargeIndicators();
        UpdateChargeText();
        if (levelEventManager != null)
        {
            levelEventManager.OnActivateEvent += ActivateTeleporter;
            levelEventManager.OnResetEvent += ResetTeleporter;
        }
    }

    private void Update()
    {
        if (!isActivated && levelEventManager != null)
        {
            chargeLevel = levelEventManager.GetTimerProgress();
            UpdateChargeIndicators();
            UpdateChargeText();
        }
    }

    public void ActivateTeleporter()
    {
        isActivated = true;
        chargeLevel = 1.0f;
        UpdateChargeIndicators();
        UpdateChargeText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated || chargeLevel <= 0 || linkedTeleporter == null || linkedTeleporter.exitPoint == null)
            return;

        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Vector2 velocity = Vector2.zero;
        if (rb != null)
        {
            velocity = rb.linearVelocity;
        }
        collision.transform.position = linkedTeleporter.exitPoint.position;
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
        UseCharge();
    }

    private void UseCharge()
    {
        chargeLevel -= 0.25f;
        if (chargeLevel < 0)
            chargeLevel = 0;
        UpdateChargeIndicators();
        UpdateChargeText();
        if (!isRecharging && chargeLevel < 1.0f)
        {
            StartCoroutine(RechargeOverTime());
        }
    }

    private IEnumerator RechargeOverTime()
    {
        isRecharging = true;
        while (chargeLevel < 1.0f)
        {
            yield return new WaitForSeconds(rechargeTimePerLevel);
            chargeLevel += 0.25f;
            if (chargeLevel > 1.0f)
                chargeLevel = 1.0f;
            UpdateChargeIndicators();
            UpdateChargeText();
        }
        isRecharging = false;
    }

    private void UpdateChargeIndicators()
    {
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

    private void UpdateChargeText()
    {
        if (chargeText != null)
        {
            int percentage = Mathf.RoundToInt(chargeLevel * 100f);
            chargeText.text = percentage + "%";
        }
    }

    public void ResetTeleporter()
    {
        isActivated = false;
        chargeLevel = 0.0f;
        StopAllCoroutines();
        isRecharging = false;
        UpdateChargeIndicators();
        UpdateChargeText();
    }
}