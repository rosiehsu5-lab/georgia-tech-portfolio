using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to a UI GameObject. Assign the fill Image and optionally a label.
// Reads CurrentNoiseLevel from PenguNoise each frame and updates the bar.
public class NoiseMeterUI : MonoBehaviour
{
    [Header("References")]
    public PenguNoiseController penguNoise;
    public Image fillBar;           // Image with Image Type set to Filled
    public TextMeshProUGUI label;   // Optional label e.g. "NOISE"

    [Header("Colours")]
    public Color quietColour = new Color(0.2f, 0.8f, 0.2f); // green
    public Color mediumColour = new Color(1.0f, 0.8f, 0.0f); // yellow
    public Color loudColour = new Color(1.0f, 0.2f, 0.2f); // red

    [Header("Thresholds")]
    [Range(0f, 1f)] public float mediumThreshold = 0.4f;
    [Range(0f, 1f)] public float loudThreshold = 0.75f;

    void Update()
    {
        if (penguNoise == null || fillBar == null) return;

        float level = penguNoise.CurrentNoiseLevel;

        // Update fill amount
        fillBar.fillAmount = level;

        // Lerp colour through quiet -> medium -> loud
        if (level < mediumThreshold)
        {
            float t = Mathf.InverseLerp(0f, mediumThreshold, level);
            fillBar.color = Color.Lerp(quietColour, mediumColour, t);
        }
        else
        {
            float t = Mathf.InverseLerp(mediumThreshold, loudThreshold, level);
            fillBar.color = Color.Lerp(mediumColour, loudColour, t);
        }
    }
}