using UnityEngine;
using System.Collections;

public class BlockFade : MonoBehaviour
{
   public float fadeDuration = 3f; // Time to fully desaturate
    public Color fullColor = Color.white;
    public Color desaturatedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private SpriteRenderer blockSprite;
    private bool isPlayerInside = false;

    private void Start()
    {
        blockSprite = GetComponent<SpriteRenderer>();
        if (blockSprite != null)
        {
            blockSprite.enabled = false; // Start with the sprite hidden
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (blockSprite != null)
        {
            StopAllCoroutines(); // Stop any existing fade timers
            blockSprite.enabled = true;
            blockSprite.color = fullColor; // Instantly reset to full color
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (blockSprite != null)
        {
            isPlayerInside = false;
            StartCoroutine(FadeOutSprite());
        }
    }

    private IEnumerator FadeOutSprite()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            if (isPlayerInside) yield break; // Stop fading if the player re-enters
            
            blockSprite.color = Color.Lerp(fullColor, desaturatedColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        blockSprite.color = desaturatedColor;
    }
}
