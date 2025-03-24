// Title: Advanced Scrolling Text 
// Author: DarkanVIII
// Date: 15 March  2025
// Link: https://youtu.be/xa94F42j8bA?si=qvMLiGMLXUUAGeOZ

using UnityEngine;
using UnityEngine.UI; 

public class ScrollingElement : MonoBehaviour
{
    public float scrollSpeed = 100f; // Speed of scrolling
    public RectTransform targetRect;  
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

    
        duplicateRect.anchoredPosition = new Vector2(targetRect.anchoredPosition.x + elementWidth, targetRect.anchoredPosition.y);
    }

    private void Update()
    {
       
        float moveAmount = scrollSpeed * Time.deltaTime;
        targetRect.anchoredPosition += Vector2.left * moveAmount;
        duplicateRect.anchoredPosition += Vector2.left * moveAmount;

        
        if (targetRect.anchoredPosition.x <= -elementWidth)
        {
            targetRect.anchoredPosition = new Vector2(duplicateRect.anchoredPosition.x + elementWidth, targetRect.anchoredPosition.y);
        }

       
        if (duplicateRect.anchoredPosition.x <= -elementWidth)
        {
            duplicateRect.anchoredPosition = new Vector2(targetRect.anchoredPosition.x + elementWidth, duplicateRect.anchoredPosition.y);
        }
    }
}
