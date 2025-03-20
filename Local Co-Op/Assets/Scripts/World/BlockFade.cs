using UnityEngine;
using System.Collections;

public class BlockFade : MonoBehaviour
{
    [Header("Effect Settings")]
    public bool useShrinkEffect = true; // True for shrink effect, false for flash effect
    
    [Header("Timing Settings")]
    public float standDuration = 2f; // Time before disappearing after stepping on it
    public float regenDuration = 3f; // Time to reappear after disappearing
    
    [Header("Shrink Effect Settings")]
    public float shrinkDuration = 2f; // Time to fully shrink before disappearing
    public float growDuration = 1.5f; // Time to grow back to original size
    
    [Header("Flash Effect Settings")]
    public float maxFlashInterval = 0.5f; // Initial flash speed
    public float minFlashInterval = 0.05f; // Fastest flash speed before disappearing
    
    private bool isActive = true; // Prevents reactivation until fully restored
    private SpriteRenderer blockSprite;
    private Collider2D blockCollider;
    private Vector3 initialScale;

    private void Start()
    {
        blockSprite = GetComponent<SpriteRenderer>();
        blockCollider = GetComponent<Collider2D>();
        initialScale = transform.localScale;
        
        if (blockSprite != null)
        {
            blockSprite.enabled = true;
        }
        if (blockCollider != null)
        {
            blockCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && blockSprite != null && blockCollider != null)
        {
            isActive = false;
            StopAllCoroutines();
            if (useShrinkEffect)
                StartCoroutine(ShrinkBeforeDisappearing());
            else
                StartCoroutine(FlashBeforeDisappearing());
        }
    }

    private IEnumerator ShrinkBeforeDisappearing()
    {
        float elapsedTime = 0f;
        Vector3 startScale = initialScale;
        Vector3 endScale = Vector3.zero;
        
        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = endScale;
        blockSprite.enabled = false;
        blockCollider.enabled = false;
        
        yield return new WaitForSeconds(regenDuration);
        StartCoroutine(GrowBack());
    }

    private IEnumerator FlashBeforeDisappearing()
    {
        float elapsedTime = 0f;
        while (elapsedTime < standDuration)
        {
            float t = elapsedTime / standDuration;
            float currentFlashInterval = Mathf.Lerp(maxFlashInterval, minFlashInterval, t);
            blockSprite.enabled = !blockSprite.enabled;
            yield return new WaitForSeconds(currentFlashInterval);
            elapsedTime += currentFlashInterval;
        }
        
        blockSprite.enabled = false;
        blockCollider.enabled = false;
        
        yield return new WaitForSeconds(regenDuration);
        StartCoroutine(GrowBack());
    }

    private IEnumerator GrowBack()
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = initialScale;
        blockSprite.enabled = true;
        
        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = endScale;
        blockCollider.enabled = true;
        isActive = true;
    }
}
