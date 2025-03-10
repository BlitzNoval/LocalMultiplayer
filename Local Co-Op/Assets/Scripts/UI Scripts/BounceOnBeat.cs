using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MesmerizingBounceBeatEffects : MonoBehaviour
{
    // Bounce settings
    public float bounceMagnitude = 10f; // How high the bounce goes
    public float bounceDuration = 0.5f; // How long the bounce lasts

    // Beat-synced color change settings
    public AudioSource musicSource; // Assign the AudioSource playing the music
    public float bpm = 120; // Beats per minute of the song
    public Color[] beatColors; // Colors to cycle through on each beat
    public float colorLerpSpeed = 5f; // How quickly the color changes

    // Rotation settings
    public float minRotationSpeed = 50f; // Minimum rotation speed in degrees per second
    public float maxRotationSpeed = 150f; // Maximum rotation speed in degrees per second
    private float[] rotationSpeeds; // Individual rotation speeds for each child

    // Scale pulse settings
    public float pulseMagnitude = 0.2f; // How much the scale changes
    public float pulseSpeed = 2f; // Speed of the pulse effect

    // Visibility settings
    public float visibilityChangeIntervalBeats = 1f; // Number of beats between visibility changes
    public float minVisibleBeats = 1f; // Minimum number of beats an object stays visible
    public float maxVisibleBeats = 2f; // Maximum number of beats an object stays visible

    // Private variables
    private RectTransform[] childRectTransforms;
    private Image[] childImages;
    private Vector2[] originalPositions;
    private Vector3[] originalScales;
    private int currentColorIndex;
    private Color targetColor;

    void Start()
    {
        // Get all child RectTransforms and Images
        int childCount = transform.childCount;
        childRectTransforms = new RectTransform[childCount];
        childImages = new Image[childCount];
        originalPositions = new Vector2[childCount];
        originalScales = new Vector3[childCount];
        rotationSpeeds = new float[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childRectTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
            childImages[i] = transform.GetChild(i).GetComponent<Image>();
            originalPositions[i] = childRectTransforms[i].anchoredPosition;
            originalScales[i] = childRectTransforms[i].localScale;
            rotationSpeeds[i] = Random.Range(minRotationSpeed, maxRotationSpeed); // Randomize rotation speed
        }

        // Initialize color
        currentColorIndex = 0;
        targetColor = beatColors.Length > 0 ? beatColors[0] : Color.white;

        // Start checking for beats and handling visibility
        StartCoroutine(BeatCheck());
        StartCoroutine(VisibilityRoutine());
    }

    void Update()
    {
        // Smoothly transition the color for all child images
        for (int i = 0; i < childImages.Length; i++)
        {
            if (childImages[i] != null)
            {
                childImages[i].color = Color.Lerp(childImages[i].color, targetColor, colorLerpSpeed * Time.deltaTime);
            }
        }

        // Rotate all child objects with individual speeds
        for (int i = 0; i < childRectTransforms.Length; i++)
        {
            if (childRectTransforms[i] != null)
            {
                childRectTransforms[i].Rotate(Vector3.forward * rotationSpeeds[i] * Time.deltaTime);
            }
        }

        // Apply scale pulse effect
        for (int i = 0; i < childRectTransforms.Length; i++)
        {
            if (childRectTransforms[i] != null)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseMagnitude;
                childRectTransforms[i].localScale = originalScales[i] * (1 + pulse);
            }
        }
    }

    // Beat detection and color change logic
    IEnumerator BeatCheck()
    {
        float beatInterval = 60f / bpm; // Calculate time between beats
        double nextBeatTime = AudioSettings.dspTime; // Get the current audio time

        while (true)
        {
            // Check if the music is playing and if a beat has occurred
            if (musicSource.isPlaying && AudioSettings.dspTime >= nextBeatTime)
            {
                ChangeColor(); // Change the color
                StartBounce(); // Start the bounce effect
                nextBeatTime += beatInterval; // Schedule the next beat
            }
            yield return null;
        }
    }

    // Change the color to the next in the array
    void ChangeColor()
    {
        if (beatColors.Length == 0) return;

        // Cycle through the colors
        currentColorIndex = (currentColorIndex + 1) % beatColors.Length;
        targetColor = beatColors[currentColorIndex];
    }

    // Start the bounce effect for all children
    void StartBounce()
    {
        for (int i = 0; i < childRectTransforms.Length; i++)
        {
            if (childRectTransforms[i] != null)
            {
                StartCoroutine(BounceRoutine(childRectTransforms[i], originalPositions[i]));
            }
        }
    }

    // Bounce animation for a single RectTransform
    IEnumerator BounceRoutine(RectTransform rectTransform, Vector2 originalPosition)
    {
        float timer = 0f;
        Vector2 startPos = originalPosition;

        while (timer < bounceDuration)
        {
            timer += Time.deltaTime;
            // Calculate bounce offset using a sine wave
            float bounceOffset = Mathf.Sin(timer / bounceDuration * Mathf.PI) * bounceMagnitude;
            rectTransform.anchoredPosition = startPos + Vector2.up * bounceOffset;
            yield return null;
        }

        // Reset to the original position
        rectTransform.anchoredPosition = startPos;
    }

    // Randomly toggle visibility of child objects based on BPM
    IEnumerator VisibilityRoutine()
    {
        float beatInterval = 60f / bpm; // Calculate time between beats

        while (true)
        {
            // Randomly enable or disable each child object
            for (int i = 0; i < childImages.Length; i++)
            {
                if (childImages[i] != null)
                {
                    bool isVisible = Random.Range(0, 2) == 0; // 50% chance to be visible
                    childImages[i].enabled = isVisible;
                }
            }

            // Wait for a random number of beats before changing visibility again
            float waitBeats = Random.Range(minVisibleBeats, maxVisibleBeats);
            yield return new WaitForSeconds(waitBeats * beatInterval);
        }
    }
}