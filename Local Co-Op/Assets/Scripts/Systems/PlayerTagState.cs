using UnityEngine;

public class PlayerTagState : MonoBehaviour
{
    [Header("Player Role")]
    public bool isTagger = false;  
    public bool inGracePeriod = false; 

    [Header("Visual Indicators")]
    public GameObject taggerIndicator;
    public GameObject runnerIndicator; 
    public GameObject shieldObject; 

    void Start()
    {
        UpdateIndicator();
    }

   
    public void UpdateIndicator()
    {
        if (taggerIndicator != null && runnerIndicator != null)
        {
            taggerIndicator.SetActive(isTagger);
            runnerIndicator.SetActive(!isTagger);
        }
    }

    
    public void ActivateShield(float duration)
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(true);
            inGracePeriod = true;
            Invoke("DeactivateShield", duration); 
        }
    }

    void DeactivateShield()
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(false); 
        }
        inGracePeriod = false;
    }
}
