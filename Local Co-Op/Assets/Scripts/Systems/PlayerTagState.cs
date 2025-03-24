using UnityEngine;

public class PlayerTagState : MonoBehaviour
{
    [Header("Player Role")]
    public bool isTagger = false;  // True if this player is the tagger
    public bool inGracePeriod = false; // Prevents instant re-tagging

    [Header("Visual Indicators")]
    public GameObject taggerIndicator; // Crown image (assign in Inspector)
    public GameObject runnerIndicator; // Runner image (assign in Inspector)
    public GameObject shieldObject; // Shield visual (assign in Inspector)

    void Start()
    {
        UpdateIndicator();
    }

    // Updates which indicator is active based on the role
    public void UpdateIndicator()
    {
        if (taggerIndicator != null && runnerIndicator != null)
        {
            taggerIndicator.SetActive(isTagger);
            runnerIndicator.SetActive(!isTagger);
        }
    }

    // Enables the shield to prevent immediate re-tagging
    public void ActivateShield(float duration)
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(true); // Show shield
            inGracePeriod = true;
            Invoke("DeactivateShield", duration); // Schedule disabling
        }
    }

    void DeactivateShield()
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(false); // Hide shield
        }
        inGracePeriod = false;
    }
}
