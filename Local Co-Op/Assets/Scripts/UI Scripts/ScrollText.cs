using UnityEngine;
using UnityEngine.UI; // Include for handling images

public class ScrollingElement : MonoBehaviour
{
    public float scrollSpeed = 100f; // Speed of scrolling
    public RectTransform targetRect;  // Assign the RectTransform of the element (text or image)
    public RectTransform duplicateRect; // Second copy for looping

    private float elementWidth;

    private void Start()
    {
        if (targetRect == null || duplicateRect == null)
        {
            Debug.LogError("Assign both targetRect and duplicateRect in the Inspector!");
            return;
        }

        elementWidth = targetRect.rect.width;

        // Position the duplicate to start after the original
        duplicateRect.anchoredPosition = new Vector2(targetRect.anchoredPosition.x + elementWidth, targetRect.anchoredPosition.y);
    }

    private void Update()
    {
        // Move both target and duplicate elements
        float moveAmount = scrollSpeed * Time.deltaTime;
        targetRect.anchoredPosition += Vector2.left * moveAmount;
        duplicateRect.anchoredPosition += Vector2.left * moveAmount;

        // Reset position when the original element moves off the screen
        if (targetRect.anchoredPosition.x <= -elementWidth)
        {
            targetRect.anchoredPosition = new Vector2(duplicateRect.anchoredPosition.x + elementWidth, targetRect.anchoredPosition.y);
        }

        // Reset position for the duplicate when it moves off the screen
        if (duplicateRect.anchoredPosition.x <= -elementWidth)
        {
            duplicateRect.anchoredPosition = new Vector2(targetRect.anchoredPosition.x + elementWidth, duplicateRect.anchoredPosition.y);
        }
    }
}
