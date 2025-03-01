using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ButtonStateTester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Selectable selectable;
    private ColorBlock colors;
    private Image buttonImage;
    
    [Header("Test Controls")]
    [Tooltip("Enable to simulate a pressed state")]
    public bool simulatePressed = false;
    
    [Tooltip("Enable to simulate a selected state")]
    public bool simulateSelected = false;
    
    [Tooltip("Enable to simulate a disabled state")]
    public bool simulateDisabled = false;
    
    [Tooltip("Enable to simulate a highlighted state")]
    public bool simulateHighlighted = false;
    
    [Header("Debug Info")]
    [SerializeField] private string currentState = "Normal";
    
    private void Awake()
    {
        button = GetComponent<Button>();
        selectable = button as Selectable;
        buttonImage = GetComponent<Image>();
        colors = button.colors;
    }
    
    private void Update()
    {
        // Store original interactable state
        bool wasInteractable = button.interactable;
        
        // Update button state based on inspector settings
        if (simulateDisabled)
        {
            button.interactable = false;
            currentState = "Disabled";
            UpdateButtonColor(colors.disabledColor);
        }
        else 
        {
            // Restore interactable state if we're not simulating disabled
            button.interactable = true;
            
            if (simulatePressed)
            {
                currentState = "Pressed";
                UpdateButtonColor(colors.pressedColor);
            }
            else if (simulateSelected)
            {
                currentState = "Selected";
                UpdateButtonColor(colors.selectedColor);
            }
            else if (simulateHighlighted)
            {
                currentState = "Highlighted";
                UpdateButtonColor(colors.highlightedColor);
            }
            else
            {
                currentState = "Normal";
                UpdateButtonColor(colors.normalColor);
            }
        }
    }
    
    private void UpdateButtonColor(Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }
    
    // Implement pointer callbacks to see real interactions
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!simulatePressed && !simulateSelected && !simulateDisabled && !simulateHighlighted)
        {
            currentState = "Highlighted (Real)";
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!simulatePressed && !simulateSelected && !simulateDisabled && !simulateHighlighted)
        {
            currentState = "Normal";
        }
    }
    
    // Helper method to simulate a full button press sequence
    public void SimulateFullButtonPress()
    {
        StartCoroutine(ButtonPressSequence());
    }
    
    private IEnumerator ButtonPressSequence()
    {
        // Reset all states first
        simulatePressed = false;
        simulateSelected = false;
        simulateHighlighted = false;
        simulateDisabled = false;
        
        // Highlighted state
        simulateHighlighted = true;
        yield return new WaitForSeconds(0.2f);
        
        // Pressed state
        simulateHighlighted = false;
        simulatePressed = true;
        yield return new WaitForSeconds(0.1f);
        
        // Back to normal
        simulatePressed = false;
        yield return null;
    }
}